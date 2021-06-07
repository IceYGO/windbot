using System;
using System.Runtime.Serialization;

namespace WindBot
{
    [DataContract]
    public class CreateGameInfo
    {
        // Although not set when deserializing, default values
        // are the options for a basic MR5 OCG/TCG room.
        // NOTE: Most of these member variables are written as is to a
        // BinaryStream, so watch your step when changing their type!
        [DataMember] public uint banlistHash { get; set; } = 0;
        [DataMember] public byte allowed { get; set; } = 3;
        [DataMember] public bool dontCheckDeck { get; set; } = false;
        [DataMember] public bool dontShuffleDeck { get; set; } = false;
        [DataMember] public uint startingLP { get; set; } = 8000;
        [DataMember] public byte startingDrawCount { get; set; } = 5;
        [DataMember] public byte drawCountPerTurn { get; set; } = 1;
        [DataMember] public ushort timeLimitInSeconds { get; set; } = 180;
        [DataMember] public ulong duelFlags { get; set; } = 190464;
        [DataMember] public int t0Count { get; set; } = 1;
        [DataMember] public int t1Count { get; set; } = 1;
        [DataMember] public int bestOf { get; set; } = 1;
        [DataMember] public int forb { get; set; } = 0;
        [DataMember] public ushort extraRules { get; set; } = 0;
        [DataMember] public string notes { get; set; } = "";
    }

    public class WindBotInfo
    {
        public string Name { get; set; }
        public string Deck { get; set; }
        public string DeckFile { get; set; }
        public string Dialog { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string HostInfo { get; set; }
        public int Version { get; set; }
        public int Hand { get; set; }
        public bool Debug { get; set; }
        public bool Chat { get; set; }
        public int RoomId { get; set; }
        public CreateGameInfo CreateGame { get; set; }
        public WindBotInfo()
        {
            Name = "WindBot";
            Deck = null;
            DeckFile = null;
            Dialog = "default";
            Host = "127.0.0.1";
            Port = 7911;
            HostInfo = "";
            Version = 39|1<<8|9<<16;
            Hand = 0;
            Debug = false;
            Chat = true;
            RoomId = 0;
            CreateGame = null;
        }
    }
}
