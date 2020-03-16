using System;
using System.Net;
using System.Net.Sockets;

namespace YGOSharp.Network
{
    public class AsyncNetworkServer
    {
        public bool IsListening { get; private set; }

        public event Action<NetworkClient> ClientConnected;

        private TcpListener _listener;
        private bool _isClosed;

        public AsyncNetworkServer(IPAddress address, int port)
        {
            _listener = new TcpListener(address, port);
        }

        public void Start()
        {
            if (!IsListening && !_isClosed)
            {
                IsListening = true;
                _listener.Start();
                BeginAcceptSocket();
            }
        }

        public void Close()
        {
            if (!_isClosed)
            {
                _isClosed = true;
                IsListening = false;
                _listener.Stop();
            }
        }

        private void BeginAcceptSocket()
        {
            try
            {
                _listener.BeginAcceptSocket(AcceptSocketCallback, null);
            }
            catch (Exception)
            {
                Close();
            }
        }

        private void AcceptSocketCallback(IAsyncResult result)
        {
            try
            {
                Socket socket = _listener.EndAcceptSocket(result);
                ClientConnected?.Invoke(new NetworkClient(socket));
                BeginAcceptSocket();
            }
            catch (Exception)
            {
                Close();
            }
        }
    }
}
