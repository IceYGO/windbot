using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;

namespace YGOSharp.OCGWrapper
{
    internal static class CardsManager
    {
        private static IDictionary<int, Card> _cards;

        internal static void Init(string databaseFullPath)
        {
            _cards = new Dictionary<int, Card>();

            using (SqliteConnection connection = new SqliteConnection("Data Source=" + databaseFullPath))
            {
                connection.Open();

                using (IDbCommand command = new SqliteCommand("SELECT id, ot, alias, setcode, type, level, race, attribute, atk, def FROM datas", connection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LoadCard(reader);
                        }
                    }
                }
            }
        }

        internal static Card GetCard(int id)
        {
            if (_cards.ContainsKey(id))
                return _cards[id];
            return null;
        }

        private static void LoadCard(IDataRecord reader)
        {
            Card card = new Card(reader);
            _cards.Add(card.Id, card);
        }
    }
}