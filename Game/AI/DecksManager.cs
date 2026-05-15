using System;
using System.Collections.Generic;
using System.Reflection;

namespace WindBot.Game.AI
{
    public static class DecksManager
    {
        private class DeckInstance
        {
            public string Deck { get; private set; }
            public Type Type { get; private set; }
            public string Level { get; private set; }

            public DeckInstance(string deck, Type type, string level)
            {
                Deck = deck;
                Type = type;
                Level = level;
            }
        }

        private static Dictionary<string, DeckInstance> _decks;
        private static List<DeckInstance> _list;
        private static Random _rand;

        public static void Init()
        {
            _decks = new Dictionary<string, DeckInstance>();
            _rand = new Random();

            Assembly asm = Assembly.GetExecutingAssembly();
            Type[] types = asm.GetTypes();
            
            foreach (Type type in types)
            {
                MemberInfo info = type;
                object[] attributes = info.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    if (attribute is DeckAttribute)
                    {
                        DeckAttribute deck = (DeckAttribute)attribute;
                        _decks.Add(deck.Name, new DeckInstance(deck.File, type, deck.Level));
                    }
                }
            }

            _list = new List<DeckInstance>();
            _list.AddRange(_decks.Values);

            Logger.WriteLine("Decks initialized, " + _decks.Count + " found.");
        }

        public static Executor Instantiate(GameAI ai, Duel duel)
        {
            DeckInstance infos;

            string deck = ai.Game.Deck;

            if (deck != null && _decks.ContainsKey(deck))
                infos = _decks[deck];
            else
            {
                do
                {
                    infos = _list[_rand.Next(_list.Count)];
                }
                while (infos.Level != "Normal");
            }

            Executor executor = (Executor)Activator.CreateInstance(infos.Type, ai, duel);
            executor.Deck = infos.Deck;
            return executor;
        }
    }
}
