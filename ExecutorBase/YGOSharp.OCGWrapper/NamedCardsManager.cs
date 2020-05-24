using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System;
using System.IO;

namespace YGOSharp.OCGWrapper
{
    public static class NamedCardsManager
    {
        private static IDictionary<int, NamedCard> _cards = new Dictionary<int, NamedCard>();

        public static void Init(string databaseFullPath)
        {
            try
            {
                if (!File.Exists(databaseFullPath))
                {
                    throw new Exception("Could not find the cards database.");
                }

                using (SqliteConnection connection = new SqliteConnection("Data Source=" + databaseFullPath))
                {
                    connection.Open();

                    using (IDbCommand command = new SqliteCommand(
                        "SELECT datas.id, ot, alias, setcode, type, level, race, attribute, atk, def, texts.name, texts.desc"
                        + " FROM datas INNER JOIN texts ON datas.id = texts.id",
                        connection))
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
            catch (Exception ex)
            {
                throw new Exception("Could not initialize the cards database. Check the inner exception for more details.", ex);
            }
        }

        internal static NamedCard GetCard(int id)
        {
            if (_cards.ContainsKey(id))
                return _cards[id];
            return null;
        }

        private static void LoadCard(IDataRecord reader)
        {
            NamedCard card = new NamedCard(reader);
            _cards.Add(card.Id, card);
        }
    }
}
