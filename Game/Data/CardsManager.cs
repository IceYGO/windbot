using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using Mono.Data.Sqlite;

namespace WindBot.Game.Data
{
    public class CardsManager
    {
        private static IDictionary<int, CardData> _cards;

        public static void Init()
        {
            try
            {
                _cards = new Dictionary<int, CardData>();

                string currentPath = Assembly.GetExecutingAssembly().Location;
                currentPath = Path.GetDirectoryName(currentPath) ?? "";
                string absolutePath = Path.Combine(currentPath, "cards.cdb");

                if (!File.Exists(absolutePath))
                {
                    throw new Exception("Could not find the cards database.");
                }

                using (SqliteConnection connection = new SqliteConnection("Data Source=" + absolutePath))
                {
                    connection.Open();

                    const string select =
                        "SELECT datas.id, alias, type, level, race, attribute, atk, def, name, desc " +
                        "FROM datas INNER JOIN texts ON datas.id = texts.id";

                    using (SqliteCommand command = new SqliteCommand(select, connection))
                    using (SqliteDataReader reader = command.ExecuteReader())
                        InitCards(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not initialize the cards database. Check the inner exception for more details.", ex);
            }
        }

        private static void InitCards(IDataReader reader)
        {
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                CardData card = new CardData(id)
                {
                    AliasId = reader.GetInt32(1),
                    Type = reader.GetInt32(2),
                    Level = reader.GetInt32(3) & 0xFF,
                    Race = reader.GetInt32(4),
                    Attribute = reader.GetInt32(5),
                    Atk = reader.GetInt32(6),
                    Def = reader.GetInt32(7),
                    Name = reader.GetString(8),
                    Description = reader.GetString(9)
                };
                _cards.Add(id, card);

            }
        }

        public static int GetCount()
        {
            return _cards.Count;
        }

        public static CardData GetCard(int id)
        {
            if (_cards.ContainsKey(id))
            {
                return _cards[id];
            }
            return null;
        }
    }
}