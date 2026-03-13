using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI
{
    public class CardSelector
    {
        private enum SelectType
        {
            Card,
            Cards,
            Id,
            Ids,
            Location
        }

        private SelectType _type;
        private ClientCard _card;
        private IList<ClientCard> _cards;
        private int _id;
        private IList<int> _ids;
        private CardLocation _location;

        public CardSelector(ClientCard card)
        {
            _type = SelectType.Card;
            _card = card;
        }

        public CardSelector(IList<ClientCard> cards)
        {
            _type = SelectType.Cards;
            _cards = cards;
        }

        public CardSelector(int cardId)
        {
            _type = SelectType.Id;
            _id = cardId;
        }

        public CardSelector(IList<int> ids)
        {
            _type = SelectType.Ids;
            _ids = ids;
        }

        public CardSelector(CardLocation location)
        {
            _type = SelectType.Location;
            _location = location;
        }

        public IList<ClientCard> Select(IList<ClientCard> cards, int min, int max)
        {
            IList<ClientCard> result = new List<ClientCard>();

            switch (_type)
            {
                case SelectType.Card:
                    if (cards.Contains(_card))
                        result.Add(_card);
                    break;
                case SelectType.Cards:
                    foreach (ClientCard card in _cards)
                        if (cards.Contains(card) && !result.Contains(card))
                            result.Add(card);
                    break;
                case SelectType.Id:
                    foreach (ClientCard card in cards)
                        if (card.IsCode(_id))
                            result.Add(card);
                    break;
                case SelectType.Ids:
                    foreach (int id in _ids)
                        foreach (ClientCard card in cards)
                            if (card.IsCode(id) && !result.Contains(card))
                                result.Add(card);
                    break;
                case SelectType.Location:
                    foreach (ClientCard card in cards)
                        if (card.Location == _location)
                            result.Add(card);
                    break;
            }

            if (result.Count < min)
            {
                foreach (ClientCard card in cards)
                {
                    if (!result.Contains(card))
                        result.Add(card);
                    if (result.Count >= min)
                        break;
                }
            }

            while (result.Count > max)
                result.RemoveAt(result.Count - 1);

            return result;
        }
    }
}