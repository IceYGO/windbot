using System;
using System.Collections.Generic;
using System.IO;

namespace WindBot.Game.Data
{
    public class Deck
    {
        public IList<CardData> Cards { get; private set; }
        public IList<CardData> ExtraCards { get; private set; }
        public IList<CardData> SideCards { get; private set; }

        public Deck()
        {
            Cards = new List<CardData>();
            ExtraCards = new List<CardData>();
            SideCards = new List<CardData>();
        }

        private void AddNewCard(int cardId, bool sideDeck)
        {
            CardData newCard = CardsManager.GetCard(cardId);
            if (newCard == null)
                return;

            if (!sideDeck)
                AddCard(newCard);
            else
                SideCards.Add(newCard);
        }

        private void AddCard(CardData card)
        {
            if (card.IsExtraCard())
                ExtraCards.Add(card);
            else
                Cards.Add(card);
        }

        public static Deck Load(string name)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(new FileStream("Decks/" + name + ".ydk", FileMode.Open, FileAccess.Read));

                Deck deck = new Deck();
                bool side = false;

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        continue;

                    line = line.Trim();
                    if (line.StartsWith("#"))
                        continue;
                    if (line.Equals("!side"))
                    {
                        side = true;
                        continue;
                    }

                    int id;
                    if (!int.TryParse(line, out id))
                        continue;

                    deck.AddNewCard(id, side);
                }

                reader.Close();

                if (deck.Cards.Count > 60)
                    return null;
                if (deck.ExtraCards.Count > 15)
                    return null;
                if (deck.SideCards.Count > 15)
                    return null;

                return deck;
            }
            catch (Exception)
            {
                if (reader != null)
                    reader.Close();
                return null;
            }
        }
    }
}