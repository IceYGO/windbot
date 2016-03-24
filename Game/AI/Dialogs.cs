using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace WindBot.Game.AI
{
    [DataContract]
    public class DialogsData
    {
        [DataMember]
        public string duelstart { get; set; }
        [DataMember]
        public string newturn { get; set; }
        [DataMember]
        public string endturn { get; set; }
        [DataMember]
        public string directattack { get; set; }
        [DataMember]
        public string attack { get; set; }
        [DataMember]
        public string activate { get; set; }
        [DataMember]
        public string summon { get; set; }
        [DataMember]
        public string setmonster { get; set; }
        [DataMember]
        public string chaining { get; set; }                                                
    }
    public class Dialogs
    {
        private GameClient _game;

        private string[] _duelstart;
        private string[] _newturn;
        private string[] _endturn;
        private string[] _directattack;
        private string[] _attack;
        private string[] _activate;
        private string[] _summon;
        private string[] _setmonster;
        private string[] _chaining;
        
        public Dialogs(GameClient game)
        {
            _game = game;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(DialogsData));
            using (FileStream fs = File.OpenRead("dialogs/" + Environment.GetEnvironmentVariable("YGOPRO_DIALOG") + ".json")) 
            {
                DialogsData data = (DialogsData)serializer.ReadObject();
                _duelstart = data.duelstart;
                _newturn = data.newturn;
                _endturn = data.endturn;
                _directattack = data.directattack;
                _attack = data.attack;
                _activate = data.activate;
                _summon = data.summon;
                _setmonster = data.setmonster;
                _chaining = data.chaining;
            }
        }

        public void SendDuelStart()
        {
            InternalSendMessage(_duelstart);
        }

        public void SendNewTurn()
        {
            InternalSendMessage(_newturn);
        }

        public void SendEndTurn()
        {
            InternalSendMessage(_endturn);
        }

        public void SendDirectAttack(string attacker)
        {
            InternalSendMessage(_directattack, attacker);
        }

        public void SendAttack(string attacker, string defender)
        {
            InternalSendMessage(_attack, attacker, defender);
        }

        public void SendActivate(string spell)
        {
            InternalSendMessage(_activate, spell);
        }

        public void SendSummon(string monster)
        {
            InternalSendMessage(_summon, monster);
        }

        public void SendSetMonster()
        {
            InternalSendMessage(_setmonster);
        }

        public void SendChaining(string card)
        {
            InternalSendMessage(_chaining, card);
        }

        private void InternalSendMessage(IList<string> array, params object[] opts)
        {
            string message = string.Format(array[Program.Rand.Next(array.Count)], opts);
            _game.Chat(message);
        }
    }
}