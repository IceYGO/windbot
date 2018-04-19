using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
        private int _select_hint;

        public GameBehavior(GameClient game)
        {
            Game = game;
            Connection = game.Connection;
            _hand = game.Hand;

            _packets = new Dictionary<StocMessage, Action<BinaryReader>>();
            _messages = new Dictionary<GameMessage, Action<BinaryReader>>();
            RegisterPackets();

            _room = new Room();
            _duel = new Duel();

            _ai = new GameAI(Game, _duel);
            _ai.Executor = DecksManager.Instantiate(_ai, _duel);
            Deck = Deck.Load(_ai.Executor.Deck);

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

            _messages.Add(GameMessage.Retry, OnRetry);
            _messages.Add(GameMessage.Start, OnStart);
            _messages.Add(GameMessage.Hint, OnHint);
            _messages.Add(GameMessage.Win, OnWin);
            _messages.Add(GameMessage.Draw, OnDraw);
            _messages.Add(GameMessage.ShuffleDeck, OnShuffleDeck);
            _messages.Add(GameMessage.ShuffleHand, OnShuffleHand);
            _messages.Add(GameMessage.ShuffleExtra, OnShuffleExtra);
            _messages.Add(GameMessage.TagSwap, OnTagSwap);
            _messages.Add(GameMessage.NewTurn, OnNewTurn);
            _messages.Add(GameMessage.NewPhase, OnNewPhase);
            _messages.Add(GameMessage.Damage, OnDamage);
            _messages.Add(GameMessage.PayLpCost, OnDamage);
            _messages.Add(GameMessage.Recover, OnRecover);
            _messages.Add(GameMessage.LpUpdate, OnLpUpdate);
            _messages.Add(GameMessage.Move, OnMove);
            _messages.Add(GameMessage.Attack, OnAttack);
            _messages.Add(GameMessage.PosChange, OnPosChange);
            _messages.Add(GameMessage.Chaining, OnChaining);
            _messages.Add(GameMessage.ChainEnd, OnChainEnd);
            _messages.Add(GameMessage.SortCard, OnCardSorting);
            _messages.Add(GameMessage.SortChain, OnChainSorting);
            _messages.Add(GameMessage.UpdateCard, OnUpdateCard);
            _messages.Add(GameMessage.UpdateData, OnUpdateData);
            _messages.Add(GameMessage.BecomeTarget, OnBecomeTarget);
            _messages.Add(GameMessage.SelectBattleCmd, OnSelectBattleCmd);
            _messages.Add(GameMessage.SelectCard, OnSelectCard);
            _messages.Add(GameMessage.SelectUnselectCard, OnSelectUnselectCard);
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
            _messages.Add(GameMessage.AnnounceCardFilter, OnAnnounceCard);
            _messages.Add(GameMessage.RockPaperScissors, OnRockPaperScissors);

            _messages.Add(GameMessage.SpSummoning, OnSpSummon);
            _messages.Add(GameMessage.SpSummoned, OnSpSummon);

        }

        private void OnJoinGame(BinaryReader packet)
        {
            /*int lflist = (int)*/ packet.ReadUInt32();
            /*int rule = */ packet.ReadByte();
            /*int mode = */ packet.ReadByte();
            int duel_rule = packet.ReadByte();
            _ai.Duel.IsNewRule = (duel_rule == 4);
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
            Connection.Close();
            Logger.DebugWriteLine("********************* Duel end *********************");
        }

        private void OnChat(BinaryReader packet)
        {
            int player = packet.ReadInt16();
            string message = packet.ReadUnicode(256);
            string myName = (player != 0) ? _room.Names[1] : _room.Names[0];
            string otherName = (player == 0) ? _room.Names[1] : _room.Names[0];
            if (player < 4)
                Logger.DebugWriteLine(otherName + " say to " + myName + ": " + message);
        }

        private void OnErrorMsg(BinaryReader packet)
        {
            int msg = packet.ReadByte();
            // align
            packet.ReadByte();
            packet.ReadByte();
            packet.ReadByte();
            int pcode = packet.ReadInt32();
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

        private void OnRetry(BinaryReader packet)
        {
            _ai.OnRetry();
            Connection.Close();
            throw new Exception("Got MSG_RETRY.");
        }

        private void OnHint(BinaryReader packet)
        {
            int type = packet.ReadByte();
            int player = packet.ReadByte();
            int data = packet.ReadInt32();
            if (type == 3) // HINT_SELECTMSG
            {
                _select_hint = data;
            }
        }

        private void OnStart(BinaryReader packet)
        {
            int type = packet.ReadByte();
            _duel.IsFirst = (type & 0xF) == 0;
            _duel.Turn = 0;
            _duel.Fields[GetLocalPlayer(0)].LifePoints = packet.ReadInt32();
            _duel.Fields[GetLocalPlayer(1)].LifePoints = packet.ReadInt32();
            int deck = packet.ReadInt16();
            int extra = packet.ReadInt16();
            _duel.Fields[GetLocalPlayer(0)].Init(deck, extra);
            deck = packet.ReadInt16();
            extra = packet.ReadInt16();
            _duel.Fields[GetLocalPlayer(1)].Init(deck, extra);

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
            Logger.DebugWriteLine("(" + player.ToString() + "抽了" + count.ToString() + "张卡)");

            for (int i = 0; i < count; ++i)
            {
                _duel.Fields[player].Deck.RemoveAt(_duel.Fields[player].Deck.Count - 1);
                _duel.Fields[player].Hand.Add(new ClientCard(0, CardLocation.Hand));
            }
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
                _duel.Fields[player].Deck.Add(new ClientCard(0, CardLocation.Deck));
            }
            _duel.Fields[player].ExtraDeck.Clear();
            for (int i = 0; i < ecount; ++i)
            {
                int code = packet.ReadInt32() & 0x7fffffff;
                _duel.Fields[player].ExtraDeck.Add(new ClientCard(code, CardLocation.Extra));
            }
            _duel.Fields[player].Hand.Clear();
            for (int i = 0; i < hcount; ++i)
            {
                int code = packet.ReadInt32();
                _duel.Fields[player].Hand.Add(new ClientCard(code, CardLocation.Hand));
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
            Logger.DebugWriteLine("(进入" + (_duel.Phase.ToString()) + ")");
            _duel.LastSummonPlayer = -1;
            _duel.Fields[0].BattlingMonster = null;
            _duel.Fields[1].BattlingMonster = null;
            _ai.OnNewPhase();
        }

        private void OnDamage(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            int final = _duel.Fields[player].LifePoints - packet.ReadInt32();
            if (final < 0) final = 0;
            Logger.DebugWriteLine("(" + player.ToString() + "受到了伤害，当前为" + final.ToString() + ")");
            _duel.Fields[player].LifePoints = final;
        }

        private void OnRecover(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            _duel.Fields[player].LifePoints += packet.ReadInt32();
        }

        private void OnLpUpdate(BinaryReader packet)
        {
            int player = GetLocalPlayer(packet.ReadByte());
            _duel.Fields[player].LifePoints = packet.ReadInt32();
        }

        private void OnMove(BinaryReader packet)
        {
            int cardId = packet.ReadInt32();
            int pc = GetLocalPlayer(packet.ReadByte());
            int pl = packet.ReadByte();
            int ps = packet.ReadSByte();
            packet.ReadSByte(); // pp
            int cc = GetLocalPlayer(packet.ReadByte());
            int cl = packet.ReadByte();
            int cs = packet.ReadSByte();
            int cp = packet.ReadSByte();
            packet.ReadInt32(); // reason

            ClientCard card = _duel.GetCard(pc, (CardLocation)pl, ps);
            if (card != null) Logger.DebugWriteLine("(" + pc.ToString() + "的" + (card.Name ?? "未知卡片") + "从" + (CardLocation)pl + "移动到了" + (CardLocation)cl + ")");

            if ((pl & (int)CardLocation.Overlay) != 0)
            {
                pl = pl & 0x7f;
                card = _duel.GetCard(pc, (CardLocation)pl, ps);
                if (card != null)
                    card.Overlays.Remove(cardId);
            }
            else
                _duel.RemoveCard((CardLocation)pl, card, pc, ps);

            if ((cl & (int)CardLocation.Overlay) != 0)
            {
                cl = cl & 0x7f;
                card = _duel.GetCard(cc, (CardLocation)cl, cs);
                if (card != null)
                    card.Overlays.Add(cardId);
            }
            else
            {
                _duel.AddCard((CardLocation)cl, cardId, cc, cs, cp);
                if ((pl & (int)CardLocation.Overlay) == 0 && card != null)
                {
                    ClientCard newcard = _duel.GetCard(cc, (CardLocation)cl, cs);
                    if (newcard != null)
                        newcard.Overlays.AddRange(card.Overlays);
                }
            }
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
            if (defendcard == null) Logger.DebugWriteLine("(" + (attackcard.Name ?? "未知卡片") + "直接攻击)");
            else Logger.DebugWriteLine("(" + ca.ToString() + "的" + (attackcard.Name ?? "未知卡片") + "攻击了" + cd.ToString() + "的" + (defendcard.Name ?? "未知卡片") + ")");
            _duel.Fields[attackcard.Controller].BattlingMonster = attackcard;
            _duel.Fields[1 - attackcard.Controller].BattlingMonster = defendcard;

            if (ld == 0 && (attackcard != null) && (ca != 0))
            {
                _ai.OnDirectAttack(attackcard);
            }
        }

        private void OnPosChange(BinaryReader packet)
        {
            packet.ReadInt32(); // card id
            int pc = GetLocalPlayer(packet.ReadByte());
            int pl = packet.ReadByte();
            int ps = packet.ReadSByte();
            packet.ReadSByte(); // pp
            int cp = packet.ReadSByte();
            ClientCard card = _duel.GetCard(pc, (CardLocation)pl, ps);
            if (card != null)
            {
                card.Position = cp;
                Logger.DebugWriteLine("(" + (card.Name ?? "未知卡片") + "改变了表示形式为" + (CardPosition)cp + ")");
            }
        }

        private void OnChaining(BinaryReader packet)
        {
            packet.ReadInt32(); // card id
            int pcc = GetLocalPlayer(packet.ReadByte());
            int pcl = packet.ReadByte();
            int pcs = packet.ReadSByte();
            int subs = packet.ReadSByte();
            ClientCard card = _duel.GetCard(pcc, pcl, pcs, subs);
            int cc = GetLocalPlayer(packet.ReadByte());
            if (card != null) Logger.DebugWriteLine("(" + cc.ToString() + "的" + (card.Name ?? "未知卡片") + "发动了效果)");
            _ai.OnChaining(card, cc);
            _duel.ChainTargets.Clear();
            _duel.LastSummonPlayer = -1;
            _duel.CurrentChain.Add(card);
            _duel.LastChainPlayer = cc;
        }

        private void OnChainEnd(BinaryReader packet)
        {
            _ai.OnChainEnd();
            _duel.LastChainPlayer = -1;
            _duel.CurrentChain.Clear();
            //_duel.ChainTargets.Clear();
        }

        private void OnCardSorting(BinaryReader packet)
        {
            /*BinaryWriter writer =*/ GamePacketFactory.Create(CtosMessage.Response);
            Connection.Send(CtosMessage.Response, -1);
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
            if (card == null) return;

            card.Update(packet, _duel);
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
            int count = packet.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                int player = GetLocalPlayer(packet.ReadByte());
                int loc = packet.ReadByte();
                int seq = packet.ReadByte();
                /*int sseq = */packet.ReadByte();
                ClientCard card = _duel.GetCard(player, (CardLocation)loc, seq);
                if (card == null) continue;
                Logger.DebugWriteLine("(" + (CardLocation)loc + "的" + (card.Name ?? "未知卡片") + "成为了对象)");
                _duel.ChainTargets.Add(card);
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
                    card = new ClientCard(id, CardLocation.Overlay);
                else
                    card = _duel.GetCard(player, loc, seq);
                if (card == null) continue;
                if (card.Id == 0)
                    card.SetId(id);
                cards.Add(card);
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
                    card = new ClientCard(id, CardLocation.Overlay);
                else
                    card = _duel.GetCard(player, loc, seq);
                if (card == null) continue;
                if (card.Id == 0)
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
            }

            IList<ClientCard> selected = func(cards, (finishable ? 0 : 1), 1, _select_hint, cancelable);
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
            bool forced = packet.ReadByte() != 0;
            packet.ReadInt32(); // hint1
            packet.ReadInt32(); // hint2

            IList<ClientCard> cards = new List<ClientCard>();
            IList<int> descs = new List<int>();

            for (int i = 0; i < count; ++i)
            {
                packet.ReadByte(); // flag
                packet.ReadInt32(); // card id
                int con = GetLocalPlayer(packet.ReadByte());
                int loc = packet.ReadByte();
                int seq = packet.ReadByte();
                int sseq = packet.ReadByte();

                int desc = packet.ReadInt32();
                cards.Add(_duel.GetCard(con, loc, seq, sseq));
                descs.Add(desc);
            }

            if (cards.Count == 0)
            {
                Connection.Send(CtosMessage.Response, -1);
                return;
            }

            if (cards.Count == 1 && forced)
            {
                Connection.Send(CtosMessage.Response, 0);
                return;
            }

            Connection.Send(CtosMessage.Response, _ai.OnSelectChain(cards, descs, forced));
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
            OnSelectPlace(packet);
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

            ClientCard card = _duel.GetCard(player, loc, seq);
            if (card == null)
            {
                Connection.Send(CtosMessage.Response, 0);
                return;
            }
            
            if (card.Id == 0) card.SetId(cardId);

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

            byte[] resp = new byte[3];

            bool pendulumZone = false;

            int filter;
            if ((field & 0x7f) != 0)
            {
                resp[0] = (byte)GetLocalPlayer(0);
                resp[1] = 0x4;
                filter = field & 0x7f;
            }
            else if ((field & 0x1f00) != 0)
            {
                resp[0] = (byte)GetLocalPlayer(0);
                resp[1] = 0x8;
                filter = (field >> 8) & 0x1f;
            }
            else if ((field & 0xc000) != 0)
            {
                resp[0] = (byte)GetLocalPlayer(0);
                resp[1] = 0x8;
                filter = (field >> 14) & 0x3;
                pendulumZone = true;
            }
            else if ((field & 0x7f0000) != 0)
            {
                resp[0] = (byte)GetLocalPlayer(1);
                resp[1] = 0x4;
                filter = (field >> 16) & 0x7f;
            }
            else if ((field & 0x1f000000) != 0)
            {
                resp[0] = (byte) GetLocalPlayer(1);
                resp[1] = 0x8;
                filter = (field >> 24) & 0x1f;
            }
            else
            {
                resp[0] = (byte) GetLocalPlayer(1);
                resp[1] = 0x8;
                filter = (field >> 30) & 0x3;
                pendulumZone = true;
            }

            if (!pendulumZone)
            {
                if ((filter & 0x40) != 0) resp[2] = 6;
                else if ((filter & 0x20) != 0) resp[2] = 5;
                else if ((filter & 0x4) != 0) resp[2] = 2;
                else if ((filter & 0x2) != 0) resp[2] = 1;
                else if ((filter & 0x8) != 0) resp[2] = 3;
                else if ((filter & 0x1) != 0) resp[2] = 0;
                else if ((filter & 0x10) != 0) resp[2] = 4;
            }
            else
            {
                if ((filter & 0x1) != 0) resp[2] = 6;
                if ((filter & 0x2) != 0) resp[2] = 7;
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
            for (int i = 0; i < selected.Count; ++i)
            {
                result[index++] = (byte)selected[i].SelectSeq;
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
            // not fully implemented
            Connection.Send(CtosMessage.Response, _ai.OnAnnounceCard());
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
            for (int i = 0; i < 23; ++i)
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

        private void OnSpSummon(BinaryReader packet)
        {
            _ai.CleanSelectMaterials();
        }
    }
}
