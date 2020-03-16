using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System;
using System.IO;

namespace YGOSharp.OCGWrapper
{
    public static class NamedCardsManager
    {
        private static IDictionary<int, NamedCard> _cards;

        public static void Init(string databaseFullPath)
        {
            //_cards = new Dictionary<int, NamedCard>();
        }

        internal static NamedCard GetCard(int id)
        {
            /*if (_cards.ContainsKey(id))
                return _cards[id];*/
            return null;
        }

        private static void LoadCard(IDataRecord reader)
        {
           /* NamedCard card = new NamedCard(reader);
            _cards.Add(card.Id, card);*/
        }
    }
}