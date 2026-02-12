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
            Deck = Deck.Load(Game.DeckFile ?? _ai.Executor.Deck);

            _select_hint = 0;
        }

        public int GetLocalPlayer(int player)
        {
            return _duel.IsFirst ? player : 1 - player;
        }

        public void OnPacket(BinaryReader packet)
        {
            StocMessage id = (StocMessage)packet.ReadByte();
            if (id == StocMessage.GameMsg)
            {
                GameMessage msg = (GameMessage)packet.ReadByte();
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
            _messages.Add(GameMessage.FlipSummoning, OnSummoning);
            _messages.Add(GameMessage.FlipSummoned, OnSummoned);
            _messages.Add(GameMessage.ConfirmCards, OnConfirmCards);
        }

        private void OnJoinGame(BinaryReader packet)
        {
            /*int lflist = (int)*/ packet.ReadUInt32();
            /*int rule = */ packet.ReadByte();
            /*int mode = */ packet.ReadByte();
            int duel_rule = packet.ReadByte();
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

            if (_room.IsHost && _room.IsReady[0] && _room.IsReady[1])
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
            int player = packet.ReadInt16();
            string message = packet.ReadUnicode(256);
            string myName = (player != 0) ? _room.Names[1] : _room.Names[0];
            string otherName = (player == 0) ? _room.Names[1] : _room.Names[0];
            if (player < 4)
                Logger.DebugWriteLine(otherName + " say to " + myName + ": " + message);
            else
                Logger.DebugWriteLine("System message(" + player + "): " + message);
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
            throw new Exception("Got MSG_RETRY. Last message is " + _lastMessage);
        }

        private void OnHint(BinaryReader packet)
        {
            int type = packet.ReadByte();
            int player = packet.ReadByte();
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
        }

        private void OnStart(BinaryReader packet)
        {
            int type = packet.ReadByte();
            _duel.IsFirst = (type & 0xF) == 0;
            _duel.Turn = 0;
            _duel.LastChainLocation = 0;
            _duel.LastChainPlayer = -1;
            _duel.LastChainTargets.Clear();
            _duel.LastSummonedCards.Clear();
            _duel.LastSummonPlayer = -1;
            int duel_rule = packet.ReadByte();
            _ai.Duel.IsNewRule = (duel_rule >= 4);
            _ai.Duel.IsNewRule2020 = (duel_rule >= 5);
            _duel.Fields[GetLocalPlayer(0)].LifePoints = packet.ReadInt32();
            _duel.Fields[GetLocalPlayer(1)].LifePoints = packet.ReadInt32();
            int deck = packet.ReadInt16();
            int extra = packet.ReadInt16();
            _duel.Fields[GetLocalPlayer(0)].Init(deck, extra);
            deck = packet.ReadInt16();
            extra = packet.ReadInt16();
            _duel.Fields[GetLocalPlayer(1)].Init(deck, extra);

            // in case of ending duel in chain's solving
            _duel.CurrentChain.Clear();
            _duel.CurrentChainInfo.Clear();
            _duel.ChainTargets.Clear();
            _duel.ChainTargetOnly.Clear();
            _duel.SummoningCards.Clear();
            _duel.SolvingChainIndex = 0;
            _duel.NegatedChainIndexList.Clear();

            Logger.DebugWriteLine("Duel started: " + _room.Names[0] + " versus " + _room.Names[1]);
            _ai.OnStart();
        }

        private void OnWin(BinaryReader packet)
        {
            int result = GetLocalPlayer(packet.ReadByte());

            string otherName = _room.Position == 0 ? _room.Names[1] : _room.Names[0];
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
                _duel.Fields[player].Deck.RemoveAt(_duel.Fields[player].Deck.Count - 1);
                _duel.Fields[player].Hand.Add(new ClientCard(0, CardLocation.Hand, -1));
            }
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
                ClientCard card = _duel.GetCard(player, (CardLocation)loc, seq);
                if (card == null) continue;
                ClientCard[] zone = (loc == (int)CardLocation.MonsterZone) ? _duel.Fields[player].MonsterZone : _duel.Fields[player].SpellZone;
                zone[seq] = list[i];
            }
        }

        private void OnSwapGraveDeck(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            IList<ClientCard> tmpDeckList = _duel.Fields[player].Deck.ToList();
            _duel.Fields[player].Deck.Clear();
            int seq = 0;
            foreach(var card in _duel.Fields[player].Graveyard)
            {
                if (card.IsExtraCard())
                {
                    _duel.Fields[player].ExtraDeck.Add(card);
                    card.Location = CardLocation.Extra;
                    card.Position = (int)CardPosition.FaceDown;
                    // TODO: face-up P cards
                }
                else
                {
                    _duel.Fields[player].Deck.Add(card);
                    card.Location = CardLocation.Deck;
                    card.Sequence = seq++;
                }
            }
            _duel.Fields[player].Graveyard.Clear();
            foreach (var card in tmpDeckList)
            {
                _duel.Fields[player].Graveyard.Add(card);
                card.Location = CardLocation.Grave;
            }
        }

        private void OnTagSwap(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int mcount = packet.ReadByte();
            int ecount = packet.ReadByte();
            /*int pcount = */ packet.ReadByte();
            int hcount = packet.ReadByte();
            /*int topcode =*/ packet.ReadInt32();
            _duel.Fields[player].Deck.Clear();
            for (int i = 0; i < mcount; ++i)
            {
                _duel.Fields[player].Deck.Add(new ClientCard(0, CardLocation.Deck, -1));
            }
            _duel.Fields[player].ExtraDeck.Clear();
            for (int i = 0; i < ecount; ++i)
            {
                int code = packet.ReadInt32() & 0x7fffffff;
                _duel.Fields[player].ExtraDeck.Add(new ClientCard(code, CardLocation.Extra, -1));
            }
            _duel.Fields[player].Hand.Clear();
            for (int i = 0; i < hcount; ++i)
            {
                int code = packet.ReadInt32();
                _duel.Fields[player].Hand.Add(new ClientCard(code, CardLocation.Hand,-1));
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
            // TODO: update equip cards and target cards
            int cardId = packet.ReadInt32();
            int previousControler = GetLocalPlayer(packet.ReadByte());
            int previousLocation = packet.ReadByte();
            int previousSequence = packet.ReadSByte();
            /*int previousPosotion = */packet.ReadSByte();
            int currentControler = GetLocalPlayer(packet.ReadByte());
            int currentLocation = packet.ReadByte();
            int currentSequence = packet.ReadSByte();
            int currentPosition = packet.ReadSByte();
            packet.ReadInt32(); // reason

            ClientCard card = _duel.GetCard(previousControler, (CardLocation)previousLocation, previousSequence);
            if (card != null)
            {
                card.LastLocation = (CardLocation)previousLocation;
            }
            if ((previousLocation & (int)CardLocation.Overlay) != 0)
            {
                previousLocation = previousLocation & 0x7f;
                card = _duel.GetCard(previousControler, (CardLocation)previousLocation, previousSequence);
                if (card != null)
                {
                    if (_debug)
                        Logger.WriteLine("(" + previousControler.ToString() + " 's " + (card.Name ?? "UnKnowCard") + " deattach " + (NamedCard.Get(cardId)?.Name) + ")");
                    card.Overlays.Remove(cardId);
                }
                previousLocation = 0; // the card is removed when it go to overlay, so here we treat it as a new card
            }
            else
                _duel.RemoveCard((CardLocation)previousLocation, card, previousControler, previousSequence);

            if ((currentLocation & (int)CardLocation.Overlay) != 0)
            {
                currentLocation = currentLocation & 0x7f;
                card = _duel.GetCard(currentControler, (CardLocation)currentLocation, currentSequence);
                if (card != null)
                {
                    if (_debug)
                        Logger.WriteLine("(" + previousControler.ToString() + " 's " + (card.Name ?? "UnKnowCard") + " overlay " + (NamedCard.Get(cardId)?.Name) + ")");
                    card.Overlays.Add(cardId);
                }
            }
            else
            {
                if (previousLocation == 0)
                {
                    if (_debug)
                        Logger.WriteLine("(" + previousControler.ToString() + " 's " + (NamedCard.Get(cardId)?.Name)
                        + " appear in " + (CardLocation)currentLocation + ")");
                    _duel.AddCard((CardLocation)currentLocation, cardId, currentControler, currentSequence, currentPosition);
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
        
            _ai.OnMove(card, previousControler, previousLocation, currentControler, currentLocation);
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
            _duel.RemoveCard((CardLocation)location1, card1, controler1, sequence1);
            _duel.RemoveCard((CardLocation)location2, card2, controler2, sequence2);
            _duel.AddCard((CardLocation)location2, card1, controler2, sequence2, card1.Position, cardId1);
            _duel.AddCard((CardLocation)location1, card2, controler1, sequence1, card2.Position, cardId2);
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
                    card.ClearCardTargets();
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

        private void InternalOnSelectCard(BinaryReader packet, Func<IList<ClientCard>, int, int, int, bool, IList<ClientCard>> func)
        {
            packet.ReadByte(); // player
            bool cancelable = packet.ReadByte() != 0;
            int min = packet.ReadByte();
            int max = packet.ReadByte();

            IList<ClientCard> cards = new List<ClientCard>();
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
                    card.Controller = player;
                }
                if (card == null) continue;
                if (card.Id == 0 || card.Location == CardLocation.Deck)
                    card.SetId(id);
                cards.Add(card);
            }

            if (_select_hint == 575 && cancelable) // HINTMSG_FIELD_FIRST
            {
                _select_hint = 0;
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            IList<ClientCard> selected = func(cards, min, max, _select_hint, cancelable);
            _select_hint = 0;

            if (selected.Count == 0 && cancelable)
            {
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            byte[] result = new byte[selected.Count + 1];
            result[0] = (byte)selected.Count;
            for (int i = 0; i < selected.Count; ++i)
            {
                int id = 0;
                for (int j = 0; j < count; ++j)
                {
                    if (cards[j] == null) continue;
                    if (cards[j].Equals(selected[i]))
                    {
                        id = j;
                        break;
                    }
                }
                result[i + 1] = (byte)id;
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
        }

        private void InternalOnSelectUnselectCard(BinaryReader packet, Func<IList<ClientCard>, int, int, int, bool, IList<ClientCard>> func)
        {
            packet.ReadByte(); // player
            bool finishable = packet.ReadByte() != 0;
            bool cancelable = packet.ReadByte() != 0 || finishable;
            int min = packet.ReadByte();
            int max = packet.ReadByte();

            IList<ClientCard> cards = new List<ClientCard>();
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
                    card = _duel.GetCard(player, loc, seq);
                if (card == null) continue;
                if (card.Id == 0 || card.Location == CardLocation.Deck)
                    card.SetId(id);
                cards.Add(card);
            }
            int count2 = packet.ReadByte();
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
            if (count2 == 0) cancelable = false;

            IList<ClientCard> selected = func(cards, (finishable ? 0 : 1), 1, _select_hint, cancelable);

            if (selected.Count == 0 && cancelable)
            {
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            byte[] result = new byte[selected.Count + 1];
            result[0] = (byte)selected.Count;
            for (int i = 0; i < selected.Count; ++i)
            {
                int id = 0;
                for (int j = 0; j < count; ++j)
                {
                    if (cards[j] == null) continue;
                    if (cards[j].Equals(selected[i]))
                    {
                        id = j;
                        break;
                    }
                }
                result[i + 1] = (byte)id;
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(result);
            Connection.Send(reply);
        }

        private void OnSelectCard(BinaryReader packet)
        {
            InternalOnSelectCard(packet, _ai.OnSelectCard);
        }

        private void OnSelectUnselectCard(BinaryReader packet)
        {
            InternalOnSelectUnselectCard(packet, _ai.OnSelectCard);
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

        private void OnSelectDisfield(BinaryReader packet)
        {
            packet.ReadByte(); // player
            packet.ReadByte(); // TODO: min
            int field = ~packet.ReadInt32();

            int player;
            CardLocation location;
            int filter;
            if ((field & 0x7f0000) != 0)
            {
                player = 1;
                location = CardLocation.MonsterZone;
                filter = (field >> 16) & Zones.MonsterZones;
            }
            else if ((field & 0x1f000000) != 0)
            {
                player = 1;
                location = CardLocation.SpellZone;
                filter = (field >> 24) & Zones.SpellZones;
            }
            else if ((field & 0x7f) != 0)
            {
                player = 0;
                location = CardLocation.MonsterZone;
                filter = field & Zones.MonsterZones;
            }
            else if ((field & 0x1f00) != 0)
            {
                player = 0;
                location = CardLocation.SpellZone;
                filter = (field >> 8) & Zones.SpellZones;
            }
            else if ((field & 0x2000) != 0)
            {
                player = 0;
                location = CardLocation.FieldZone;
                filter = Zones.FieldZone;
            }
            else if ((field & 0xc000) != 0)
            {
                player = 0;
                location = CardLocation.PendulumZone;
                filter = (field >> 14) & Zones.PendulumZones;
            }
            else if ((field & 0x20000000) != 0)
            {
                player = 1;
                location = CardLocation.FieldZone;
                filter = Zones.FieldZone;
            }
            else
            {
                player = 1;
                location = CardLocation.PendulumZone;
                filter = (field >> 30) & Zones.PendulumZones;
            }

            int selected = _ai.OnSelectPlace(_select_hint, player, location, filter);
            _select_hint = 0;

            byte[] resp = new byte[3];
            resp[0] = (byte)GetLocalPlayer(player);

            if (location != CardLocation.PendulumZone && location != CardLocation.FieldZone)
            {
                resp[1] = (byte)location;
                if ((selected & filter) > 0)
                    filter &= selected;

                if ((filter & Zones.z2) != 0) resp[2] = 2;
                else if ((filter & Zones.z1) != 0) resp[2] = 1;
                else if ((filter & Zones.z3) != 0) resp[2] = 3;
                else if ((filter & Zones.z0) != 0) resp[2] = 0;
                else if ((filter & Zones.z4) != 0) resp[2] = 4;
                else if ((filter & Zones.z6) != 0) resp[2] = 6;
                else if ((filter & Zones.z5) != 0) resp[2] = 5;
            }
            else
            {
                resp[1] = (byte)CardLocation.SpellZone;
                if ((selected & filter) > 0)
                    filter &= selected;

                if ((filter & Zones.FieldZone) != 0) resp[2] = 5;
                if ((filter & Zones.z0) != 0) resp[2] = 6; // left pendulum zone
                if ((filter & Zones.z1) != 0) resp[2] = 7; // right pendulum zone
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(resp);
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

        private void OnSelectPlace(BinaryReader packet)
        {
            packet.ReadByte(); // player
            packet.ReadByte(); // min
            int field = ~packet.ReadInt32();

            int player;
            CardLocation location;
            int filter;

            if ((field & 0x7f) != 0)
            {
                player = 0;
                location = CardLocation.MonsterZone;
                filter = field & Zones.MonsterZones;
            }
            else if ((field & 0x1f00) != 0)
            {
                player = 0;
                location = CardLocation.SpellZone;
                filter = (field >> 8) & Zones.SpellZones;
            }
            else if ((field & 0x2000) != 0)
            {
                player = 0;
                location = CardLocation.FieldZone;
                filter = Zones.FieldZone;
            }
            else if ((field & 0xc000) != 0)
            {
                player = 0;
                location = CardLocation.PendulumZone;
                filter = (field >> 14) & Zones.PendulumZones;
            }
            else if ((field & 0x7f0000) != 0)
            {
                player = 1;
                location = CardLocation.MonsterZone;
                filter = (field >> 16) & Zones.MonsterZones;
            }
            else if ((field & 0x1f000000) != 0)
            {
                player = 1;
                location = CardLocation.SpellZone;
                filter = (field >> 24) & Zones.SpellZones;
            }
            else if ((field & 0x20000000) != 0)
            {
                player = 1;
                location = CardLocation.FieldZone;
                filter = Zones.FieldZone;
            }
            else
            {
                player = 1;
                location = CardLocation.PendulumZone;
                filter = (field >> 30) & Zones.PendulumZones;
            }

            int selected = _ai.OnSelectPlace(_select_hint, player, location, filter);
            _select_hint = 0;

            byte[] resp = new byte[3];
            resp[0] = (byte)GetLocalPlayer(player);

            if (location != CardLocation.PendulumZone && location != CardLocation.FieldZone)
            {
                resp[1] = (byte)location;
                if ((selected & filter) > 0)
                    filter &= selected;

                if ((filter & Zones.z2) != 0) resp[2] = 2;
                else if ((filter & Zones.z1) != 0) resp[2] = 1;
                else if ((filter & Zones.z3) != 0) resp[2] = 3;
                else if ((filter & Zones.z0) != 0) resp[2] = 0;
                else if ((filter & Zones.z4) != 0) resp[2] = 4;
                else if ((filter & Zones.z6) != 0) resp[2] = 6;
                else if ((filter & Zones.z5) != 0) resp[2] = 5;
            }
            else
            {
                resp[1] = (byte)CardLocation.SpellZone;
                if ((selected & filter) > 0)
                    filter &= selected;

                if ((filter & Zones.FieldZone) != 0) resp[2] = 5;
                if ((filter & Zones.z0) != 0) resp[2] = 6; // left pendulum zone
                if ((filter & Zones.z1) != 0) resp[2] = 7; // right pendulum zone
            }

            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.Response);
            reply.Write(resp);
            Connection.Send(reply);
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
                    ClientCard card = _duel.GetCard(player, loc, seq);
                    if (cardId != 0 && card.Id != cardId)
                        card.SetId(cardId);
                    card.SelectSeq = i;
                    int OpParam = packet.ReadInt32();
                    int OpParam1 = OpParam & 0xffff;
                    int OpParam2 = OpParam >> 16;
                    if ((OpParam & 0x80000000) > 0)
                    {
                        OpParam1 = OpParam & 0x7fffffff;
                        OpParam2 = 0;
                    }
                    if (OpParam2 > 0 && OpParam1 > OpParam2)
                    {
                        card.OpParam1 = OpParam2;
                        card.OpParam2 = OpParam1;
                    }
                    else
                    {
                        card.OpParam1 = OpParam1;
                        card.OpParam2 = OpParam2;
                    }
                    if (j == 0)
                        mandatoryCards.Add(card);
                    else
                        cards.Add(card);
                }
            }

            for (int k = 0; k < mandatoryCards.Count; ++k)
            {
                sumval -= mandatoryCards[k].OpParam1;
            }

            IList<ClientCard> selected = _ai.OnSelectSum(cards, sumval, min, max, _select_hint, mode);
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
            InternalOnSelectCard(packet, _ai.OnSelectTribute);
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
                if (card.HasType(CardType.Token) || (card.Alias > 0 && card.Id - card.Alias < 10)) continue;
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
            equipCard.EquipTarget?.EquipCards.Remove(equipCard);
            equipCard.EquipTarget = targetCard;
            targetCard.EquipCards.Add(equipCard);
        }

        private void OnUnEquip(BinaryReader packet)
        {
            int equipCardControler = GetLocalPlayer(packet.ReadByte());
            int equipCardLocation = packet.ReadByte();
            int equipCardSequence = packet.ReadSByte();
            packet.ReadByte();
            ClientCard equipCard = _duel.GetCard(equipCardControler, (CardLocation)equipCardLocation, equipCardSequence);
            if (equipCard == null) return;
            if (equipCard.EquipTarget != null)
            {
                equipCard.EquipTarget.EquipCards.Remove(equipCard);
                equipCard.EquipTarget = null;
            }
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
            ownerCard.TargetCards.Add(targetCard);
            targetCard.OwnTargets.Add(ownerCard);
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
            ownerCard.TargetCards.Remove(targetCard);
            targetCard.OwnTargets.Remove(ownerCard);
        }

        private void OnSummoning(BinaryReader packet)
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
    }
}
