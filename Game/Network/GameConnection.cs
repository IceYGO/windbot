using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WindBot.Game.Network.Enums;

namespace WindBot.Game.Network
{
    public class GameConnection
    {
        public bool IsConnected { get; private set; }

        private TcpClient _client;
        private BinaryReader _reader;
        private Thread _thread;

        private Queue<GameClientPacket> _sendQueue;
        private Queue<GameServerPacket> _receiveQueue;

        private DateTime _lastAction;

        public GameConnection(IPAddress address, int port)
        {
            _sendQueue = new Queue<GameClientPacket>();
            _receiveQueue = new Queue<GameServerPacket>();
            _lastAction = DateTime.Now;
            _client = new TcpClient(address.ToString(), port);
            IsConnected = true;
            _reader = new BinaryReader(_client.GetStream());
            _thread = new Thread(NetworkTick);
            _thread.Start();
        }

        public void Send(GameClientPacket packet)
        {
            lock (_sendQueue)
                _sendQueue.Enqueue(packet);
        }

        public void Send(CtosMessage message)
        {
            Send(new GameClientPacket(message));
        }

        public void Send(CtosMessage message, byte value)
        {
            GameClientPacket packet = new GameClientPacket(message);
            packet.Write(value);
            Send(packet);
        }

        public void Send(CtosMessage message, int value)
        {
            GameClientPacket packet = new GameClientPacket(message);
            packet.Write(value);
            Send(packet);
        }

        public bool HasPacket()
        {
            lock (_receiveQueue)
                return _receiveQueue.Count > 0;
        }

        public GameServerPacket Receive()
        {
            lock (_receiveQueue)
            {
                if (_receiveQueue.Count == 0)
                    return null;
                return _receiveQueue.Dequeue();
            }
        }

        public void Close()
        {
            if (!IsConnected) return;
            IsConnected = false;
            _client.Close();
            _thread.Join();
        }

        private void NetworkTick()
        {
            try
            {
                int connectionCheckTick = 100;
                while (IsConnected)
                {
                    InternalTick();
                    if (--connectionCheckTick <= 0)
                    {
                        connectionCheckTick = 100;
                        if (!CheckIsConnected())
                            Close();
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception)
            {
                Close();
            }
        }

        private void InternalTick()
        {
            lock (_sendQueue)
            {
                while (_sendQueue.Count > 0)
                    InternalSend(_sendQueue.Dequeue());
            }
            while (_client.Available > 1)
            {
                GameServerPacket packet = InternalReceive();
                lock (_receiveQueue)
                {
                    _receiveQueue.Enqueue(packet);
                }
            }
        }

        private void InternalSend(GameClientPacket packet)
        {
            _lastAction = DateTime.Now;
            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);
            byte[] content = packet.GetContent();
            writer.Write((short)content.Length);
            if (content.Length > 0)
                writer.Write(content);
            byte[] data = ms.ToArray();
            _client.Client.Send(data);
        }

        private GameServerPacket InternalReceive()
        {
            _lastAction = DateTime.Now;
            int len = _reader.ReadInt16();
            GameServerPacket packet = new GameServerPacket(_reader.ReadBytes(len));
            return packet;
        }

        private bool CheckIsConnected()
        {
            TimeSpan diff = DateTime.Now - _lastAction;
            if (diff.TotalMinutes > 5)
                return false;
            return true;
        }
    }
}