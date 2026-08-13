using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using WindBot.Game.AI;
using YGOSharp.Network;
using YGOSharp.Network.Enums;
using YGOSharp.Network.Utils;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game
{
    public class GameBehavior
    {
        public GameClient Game { get; private set; }
        public YGOClient Connection { get; private set; }
        public Deck Deck { get; private set; }

        private GameAI _ai;

        private IDictionary<StocMessage, Action<BinaryReader>> _packets;
        private IDictionary<GameMessage, Action<BinaryReader>> _messages;

        private Room _room;
        private Duel _duel;
        private int _hand;
        private bool _debug;
        private bool _isTag;
        private bool _botDeckActive;
        private bool _botDeckNeedsInitialHandSync;
        private bool _chatPlayerOrderSwapped;
        private int _select_hint;
        private GameMessage _lastMessage;

        public GameBehavior(GameClient game)
        {
            Game = game;
            Connection = game.Connection;
            _hand = game.Hand;
            _debug = game.Debug;           
            _packets = new Dictionary<StocMessage, Action<BinaryReader>>();
            _messages = new Dictionary<GameMessage, Action<BinaryReader>>();
            RegisterPackets();

            _room = new Room();
            _duel = new Duel();

            _ai = new GameAI(Game, _duel);
            _ai.Executor = DecksManager.Instantiate(_ai, _duel);
            Game.SetDeckContext(_ai.Executor.GetType().Name);
            Deck = Deck.Load(Game.DeckFile ?? _ai.Executor.Deck);

            _select_hint = 0;
        }

        public int GetLocalPlayer(int player)
        {
            return _duel.IsFirst ? player : 1 - player;
        }

        private void TrackDeckMove(int cardId, int previousController, int previousLocation, int currentController, int currentLocation)
        {
            if (!_botDeckActive)
                return;

            bool leavesBotDeck = previousController == 0 && previousLocation == (int)CardLocation.Deck;
            bool entersBotDeck = currentController == 0 && currentLocation == (int)CardLocation.Deck;
            if (leavesBotDeck == entersBotDeck)
                return;

            if (leavesBotDeck)
                _duel.Fields[0].RemoveFromDeck(cardId);
            else
                _duel.Fields[0].AddToDeck(cardId);
        }

        private void ValidateBotDeckCount()
        {
            if (_botDeckActive)
                _duel.Fields[0].ValidateDeckCount(_duel.Fields[0].Deck.Count);
        }

        public void OnPacket(BinaryReader packet)
        {
            StocMessage id = (StocMessage)packet.ReadByte();
            Game.SetCurrentSTOCMessage(id.ToString());
            if (id == StocMessage.GameMsg)
            {
                GameMessage msg = (GameMessage)packet.ReadByte();
                Game.SetCurrentSTOCMessage(msg.ToString());
                if (_messages.ContainsKey(msg))
                    _messages[msg](packet);
                _lastMessage = msg;
                return;
            }
            if (_packets.ContainsKey(id))
                _packets[id](packet);
        }

        private void RegisterPackets()
        {
            _packets.Add(StocMessage.JoinGame, OnJoinGame);
            _packets.Add(StocMessage.TypeChange, OnTypeChange);
            _packets.Add(StocMessage.HsPlayerEnter, OnPlayerEnter);
            _packets.Add(StocMessage.HsPlayerChange, OnPlayerChange);
            _packets.Add(StocMessage.SelectHand, OnSelectHand);
            _packets.Add(StocMessage.SelectTp, OnSelectTp);
            _packets.Add(StocMessage.TimeLimit, OnTimeLimit);
            _packets.Add(StocMessage.Replay, OnReplay);
            _packets.Add(StocMessage.DuelEnd, OnDuelEnd);
            _packets.Add(StocMessage.Chat, OnChat);
            _packets.Add(StocMessage.ChangeSide, OnChangeSide);
            _packets.Add(StocMessage.ErrorMsg, OnErrorMsg);
            _packets.Add(StocMessage.TeammateSurrender, OnTeammateSurrender);

            _messages.Add(GameMessage.Retry, OnRetry);
            _messages.Add(GameMessage.Start, OnStart);
            _messages.Add(GameMessage.Hint, OnHint);
            _messages.Add(GameMessage.Win, OnWin);
            _messages.Add(GameMessage.Draw, OnDraw);
            _messages.Add(GameMessage.ShuffleDeck, OnShuffleDeck);
            _messages.Add(GameMessage.ShuffleHand, OnShuffleHand);
            _messages.Add(GameMessage.ShuffleExtra, OnShuffleExtra);
            _messages.Add(GameMessage.ShuffleSetCard, OnShuffleSetCard);
            _messages.Add(GameMessage.SwapGraveDeck, OnSwapGraveDeck);
            _messages.Add(GameMessage.ReverseDeck, OnReverseDeck);
            _messages.Add(GameMessage.TagSwap, OnTagSwap);
            _messages.Add(GameMessage.NewTurn, OnNewTurn);
            _messages.Add(GameMessage.NewPhase, OnNewPhase);
            _messages.Add(GameMessage.Damage, OnDamage);
            _messages.Add(GameMessage.PayLpCost, OnDamage);
            _messages.Add(GameMessage.Recover, OnRecover);
            _messages.Add(GameMessage.LpUpdate, OnLpUpdate);
            _messages.Add(GameMessage.Move, OnMove);
            _messages.Add(GameMessage.Swap, OnSwap);
            _messages.Add(GameMessage.Attack, OnAttack);
            _messages.Add(GameMessage.Battle, OnBattle);
            _messages.Add(GameMessage.AttackDisabled, OnAttackDisabled);
            _messages.Add(GameMessage.PosChange, OnPosChange);
            _messages.Add(GameMessage.Chaining, OnChaining);
            _messages.Add(GameMessage.ChainSolving, OnChainSolving);
            _messages.Add(GameMessage.ChainNegated, OnChainNegated);
            _messages.Add(GameMessage.ChainDisabled, OnChainDisabled);
            _messages.Add(GameMessage.ChainSolved, OnChainSolved);
            _messages.Add(GameMessage.ChainEnd, OnChainEnd);
            _messages.Add(GameMessage.SortCard, OnCardSorting);
            _messages.Add(GameMessage.SortChain, OnChainSorting);
            _messages.Add(GameMessage.UpdateCard, OnUpdateCard);
            _messages.Add(GameMessage.UpdateData, OnUpdateData);
            _messages.Add(GameMessage.BecomeTarget, OnBecomeTarget);
            _messages.Add(GameMessage.SelectBattleCmd, OnSelectBattleCmd);
            _messages.Add(GameMessage.SelectCard, OnSelectCard);
            _messages.Add(GameMessage.SelectUnselect, OnSelectUnselectCard);
            _messages.Add(GameMessage.SelectChain, OnSelectChain);
            _messages.Add(GameMessage.SelectCounter, OnSelectCounter);
            _messages.Add(GameMessage.SelectDisfield, OnSelectDisfield);
            _messages.Add(GameMessage.SelectEffectYn, OnSelectEffectYn);
            _messages.Add(GameMessage.SelectIdleCmd, OnSelectIdleCmd);
            _messages.Add(GameMessage.SelectOption, OnSelectOption);
            _messages.Add(GameMessage.SelectPlace, OnSelectPlace);
            _messages.Add(GameMessage.SelectPosition, OnSelectPosition);
            _messages.Add(GameMessage.SelectSum, OnSelectSum);
            _messages.Add(GameMessage.SelectTribute, OnSelectTribute);
            _messages.Add(GameMessage.SelectYesNo, OnSelectYesNo);
            _messages.Add(GameMessage.AnnounceAttrib, OnAnnounceAttrib);
            _messages.Add(GameMessage.AnnounceCard, OnAnnounceCard);
            _messages.Add(GameMessage.AnnounceNumber, OnAnnounceNumber);
            _messages.Add(GameMessage.AnnounceRace, OnAnnounceRace);
            _messages.Add(GameMessage.RockPaperScissors, OnRockPaperScissors);
            _messages.Add(GameMessage.Equip, OnEquip);
            _messages.Add(GameMessage.Unequip, OnUnEquip);
            _messages.Add(GameMessage.CardTarget, OnCardTarget);
            _messages.Add(GameMessage.CancelTarget, OnCancelTarget);
            _messages.Add(GameMessage.Summoning, OnSummoning);
            _messages.Add(GameMessage.Summoned, OnSummoned);
            _messages.Add(GameMessage.SpSummoning, OnSpSummoning);
            _messages.Add(GameMessage.SpSummoned, OnSpSummoned);
            _messages.Add(GameMessage.FlipSummoning, OnFlipSummoning);
            _messages.Add(GameMessage.FlipSummoned, OnSummoned);
            _messages.Add(GameMessage.ConfirmCards, OnConfirmCards);
            _messages.Add(GameMessage.PlayerHint, OnPlayerHint);

            // ConfirmDecktop, DeckTop and ConfirmExtratop are intentionally not registered.
            // Effects that inspect or return cards may temporarily reveal cards in either Main Deck
            // or the opponent's face-down Extra Deck, but deck executors currently cannot rely on
            // that transient knowledge when making decisions.
        }

        private void OnJoinGame(BinaryReader packet)
        {
            /*int lflist = (int)*/ packet.ReadUInt32();
            /*int rule = */ packet.ReadByte();
            int mode = packet.ReadByte();
            int duel_rule = packet.ReadByte();
            _isTag = (mode == 2);
            _ai.Duel.IsNewRule = (duel_rule >= 4);
            _ai.Duel.IsNewRule2020 = (duel_rule >= 5);
            BinaryWriter deck = GamePacketFactory.Create(CtosMessage.UpdateDeck);
            deck.Write(Deck.Cards.Count + Deck.ExtraCards.Count);
            deck.Write(Deck.SideCards.Count);
            foreach (NamedCard card in Deck.Cards)
                deck.Write(card.Id);
            foreach (NamedCard card in Deck.ExtraCards)
                deck.Write(card.Id);
            foreach (NamedCard card in Deck.SideCards)
                deck.Write(card.Id);
            Connection.Send(deck);
            _ai.OnJoinGame();
        }

        private void OnChangeSide(BinaryReader packet)
        {
            _chatPlayerOrderSwapped = false;
            BinaryWriter deck = GamePacketFactory.Create(CtosMessage.UpdateDeck);
            deck.Write(Deck.Cards.Count + Deck.ExtraCards.Count);
            deck.Write(Deck.SideCards.Count);
            foreach (NamedCard card in Deck.Cards)
                deck.Write(card.Id);
            foreach (NamedCard card in Deck.ExtraCards)
                deck.Write(card.Id);
            foreach (NamedCard card in Deck.SideCards)
                deck.Write(card.Id);
            Connection.Send(deck);
            _ai.OnJoinGame();
        }

        private void OnTypeChange(BinaryReader packet)
        {
            int type = packet.ReadByte();
            int pos = type & 0xF;
            if (pos < 0 || pos > 3)
            {
                Connection.Close();
                return;
            }
            _room.Position = pos;
            _room.IsHost = ((type >> 4) & 0xF) != 0;
            _room.IsReady[pos] = true;
            Connection.Send(CtosMessage.HsReady);
        }

        private void OnPlayerEnter(BinaryReader packet)
        {
            string name = packet.ReadUnicode(20);
            int pos = packet.ReadByte();
            if (pos < 8)
                _room.Names[pos] = name;
        }

        private void OnPlayerChange(BinaryReader packet)
        {
            int change = packet.ReadByte();
            int pos = (change >> 4) & 0xF;
            int state = change & 0xF;
            if (pos > 3)
                return;
            if (state < 8)
            {
                string oldname = _room.Names[pos];
                _room.Names[pos] = null;
                _room.Names[state] = oldname;
                _room.IsReady[pos] = false;
                _room.IsReady[state] = false;
            }
            else if (state == (int)PlayerChange.Ready)
                _room.IsReady[pos] = true;
            else if (state == (int)PlayerChange.NotReady)
                _room.IsReady[pos] = false;
            else if (state == (int)PlayerChange.Leave || state == (int)PlayerChange.Observe)
            {
                _room.IsReady[pos] = false;
                _room.Names[pos] = null;
            }

            bool allPlayersReady = _room.IsReady[0] && _room.IsReady[1] &&
                (!_isTag || (_room.IsReady[2] && _room.IsReady[3]));
            if (_room.IsHost && allPlayersReady)
                Connection.Send(CtosMessage.HsStart);
        }

        private void OnSelectHand(BinaryReader packet)
        {
            int result;
            if (_hand > 0)
                result = _hand;
            else
                result = _ai.OnRockPaperScissors();
            Connection.Send(CtosMessage.HandResult, (byte)result);
        }

        private void OnSelectTp(BinaryReader packet)
        {
            bool start = _ai.OnSelectHand();
            Connection.Send(CtosMessage.TpResult, (byte)(start ? 1 : 0));
        }

        private void OnTimeLimit(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            if (player == 0)
                Connection.Send(CtosMessage.TimeConfirm);
        }

        private void OnReplay(BinaryReader packet)
        {
            /*byte[] replay =*/ packet.ReadToEnd();

            /*
            const string directory = "Replays";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string otherName = _room.Position == 0 ? _room.Names[1] : _room.Names[0];
            string file = DateTime.Now.ToString("yyyy-MM-dd.HH-mm.") + otherName + ".yrp";
            string fullname = Path.Combine(directory, file);

            if (Regex.IsMatch(file, @"^[\w\-. ]+$"))
                File.WriteAllBytes(fullname, replay);
            */

            //Connection.Close();
        }
        
        private void OnDuelEnd(BinaryReader packet)
        {
            Thread.Sleep(500);
            Connection.Close();
        }

        private void OnChat(BinaryReader packet)
        {
            if (Program.ServerMode) return;
            int player = packet.ReadUInt16();
            string message = packet.ReadUnicode(256);
            if (player < 4)
            {
                int namePosition = player;
                if (_chatPlayerOrderSwapped)
                    namePosition = _isTag ? player ^ 2 : 1 - player;
                string playerName = _room.Names[namePosition] ?? "Player " + namePosition;
                Logger.DebugWriteLine(playerName + " says: " + message);
            }
            else if (player == 8 || (player >= 11 && player <= 19))
                Logger.DebugWriteLine("System message(" + player + "): " + message);
            else
                Logger.DebugWriteLine("Spectator or unknown message(" + player + "): " + message);
        }

        private void OnErrorMsg(BinaryReader packet)
        {
            int msg = packet.ReadByte();
            // align
            packet.ReadByte();
            packet.ReadByte();
            packet.ReadByte();
            int pcode = packet.ReadInt32();
            Logger.DebugWriteLine("Error message received: " + msg + ", code: " + pcode);
            if (msg == 2) //ERRMSG_DECKERROR
            {
                int code = pcode & 0xFFFFFFF;
                int flag = pcode >> 28;
                if (flag <= 5) //DECKERROR_CARDCOUNT
                {
                    NamedCard card = NamedCard.Get(code);
                    if (card != null)
                        _ai.OnDeckError(card.Name);
                    else
                        _ai.OnDeckError("Unknown Card");
                }
                else
                    _ai.OnDeckError("DECK");
            }
            //Connection.Close();
        }

        private void OnTeammateSurrender(BinaryReader packet)
        {
            Thread.Sleep(500);
            Game.Surrender();
        }

        private void OnRetry(BinaryReader packet)
        {
            _ai.OnRetry();
            Connection.Close();
            Logger.WriteErrorLine("Got MSG_RETRY. Last message is " + _lastMessage);
        }

        private void OnHint(BinaryReader packet)
        {
            int type = packet.ReadByte();
            int player = GetLocalPlayer(packet.ReadByte());
            int data = packet.ReadInt32();
            if (type == 1) // HINT_EVENT
            {
                if (data == 24) // battling
                {
                    _duel.Fields[0].UnderAttack = false;
                    _duel.Fields[1].UnderAttack = false;
                }
            }
            if (type == 3) // HINT_SELECTMSG
            {
                _select_hint = data;
            }
            if (type == 4) // HINT_OPSELECTED
            {
                _ai.OnReceivingAnnouce(player, data);
            }
            if (type == 11) // HINT_ZONE
            {
                Logger.DebugWriteLine("HINT_ZONE received: player=" + player + ", zone=" + data);
                _ai.OnHintZone(player, data);
            }
        }

        private void OnStart(BinaryReader packet)
        {
            int type = packet.ReadByte();
            _duel.IsFirst = (type & 0xF) == 0;
            // The server may reorder player types for turn order, while room names remain in lobby slots.
            int roomTeam = _isTag ? _room.Position / 2 : _room.Position;
            _chatPlayerOrderSwapped = roomTeam != (_duel.IsFirst ? 0 : 1);
            _duel.Turn = 0;
            _duel.LastChainLocation = 0;
            _duel.LastChainPlayer = -1;
            _duel.LastChainTargets.Clear();
            _duel.LastSummonedCards.Clear();
            _duel.LastSummonPlayer = -1;
            int duel_rule = packet.ReadByte();
            _ai.Duel.IsNewRule = (duel_rule >= 4);
            _ai.Duel.IsNewRule2020 = (duel_rule >= 5);
            _duel.DeckReversed = false;
            _duel.Fields[GetLocalPlayer(0)].LifePoints = packet.ReadInt32();
            _duel.Fields[GetLocalPlayer(1)].LifePoints = packet.ReadInt32();
            int deck = packet.ReadInt16();
            int extra = packet.ReadInt16();
            _duel.Fields[GetLocalPlayer(0)].Init(deck, extra, GetLocalPlayer(0));
            deck = packet.ReadInt16();
            extra = packet.ReadInt16();
            _duel.Fields[GetLocalPlayer(1)].Init(deck, extra, GetLocalPlayer(1));

            _duel.Fields[0].SetInitialDeck(Deck.Cards);
            // In tag duels the first team's lower lobby slot and the second team's upper
            // lobby slot begin with their physical deck active. Later TAG_SWAP messages
            // toggle the active teammate for that side.
            _botDeckActive = !_isTag || (_duel.IsFirst
                ? _room.Position % 2 == 0
                : _room.Position % 2 == 1);
            _botDeckNeedsInitialHandSync = _isTag && !_botDeckActive;
            _duel.Fields[0].SetDeckTrackingActive(_botDeckActive);
            ValidateBotDeckCount();

            // in case of ending duel in chain's solving
            _duel.CurrentChain.Clear();
            _duel.CurrentChainInfo.Clear();
            _duel.ChainTargets.Clear();
            _duel.ChainTargetOnly.Clear();
            _duel.SummoningCards.Clear();
            _duel.SolvingChainIndex = 0;
            _duel.NegatedChainIndexList.Clear();

            if (_isTag)
            {
                Logger.DebugWriteLine("Duel started: " + _room.Names[0] + " and " + _room.Names[1] +
                    " versus " + _room.Names[2] + " and " + _room.Names[3]);
            }
            else
            {
                Logger.DebugWriteLine("Duel started: " + _room.Names[0] + " versus " + _room.Names[1]);
            }
            _ai.OnStart();
        }

        private void OnWin(BinaryReader packet)
        {
            int result = GetLocalPlayer(packet.ReadByte());

            string otherName;
            if (_isTag)
            {
                int opponentTeamPosition = _room.Position < 2 ? 2 : 0;
                otherName = _room.Names[opponentTeamPosition] + " and " +
                    _room.Names[opponentTeamPosition + 1];
            }
            else
            {
                otherName = _room.Position == 0 ? _room.Names[1] : _room.Names[0];
            }
            string textResult = (result == 2 ? "Draw" : result == 0 ? "Win" : "Lose");
            Logger.DebugWriteLine("Duel finished against " + otherName + ", result: " + textResult);
        }

        private void OnDraw(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int count = packet.ReadByte();
            if (_debug)
                Logger.WriteLine("(" + player.ToString() + " draw " + count.ToString() + " card)");

            for (int i = 0; i < count; ++i)
            {
                int cardId = packet.ReadInt32() & 0x7fffffff;
                TrackDeckMove(cardId, player, (int)CardLocation.Deck, player, (int)CardLocation.Hand);
                int deckIndex = _duel.Fields[player].Deck.Count - 1;
                ClientCard card = _duel.Fields[player].Deck[deckIndex];
                _duel.Fields[player].Deck.RemoveAt(deckIndex);
                _duel.AddCard(CardLocation.Hand, card, player, -1, 0, cardId);
            }
            ValidateBotDeckCount();
            _ai.OnDraw(player);
        }

        private void OnShuffleDeck(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            foreach (ClientCard card in _duel.Fields[player].Deck)
                card.SetId(0);
        }

        private void OnShuffleHand(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            packet.ReadByte();
            foreach (ClientCard card in _duel.Fields[player].Hand)
                card.SetId(packet.ReadInt32());
        }

        private void OnShuffleExtra(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            packet.ReadByte();
            foreach (ClientCard card in _duel.Fields[player].ExtraDeck)
            {
                if (!card.IsFaceup())
                    card.SetId(packet.ReadInt32());
            }
        }

        private void OnShuffleSetCard(BinaryReader packet)
        {
            int location = packet.ReadByte();
            int count = packet.ReadByte();
            ClientCard[] list = new ClientCard[5];
            for (int i = 0; i < count; ++i)
            {
                int player = GetLocalPlayer(packet.ReadByte());
                int loc = packet.ReadByte();
                int seq = packet.ReadByte();
                /*int sseq = */packet.ReadByte();
                ClientCard card = _duel.GetCard(player, (CardLocation)loc, seq);
                if (card == null) continue;
                list[i] = card;
                card.SetId(0);
            }
            for (int i = 0; i < count; ++i)
            {
                int player = GetLocalPlayer(packet.ReadByte());
                int loc = packet.ReadByte();
                int seq = packet.ReadByte();
                /*int sseq = */packet.ReadByte();
                ClientCard card = list[i];
                if (loc == 0 || card == null) continue;
                ClientCard[] zone = (location == (int)CardLocation.MonsterZone) ? _duel.Fields[player].MonsterZone : _duel.Fields[player].SpellZone;
                int previousSequence = card.Sequence;
                ClientCard swappedCard = zone[seq];
                zone[previousSequence] = swappedCard;
                zone[seq] = card;
                card.Sequence = seq;
                if (swappedCard != null)
                    swappedCard.Sequence = previousSequence;
            }
        }

        private void OnSwapGraveDeck(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            ClientField field = _duel.Fields[player];
            IList<ClientCard> oldDeck = field.Deck.ToList();
            IList<ClientCard> oldGraveyard = field.Graveyard.ToList();
            if (player == 0 && _botDeckActive)
                field.ReplaceDeck(oldGraveyard.Where(card => card != null && !card.IsExtraCard()));
            field.Deck.Clear();
            field.Graveyard.Clear();

            int deckSequence = 0;
            int extraSequence = field.ExtraDeck.TakeWhile(card => !card.IsFaceup()).Count();
            foreach (ClientCard card in oldGraveyard)
            {
                card.LastLocation = CardLocation.Grave;
                if (card.IsExtraCard())
                {
                    _duel.AddCard(CardLocation.Extra, card, player, extraSequence++,
                        (int)CardPosition.FaceDownDefence, card.Id);
                }
                else
                {
                    _duel.AddCard(CardLocation.Deck, card, player, deckSequence++,
                        (int)CardPosition.FaceDownDefence, card.Id);
                }
            }

            int graveSequence = 0;
            foreach (ClientCard card in oldDeck)
            {
                card.LastLocation = CardLocation.Deck;
                _duel.AddCard(CardLocation.Grave, card, player, graveSequence++,
                    (int)CardPosition.FaceUp, card.Id);
            }
            ValidateBotDeckCount();
        }

        private void OnReverseDeck(BinaryReader packet)
        {
            // DeckReversed is currently unused by the bot.
            // YGOPro does not refresh deck card sequences on reverse.
            _duel.DeckReversed = !_duel.DeckReversed;
        }

        private void OnTagSwap(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int mcount = packet.ReadByte();
            int ecount = packet.ReadByte();
            int pcount = packet.ReadByte();
            int hcount = packet.ReadByte();
            /*int topcode = */ packet.ReadInt32();
            ClientField field = _duel.Fields[player];

            if (player == 0)
            {
                if (_botDeckActive)
                    ValidateBotDeckCount();
                _botDeckActive = !_botDeckActive;
                field.SetDeckTrackingActive(_botDeckActive);
            }

            field.Deck.Clear();
            for (int i = 0; i < mcount; ++i)
            {
                ClientCard card = new ClientCard(0, CardLocation.Deck, i, (int)CardPosition.FaceDownDefence);
                card.Owner = player;
                card.Controller = player;
                field.Deck.Add(card);
            }

            field.Hand.Clear();
            for (int i = 0; i < hcount; ++i)
            {
                uint encodedCode = packet.ReadUInt32();
                int code = (int)(encodedCode & 0x7fffffff);
                // ocgcore draws an inactive tag partner's opening hand without sending DRAW
                // to that client, so its first visible hand must be deducted here.
                if (player == 0 && _botDeckActive && _botDeckNeedsInitialHandSync)
                    field.RemoveFromDeck(code);
                int position = (encodedCode & 0x80000000) != 0
                    ? (int)CardPosition.FaceUp
                    : (int)CardPosition.FaceDown;
                ClientCard card = new ClientCard(code, CardLocation.Hand, i, position);
                card.Owner = player;
                card.Controller = player;
                field.Hand.Add(card);
            }

            field.ExtraDeck.Clear();
            int faceupSequence = Math.Max(0, ecount - pcount);
            for (int i = 0; i < ecount; ++i)
            {
                uint encodedCode = packet.ReadUInt32();
                int code = (int)(encodedCode & 0x7fffffff);
                bool isFaceup = i >= faceupSequence || (encodedCode & 0x80000000) != 0;
                int position = isFaceup
                    ? (int)CardPosition.FaceUpDefence
                    : (int)CardPosition.FaceDownDefence;
                ClientCard card = new ClientCard(code, CardLocation.Extra, i, position);
                card.Owner = player;
                card.Controller = player;
                field.ExtraDeck.Add(card);
            }
            if (player == 0)
            {
                if (_botDeckActive)
                    _botDeckNeedsInitialHandSync = false;
                ValidateBotDeckCount();
            }
        }

        private void OnNewTurn(BinaryReader packet)
        {
            _duel.Turn++;
            _duel.Player = GetLocalPlayer(packet.ReadByte());
            _ai.OnNewTurn();
        }

        private void OnNewPhase(BinaryReader packet)
        {
            _duel.Phase = (DuelPhase)packet.ReadInt16();
            if (_debug && _duel.Phase == DuelPhase.Standby)
            {
                Logger.WriteLine("*********Bot Hand*********");
                foreach (ClientCard card in _duel.Fields[0].Hand)
                {
                    Logger.WriteLine(card.Name);
                }
                Logger.WriteLine("*********Bot Spell*********");
                foreach (ClientCard card in _duel.Fields[0].SpellZone)
                {
                    Logger.WriteLine(card?.Name);
                }
                Logger.WriteLine("*********Bot Monster*********");
                foreach (ClientCard card in _duel.Fields[0].MonsterZone)
                {
                    Logger.WriteLine(card?.Name);
                }
                Logger.WriteLine("*********Finish*********");
            }
            if (_debug)
                Logger.WriteLine("(Go to " + (_duel.Phase.ToString()) + ")");
            _duel.LastSummonPlayer = -1;
            _duel.SummoningCards.Clear();
            _duel.LastSummonedCards.Clear();
            _duel.Fields[0].BattlingMonster = null;
            _duel.Fields[1].BattlingMonster = null;
            _duel.Fields[0].UnderAttack = false;
            _duel.Fields[1].UnderAttack = false;
            List<ClientCard> monsters = _duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                monster.Attacked = false;
            }
            _select_hint = 0;
            _ai.OnNewPhase();
        }

        private void OnDamage(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int final = _duel.Fields[player].LifePoints - packet.ReadInt32();
            if (final < 0) final = 0;
            if (_debug)
                Logger.WriteLine("(" + player.ToString() + " got damage , LifePoint left = " + final.ToString() + ")");
            _duel.Fields[player].LifePoints = final;
        }

        private void OnRecover(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int final = _duel.Fields[player].LifePoints + packet.ReadInt32();
            if (_debug)
                Logger.WriteLine("(" + player.ToString() + " got healed , LifePoint left = " + final.ToString() + ")");
            _duel.Fields[player].LifePoints = final;
        }

        private void OnLpUpdate(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            _duel.Fields[player].LifePoints = packet.ReadInt32();
        }

        private void OnMove(BinaryReader packet)
        {
            // MSG_MOVE stores an overlay material's index in the position byte and combines
            // CardLocation.Overlay with the host card's zone in the location byte.
            int cardId = packet.ReadInt32();
            int previousControler = GetLocalPlayer(packet.ReadByte());
            int previousLocation = packet.ReadByte();
            int previousSequence = packet.ReadSByte();
            int previousPosition = packet.ReadByte();
            int currentControler = GetLocalPlayer(packet.ReadByte());
            int currentLocation = packet.ReadByte();
            int currentSequence = packet.ReadSByte();
            int currentPosition = packet.ReadByte();
            packet.ReadInt32(); // reason

            // Keep logical locations for AI callbacks. The compound protocol locations are
            // still needed below to find the host card and the material by index.
            int previousMoveLocation = (previousLocation & (int)CardLocation.Overlay) != 0
                ? (int)CardLocation.Overlay
                : previousLocation;
            int currentMoveLocation = (currentLocation & (int)CardLocation.Overlay) != 0
                ? (int)CardLocation.Overlay
                : currentLocation;

            // GetCard uses previousPosition as the material index when previousLocation has
            // the Overlay flag. Overlay materials are otherwise stored only as IDs on their host.
            ClientCard card = _duel.GetCard(previousControler, previousLocation, previousSequence, previousPosition);
            if (card != null)
            {
                card.LastLocation = (CardLocation)previousLocation;
                if (previousLocation != currentLocation)
                {
                    card.ClearEquipRelations();
                    card.ClearCardTargets();
                }
            }
            int trackedCardId = cardId;
            if (trackedCardId == 0 && card != null)
                trackedCardId = card.Id;
            TrackDeckMove(trackedCardId, previousControler, previousLocation, currentControler, currentLocation);
            if ((previousLocation & (int)CardLocation.Overlay) != 0)
            {
                // Detach by index rather than ID because a host may have multiple materials
                // with the same card ID. GetCard reconstructed the material object above.
                int overlayTargetLocation = previousLocation & 0x7f;
                ClientCard overlayTarget = _duel.GetCard(previousControler, (CardLocation)overlayTargetLocation, previousSequence);
                if (overlayTarget != null && previousPosition < overlayTarget.Overlays.Count)
                {
                    if (_debug)
                        Logger.WriteLine("(" + previousControler.ToString() + " 's " + (overlayTarget.Name ?? "UnKnowCard") + " deattach " + (NamedCard.Get(cardId)?.Name) + ")");
                    overlayTarget.Overlays.RemoveAt(previousPosition);
                }
                if (card == null)
                    card = new ClientCard(cardId, CardLocation.Overlay, 0, 0);
                card.LastLocation = CardLocation.Overlay;
                // The reconstructed material is not present in any regular field list. Route it
                // through the same add path as a newly appearing card if it leaves the overlay.
                previousLocation = 0;
            }
            else
                _duel.RemoveCard((CardLocation)previousLocation, card, previousControler, previousSequence);

            if ((currentLocation & (int)CardLocation.Overlay) != 0)
            {
                // WindBot stores attached materials only as IDs on the host, so Duel no longer
                // retains this ClientCard. Normalize it for OnMove and any executor references to it.
                int overlayTargetLocation = currentLocation & 0x7f;
                ClientCard overlayTarget = _duel.GetCard(currentControler, (CardLocation)overlayTargetLocation, currentSequence);
                if (overlayTarget != null)
                {
                    if (_debug)
                        Logger.WriteLine("(" + previousControler.ToString() + " 's " + (overlayTarget.Name ?? "UnKnowCard") + " overlay " + (NamedCard.Get(cardId)?.Name) + ")");
                    overlayTarget.Overlays.Add(cardId);
                }
                if (card != null)
                {
                    card.SetId(cardId);
                    card.Location = CardLocation.Overlay;
                    card.Sequence = currentPosition;
                    card.Position = 0;
                    card.Controller = currentControler;
                }
            }
            else
            {
                if (previousLocation == 0)
                {
                    if (_debug)
                        Logger.WriteLine("(" + previousControler.ToString() + " 's " + (NamedCard.Get(cardId)?.Name)
                        + " appear in " + (CardLocation)currentLocation + ")");
                    // Reuse a reconstructed overlay material so its LastLocation survives the
                    // move. For a genuinely new card, create the client object before adding it.
                    if (card == null)
                        card = new ClientCard(cardId, (CardLocation)currentLocation, currentSequence, currentPosition);
                    _duel.AddCard((CardLocation)currentLocation, card, currentControler, currentSequence, currentPosition, cardId);
                }
                else
                {
                    _duel.AddCard((CardLocation)currentLocation, card, currentControler, currentSequence, currentPosition, cardId);
                    if (card != null && previousLocation != currentLocation)
                        card.IsSpecialSummoned = false;
                    if (_debug && card != null)
                        Logger.WriteLine("(" + previousControler.ToString() + " 's " + (card.Name ?? "UnKnowCard")
                        + " from " +
                        (CardLocation)previousLocation + " move to " + (CardLocation)currentLocation + ")");
                }
            }

            // Report Overlay as the material's location instead of the encoded host zone. This
            // prevents deck executors from treating material attachment as entry to a field zone.
            ValidateBotDeckCount();
            _ai.OnMove(card, previousControler, previousMoveLocation, currentControler, currentMoveLocation);
        }

        private void OnSwap(BinaryReader packet)
        {
            int cardId1 = packet.ReadInt32();
            int controler1 = GetLocalPlayer(packet.ReadByte());
            int location1 = packet.ReadByte();
            int sequence1 = packet.ReadByte();
            packet.ReadByte();
            int cardId2 = packet.ReadInt32();
            int controler2 = GetLocalPlayer(packet.ReadByte());
            int location2 = packet.ReadByte();
            int sequence2 = packet.ReadByte();
            packet.ReadByte();
            ClientCard card1 = _duel.GetCard(controler1, (CardLocation)location1, sequence1);
            ClientCard card2 = _duel.GetCard(controler2, (CardLocation)location2, sequence2);
            if (card1 == null || card2 == null) return;
            int trackedCardId1 = cardId1 != 0 ? cardId1 : card1.Id;
            int trackedCardId2 = cardId2 != 0 ? cardId2 : card2.Id;
            TrackDeckMove(trackedCardId1, controler1, location1, controler2, location2);
            TrackDeckMove(trackedCardId2, controler2, location2, controler1, location1);
            _duel.RemoveCard((CardLocation)location1, card1, controler1, sequence1);
            _duel.RemoveCard((CardLocation)location2, card2, controler2, sequence2);
            _duel.AddCard((CardLocation)location2, card1, controler2, sequence2, card1.Position, cardId1);
            _duel.AddCard((CardLocation)location1, card2, controler1, sequence1, card2.Position, cardId2);
            ValidateBotDeckCount();
        }

        private void OnAttack(BinaryReader packet)
        {
            int ca = GetLocalPlayer(packet.ReadByte());
            int la = packet.ReadByte();
            int sa = packet.ReadByte();
            packet.ReadByte(); //
            int cd = GetLocalPlayer(packet.ReadByte());
            int ld = packet.ReadByte();
            int sd = packet.ReadByte();
            packet.ReadByte(); //

            ClientCard attackcard = _duel.GetCard(ca, (CardLocation)la, sa);
            ClientCard defendcard = _duel.GetCard(cd, (CardLocation)ld, sd);
            if (_debug)
            {
                if (defendcard == null) Logger.WriteLine("(" + (attackcard.Name ?? "UnKnowCard") + " direct attack!!)");
                else Logger.WriteLine("(" + ca.ToString() + " 's " + (attackcard.Name ?? "UnKnowCard") + " attack  " + cd.ToString() + " 's " + (defendcard.Name ?? "UnKnowCard") + ")");
            }                
            _duel.Fields[attackcard.Controller].BattlingMonster = attackcard;
            _duel.Fields[1 - attackcard.Controller].BattlingMonster = defendcard;
            _duel.Fields[1 - attackcard.Controller].UnderAttack = true;

            if (ld == 0 && ca != 0)
            {
                _ai.OnDirectAttack(attackcard);
            }
        }

        private void OnBattle(BinaryReader packet)
        {
            _duel.Fields[0].UnderAttack = false;
            _duel.Fields[1].UnderAttack = false;
        }

        private void OnAttackDisabled(BinaryReader packet)
        {
            _duel.Fields[0].UnderAttack = false;
            _duel.Fields[1].UnderAttack = false;
        }

        private void OnPosChange(BinaryReader packet)
        {
            packet.ReadInt32(); // card id
            int pc = GetLocalPlayer(packet.ReadByte());
            int pl = packet.ReadByte();
            int ps = packet.ReadSByte();
            int pp = packet.ReadSByte();
            int cp = packet.ReadSByte();
            ClientCard card = _duel.GetCard(pc, (CardLocation)pl, ps);
            if (card != null)
            {
                card.Position = cp;
                if ((pp & (int) CardPosition.FaceUp) > 0 && (cp & (int) CardPosition.FaceDown) > 0)
                {
                    card.ClearEquipRelations();
                    card.ClearCardTargets();
                }
                if (_debug)
                    Logger.WriteLine("(" + (card.Name ?? "UnKnowCard") + " change position to " + (CardPosition)cp + ")");
            }
        }

        private void OnChaining(BinaryReader packet)
        {
            int cardId = packet.ReadInt32();
            int pcc = GetLocalPlayer(packet.ReadByte());
            int pcl = packet.ReadByte();
            int pcs = packet.ReadSByte();
            int subs = packet.ReadSByte();
            ClientCard card = _duel.GetCard(pcc, pcl, pcs, subs);
            if (card.Id == 0)
                card.SetId(cardId);
            int cc = GetLocalPlayer(packet.ReadByte());
            packet.ReadInt16(); // trigger location + trigger sequence
            int desc = packet.ReadInt32();
            if (_debug)
                if (card != null) Logger.WriteLine("(" + cc.ToString() + " 's " + (card.Name ?? "UnKnowCard") + " activate effect from " + (CardLocation)pcl + ")");
            _duel.LastChainLocation = (CardLocation)pcl;
            _ai.OnChaining(card, cc);
            //_duel.ChainTargets.Clear();
            _duel.ChainTargetOnly.Clear();
            _duel.LastSummonPlayer = -1;
            _duel.CurrentChain.Add(card);
            _duel.CurrentChainInfo.Add(new ChainInfo(card, cc, desc));
            _duel.LastChainPlayer = cc;

        }

        private void OnChainSolving(BinaryReader packet)
        {
            int chainIndex = packet.ReadByte();
            _duel.SolvingChainIndex = chainIndex;
        }

        private void OnChainNegated(BinaryReader packet)
        {
            int chainIndex = packet.ReadByte();
            _duel.NegatedChainIndexList.Add(chainIndex);
        }

        private void OnChainDisabled(BinaryReader packet)
        {
            int chainIndex = packet.ReadByte();
            _duel.NegatedChainIndexList.Add(chainIndex);
        }

        private void OnChainSolved(BinaryReader packet)
        {
            int chainIndex = packet.ReadByte();
            _ai.OnChainSolved(chainIndex);
        }

        private void OnChainEnd(BinaryReader packet)
        {
            _ai.OnChainEnd();
            _duel.LastChainPlayer = -1;
            _duel.LastChainLocation = 0;
            _duel.CurrentChain.Clear();
            _duel.CurrentChainInfo.Clear();
            _duel.ChainTargets.Clear();
            _duel.LastChainTargets.Clear();
            _duel.ChainTargetOnly.Clear();
            _duel.SolvingChainIndex = 0;
            _duel.NegatedChainIndexList.Clear();
            _duel.SummoningCards.Clear();
        }

        private void OnCardSorting(BinaryReader packet)
        {
            /*int player =*/ GetLocalPlayer(packet.ReadByte());
            IList<ClientCard> originalCards = new List<ClientCard>();
            IList<ClientCard> cards = new List<ClientCard>();
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                int id = packet.ReadInt32();
                int controler = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                ClientCard card;
                if (((int)loc & (int)CardLocation.Overlay) != 0)
                    card = new ClientCard(id, CardLocation.Overlay, -1);
                else
                    card = _duel.GetCard(controler, loc, seq);
                if (card == null) continue;
                if (id != 0)
                    card.SetId(id);
                originalCards.Add(card);
                cards.Add(card);
            }

            IList<ClientCard> selected = _ai.OnCardSorting(cards);
            byte[] result = new byte[count];
            for (int i = 0; i < count; ++i)
            {
                int id = 0;
                for (int j = 0; j < count; ++j)
                {
                    if (selected[j] == null) continue;
                    if (selected[j].Equals(originalCards[i]))
                    {
                        id = j;
                        break;
                    }
                }
                result[i] = (byte)id;
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
        }

        private void OnChainSorting(BinaryReader packet)
        {
            /*BinaryWriter writer =*/ GamePacketFactory.Create(CtosMessage.Response);
            Connection.Send(CtosMessage.Response, -1);
        }

        private void OnUpdateCard(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int loc = packet.ReadByte();
            int seq = packet.ReadByte();

            packet.ReadInt32(); // ???

            ClientCard card = _duel.GetCard(player, (CardLocation)loc, seq);

            card?.Update(packet, _duel);
        }

        private void OnUpdateData(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            CardLocation loc = (CardLocation)packet.ReadByte();
            IList<ClientCard> cards = null;
            switch (loc)
            {
                case CardLocation.Hand:
                    cards = _duel.Fields[player].Hand;
                    break;
                case CardLocation.MonsterZone:
                    cards = _duel.Fields[player].MonsterZone;
                    break;
                case CardLocation.SpellZone:
                    cards = _duel.Fields[player].SpellZone;
                    break;
                case CardLocation.Grave:
                    cards = _duel.Fields[player].Graveyard;
                    break;
                case CardLocation.Removed:
                    cards = _duel.Fields[player].Banished;
                    break;
                case CardLocation.Deck:
                    cards = _duel.Fields[player].Deck;
                    break;
                case CardLocation.Extra:
                    cards = _duel.Fields[player].ExtraDeck;
                    break;
            }
            if (cards != null)
            {
                foreach (ClientCard card in cards)
                {
                    int len = packet.ReadInt32();
                    long pos = packet.BaseStream.Position;
                    if (len > 8)
                      card.Update(packet, _duel);
                    packet.BaseStream.Position = pos + len - 4;
                }
            }
        }

        private void OnBecomeTarget(BinaryReader packet)
        {
            _duel.LastChainTargets.Clear();
            int currentChainIndex = _duel.SolvingChainIndex > 0
                ? _duel.SolvingChainIndex - 1 // record MSG_BECOME_TARGET during chain solving too 
                : _duel.CurrentChainInfo.Count - 1;
            ChainInfo currentChainInfo = currentChainIndex >= 0 && currentChainIndex < _duel.CurrentChainInfo.Count
                ? _duel.CurrentChainInfo[currentChainIndex]
                : null;
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                int player = GetLocalPlayer(packet.ReadByte());
                int loc = packet.ReadByte();
                int seq = packet.ReadByte();
                /*int sseq = */packet.ReadByte();
                ClientCard card = _duel.GetCard(player, (CardLocation)loc, seq);
                if (card == null) continue;
                if (_debug)
                    Logger.WriteLine("(" + (CardLocation)loc + " 's " + (card.Name ?? "UnKnowCard") + " become target)");
                _duel.ChainTargets.Add(card);
                _duel.LastChainTargets.Add(card);
                _duel.ChainTargetOnly.Add(card);
                if (currentChainInfo != null && !currentChainInfo.Targets.Contains(card))
                    currentChainInfo.Targets.Add(card);
            }
        }

        private void OnSelectBattleCmd(BinaryReader packet)
        {
            packet.ReadByte(); // player
            _duel.BattlePhase = new BattlePhase();
            BattlePhase battle = _duel.BattlePhase;

            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                int desc = packet.ReadInt32();

                ClientCard card = _duel.GetCard(con, loc, seq);
                if (card != null)
                {
                    card.ActionIndex[0] = i;
                    battle.ActivableCards.Add(card);
                    battle.ActivableDescs.Add(desc);
                }
            }

            count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                int diratt = packet.ReadByte();

                ClientCard card = _duel.GetCard(con, loc, seq);
                if (card != null)
                {
                    card.ActionIndex[1] = i;
                    if (diratt > 0)
                        card.CanDirectAttack = true;
                    else
                        card.CanDirectAttack = false;
                    battle.AttackableCards.Add(card);
                    card.Attacked = false;
                }
            }
            List<ClientCard> monsters = _duel.Fields[0].GetMonsters();
            foreach (ClientCard monster in monsters)
            {
                if (!battle.AttackableCards.Contains(monster))
                    monster.Attacked = true;
            }

            battle.CanMainPhaseTwo = packet.ReadByte() != 0;
            battle.CanEndPhase = packet.ReadByte() != 0;

            Connection.Send(CtosMessage.Response, _ai.OnSelectBattleCmd(battle).ToValue());
        }

        private void InternalOnSelectCard(BinaryReader packet,
            Func<IList<ClientCard>, int, int, int, bool, IList<ClientCard>> func, bool isTribute = false)
        {
            packet.ReadByte(); // player
            bool cancelable = packet.ReadByte() != 0;
            int min = packet.ReadByte();
            int max = packet.ReadByte();

            IList<ClientCard> cards = new List<ClientCard>();
            IList<int> candidateIndexes = new List<int>();
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                int id = packet.ReadInt32();
                int player = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                int param = packet.ReadByte();
                ClientCard card;
                if (((int)loc & (int)CardLocation.Overlay) != 0)
                {
                    card = new ClientCard(id, CardLocation.Overlay, -1);
                    CardLocation ownerLoc = loc ^ CardLocation.Overlay;
                    ClientCard ownerCard = _duel.GetCard(player, ownerLoc, seq);
                    if (ownerCard != null)
                        card.OwnTargets.Add(ownerCard);
                }
                else
                {
                    card = _duel.GetCard(player, loc, seq);
                    if (card == null)
                        card = new ClientCard(id, loc, seq);
                }
                card.Controller = player;
                if (card.Id == 0 || card.Location == CardLocation.Deck)
                    card.SetId(id);
                if (isTribute)
                {
                    card.OpParam1 = 1;
                    card.OpParam2 = param;
                }
                cards.Add(card);
                candidateIndexes.Add(i);
            }

            if (_select_hint == 575 && cancelable) // HINTMSG_FIELD_FIRST
            {
                _select_hint = 0;
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            IList<ClientCard> selected = func(cards, min, max, _select_hint, cancelable);
            _select_hint = 0;

            SendCardSelectionResponse(cards, candidateIndexes, selected, min, max, cancelable, isTribute);
        }

        private void SendCardSelectionResponse(IList<ClientCard> cards, IList<int> candidateIndexes,
            IList<ClientCard> selected, int min, int max, bool cancelable, bool isTribute = false)
        {
            bool validCount = selected != null
                && (isTribute ? _ai.IsValidTributeSelection(selected, min, max) : selected.Count >= min && selected.Count <= max);
            if (selected != null && cancelable && selected.Count == 0)
                validCount = true;

            bool isValid = validCount
                && selected.Distinct().Count() == selected.Count
                && selected.All(card => card != null && cards.Contains(card));
            if (!isValid)
            {
                Logger.WriteErrorLine("Invalid card selection, using a legal fallback.");
                IList<ClientCard> orderedCards = new List<ClientCard>();
                if (selected != null)
                {
                    foreach (ClientCard card in selected)
                    {
                        if (card != null && cards.Contains(card) && !orderedCards.Contains(card))
                            orderedCards.Add(card);
                    }
                }
                foreach (ClientCard card in cards)
                {
                    if (!orderedCards.Contains(card))
                        orderedCards.Add(card);
                }

                if (isTribute)
                {
                    selected = _ai.FindTributeSelection(orderedCards, min, max) ?? new List<ClientCard>();
                }
                else
                {
                    IList<ClientCard> fallback = new List<ClientCard>();
                    foreach (ClientCard card in orderedCards)
                    {
                        if (fallback.Count >= min || fallback.Count >= max)
                            break;
                        fallback.Add(card);
                    }
                    selected = fallback;
                }
            }

            if (selected.Count == 0 && cancelable)
            {
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            byte[] result = new byte[selected.Count + 1];
            result[0] = (byte)selected.Count;
            for (int i = 0; i < selected.Count; ++i)
            {
                int cardIndex = cards.IndexOf(selected[i]);
                result[i + 1] = (byte)candidateIndexes[cardIndex];
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
        }

        private void OnSelectUnselectCard(BinaryReader packet)
        {
            packet.ReadByte(); // player
            bool finishable = packet.ReadByte() != 0;
            bool cancelable = packet.ReadByte() != 0 || finishable;
            packet.ReadByte(); // min, display only
            packet.ReadByte(); // max, display only

            IList<ClientCard> cards = new List<ClientCard>();
            IList<int> candidateIndexes = new List<int>();
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                int id = packet.ReadInt32();
                int player = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                packet.ReadByte(); // pos
                ClientCard card;
                if (((int)loc & (int)CardLocation.Overlay) != 0)
                    card = new ClientCard(id, CardLocation.Overlay, -1);
                else
                {
                    card = _duel.GetCard(player, loc, seq);
                    if (card == null)
                        card = new ClientCard(id, loc, seq);
                }
                card.Controller = player;
                if (card.Id == 0 || card.Location == CardLocation.Deck)
                    card.SetId(id);
                cards.Add(card);
                candidateIndexes.Add(i);
            }
            int count2 = packet.ReadByte();
            // The second group contains cards that an interactive client may click to undo
            // an earlier selection. The bot only advances through the still-valid first group
            // and does not backtrack here, so these cards are consumed but not exposed to the AI.
            for (int i = 0; i < count2; ++i)
            {
                int id = packet.ReadInt32();
                int player = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                packet.ReadByte(); // pos
                ClientCard card;
                if (((int)loc & (int)CardLocation.Overlay) != 0)
                    card = new ClientCard(id, CardLocation.Overlay, -1);
                else
                    card = _duel.GetCard(player, loc, seq);
                if (card == null) continue;
                if (card.Id == 0 || card.Location == CardLocation.Deck)
                    card.SetId(id);
            }
            // Protocol cancellation abandons the entire selection rather than undoing one card.
            // WindBot never intentionally takes that path; this flag is only useful to the bot
            // as the shared finish response after a card has already been selected.
            if (count2 == 0) cancelable = false;

            // Unlike OnSelectCard, we don't reset _select_hint here.
            // Lua helpers such as SelectSubGroup use this hint message repeatedly for one selection.

            int selectionMin = finishable ? 0 : 1;
            IList<ClientCard> selected = _ai.OnSelectCard(cards, selectionMin, 1, _select_hint, cancelable);
            SendCardSelectionResponse(cards, candidateIndexes, selected, selectionMin, 1, cancelable);
        }

        private void OnSelectCard(BinaryReader packet)
        {
            InternalOnSelectCard(packet, _ai.OnSelectCard);
        }

        private void OnSelectChain(BinaryReader packet)
        {
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            packet.ReadByte(); // specount
            int hint1 = packet.ReadInt32(); // hint1
            int hint2 = packet.ReadInt32(); // hint2

            // TODO: use ChainInfo?
            IList<ClientCard> cards = new List<ClientCard>();
            IList<int> descs = new List<int>();
            IList<bool> forces = new List<bool>();

            for (int i = 0; i < count; ++i)
            {
                packet.ReadByte(); // flag
                bool forced = packet.ReadByte() != 0;

                int id = packet.ReadInt32();
                int con = GetLocalPlayer(packet.ReadByte());
                int loc = packet.ReadByte();
                int seq = packet.ReadByte();
                int sseq = packet.ReadByte();

                int desc = packet.ReadInt32();
                if (desc == 221) // trigger effect
                {
                    desc = 0;
                }

                ClientCard card = _duel.GetCard(con, loc, seq, sseq);
                if (card.Id == 0)
                    card.SetId(id);

                cards.Add(card);
                descs.Add(desc);
                forces.Add(forced);
            }

            if (cards.Count == 0)
            {
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            if (cards.Count == 1 && forces[0])
            {
                Connection.Send(CtosMessage.Response, 0);
                return;
            }

            Connection.Send(CtosMessage.Response, _ai.OnSelectChain(cards, descs, forces, hint1 | hint2));
        }

        private void OnSelectCounter(BinaryReader packet)
        {
            packet.ReadByte(); // player
            int type = packet.ReadInt16();
            int quantity = packet.ReadInt16();

            IList<ClientCard> cards = new List<ClientCard>();
            IList<int> counters = new List<int>();
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int player = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation) packet.ReadByte();
                int seq = packet.ReadByte();
                int num = packet.ReadInt16();
                cards.Add(_duel.GetCard(player, loc, seq));
                counters.Add(num);
            }

            IList<int> used = _ai.OnSelectCounter(type, quantity, cards, counters);
            byte[] result = new byte[used.Count * 2];
            for (int i = 0; i < used.Count; ++i)
            {
                result[i * 2] = (byte)(used[i] & 0xff);
                result[i * 2 + 1] = (byte)(used[i] >> 8);
            }
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
        }

        private void OnSelectEffectYn(BinaryReader packet)
        {
            packet.ReadByte(); // player

            int cardId = packet.ReadInt32();
            int player = GetLocalPlayer(packet.ReadByte());
            CardLocation loc = (CardLocation)packet.ReadByte();
            int seq = packet.ReadByte();
            packet.ReadByte();
            int desc = packet.ReadInt32();

            if (desc == 0 || desc == 221)
            {
                // 0: phase trigger effect
                // 221: trigger effect
                // for compatibility
                desc = -1;
            }

            ClientCard card = _duel.GetCard(player, loc, seq);
            if (card == null)
            {
                Connection.Send(CtosMessage.Response, 0);
                return;
            }
            
            if (card.Id == 0)
                card.SetId(cardId);

            int reply = _ai.OnSelectEffectYn(card, desc) ? (1) : (0);
            Connection.Send(CtosMessage.Response, reply);
        }

        private void OnSelectIdleCmd(BinaryReader packet)
        {
            packet.ReadByte(); // player

            _duel.MainPhase = new MainPhase();
            MainPhase main = _duel.MainPhase;
            int count;
            for (int k = 0; k < 5; k++)
            {
                count = packet.ReadByte();
                for (int i = 0; i < count; ++i)
                {
                    packet.ReadInt32(); // card id
                    int con = GetLocalPlayer(packet.ReadByte());
                    CardLocation loc = (CardLocation)packet.ReadByte();
                    int seq = packet.ReadByte();
                    ClientCard card = _duel.GetCard(con, loc, seq);
                    if (card == null) continue;
                    card.ActionIndex[k] = i;
                    switch (k)
                    {
                        case 0:
                            main.SummonableCards.Add(card);
                            break;
                        case 1:
                            main.SpecialSummonableCards.Add(card);
                            break;
                        case 2:
                            main.ReposableCards.Add(card);
                            break;
                        case 3:
                            main.MonsterSetableCards.Add(card);
                            break;
                        case 4:
                            main.SpellSetableCards.Add(card);
                            break;
                    }
                }
            }
            count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte());
                CardLocation loc = (CardLocation)packet.ReadByte();
                int seq = packet.ReadByte();
                int desc = packet.ReadInt32();

                ClientCard card = _duel.GetCard(con, loc, seq);
                if (card == null) continue;
                card.ActionIndex[5] = i;
                if (card.ActionActivateIndex.ContainsKey(desc))
                    card.ActionActivateIndex.Remove(desc);
                card.ActionActivateIndex.Add(desc, i);
                main.ActivableCards.Add(card);
                main.ActivableDescs.Add(desc);
            }

            main.CanBattlePhase = packet.ReadByte() != 0;
            main.CanEndPhase = packet.ReadByte() != 0;
            packet.ReadByte(); // CanShuffle

            Connection.Send(CtosMessage.Response, _ai.OnSelectIdleCmd(main).ToValue());
        }

        private void OnSelectOption(BinaryReader packet)
        {
            IList<int> options = new List<int>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
                options.Add(packet.ReadInt32());
            Connection.Send(CtosMessage.Response, _ai.OnSelectOption(options));
        }

        private void InternalOnSelectPlace(BinaryReader packet, Func<int, int, uint, uint> func)
        {
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            uint field = ~packet.ReadUInt32();

            uint selected = func(_select_hint, count, field);
            _select_hint = 0;

            byte[] resp = new byte[Math.Max(1, count) * 3];
            int responseIndex = 0;
            for (int zone = 0; zone < 32 && responseIndex < resp.Length; ++zone)
            {
                if ((selected & (1u << zone)) == 0)
                    continue;

                int sequence = zone & 0xf;
                if (sequence == 7)
                    continue;

                int player = zone >> 4;
                resp[responseIndex++] = (byte)GetLocalPlayer(player);
                if (sequence < 7)
                {
                    resp[responseIndex++] = (byte)CardLocation.MonsterZone;
                    resp[responseIndex++] = (byte)sequence;
                }
                else
                {
                    resp[responseIndex++] = (byte)CardLocation.SpellZone;
                    resp[responseIndex++] = (byte)(sequence - 8);
                }
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(resp);
            Connection.Send(reply);
        }

        private void OnSelectDisfield(BinaryReader packet)
        {
            InternalOnSelectPlace(packet, _ai.OnSelectDisfield);
        }

        private void OnSelectPlace(BinaryReader packet)
        {
            InternalOnSelectPlace(packet, _ai.OnSelectPlace);
        }

        private void OnSelectPosition(BinaryReader packet)
        {
            packet.ReadByte(); // player
            int cardId = packet.ReadInt32();
            int pos = packet.ReadByte();
            if (pos == 0x1 || pos == 0x2 || pos == 0x4 || pos == 0x8)
            {
                Connection.Send(CtosMessage.Response, pos);
                return;
            }
            IList<CardPosition> positions = new List<CardPosition>();
            if ((pos & (int)CardPosition.FaceUpAttack) != 0)
                positions.Add(CardPosition.FaceUpAttack);
            if ((pos & (int)CardPosition.FaceDownAttack) != 0)
                positions.Add(CardPosition.FaceDownAttack);
            if ((pos & (int)CardPosition.FaceUpDefence) != 0)
                positions.Add(CardPosition.FaceUpDefence);
            if ((pos & (int)CardPosition.FaceDownDefence) != 0)
                positions.Add(CardPosition.FaceDownDefence);
            Connection.Send(CtosMessage.Response, (int)_ai.OnSelectPosition(cardId, positions));
        }

        private void OnSelectSum(BinaryReader packet)
        {
            bool mode = packet.ReadByte() == 0;
            packet.ReadByte(); // player
            int sumval = packet.ReadInt32();
            int min = packet.ReadByte();
            int max = packet.ReadByte();

            if (max <= 0)
                max = 99;
            
            IList<ClientCard> mandatoryCards = new List<ClientCard>();
            IList<ClientCard> cards = new List<ClientCard>();

            for (int j = 0; j < 2; ++j)
            {
                int count = packet.ReadByte();
                for (int i = 0; i < count; ++i)
                {
                    int cardId = packet.ReadInt32();
                    int player = GetLocalPlayer(packet.ReadByte());
                    CardLocation loc = (CardLocation)packet.ReadByte();
                    int seq = packet.ReadByte();
                    ClientCard card;
                    if (((int)loc & (int)CardLocation.Overlay) != 0)
                    {
                        card = new ClientCard(cardId, CardLocation.Overlay, -1);
                    }
                    else
                    {
                        card = _duel.GetCard(player, loc, seq);
                        if (card == null)
                            card = new ClientCard(cardId, loc, seq);
                    }
                    card.Controller = player;
                    if (cardId != 0 && card.Id != cardId)
                        card.SetId(cardId);
                    card.SelectSeq = i;
                    uint opParam = packet.ReadUInt32();
                    int opParam1 = (int)(opParam & 0xffff);
                    int opParam2 = (int)((opParam >> 16) & 0xffff);
                    if ((opParam2 & 0x8000) != 0)
                    {
                        opParam1 = (int)(opParam & 0x7fffffff);
                        opParam2 = 0;
                    }
                    if (opParam1 == 0)
                    {
                        Logger.WriteErrorLine("Unexpected select sum parameter for card " + cardId
                            + ": OpParam1 is 0 (raw opParam = 0x" + opParam.ToString("X8") + ").");
                    }
                    if (opParam2 > 0 && opParam1 > opParam2)
                    {
                        card.OpParam1 = opParam2;
                        card.OpParam2 = opParam1;
                    }
                    else
                    {
                        card.OpParam1 = opParam1;
                        card.OpParam2 = opParam2;
                    }
                    if (j == 0)
                        mandatoryCards.Add(card);
                    else
                        cards.Add(card);
                }
            }

            IList<ClientCard> selected = _ai.OnSelectSum(cards, mandatoryCards, sumval, min, max, _select_hint, mode);
            _select_hint = 0;

            byte[] result = new byte[mandatoryCards.Count + selected.Count + 1];
            int index = 0;

            result[index++] = (byte)(mandatoryCards.Count + selected.Count);
            while (index <= mandatoryCards.Count)
            {
                result[index++] = 0;
            }
            int l = 0;
            while (l < selected.Count)
            {
                result[index++] = (byte)selected[l].SelectSeq;
                ++l;
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
        }

        private void OnSelectTribute(BinaryReader packet)
        {
            InternalOnSelectCard(packet, _ai.OnSelectTribute, true);
        }

        private void OnSelectYesNo(BinaryReader packet)
        {
            packet.ReadByte(); // player
            int desc = packet.ReadInt32();
            int reply;
            if (desc == 30)
                reply = _ai.OnSelectBattleReplay() ? 1 : 0;
            else
                reply = _ai.OnSelectYesNo(desc) ? 1 : 0;
            Connection.Send(CtosMessage.Response, reply);
        }

        private void OnAnnounceAttrib(BinaryReader packet)
        {
            IList<CardAttribute> attributes = new List<CardAttribute>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            int available = packet.ReadInt32();
            int filter = 0x1;
            for (int i = 0; i < 7; ++i)
            {
                if ((available & filter) != 0)
                    attributes.Add((CardAttribute) filter);
                filter <<= 1;
            }
            attributes = _ai.OnAnnounceAttrib(count, attributes);
            int reply = 0;
            for (int i = 0; i < count; ++i)
                reply += (int)attributes[i];
            Connection.Send(CtosMessage.Response, reply);
        }

        private void OnAnnounceCard(BinaryReader packet)
        {
            IList<int> opcodes = new List<int>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
                opcodes.Add(packet.ReadInt32());

            IList<int> avail = new List<int>();
            IList<NamedCard> all = NamedCardsManager.GetAllCards();
            foreach (NamedCard card in all)
            {
                if (card.HasType(CardType.Token) || NamedCard.IsAltartAlias(card.Id, card.Alias)) continue;
                Stack<int> stack = new Stack<int>();
                for (int i = 0; i < opcodes.Count; i++)
                {
                    switch (opcodes[i])
                    {
                        case Opcodes.OPCODE_ADD:
                            if (stack.Count >= 2)
                            {
                                int rhs = stack.Pop();
                                int lhs = stack.Pop();
                                stack.Push(lhs + rhs);
                            }
                            break;
                        case Opcodes.OPCODE_SUB:
                            if (stack.Count >= 2)
                            {
                                int rhs = stack.Pop();
                                int lhs = stack.Pop();
                                stack.Push(lhs - rhs);
                            }
                            break;
                        case Opcodes.OPCODE_MUL:
                            if (stack.Count >= 2)
                            {
                                int rhs = stack.Pop();
                                int lhs = stack.Pop();
                                stack.Push(lhs * rhs);
                            }
                            break;
                        case Opcodes.OPCODE_DIV:
                            if (stack.Count >= 2)
                            {
                                int rhs = stack.Pop();
                                int lhs = stack.Pop();
                                stack.Push(lhs / rhs);
                            }
                            break;
                        case Opcodes.OPCODE_AND:
                            if (stack.Count >= 2)
                            {
                                int rhs = stack.Pop();
                                int lhs = stack.Pop();
                                bool b0 = rhs != 0;
                                bool b1 = lhs != 0;
                                if (b0 && b1)
                                    stack.Push(1);
                                else
                                    stack.Push(0);
                            }
                            break;
                        case Opcodes.OPCODE_OR:
                            if (stack.Count >= 2)
                            {
                                int rhs = stack.Pop();
                                int lhs = stack.Pop();
                                bool b0 = rhs != 0;
                                bool b1 = lhs != 0;
                                if (b0 || b1)
                                    stack.Push(1);
                                else
                                    stack.Push(0);
                            }
                            break;
                        case Opcodes.OPCODE_NEG:
                            if (stack.Count >= 1)
                            {
                                int rhs = stack.Pop();
                                stack.Push(-rhs);
                            }
                            break;
                        case Opcodes.OPCODE_NOT:
                            if (stack.Count >= 1)
                            {
                                int rhs = stack.Pop();
                                bool b0 = rhs != 0;
                                if (b0)
                                    stack.Push(0);
                                else
                                    stack.Push(1);
                            }
                            break;
                        case Opcodes.OPCODE_ISCODE:
                            if (stack.Count >= 1)
                            {
                                int code = stack.Pop();
                                bool b0 = code == card.Id;
                                if (b0)
                                    stack.Push(1);
                                else
                                    stack.Push(0);
                            }
                            break;
                        case Opcodes.OPCODE_ISSETCARD:
                            if (stack.Count >= 1)
                            {
                                if (card.HasSetcode(stack.Pop()))
                                    stack.Push(1);
                                else
                                    stack.Push(0);
                            }
                            break;
                        case Opcodes.OPCODE_ISTYPE:
                            if (stack.Count >= 1)
                            {
                                if ((stack.Pop() & card.Type) > 0)
                                    stack.Push(1);
                                else
                                    stack.Push(0);
                            }
                            break;
                        case Opcodes.OPCODE_ISRACE:
                            if (stack.Count >= 1)
                            {
                                if ((stack.Pop() & card.Race) > 0)
                                    stack.Push(1);
                                else
                                    stack.Push(0);
                            }
                            break;
                        case Opcodes.OPCODE_ISATTRIBUTE:
                            if (stack.Count >= 1)
                            {
                                if ((stack.Pop() & card.Attribute) > 0)
                                    stack.Push(1);
                                else
                                    stack.Push(0);
                            }
                            break;
                        default:
                            stack.Push(opcodes[i]);
                            break;
                    }
                }
                if (stack.Count == 1 && stack.Pop() != 0)
                    avail.Add(card.Id);
            }
            if (avail.Count == 0)
                throw new Exception("No avail card found for announce!");
            Connection.Send(CtosMessage.Response, _ai.OnAnnounceCard(avail));
        }

        private void OnAnnounceNumber(BinaryReader packet)
        {
            IList<int> numbers = new List<int>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
                numbers.Add(packet.ReadInt32());
            Connection.Send(CtosMessage.Response, _ai.OnAnnounceNumber(numbers));
        }

        private void OnAnnounceRace(BinaryReader packet)
        {
            IList<CardRace> races = new List<CardRace>();
            packet.ReadByte(); // player
            int count = packet.ReadByte();
            int available = packet.ReadInt32();
            int filter = 0x1;
            for (int i = 0; i < 26; ++i)
            {
                if ((available & filter) != 0)
                    races.Add((CardRace)filter);
                filter <<= 1;
            }
            races = _ai.OnAnnounceRace(count, races);
            int reply = 0;
            for (int i = 0; i < count; ++i)
                reply += (int)races[i];
            Connection.Send(CtosMessage.Response, reply);
        }

        private void OnRockPaperScissors(BinaryReader packet)
        {
            packet.ReadByte(); // player
            int result;
            if (_hand > 0)
                result = _hand;
            else
                result = _ai.OnRockPaperScissors();
            Connection.Send(CtosMessage.Response, result);
        }

        private void OnEquip(BinaryReader packet)
        {
            int equipCardControler = GetLocalPlayer(packet.ReadByte());
            int equipCardLocation = packet.ReadByte();
            int equipCardSequence = packet.ReadSByte();
            packet.ReadByte();
            int targetCardControler = GetLocalPlayer(packet.ReadByte());
            int targetCardLocation = packet.ReadByte();
            int targetCardSequence = packet.ReadSByte();
            packet.ReadByte();
            ClientCard equipCard = _duel.GetCard(equipCardControler, (CardLocation)equipCardLocation, equipCardSequence);
            ClientCard targetCard = _duel.GetCard(targetCardControler, (CardLocation)targetCardLocation, targetCardSequence);
            if (equipCard == null || targetCard == null) return;
            equipCard.SetEquipTarget(targetCard);
        }

        private void OnUnEquip(BinaryReader packet)
        {
            int equipCardControler = GetLocalPlayer(packet.ReadByte());
            int equipCardLocation = packet.ReadByte();
            int equipCardSequence = packet.ReadSByte();
            packet.ReadByte();
            ClientCard equipCard = _duel.GetCard(equipCardControler, (CardLocation)equipCardLocation, equipCardSequence);
            if (equipCard == null) return;
            equipCard.SetEquipTarget(null);
        }

        private void OnCardTarget(BinaryReader packet)
        {
            int ownerCardControler = GetLocalPlayer(packet.ReadByte());
            int ownerCardLocation = packet.ReadByte();
            int ownerCardSequence = packet.ReadSByte();
            packet.ReadByte();
            int targetCardControler = GetLocalPlayer(packet.ReadByte());
            int targetCardLocation = packet.ReadByte();
            int targetCardSequence = packet.ReadSByte();
            packet.ReadByte();
            ClientCard ownerCard = _duel.GetCard(ownerCardControler, (CardLocation)ownerCardLocation, ownerCardSequence);
            ClientCard targetCard = _duel.GetCard(targetCardControler, (CardLocation)targetCardLocation, targetCardSequence);
            if (ownerCard == null || targetCard == null) return;
            ownerCard.AddCardTarget(targetCard);
        }

        private void OnCancelTarget(BinaryReader packet)
        {
            int ownerCardControler = GetLocalPlayer(packet.ReadByte());
            int ownerCardLocation = packet.ReadByte();
            int ownerCardSequence = packet.ReadSByte();
            packet.ReadByte();
            int targetCardControler = GetLocalPlayer(packet.ReadByte());
            int targetCardLocation = packet.ReadByte();
            int targetCardSequence = packet.ReadSByte();
            packet.ReadByte();
            ClientCard ownerCard = _duel.GetCard(ownerCardControler, (CardLocation)ownerCardLocation, ownerCardSequence);
            ClientCard targetCard = _duel.GetCard(targetCardControler, (CardLocation)targetCardLocation, targetCardSequence);
            if (ownerCard == null || targetCard == null) return;
            ownerCard.RemoveCardTarget(targetCard);
        }

        private void OnSummoning(BinaryReader packet)
        {
            InternalOnSummoning(packet);
            _ai.OnSummoning();
        }

        private void OnFlipSummoning(BinaryReader packet)
        {
            InternalOnSummoning(packet);
        }

        private void InternalOnSummoning(BinaryReader packet)
        {
            _duel.LastSummonedCards.Clear();
            int code = packet.ReadInt32();
            int currentControler = GetLocalPlayer(packet.ReadByte());
            int currentLocation = packet.ReadByte();
            int currentSequence = packet.ReadSByte();
            int currentPosition = packet.ReadSByte();
            ClientCard card = _duel.GetCard(currentControler, (CardLocation)currentLocation, currentSequence);
            _duel.SummoningCards.Add(card);
            _duel.LastSummonPlayer = currentControler;
        }

        private void OnSummoned(BinaryReader packet)
        {
            foreach (ClientCard card in _duel.SummoningCards)
            {
                _duel.LastSummonedCards.Add(card);
            }
            _duel.SummoningCards.Clear();
        }

        private void OnSpSummoning(BinaryReader packet)
        {
            _duel.LastSummonedCards.Clear();
            // Material selection may span multiple SelectUnselect messages; summon start is the first reliable
            // signal that the complete material selection is done and the AI can clean up its internal state.
            _ai.CleanSelectMaterials();
            int code = packet.ReadInt32();
            int currentControler = GetLocalPlayer(packet.ReadByte());
            int currentLocation = packet.ReadByte();
            int currentSequence = packet.ReadSByte();
            int currentPosition = packet.ReadSByte();
            ClientCard card = _duel.GetCard(currentControler, (CardLocation)currentLocation, currentSequence);
            _duel.SummoningCards.Add(card);
            _duel.LastSummonPlayer = currentControler;
        }

        private void OnSpSummoned(BinaryReader packet)
        {
            foreach (ClientCard card in _duel.SummoningCards)
            {
                card.IsSpecialSummoned = true;
                _duel.LastSummonedCards.Add(card);
            }
            _ai.OnSpSummoned();
            _duel.SummoningCards.Clear();
        }

        private void OnConfirmCards(BinaryReader packet)
        {
            /*int playerid = */packet.ReadByte();
            /*int skip_panel = */packet.ReadByte();
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++ i)
            {
                int cardId = packet.ReadInt32();
                int player = GetLocalPlayer(packet.ReadByte());
                int loc = packet.ReadByte();
                int seq = packet.ReadByte();
                ClientCard card = _duel.GetCard(player, (CardLocation)loc, seq);
                if (cardId > 0) card.SetId(cardId);
                if (_debug)
                    Logger.WriteLine("(Confirm " + player.ToString() + "'s " + (CardLocation)loc + " card: " + (card.Name ?? "UnKnowCard") + ")");
            }
        }

        /// <summary>
        /// Handles PlayerHint message. Protocol: player(buffer8), hintType(buffer8), description(buffer32).
        /// hintType values: PlayerHintType (e.g. PHINT_DESC_ADD=6, PHINT_DESC_REMOVE=7).
        /// </summary>
        private void OnPlayerHint(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int hintType = packet.ReadByte();
            int description = packet.ReadInt32();
            Logger.DebugWriteLine("PlayerHint received: player=" + player + ", hintType=" + hintType + " (" + (PlayerHintType)hintType + "), description=" + description);
            _ai.OnPlayerHint(player, hintType, description);
        }
    }
}
