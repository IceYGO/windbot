using System.Collections.Generic;

namespace WindBot.Game.AI
{
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
            _duelstart = new[]
                {
                    "Good luck, have fun."
                };
            _newturn = new[]
                {
                    "It's my turn, draw.",
                    "My turn, draw.",
                    "I draw a card."
                };
            _endturn = new[]
                {
                    "I end my turn.",
                    "My turn is over.",
                    "Your turn."
                };
            _directattack = new[]
                {
                    "{0}, direct attack!",
                    "{0}, attack him directly!",
                    "{0}, he's defenseless, attack!",
                    "{0}, attack his life points!",
                    "{0}, attack his life points directly!",
                    "{0}, attack him through a direct attack!",
                    "{0}, attack him using a direct attack!",
                    "{0}, unleash your power through a direct attack!",
                    "My {0} is going to smash your life points!",
                    "Show your power to my opponent, {0}!",
                    "You can't stop me. {0}, attack!"
                };
            _attack = new[]
                {
                    "{0}, attack this {1}!",
                    "{0}, destroy this {1}!",
                    "{0}, charge the {1}!",
                    "{0}, strike that {1}!",
                    "{0}, unleash your power on this {1}!"
                };
            _activate = new[]
                {
                    "I'm activating {0}.",
                    "I'm using the effect of {0}.",
                    "I use the power of {0}."
                };
            _summon = new[]
                {
                    "I'm summoning {0}.",
                    "Come on, {0}!",
                    "Appear, {0}!",
                    "I summon the powerful {0}.",
                    "I call {0} to the battle!",
                    "I'm calling {0}.",
                    "Let's summon {0}."
                };
            _setmonster = new[]
                {
                    "I'm setting a monster.",
                    "I set a face-down monster.",
                    "I place a hidden monster."
                };
            _chaining = new[]
                {
                    "Look at that! I'm activating {0}.",
                    "I use the power of {0}.",
                    "Get ready! I use {0}.",
                    "I don't think so. {0}, activation!",
                    "Looks like you forgot my {0}.",
                    "Did you consider the fact I have {0}?"
                };
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