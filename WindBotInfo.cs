using System;

namespace WindBot
{
    public class WindBotInfo
    {
        public string Name { get; set; }
        public string Deck { get; set; }
        public string Dialog { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string HostInfo { get; set; }
        public int Version { get; set; }
        public int Hand { get; set; }

        public WindBotInfo()
        {
            Name = "WindBot";
            Deck = null;
            Dialog = "default";
            Host = "127.0.0.1";
            Port = 7911;
            HostInfo = "";
            Version = 0x1342;
            Hand = 0;
        }
    }
}
