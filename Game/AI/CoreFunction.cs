using System;
using System.Collections.Generic;
using System.Linq;
using YGOSharp.Network;
using YGOSharp.Network.Enums;
using System.IO;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI
{
    /// <summary>
    /// function of link ocgcore,approximately equivalent to core lua function 
    /// </summary>
    public class CoreFunction
    {
        public enum ReceiveType
        {
            Null,
            Success,
            LuaError,
            CoreError,
            BufferError,
            ClientError

        }
        public byte[] Data { get; set; }
        public Duel Duel { get; private set; }
        public YGOClient Connection { get; private set; }
        /*
         If true, we can get information about all cards by CoreFunction, 
         including the facdedown cards and the enemy cards, which are not allowed by rules.
         For certain special situations, it can be used appropriately, 
         such as get card information when viewing the enemy cards.
         */
        public bool GodMode { get; set; }

        public ReceiveType type;

        private IList<ClientCard> resetCards = new List<ClientCard>();

        private const int PLAYER_NONE = 2;
        private const int PLAYER_ALL = 3;
        private const int PLAYER_NULL = 4;
        private const int MAX_EX_PARAMETERS = 11;
        private const int LOCATION_REASON_TOFIELD = 0x1;
        private static class FunctionCode
        {
            public enum Type
            {
                Card,
                Duel
            }
            public enum Card
            {
                GetCode,
                GetOriginalCodeRule,
                GetPreviousCodeOnField,
                GetUnionCount,
                GetTributeRequirement,

                GetOriginalCode,
                GetType,
                GetOriginalType,
                GetFusionType,
                GetSynchroType,
                GetXyzType,
                GetLinkType,
                GetLevel,
                GetRank,
                GetLink,
                GetOriginalLevel,
                GetOriginalRank,
                GetLeftScale,
                GetOriginalLeftScale,
                GetRightScale,
                GetOriginalRightScale,
                GetCurrentScale,
                GetLinkedGroupCount,
                GetMutualLinkedGroupCount,
                GetColumnGroupCount,
                GetAttribute,
                GetOriginalAttribute,
                GetRace,
                GetOriginalRace,
                GetAttack,
                GetBaseAttack,
                GetTextAttack,
                GetDefense,
                GetBaseDefense,
                GetTextDefense,
                GetPreviousTypeOnField,
                GetPreviousLevelOnField,
                GetPreviousRankOnField,
                GetPreviousAttributeOnField,
                GetPreviousRaceOnField,
                GetPreviousAttackOnField,
                GetPreviousDefenseOnField,
                GetOwner,
                GetControler,
                GetPreviousControler,
                GetReason,
                GetReasonPlayer,
                GetPosition,
                GetPreviousPosition,
                GetBattlePosition,
                GetLocation,
                GetPreviousLocation,
                GetSequence,
                GetSummonType,
                GetSummonLocation,
                GetSummonPlayer,
                GetDestination,
                GetLeaveFieldDest,
                GetTurnID,
                GetFieldID,
                GetRealFieldID,
                GetTurnCounter,
                GetMaterialCount,
                GetEquipCount,
                GetOverlayCount,
                GetAttackedGroupCount,
                GetAttackedCount,
                GetAttackAnnouncedCount,
                GetCardTargetCount,
                GetOwnerTargetCount,

                GetFusionCode,
                GetLinkCode,

                IsFusionCode,
                IsLinkCode,
                IsSetCard,
                IsOriginalSetCard,
                IsPreviousSetCard,
                IsFusionSetCard,
                IsLinkSetCard,
                IsOriginalCodeRule,
                IsCode,
                IsLevel,
                IsRank,
                IsLink,
                IsAttack,
                IsDefense,
                IsLinkRace,
                IsFusionAttribute,
                IsLinkAttribute,

                GetSynchroLevel,
                GetRitualLevel,

                IsLinkMarker,
                IsType,
                IsFusionType,
                IsSynchroType,
                IsXyzType,
                IsLinkType,
                IsRace,
                IsAttribute,
                IsReason,
                IsSummonType,
                IsSummonLocation,
                IsSummonPlayer,
                IsStatus,
                IsRelateToChain,
                IsPosition,
                IsPreviousPosition,
                IsControler,
                IsPreviousControler,
                IsLocation,
                IsPreviousLocation,
                IsLevelBelow,
                IsLevelAbove,
                IsRankBelow,
                IsRankAbove,
                IsLinkBelow,
                IsLinkAbove,
                IsAttackBelow,
                IsAttackAbove,
                IsDefenseBelow,
                IsDefenseAbove,
                IsCanHaveCounter,
                IsFusionSummonableCard,
                IsSpecialSummonable,
                IsAbleToHand,
                IsAbleToRemoveAsCost,
                IsDiscardable,
                IsCanOverlay,

                GetLinkedGroup,
                GetMutualLinkedGroup,
                GetColumnGroup,
                GetMaterial,
                GetEquipGroup,
                GetAttackedGroup,
                GetBattledGroup,
                GetCardTarget,
                GetOwnerTarget,

                IsLinkState,
                IsExtraLinkState,
                IsAllColumn,
                IsExtraDeckMonster,
                IsDualState,
                IsDirectAttacked,
                IsRelateToBattle,
                IsDisabled,
                IsSummonableCard,
                IsAbleToDeck,
                IsAbleToExtra,
                IsAbleToGrave,
                IsAbleToHandAsCost,
                IsAbleToDeckAsCost,
                IsAbleToExtraAsCost,
                IsAbleToDeckOrExtraAsCost,
                IsAbleToGraveAsCost,
                IsReleasable,
                IsReleasableByEffect,
                IsAttackable,
                IsFaceup,
                IsAttackPos,
                IsFacedown,
                IsDefensePos,
                IsOnField,
                IsPublic,
                IsForbidden,
                IsAbleToChangeControler,
                IsCanChangePosition,
                IsCanTurnSet,

                GetLinkedZone,
                GetMutualLinkedZone,
                GetFusionAttribute,
                GetLinkAttribute,
                GetLinkRace,

                GetReasonCard,
                GetEquipTarget,
                GetPreviousEquipTarget,
                GetOverlayTarget,
                GetFirstCardTarget,
                GetBattleTarget,

                IsNotTuner,
                CheckEquipTarget,
                CheckUnionTarget,
                IsHasCardTarget,
                IsRelateToCard,
                CheckFusionSubstitute,
                IsCanBeBattleTarget,
                IsSynchroSummonable,

                GetEffectCount,
                GetFlagEffect,
                GetCounter,

                IsSummonable,
                IsMSetable,
                IsXyzSummonable,
                IsLinkSummonable,
                IsXyzLevel,
                GetFlagEffectLabel,
                GetColumnZone,
                GetOverlayGroup,
                CheckRemoveOverlayCard,
                IsHasEffect,
                GetAttackableTarget,

                IsCanBeCardTarget,
                IsCanBeDisabledByCard,
                IsImmuneToCard,
                IsDestructable,

                IsSSetable,
                IsAbleToRemove,
                IsChainAttackable,
                IsControlerCanBeChanged,
                IsCanAddCounter,
                IsCanRemoveCounter,

                IsCanBeFusionMaterial,
                IsCanBeSynchroMaterial,
                IsCanBeRitualMaterial,
                IsCanBeXyzMaterial,
                IsCanBeLinkMaterial,
                CheckUniqueOnField

            }
            public enum Duel
            {
                GetLP,
                GetTurnCount,
                GetDrawCount,
                GetBattleDamage,
                GetLinkedZone,
                GetRitualMaterial,
                IsPlayerCanAdditionalSummon,
                GetBattledCount,

                GetTurnPlayer,
                GetCurrentChain,
                GetCurrentPhase,
                IsDamageCalculated,
                GetAttacker,
                GetAttackTarget,
                CheckPhaseActivity,
                IsAbleToEnterBP,

                GetDecktopGroup,
                GetExtraTopGroup,
                GetReleaseGroup,
                GetReleaseGroupCount,
                GetFusionMaterial,
                IsPlayerAffectedByEffect,
                IsPlayerCanDraw,
                IsPlayerCanDiscardDeck,
                IsPlayerCanDiscardDeckAsCost,
                IsPlayerCanSSet,
                IsPlayerCanSpecialSummonCount,
                IsPlayerCanRelease,
                IsPlayerCanSendtoHand,
                IsPlayerCanSendtoGrave,
                IsPlayerCanSendtoDeck,


                GetBattleMonster,
                GetFlagEffect,
                GetFlagEffectLabel,
                IsCanAddCounter,
                IsCanRemoveCounter,
                GetCounter,
                IsEnvironment,
                CheckLPCost,
                CheckSummonedCount,
                GetLocationCount,
                GetUsableMZoneCount,

                GetLinkedGroup,
                GetLinkedGroupCount,
                GetFieldCard,
                CheckLocation,
                GetFieldGroup,
                GetFieldGroupCount,
                GetOverlayGroup,
                GetOverlayCount,

                GetTributeGroup,
                CheckRemoveOverlayCard,
                IsPlayerCanSummon,
                IsPlayerCanMSet,
                IsPlayerCanSpecialSummon,
                IsPlayerCanSpecialSummonMonster,
                IsPlayerCanRemove
            }
        }
        private class DataReader
        {
            public MemoryStream Stream { get; private set; }
            public BinaryReader Packet { get; private set; }

            public DataReader(MemoryStream stream, BinaryReader packet)
            {
                Stream = stream;
                Packet = packet;
            }
            public void Close()
            {
                Packet?.Close();
                Stream?.Close();
            }
        }
       
        public CoreFunction(Duel duel, YGOClient connection) 
        {
            Duel = duel;
            Connection = connection;
            Data = null;
            GodMode = false;
        }

        private DataReader GetDataReader(byte[] data)
        {
            if (data?.Length <= 0) return null;
            MemoryStream stream = new MemoryStream(data);
            BinaryReader packet = new BinaryReader(stream);
            return new DataReader(stream, packet);
        }
        private void Close(DataReader dreader)
        {
            Data = null;
            dreader?.Close();
        }

        /// <summary>
        /// Called when determining whether a function is executing correctly
        /// </summary>
        public bool FunctionSuccess()
        {
            return type == ReceiveType.Success;
        }

        private bool CheckGodMode(ClientCard card)
        {
            if (card == null) return false;
            if (GodMode) return true;
            if (card.Sequence < 0) return false;
            if (card.Controller == 0)
            {
                if (card.Location == CardLocation.Deck) return false;
                if (card.Location == CardLocation.Removed && card.IsFacedown()) return false;
                return true;

            }
            else
            {
                if (((card.Location & CardLocation.Onfield) > 0 && !card.IsFacedown()) || card.Location == CardLocation.Overlay) return true;
                return false;
            }
        }

        private void WriteCard(BinaryWriter reply,ClientCard card) 
        {
            RestoreCardsData();
            if (card == null)
            {
                reply.Write((byte)PLAYER_NONE);
            }
            else
            {
                //if (card.Sequence < 0 && GodMode) GetFieldGroup(0, (int)card.Location, (int)card.Location);
                int controller = 0;
                int sequence = -1;
                if (GodMode && card.Sequence < 0)
                {
                    int[] data = GetRealPosition(card, card.Location);
                    controller = data[0];
                    sequence = data[1];
                }
                else
                {
                    controller = card.Controller;
                    sequence = card.Sequence;
                }
                reply.Write((byte)controller);
                reply.Write((int)card.Location);
                reply.Write(sequence);
            }
        }

        private int SearchCard(IList<ClientCard> cards,ClientCard card)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (card == cards[i]) return i;
            }
            return -1;
        }

        private int[] GetRealPosition(ClientCard card,CardLocation loction)
        {
            //Controller  Sequence
            if (card == null) return new int[] { 0, -1 };
            int sequence = -1;
            switch (loction)
            {
                case CardLocation.Deck:
                    if ((sequence = SearchCard(Duel.Fields[1].Deck, card)) > 0) return new int[] { 1, sequence };
                    if ((sequence = SearchCard(Duel.Fields[0].Deck, card)) > 0) return new int[] { 0, sequence };
                    break;
                case CardLocation.Hand:
                    if ((sequence = SearchCard(Duel.Fields[1].Hand, card)) > 0) return new int[] { 1, sequence };
                    if ((sequence = SearchCard(Duel.Fields[0].Hand, card)) > 0) return new int[] { 0, sequence };
                    break;
                case CardLocation.MonsterZone:
                    if ((sequence = SearchCard(Duel.Fields[1].MonsterZone, card)) > 0) return new int[] { 1, sequence };
                    if ((sequence = SearchCard(Duel.Fields[0].MonsterZone, card)) > 0) return new int[] { 0, sequence };
                    break;
                case CardLocation.SpellZone:
                    if ((sequence = SearchCard(Duel.Fields[1].SpellZone, card)) > 0) return new int[] { 1, sequence };
                    if ((sequence = SearchCard(Duel.Fields[0].SpellZone, card)) > 0) return new int[] { 0, sequence };
                    break;
                case CardLocation.Grave:
                    if ((sequence = SearchCard(Duel.Fields[1].Graveyard, card)) > 0) return new int[] { 1, sequence };
                    if ((sequence = SearchCard(Duel.Fields[0].Graveyard, card)) > 0) return new int[] { 0, sequence };
                    break;
                case CardLocation.Removed:
                    if ((sequence = SearchCard(Duel.Fields[1].Banished, card)) > 0) return new int[] { 1, sequence };
                    if ((sequence = SearchCard(Duel.Fields[0].Banished, card)) > 0) return new int[] { 0, sequence };
                    break;
                case CardLocation.Extra:
                    if ((sequence = SearchCard(Duel.Fields[1].ExtraDeck, card)) > 0) return new int[] { 1, sequence };
                    if ((sequence = SearchCard(Duel.Fields[0].ExtraDeck, card)) > 0) return new int[] { 0, sequence };
                    break;
                default:
                    break;
            }
            return new int[] { 0, -1 };
        }

        private void ReadGroup(BinaryReader packet,IList<ClientCard> cards)
        {
            int count = packet.ReadInt32();
            int id;
            int player;
            int loc;
            int seq;
            ClientCard rcard;
            for (int i = 0; i < count; ++i)
            {
                id = packet.ReadInt32();
                player = Duel.GetLocalPlayer(packet.ReadByte());
                loc = packet.ReadInt32();
                seq = packet.ReadInt32();
                rcard = Duel.GetCard(player, (CardLocation)loc, seq);
                UpdateCardByCore(rcard, id, player, loc, seq);
                cards.Add(rcard);
            }
        }

        /// <summary>
        /// Update card data by core,only id,player,loc,seq
        /// </summary>
        private void UpdateCardByCore(ClientCard rcard,int id,int player,int loc,int seq)
        {
            if (CheckGodMode(rcard))
            {

                if (rcard.Sequence < 0)
                {
                    resetCards.Add(rcard);
                }
                rcard.SetId(id);
                rcard.SetController(player);
                rcard.SetLocation((CardLocation)loc);
                rcard.SetSequence(seq);
            }
        }

        private void ReadCard(BinaryReader packet,ClientCard rcard)
        {
            int id = packet.ReadInt32();
            if (id == 0) return;
            int player = Duel.GetLocalPlayer(packet.ReadByte());
            int loc = packet.ReadInt32();
            int seq = packet.ReadInt32();
            rcard = Duel.GetCard(player, (CardLocation)loc, seq);
            UpdateCardByCore(rcard, id, player, loc, seq);
        }

        /// <summary>
        /// Try to manually call this function after operating the card to avoid data confusion
        /// </summary>
        public void RestoreCardsData()
        {
            if (resetCards?.Count <= 0) return;
            foreach (var resetCard in resetCards)
            {
                resetCard.SetController(0);
                resetCard.SetId(0);
                resetCard.SetSequence(-1);
            }
            resetCards.Clear();
        }

        private bool CheckError(DataReader dreader)
        {
            try
            {
                ReceiveType receiveType = (ReceiveType)dreader.Packet.ReadInt32();
                type = receiveType;
                if (receiveType != ReceiveType.Success)
                {
#if DEBUG
                    Logger.DebugWriteLine("Handle data Error!");
                    Logger.DebugWriteLine("Error Type:" + receiveType);
                    FunctionCode.Type func_type = (FunctionCode.Type)dreader.Packet.ReadByte();
                    if (func_type == FunctionCode.Type.Card)
                    {
                        FunctionCode.Card func_code = (FunctionCode.Card)dreader.Packet.ReadUInt16();
                        Logger.DebugWriteLine("Error data is:" + func_type + ":" + func_code);
                    }
                    else if (func_type == FunctionCode.Type.Duel)
                    {
                        FunctionCode.Duel func_code = (FunctionCode.Duel)dreader.Packet.ReadUInt16();
                        Logger.DebugWriteLine("Error data is:" + func_type + ":" + func_code);
                    }
#endif
                    Close(dreader);
                    return false;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Logger.DebugWriteLine("Receive Error:" + e.Message);
#endif
                type = ReceiveType.BufferError;
                Close(dreader);
                return false;
            }
            return true;
        }

        private void DefaultCardFunctionWrite(FunctionCode.Card functionCode,BinaryWriter reply, ClientCard card) 
        {
            reply.Write((byte)FunctionCode.Type.Card);
            reply.Write((short)functionCode);
            WriteCard(reply, card);
        }

        private T ClientErrorReturn<T>(T value)
        {
            type = ReceiveType.ClientError;
#if DEBUG
            Logger.DebugWriteLine("Parameter Error!");
            Logger.DebugWriteLine("Error Type:" + type);
#endif
            return value;
        }

        private IList<int> HandleCardToDoubleInt(FunctionCode.Card functionCode, ClientCard card) 
        {
            IList<int> results = new List<int>();
            if (!CheckGodMode(card)) return ClientErrorReturn(results);
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode,reply, card);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return results;
            int result;
            for (int i = 0; i < 2; ++i)
            {
                result = dreader.Packet.ReadInt32();
                if (result == 0 && (functionCode == FunctionCode.Card.GetCode ||
                    functionCode == FunctionCode.Card.GetOriginalCodeRule ||
                    functionCode == FunctionCode.Card.GetPreviousCodeOnField)) continue;
                results.Add(result);
            }
            Close(dreader);
            return results;
        }

        private int? HandleCardToInt(FunctionCode.Card functionCode, ClientCard card)
        {
            if (!CheckGodMode(card)) return ClientErrorReturn((int?)null);
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, card);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return null;
            int result = dreader.Packet.ReadInt32();
            Close(dreader);
            return result;
        }

        private int? HandleIntToInt(FunctionCode.Card functionCode, ClientCard card,int parameter)
        {
            if (!CheckGodMode(card)) return ClientErrorReturn((int?)null);
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, card);
            reply.Write((byte)parameter);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return null;
            int result = dreader.Packet.ReadInt32();
            Close(dreader);
            return result;
        }

        private IList<int> HandleCardToIntEx(FunctionCode.Card functionCode, ClientCard card)
        {
            IList<int> results = new List<int>();
            if (!CheckGodMode(card)) return ClientErrorReturn(results);
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, card);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return results;
            int count = dreader.Packet.ReadInt32();
            int result;
            for (int i = 0; i < count; ++i)
            {
                result = dreader.Packet.ReadInt32();
                if (result == 0) continue;
            }
            Close(dreader);
            return results;
        }

        private bool HandleCardIntIntExToBool(FunctionCode.Card functionCode, ClientCard card, int parameter, params int[] parameters)
        {
            if (!CheckGodMode(card)) return ClientErrorReturn(false);
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, card);
            int max_parameter_count = parameters.Count() > MAX_EX_PARAMETERS ? MAX_EX_PARAMETERS : parameters.Count();
            reply.Write((parameters.Count() + 1));
            reply.Write(parameter);
            for (int i = 0; i < max_parameter_count; ++i)
            {
                reply.Write(parameters[i]);
            };
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return false;
            byte result = dreader.Packet.ReadByte();
            Close(dreader);
            return result == 1 ? true : false;
        }

        private bool HandleCardIntToBool(FunctionCode.Card functionCode, ClientCard card, int parameter)
        {
            if (!CheckGodMode(card)) return ClientErrorReturn(false);
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, card);
            reply.Write(parameter);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return false;
            byte result = dreader.Packet.ReadByte();
            Close(dreader);
            return result == 1 ? true : false;
        }

        private bool HandleCardToBool(FunctionCode.Card functionCode, ClientCard card)
        {
            if (!CheckGodMode(card)) return ClientErrorReturn(false);
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, card);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return false;
            byte result = dreader.Packet.ReadByte();
            Close(dreader);
            return result == 1 ? true : false;
        }

        private bool HandleDoubleCardToBool(FunctionCode.Card functionCode, ClientCard cardA, ClientCard cardB)
        {
            if (!CheckGodMode(cardA) || !CheckGodMode(cardB)) return ClientErrorReturn(false);
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, cardA);
            WriteCard(reply,cardB);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return false;
            byte result = dreader.Packet.ReadByte();
            Close(dreader);
            return result == 1 ? true : false;
        }

        private ClientCard HandleCardToCard(FunctionCode.Card functionCode, ClientCard card)
        {
            ClientCard rcard = null;
            if (!CheckGodMode(card)) return ClientErrorReturn(rcard);
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, card);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return null;
            ReadCard(dreader.Packet, rcard);
            Close(dreader);
            return rcard;
        }

        private object HandleDuel(FunctionCode.Duel functionCode, params object[] parameters)
        {
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            reply.Write((byte)FunctionCode.Type.Duel);
            reply.Write((short)functionCode);
            switch (functionCode)
            {
                case FunctionCode.Duel.GetLP: case FunctionCode.Duel.GetTurnCount: case FunctionCode.Duel.GetDrawCount:
                case FunctionCode.Duel.GetBattleDamage: case FunctionCode.Duel.GetLinkedZone:case FunctionCode.Duel.GetBattleMonster:
                case FunctionCode.Duel.GetRitualMaterial: case FunctionCode.Duel.IsPlayerCanAdditionalSummon: case FunctionCode.Duel.GetBattledCount:
                    {
                        int player = (int)parameters[0];
                        reply.Write((byte)player);
                    }
                    break;
                case FunctionCode.Duel.GetTurnPlayer: case FunctionCode.Duel.GetCurrentChain: case FunctionCode.Duel.GetCurrentPhase:
                case FunctionCode.Duel.IsDamageCalculated: case FunctionCode.Duel.GetAttacker: case FunctionCode.Duel.GetAttackTarget:
                case FunctionCode.Duel.CheckPhaseActivity: case FunctionCode.Duel.IsAbleToEnterBP:
                    break;
                case FunctionCode.Duel.GetFlagEffect: case FunctionCode.Duel.GetFlagEffectLabel: case FunctionCode.Duel.GetDecktopGroup:
                case FunctionCode.Duel.GetExtraTopGroup: case FunctionCode.Duel.GetFusionMaterial: case FunctionCode.Duel.IsPlayerAffectedByEffect:
                case FunctionCode.Duel.IsPlayerCanDraw: case FunctionCode.Duel.IsPlayerCanDiscardDeck: case FunctionCode.Duel.IsPlayerCanDiscardDeckAsCost:
                case FunctionCode.Duel.IsPlayerCanSpecialSummonCount:
                    {
                        int player = (int)parameters[0];
                        int parameter = (int)parameters[1];
                        reply.Write((byte)player);
                        reply.Write(parameter);
                    }
                    break;
                case FunctionCode.Duel.GetReleaseGroup:
                case FunctionCode.Duel.GetReleaseGroupCount:
                    {
                        int player = (int)parameters[0];
                        bool parameter = (bool)parameters[1];
                        int bparameter = parameter ? 1 : 0;
                        reply.Write((byte)player);
                        reply.Write((byte)bparameter);
                    }
                    break;
                case FunctionCode.Duel.IsCanAddCounter:
                    {
                        int player = (int)parameters[0];
                        int countertype = (int)parameters[1];
                        int count = (int)parameters[2];
                        ClientCard c = (ClientCard)parameters[3];
                        reply.Write((byte)player);
                        if (countertype >= 0 && count >= 0 && c != null)
                        {
                            reply.Write((byte)1);
                            reply.Write(countertype);
                            reply.Write(count);
                            WriteCard(reply, c);
                        }
                        else 
                        {
                            reply.Write((byte)0);
                        }
                    }
                    break;
                case FunctionCode.Duel.IsCanRemoveCounter:
                    {
                        int player = (int)parameters[0];
                        int s = (int)parameters[1];
                        int o = (int)parameters[2];
                        int countertype = (int)parameters[3];
                        int count = (int)parameters[4];
                        int reason = (int)parameters[5];
                        reply.Write((byte)player);
                        reply.Write(s);
                        reply.Write(o);
                        reply.Write(countertype);
                        reply.Write(count);
                        reply.Write(reason);
                    }
                    break;
                case FunctionCode.Duel.GetCounter:
                    {
                        int player = (int)parameters[0];
                        int s = (int)parameters[1];
                        int o = (int)parameters[2];
                        int countertype = (int)parameters[3];
                        reply.Write((byte)player);
                        reply.Write(s);
                        reply.Write(o);
                        reply.Write(countertype);
                    }
                    break;
                case FunctionCode.Duel.IsEnvironment:
                    {
                        int code = (int)parameters[0];
                        int player = (int)parameters[1];
                        int loc = (int)parameters[2];
                        reply.Write(code);
                        reply.Write((byte)player);
                        reply.Write(loc);
                    }
                    break;
                case FunctionCode.Duel.CheckLPCost:
                    {
                        int player = (int)parameters[0];
                        int cost = (int)parameters[1];
                        reply.Write((byte)player);
                        reply.Write(cost);
                    }
                    break;
                case FunctionCode.Duel.CheckSummonedCount:
                case FunctionCode.Duel.GetTributeGroup:
                {
                        ClientCard c = (ClientCard)parameters[0];
                        WriteCard(reply, c);
                    }
                    break;
                case FunctionCode.Duel.GetLocationCount:
                    {
                        int player = (int)parameters[0];
                        int location = (int)parameters[1];
                        int use_player = (int)parameters[2];
                        int reason = (int)parameters[3];
                        int zone = (int)parameters[4];
                        reply.Write((byte)player);
                        reply.Write(location);
                        reply.Write((byte)use_player);
                        reply.Write(reason);
                        reply.Write(zone);
                    }
                    break;
                case FunctionCode.Duel.GetUsableMZoneCount:
                    {
                        int player = (int)parameters[0];
                        int use_player = (int)parameters[1];
                        reply.Write((byte)player);
                        reply.Write((byte)use_player);
                    }
                    break;
                case FunctionCode.Duel.GetLinkedGroup: case FunctionCode.Duel.GetLinkedGroupCount: case FunctionCode.Duel.GetFieldCard:
                case FunctionCode.Duel.CheckLocation: case FunctionCode.Duel.GetFieldGroup: case FunctionCode.Duel.GetFieldGroupCount:
                case FunctionCode.Duel.GetOverlayGroup: case FunctionCode.Duel.GetOverlayCount:
                    {
                        int player = (int)parameters[0];
                        int parameter1 = (int)parameters[1];
                        int parameter2 = (int)parameters[1];
                        reply.Write((byte)player);
                        reply.Write(parameter1);
                        reply.Write(parameter2);
                    }
                    break;
                case FunctionCode.Duel.CheckRemoveOverlayCard:
                    {
                        int player = (int)parameters[0];
                        int s = (int)parameters[1];
                        int o = (int)parameters[2];
                        int count = (int)parameters[3];
                        int reason = (int)parameters[4];
                        reply.Write((byte)player);
                        reply.Write(s);
                        reply.Write(o);
                        reply.Write(count);
                        reply.Write(reason);
                    }
                    break;
                case FunctionCode.Duel.IsPlayerCanSummon:
                case FunctionCode.Duel.IsPlayerCanMSet:
                    {
                        int player = (int)parameters[0];
                        int sumtype = (int)parameters[1];
                        ClientCard c = (ClientCard)parameters[2];
                        reply.Write((byte)player);
                        reply.Write(sumtype);
                        WriteCard(reply, c);
                    }
                    break;
                case FunctionCode.Duel.IsPlayerCanSSet:
                case FunctionCode.Duel.IsPlayerCanRelease:
                case FunctionCode.Duel.IsPlayerCanSendtoHand:
                case FunctionCode.Duel.IsPlayerCanSendtoGrave:
                case FunctionCode.Duel.IsPlayerCanSendtoDeck:
                    {
                        int player = (int)parameters[0];
                        ClientCard c = (ClientCard)parameters[1];
                        reply.Write((byte)player);
                        WriteCard(reply, c);
                    }
                    break;
                case FunctionCode.Duel.IsPlayerCanSpecialSummon:
                    {
                        int player = (int)parameters[0];
                        int sumtype = (int)parameters[1];
                        int sumpos = (int)parameters[2];
                        int target_player = (int)parameters[3];
                        ClientCard c = (ClientCard)parameters[4];
                        reply.Write((byte)player);
                        reply.Write(sumtype);
                        reply.Write(sumpos);
                        reply.Write((byte)target_player);
                        WriteCard(reply, c);
                    }
                    break;
                case FunctionCode.Duel.IsPlayerCanSpecialSummonMonster:
                    {
                        Action<BinaryWriter,int?> WriteIntOrNull = (binaryWriter, value) =>
                        {
                            if (value != null)
                            {
                                binaryWriter.Write((byte)1);
                                binaryWriter.Write((int)value);
                            }
                            else reply.Write((byte)0);
                        };
                        int player = (int)parameters[0];
                        int code = (int)parameters[1];
                        int? setcode = (int?)parameters[2];
                        int? type = (int?)parameters[3];
                        int? atk = (int?)parameters[4];
                        int? def = (int?)parameters[5];
                        int? level = (int?)parameters[6];
                        int? race = (int?)parameters[7];
                        int? attribute = (int?)parameters[8];
                        int pos = (int)parameters[9];
                        int target_player = (int)parameters[10];
                        int sumtype = (int)parameters[11];
                        reply.Write((byte)player);
                        reply.Write(code);
                        WriteIntOrNull(reply, setcode);
                        WriteIntOrNull(reply, type);
                        WriteIntOrNull(reply, atk);
                        WriteIntOrNull(reply, def);
                        WriteIntOrNull(reply, level);
                        WriteIntOrNull(reply, race);
                        WriteIntOrNull(reply, attribute);
                        reply.Write(pos);
                        reply.Write((byte)target_player);
                        reply.Write(sumtype);
                    }
                    break;
                case FunctionCode.Duel.IsPlayerCanRemove:
                    {
                        int player = (int)parameters[0];
                        ClientCard c = (ClientCard)parameters[1];
                        int reason = (int)parameters[2];
                        reply.Write((byte)player);
                        WriteCard(reply, c);
                        reply.Write(reason);
                    }
                    break;
                default:
                    return null;
            }
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return null;
            switch (functionCode)
            {
                case FunctionCode.Duel.GetLP: case FunctionCode.Duel.GetTurnCount: case FunctionCode.Duel.GetDrawCount:
                case FunctionCode.Duel.GetTurnPlayer:case FunctionCode.Duel.GetFlagEffect: case FunctionCode.Duel.GetCounter:
                case FunctionCode.Duel.GetBattleDamage: case FunctionCode.Duel.GetLocationCount: case FunctionCode.Duel.GetUsableMZoneCount:
                case FunctionCode.Duel.GetLinkedGroupCount: case FunctionCode.Duel.GetLinkedZone: case FunctionCode.Duel.GetCurrentChain:
                case FunctionCode.Duel.GetCurrentPhase: case FunctionCode.Duel.GetReleaseGroupCount: case FunctionCode.Duel.GetFieldGroupCount:
                case FunctionCode.Duel.GetOverlayCount: case FunctionCode.Duel.GetBattledCount:
                    {
                        int result = dreader.Packet.ReadInt32();
                        Close(dreader);
                        return result;
                    }
                case FunctionCode.Duel.GetFlagEffectLabel:
                    {
                        IList<int> results = new List<int>();
                        int count = dreader.Packet.ReadInt32();
                        for (int i = 0; i < count; ++i)
                        {
                            results.Add(dreader.Packet.ReadInt32());
                        }
                        Close(dreader);
                        return results;
                    }
                case FunctionCode.Duel.IsCanAddCounter: case FunctionCode.Duel.IsCanRemoveCounter:case FunctionCode.Duel.IsEnvironment:
                case FunctionCode.Duel.CheckLPCost: case FunctionCode.Duel.CheckSummonedCount: case FunctionCode.Duel.CheckLocation:
                case FunctionCode.Duel.IsDamageCalculated: case FunctionCode.Duel.CheckRemoveOverlayCard: case FunctionCode.Duel.IsPlayerAffectedByEffect:
                case FunctionCode.Duel.IsPlayerCanDraw: case FunctionCode.Duel.IsPlayerCanDiscardDeck: case FunctionCode.Duel.IsPlayerCanDiscardDeckAsCost:
                case FunctionCode.Duel.IsPlayerCanSummon: case FunctionCode.Duel.IsPlayerCanMSet: case FunctionCode.Duel.IsPlayerCanSSet:
                case FunctionCode.Duel.IsPlayerCanSpecialSummon: case FunctionCode.Duel.IsPlayerCanSpecialSummonMonster: case FunctionCode.Duel.IsPlayerCanSpecialSummonCount:
                case FunctionCode.Duel.IsPlayerCanRelease: case FunctionCode.Duel.IsPlayerCanSendtoHand: case FunctionCode.Duel.IsPlayerCanSendtoGrave:
                case FunctionCode.Duel.IsPlayerCanSendtoDeck: case FunctionCode.Duel.IsPlayerCanRemove: case FunctionCode.Duel.IsPlayerCanAdditionalSummon:
                case FunctionCode.Duel.CheckPhaseActivity: case FunctionCode.Duel.IsAbleToEnterBP:
                    {
                        byte result = dreader.Packet.ReadByte();
                        Close(dreader);
                        return result == 1 ? true : false;
                    }
                case FunctionCode.Duel.GetLinkedGroup: case FunctionCode.Duel.GetDecktopGroup: case FunctionCode.Duel.GetExtraTopGroup:
                case FunctionCode.Duel.GetReleaseGroup: case FunctionCode.Duel.GetTributeGroup: case FunctionCode.Duel.GetRitualMaterial:
                case FunctionCode.Duel.GetFieldGroup: case FunctionCode.Duel.GetOverlayGroup:
                    {
                        IList<ClientCard> results = new List<ClientCard>();
                        ReadGroup(dreader.Packet, results);
                        Close(dreader);
                        return results;
                    }
                case FunctionCode.Duel.GetBattleMonster:
                    {
                        IList<ClientCard> results = new List<ClientCard>();
                        ClientCard rcard = null;
                        byte not_null = dreader.Packet.ReadByte();
                        if (not_null == 1)
                        {
                            ReadCard(dreader.Packet, rcard);
                            results.Add(rcard);
                            ReadCard(dreader.Packet, rcard);
                            results.Add(rcard);
                            Close(dreader);
                            return results;
                        }
                        else
                        {
                            Close(dreader);
                            return null;
                        }
                    }
                case FunctionCode.Duel.GetFieldCard: case FunctionCode.Duel.GetAttacker: case FunctionCode.Duel.GetAttackTarget:
                    {
                        ClientCard rcard = null;
                        ReadCard(dreader.Packet, rcard);
                        Close(dreader);
                        return rcard;
                    }
                case FunctionCode.Duel.GetFusionMaterial:
                    {
                        IList<IList<ClientCard>> results = new List<IList<ClientCard>>();
                        results.Add(new List<ClientCard>());
                        results.Add(new List<ClientCard>());
                        ReadGroup(dreader.Packet, results[0]);
                        ReadGroup(dreader.Packet, results[1]);
                        Close(dreader);
                        return results;
                    }
                default:
                    return null;
            }
        }

        private object HandleCardSpecial(FunctionCode.Card functionCode, ClientCard card, params object[] parameters)
        {
            if (!CheckGodMode(card)) return ClientErrorReturn(card);
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, card);
            switch (functionCode)
            {
                case FunctionCode.Card.IsSummonable:
                case FunctionCode.Card.IsMSetable:
                    {
                        byte ignore_count = (bool)parameters[0] ? (byte)1 : (byte)0;
                        int minc = (int)parameters[1];
                        int zone = (int)parameters[2];
                        reply.Write(ignore_count);
                        reply.Write(minc);
                        reply.Write(zone);
                    }
                    break;
                case FunctionCode.Card.IsXyzSummonable:
                    {
                        int minc = (int)parameters[0];
                        int maxc = (int)parameters[1];
                        reply.Write(minc);
                        reply.Write(maxc);
                    }
                    break;
                case FunctionCode.Card.IsHasEffect:
                    {
                        int code = (int)parameters[0];
                        int check_player = (int)parameters[1];
                        reply.Write(code);
                        reply.Write((byte)check_player);
                    }
                    break;
                case FunctionCode.Card.IsLinkSummonable:
                    {
                        ClientCard lcard = (ClientCard)parameters[0];
                        int minc = (int)parameters[1];
                        int maxc = (int)parameters[2];
                        WriteCard(reply, lcard);
                        reply.Write(minc);
                        reply.Write(maxc);
                    }
                    break;
                case FunctionCode.Card.GetFlagEffectLabel:
                    {
                        int code = (int)parameters[0];
                        reply.Write(code);
                    }
                    break;
                case FunctionCode.Card.GetColumnZone:
                    {
                        int location = (int)parameters[0];
                        int player = (int)parameters[1];
                        reply.Write(location);
                        reply.Write(player);
                    }
                    break;
                case FunctionCode.Card.GetOverlayGroup:
                case FunctionCode.Card.GetAttackableTarget:
                    break;
                case FunctionCode.Card.CheckRemoveOverlayCard:
                    {
                        int player = (int)parameters[0];
                        int count = (int)parameters[1];
                        int reason = (int)parameters[2];
                        reply.Write((byte)player);
                        reply.Write(count);
                        reply.Write(reason);
                    }
                    break;
                case FunctionCode.Card.IsXyzLevel:
                    {
                        ClientCard xyzc = (ClientCard)parameters[0];
                        if (!CheckGodMode(xyzc)) return null;
                        int lv = (int)parameters[1];
                        WriteCard(reply,xyzc);
                        reply.Write(lv);
                    }
                    break;
                case FunctionCode.Card.IsSSetable:
                    {
                        bool ignore_field = (bool)parameters[0];
                        int b_ignore_field = ignore_field ? 1 : 0;
                        reply.Write((byte)b_ignore_field);
                    }
                    break;
                case FunctionCode.Card.IsAbleToRemove:
                    {
                        int player = (int)parameters[0];
                        int pos = (int)parameters[1];
                        int reason = (int)parameters[2];
                        reply.Write((byte)player);
                        reply.Write(pos);
                        reply.Write(reason);
                    }
                    break;
                case FunctionCode.Card.IsChainAttackable:
                    {
                        int ac = (int)parameters[0];
                        bool monsteronly = (bool)parameters[1];
                        int bmonsteronly = monsteronly ? 1 : 0;
                        reply.Write(ac);
                        reply.Write((byte)bmonsteronly);
                    }
                    break;
                case FunctionCode.Card.IsControlerCanBeChanged:
                    {
                        bool ignore_mzone = (bool)parameters[0];
                        int zone = (int)parameters[1];
                        int bignore_mzone = ignore_mzone ? 1 : 0;
                        reply.Write((byte)bignore_mzone);
                        reply.Write(zone);
                    }
                    break;
                case FunctionCode.Card.IsCanAddCounter:
                    {
                        int countertype = (int)parameters[0];
                        int count = (int)parameters[1];
                        bool singly = (bool)parameters[2];
                        int location = (int)parameters[3];
                        int bsingly = singly ? 1 : 0;
                        reply.Write(countertype);
                        reply.Write(count);
                        reply.Write((byte)bsingly);
                        reply.Write(location);
                    }
                    break;
                case FunctionCode.Card.IsCanRemoveCounter:
                    {
                        int player = (int)parameters[0];
                        int countertype = (int)parameters[1];
                        int count = (int)parameters[2];
                        int reason = (int)parameters[3];
                        reply.Write((byte)player);
                        reply.Write(countertype);
                        reply.Write(count);
                        reply.Write(reason);
                    }
                    break;
                case FunctionCode.Card.IsCanBeFusionMaterial:
                    {
                        ClientCard fc = (ClientCard)parameters[0];
                        int summon_type = (int)parameters[1];
                        WriteCard(reply, fc);
                        reply.Write(summon_type);
                    }
                    break;
                case FunctionCode.Card.IsCanBeSynchroMaterial:
                    {
                        ClientCard fc = (ClientCard)parameters[0];
                        ClientCard tuner = (ClientCard)parameters[1];
                        WriteCard(reply, fc);
                        WriteCard(reply, tuner);
                    }
                    break;
                case FunctionCode.Card.IsCanBeRitualMaterial:
                case FunctionCode.Card.IsCanBeXyzMaterial:
                case FunctionCode.Card.IsCanBeLinkMaterial:
                    {
                        ClientCard sc = (ClientCard)parameters[0];
                        WriteCard(reply, sc);
                    }
                    break;
                case FunctionCode.Card.CheckUniqueOnField:
                    {
                        int check_player = (int)parameters[0];
                        int check_location = (int)parameters[1];
                        ClientCard icard = (ClientCard)parameters[2];
                        reply.Write((byte)check_player);
                        reply.Write(check_location);
                        WriteCard(reply, icard);
                    }
                    break;
                default:
                    return null;
            }
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return null;
            switch (functionCode)
            {
                case FunctionCode.Card.IsSummonable:case FunctionCode.Card.IsMSetable: case FunctionCode.Card.IsXyzSummonable:
                case FunctionCode.Card.IsLinkSummonable: case FunctionCode.Card.CheckRemoveOverlayCard: case FunctionCode.Card.IsXyzLevel:
                case FunctionCode.Card.IsHasEffect: case FunctionCode.Card.IsSSetable: case FunctionCode.Card.IsAbleToRemove:
                case FunctionCode.Card.IsChainAttackable: case FunctionCode.Card.IsControlerCanBeChanged: case FunctionCode.Card.IsCanAddCounter:
                case FunctionCode.Card.IsCanRemoveCounter: case FunctionCode.Card.IsCanBeFusionMaterial: case FunctionCode.Card.IsCanBeSynchroMaterial:
                case FunctionCode.Card.IsCanBeRitualMaterial: case FunctionCode.Card.IsCanBeXyzMaterial: case FunctionCode.Card.IsCanBeLinkMaterial:
                case FunctionCode.Card.CheckUniqueOnField:
                    {
                        byte result = dreader.Packet.ReadByte();
                        Close(dreader);
                        return result == 1 ? true : false;
                    }
                case FunctionCode.Card.GetFlagEffectLabel:
                    {
                        IList<int> results = new List<int>();
                        int count = dreader.Packet.ReadInt32();
                        for (int i = 0; i < count; ++i)
                        {
                            results.Add(dreader.Packet.ReadInt32());
                        }
                        Close(dreader);
                        return results;
                    }
                case FunctionCode.Card.GetColumnZone:
                    {
                        int result = dreader.Packet.ReadInt32();
                        Close(dreader);
                        return result;
                    }
                case FunctionCode.Card.GetOverlayGroup:
                    {
                        IList<ClientCard> cards = new List<ClientCard>();
                        int count = dreader.Packet.ReadInt32();
                        int id;
                        int player;
                        int loc;
                        int seq;
                        ClientCard rcard;
                        for (int i = 0; i < count; ++i)
                        {
                            id = dreader.Packet.ReadInt32();
                            player = Duel.GetLocalPlayer(dreader.Packet.ReadByte());
                            if (player < 0 || player > 1)
                            {
                                player = GetControler(card);
                            }
                            loc = dreader.Packet.ReadInt32();
                            seq = dreader.Packet.ReadInt32();
                            rcard = Duel.GetCard(player, (int)((CardLocation)loc|CardLocation.MonsterZone), card.Sequence, seq);
                            UpdateCardByCore(rcard, id, player, loc, seq);
                            cards.Add(rcard);
                        }
                        Close(dreader);
                        return cards;
                    }
                case FunctionCode.Card.GetAttackableTarget:
                    {
                        object[] results = new object[2];
                        IList<ClientCard> cards = new List<ClientCard>();
                        results[0] = dreader.Packet.ReadByte();
                        int count = dreader.Packet.ReadInt32();
                        ClientCard rcard = null;
                        for (int i = 0; i < count; ++i)
                        {
                            ReadCard(dreader.Packet, rcard);
                            cards.Add(rcard);
                        }
                        Close(dreader);
                        results[1] = cards;
                        return results;
                    }
                default:
                    return null;
            }
        }

        private int? HandleDoubleCardToInt(FunctionCode.Card functionCode, ClientCard cardA, ClientCard cardB)
        {
            if (!CheckGodMode(cardA) || !CheckGodMode(cardB)) return ClientErrorReturn((int?)null); ;
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, cardA);
            WriteCard(reply, cardB);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return null;
            int result = dreader.Packet.ReadInt32();
            Close(dreader);
            return result;
        }

        private int? HandleCardIntToInt(FunctionCode.Card functionCode, ClientCard card, int parameter)
        {
            if (!CheckGodMode(card)) return ClientErrorReturn((int?)null); ;
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, card);
            reply.Write(parameter);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return null;
            int result = dreader.Packet.ReadInt32();
            Close(dreader);
            return result;
        }

        private IList<ClientCard> HandleCardToGroup(FunctionCode.Card functionCode, ClientCard card)
        {
            IList<ClientCard> cards = new List<ClientCard>();
            if (!CheckGodMode(card)) return ClientErrorReturn(cards); ;
            BinaryWriter reply = GamePacketFactory.Create(CtosMessage.AiRequest);
            DefaultCardFunctionWrite(functionCode, reply, card);
            Connection.Send(reply, true);
            DataReader dreader = GetDataReader(Data);
            if (!CheckError(dreader)) return cards;
            ReadGroup(dreader.Packet, cards);
            Close(dreader);
            return cards;
        }

        //=================== Card ===================

        /// <summary>
        /// Returns the current codename for card (possibly because the effect changes)
        /// 返回卡片 c 的当前卡号（可能因为效果改变）
        /// </summary>
        public IList<int> GetCode(ClientCard c)
        {
            return HandleCardToDoubleInt(FunctionCode.Card.GetCode, c);
        }

        /// <summary>
        /// Returns the codename on the c rule (used as a card rule on this card)
        /// 返回卡片 c 规则上的代号（这张卡规则上当作XXX使用）
        /// </summary>
        public IList<int> GetOriginalCodeRule(ClientCard c)
        {
            return HandleCardToDoubleInt(FunctionCode.Card.GetOriginalCodeRule, c);
        }

        /// <summary>
        ///Returns the card number prior to the c position change
        /// 返回卡片 c 位置变化之前在场上的卡号
        /// </summary>
        public IList<int> GetPreviousCodeOnField(ClientCard c)
        {
            return HandleCardToDoubleInt(FunctionCode.Card.GetPreviousCodeOnField, c);
        }

        /// <summary>
        /// Returns the number of ally cards for the current device
        /// 返回卡片 c 当前装备的同盟卡数量，第二个返回值是 旧同盟 的数量 
        /// </summary>
        public IList<int> GetUnionCount(ClientCard c)
        {
            return HandleCardToDoubleInt(FunctionCode.Card.GetUnionCount, c);
        }

        /// <summary>
        /// Returns the minimum and maximum number of sacrifices required to normally call c
        /// 返回通常召唤卡片 c 所需要的祭品的最小和最大数量
        /// </summary>
        public IList<int> GetTributeRequirement(ClientCard c)
        {
            return HandleCardToDoubleInt(FunctionCode.Card.GetTributeRequirement, c);
        }


        /// <summary>
        /// Returns the code number of the card in c
        /// 返回卡片 c 的卡片记载的卡号
        /// </summary>
        public int GetOriginalCode(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOriginalCode, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the Type property
        /// 返回卡片 c 的当前类型
        /// </summary>
        public int GetType(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetType, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the type of card entry for c
        /// 返回卡片 c 的卡片记载的类型
        /// </summary>
        public int GetOriginalType(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOriginalType, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// 返回卡片 c 用作融合素材时的类型（与GetType的区别在于对于魔陷区的怪兽卡，返回其原本类型
        /// </summary>
        public int GetFusionType(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetFusionType, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// 返回卡片 c 用作同调素材时的类型（与GetType的区别在于对于魔陷区的怪兽卡，返回其原本类型）
        /// </summary>
        public int GetSynchroType(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetSynchroType, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// 返回卡片 c 用作同调素材时的类型（与GetType的区别在于对于魔陷区的怪兽卡，返回其原本类型）
        /// </summary>
        public int GetXyzType(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetXyzType, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// 返回卡片 c 用作连接素材时的类型（与GetType的区别在于对于魔陷区的怪兽卡，返回其原本类型）
        /// </summary>
        public int GetLinkType(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetLinkType, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        ///Returns the current level of c
        /// 返回卡片 c 的当前等级
        /// </summary>
        public int GetLevel(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetLevel, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the current class of c
        /// 返回卡片 c 的当前阶级
        /// </summary>
        public int GetRank(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetRank, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// 返回卡片 c 的连接标记数量
        /// </summary>
        public int GetLink(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetLink, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the rank of the card in c
        /// 返回卡片 c 的卡片记载的等级
        /// </summary>
        public int GetOriginalLevel(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOriginalLevel, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the class of the card in c
        /// 返回卡片 c 的卡片记载的阶级
        /// </summary>
        public int GetOriginalRank(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOriginalRank, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the left-hand pendulum scale of c
        /// 返回卡片 c 的左灵摆刻度
        /// </summary>
        public int GetLeftScale(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetLeftScale, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the original left-hand pendulum scale of c
        /// 返回卡片 c 的原本的左灵摆刻度
        /// </summary>
        public int GetOriginalLeftScale(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOriginalLeftScale, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the right-hand pendulum scale of c
        /// 返回卡片 c 的右灵摆刻度
        /// </summary>
        public int GetRightScale(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetRightScale, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the original right pendulum scale of c
        /// 返回卡片 c 的原本的右灵摆刻度
        /// </summary>
        public int GetOriginalRightScale(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOriginalRightScale, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// 返回卡片 c 的当前灵摆刻度若 c 在左侧的灵摆区，则返回左灵摆刻度；否则返回右灵摆刻度
        /// </summary>
        public int GetCurrentScale(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetCurrentScale, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// 返回卡片 c 的连接区的卡片组的卡的数量（目前只限怪兽区）
        /// </summary>
        public int GetLinkedGroupCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetLinkedGroupCount, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// 返回和卡片 c 互相连接状态的卡片组的数量
        /// </summary>
        public int GetMutualLinkedGroupCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetMutualLinkedGroupCount, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// 返回与卡片 c 同一纵列的 c 以外的卡片的数量
        /// </summary>
        public int GetColumnGroupCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetColumnGroupCount, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the current attribute of c
        /// 返回卡片 c 的当前属性 注：对某些多属性怪兽如光与暗之龙，此函数的返回值可能是几个属性的组合值
        /// </summary>
        public int GetAttribute(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetAttribute, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the attributes of the card that c is describing
        /// 返回卡片 c 的卡片记载的属性
        /// </summary>
        public int GetOriginalAttribute(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOriginalAttribute, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the current race of c
        /// 返回卡片 c 的当前种族 注：对某些多种族怪兽如动画效果的魔术猿，此函数的返回值可能是几个种族的组合值
        /// </summary>
        public int GetRace(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetRace, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the c of the card record race
        /// 返回卡片 c 的卡片记载的种族
        /// </summary>
        public int GetOriginalRace(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOriginalRace, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Return the current attack power of c, the return value is a negative that is "?"
        /// 返回卡片 c 的当前攻击力
        /// </summary>
        public int GetAttack(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetAttack, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Return the original attack power of c
        /// 返回卡片 c 的原本攻击力
        /// </summary>
        public int GetBaseAttack(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetBaseAttack, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the attack power recorded on card c
        /// 返回卡片 c 的卡片记载的攻击力，返回值是负数表示是"?"
        /// </summary>
        public int GetTextAttack(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetTextAttack, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Return to the original c of the defense
        /// Returns the current defense of c, the return value is negative that is "?"
        /// </summary>
        public int GetDefense(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetDefense, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Return to the original c of the defense
        /// 返回卡片 c 的原本守备力
        /// </summary>
        public int GetBaseDefense(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetBaseDefense, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Return the c 's card record of the garrison
        /// 返回卡片 c 的卡片记载的守备力，返回值是负数表示是"?"
        /// </summary>
        public int GetTextDefense(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetTextDefense, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the type before the c position change
        /// 返回卡片 c 位置变化之前在场上的类型
        /// </summary>
        public int GetPreviousTypeOnField(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPreviousTypeOnField, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the rank before the c position change
        /// 返回卡片 c 位置变化之前在场上的等级
        /// </summary>
        public int GetPreviousLevelOnField(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPreviousLevelOnField, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the class before the c position changes
        /// 返回卡片 c 位置变化之前在场上的阶级
        /// </summary>
        public int GetPreviousRankOnField(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPreviousRankOnField, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the attribute before the c position change
        /// 返回卡片 c 位置变化之前在场上的属性
        /// </summary>
        public int GetPreviousAttributeOnField(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPreviousAttributeOnField, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the attribute before the c position change
        /// 返回卡片 c 位置变化之前在场上的种族
        /// </summary>
        public int GetPreviousRaceOnField(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPreviousRaceOnField, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the attack power before the c position changes
        /// 返回卡片 c 位置变化之前在场上的攻击力
        /// </summary>
        public int GetPreviousAttackOnField(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPreviousAttackOnField, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the defensive power before the c position changes
        /// 返回卡片 c 位置变化之前在场上的守备力
        /// </summary>
        public int GetPreviousDefenseOnField(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPreviousDefenseOnField, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the holder of c
        /// 返回卡片 c 的持有者
        /// </summary>
        public int GetOwner(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOwner, c);
            if (result == null) return PLAYER_NONE;
            return (int)result;
        }

        /// <summary>
        /// Returns the current controller of c
        /// 返回卡片 c 的当前控制者
        /// </summary>
        public int GetControler(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetControler, c);
            if (result == null) return PLAYER_NONE;
            return (int)result;
        }

        /// <summary>
        /// Returns the controller before the position change of c
        /// 返回卡片 c 的位置变化之前的控制者
        /// </summary>
        public int GetPreviousControler(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPreviousControler, c);
            if (result == null) return PLAYER_NONE;
            return (int)result;
        }

        /// <summary>
        /// Returns the position change reason for c
        /// 返回卡片 c 的位置变化原因
        /// </summary>
        public int GetReason(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetReason, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the player that caused the position of c to change
        /// 返回导致卡片 c 的位置变化的玩家
        /// </summary>
        public int GetReasonPlayer(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetReasonPlayer, c);
            if (result == null) return PLAYER_NONE;
            return (int)result;
        }

        /// <summary>
        /// Returns the current representation of c
        /// 返回卡片 c 当前的表示形式
        /// </summary>
        public int GetPosition(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPosition, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the representation of the position before the c position changes
        /// 返回卡片 c 位置变化前的表示形式
        /// </summary>
        public int GetPreviousPosition(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPreviousPosition, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the representation of c before this battle occurs
        /// 返回卡片 c 在本次战斗发生之前的表示形式
        /// </summary>
        public int GetBattlePosition(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetBattlePosition, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the current position of c
        /// 返回卡片 c 当前的所在位置
        /// </summary>
        public int GetLocation(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetLocation, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the position before the c position changes
        /// 返回卡片 c 位置变化前的所在的位置
        /// </summary>
        public int GetPreviousLocation(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetPreviousLocation, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the serial number of the current position
        /// 返回卡片 c 在当前位置的序号 在场上时，序号代表所在的格子，从左往右分别是0-4，5-6，场地魔法格的序号为5，左右灵摆区域为6-7在其它地方时，序号表示的是第几张卡，最下面的卡的序号为0
        /// </summary>
        public int GetSequence(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetSequence, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the call / special call of c
        /// 返回卡片 c 的召唤/特殊召唤的方式
        /// </summary>
        public int GetSummonType(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetSummonType, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the call position of c
        /// 返回卡片 c 的召唤/特殊召唤的位置
        /// </summary>
        public int GetSummonLocation(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetSummonLocation, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Return to Summoner / Special Summoner c Players
        /// 返回召唤/特殊召唤 c 上场的玩家
        /// </summary>
        public int GetSummonPlayer(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetSummonPlayer, c);
            if (result == null) return PLAYER_NONE;
            return (int)result;
        }

        /// <summary>
        /// Returns the destination of the c position change
        /// 返回卡片 c 位置变化的目的地此函数仅在处理位置转移代替效果时有效
        /// </summary>
        public int GetDestination(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetDestination, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Returns the destination of an effect (such as the universe) that was changed as a result of departure from c
        /// 返回卡片 c 离场时因改变去向的效果（如大宇宙）的目的地
        /// </summary>
        public int GetLeaveFieldDest(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetLeaveFieldDest, c);
            if (result == null) return 0;
            return (int)result;
        }

        /// <summary>
        /// Return c The round to the current position
        /// 返回卡片 c 转移到当前位置的回合
        /// </summary>
        public int GetTurnID(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetTurnID, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the time stamp of the transition to the current position
        /// 返回卡片 c 转移到当前位置的时间标识此数值唯一，越小表示c是越早出现在那个位置卡片从里侧翻开也会改变此数值
        /// </summary>
        public int GetFieldID(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetFieldID, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the actual timestamp for the c transition to the current position
        /// 返回卡片 c 转移到当前位置的真实的时间标识卡片从里侧翻开不会改变此数值
        /// </summary>
        public int GetRealFieldID(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetRealFieldID, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the round counter for c
        /// 返回卡片 c 的回合计数器
        /// </summary>
        public int GetTurnCounter(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetTurnCounter, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the amount of material used for c appearance
        /// 返回卡片 c 出场使用的素材数量
        /// </summary>
        public int GetMaterialCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetMaterialCount, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the number of cards currently loaded
        /// 返回卡片 c 当前装备着的卡片数量
        /// </summary>
        public int GetEquipCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetEquipCount, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the number of cards currently stacked
        /// 返回卡片 c 当前叠放着的卡片数量
        /// </summary>
        public int GetOverlayCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOverlayCount, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the number of cards that have been attacked this turn
        /// 返回卡片 c 本回合攻击过的卡片数量
        /// </summary>
        public int GetAttackedGroupCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetAttackedGroupCount, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the number of times this round has been attacked
        /// 返回卡片 c 本回合攻击过的次数 注：如果此值与 Card.GetAttackedGroupCount(c) 的返回值不同，那么说明卡片c本回合进行过直接攻击
        /// </summary>
        public int GetAttackedCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetAttackedCount, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// 返回卡片 c 本回合攻击宣言的次数 注：攻击被无效不会被计入攻击过的次数，但是会计入攻击宣言的次数
        /// </summary>
        public int GetAttackAnnouncedCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetAttackAnnouncedCount, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the number of current persistent objects
        /// 返回卡片 c 当前的永续对象的数量
        /// </summary>
        public int GetCardTargetCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetCardTargetCount, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the number of cards that take c as the persistent object
        /// 返回取卡片 c 作为永续对象的卡的数量
        /// </summary>
        public int GetOwnerTargetCount(ClientCard c)
        {
            int? result = HandleCardToInt(FunctionCode.Card.GetOwnerTargetCount, c);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Return c as the fusion material when the card number (including the original c card number)
        /// 返回卡片 c 作为融合素材时的卡号。第2个返回值开始是其他的卡号
        /// </summary>
        public IList<int> GetFusionCode(ClientCard c)
        {
            return HandleCardToIntEx(FunctionCode.Card.GetFusionCode, c);
        }

        /// <summary>
        /// 返回卡片 c 作为连接素材的卡号。第2个返回值开始是其他的卡号
        /// </summary>
        public IList<int> GetLinkCode(ClientCard c)
        {
            return HandleCardToIntEx(FunctionCode.Card.GetLinkCode, c);
        }

        /// <summary>
        /// Check the c as a fusion material can be used as the card number for the code card
        /// 检查卡片 c 作为融合素材时能否当作卡号为 code 的卡，额外参数是其他 code
        /// </summary>
        public bool IsFusionCode(ClientCard c,int code, params int[] ex_code)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsFusionCode, c, code, ex_code);
        }

        /// <summary>
        /// 检查卡片 c 作为连接素材时能否当作卡号为 code 的卡，额外参数是其他 code
        /// </summary>
        public bool IsLinkCode(ClientCard c, int code, params int[] ex_code)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsLinkCode, c, code, ex_code);
        }

        /// <summary>
        /// Check whether c is a card with the name setname
        /// 检查卡片 c 是否是卡名含有 setname 的卡(也就是字段)，额外参数是其他 setname
        /// </summary>
        public bool IsSetCard(ClientCard c, int setcode, params int[] ex_setcode)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsSetCard, c, setcode, ex_setcode);
        }

        /// <summary>
        /// 检查卡片 c 是否是原本卡名含有 setname 的卡(也就是字段)
        /// </summary>
        public bool IsOriginalSetCard(ClientCard c, int setcode, params int[] ex_setcode)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsOriginalSetCard, c, setcode, ex_setcode);
        }

        /// <summary>
        /// Check whether the name c contains a setname before the c position changes
        /// 检查卡片 c 位置变化之前是否是名字含有 setname 的卡(也就是字段)
        /// </summary>
        public bool IsPreviousSetCard(ClientCard c, int setcode, params int[] ex_setcode)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsPreviousSetCard, c, setcode, ex_setcode);
        }

        /// <summary>
        /// Check the c as a fusion material can be used as a name setname card
        /// 检查卡片 c 作为融合素材时能否当作名字含有 setname 的卡(也就是字段)
        /// </summary>
        public bool IsFusionSetCard(ClientCard c, int setcode, params int[] ex_setcode)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsFusionSetCard, c, setcode, ex_setcode);
        }

        /// <summary>)
        /// 检查卡片 c 作为连接素材时能否当作名字含有 setname 的卡(也就是字段)
        /// </summary>
        public bool IsLinkSetCard(ClientCard c, int setcode, params int[] ex_setcode)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsLinkSetCard, c, setcode, ex_setcode);
        }

        /// <summary>
        /// 检查卡片 c 的卡号是否规则上（就是CDB里显示的卡密）是 code1[, 或者为 code2...]
        /// </summary>
        public bool IsOriginalCodeRule(ClientCard c, int code, params int[] ex_code)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsOriginalCodeRule, c, code, ex_code);
        }

        /// <summary>
        /// Check whether the card number c is code1 [, or code2 ...]
        /// 检查卡片 c 的卡号是否是 code1[, 或者为 code2...]
        /// </summary>
        public bool IsCode(ClientCard c, int code, params int[] ex_code)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsCode, c, code, ex_code);
        }

        /// <summary>
        /// 检查卡片 c 是否是等级 level1[, 或者为 level2...]
        /// </summary>
        public bool IsLevel(ClientCard c, int level, params int[] ex_level)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsLevel, c, level, ex_level);
        }

        /// <summary>
        /// 检查卡片 c 是否是阶级 rank1[, 或者为 rank2...]
        /// </summary>
        public bool IsRank(ClientCard c, int rank, params int[] ex_rank)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsRank, c, rank, ex_rank);
        }

        /// <summary>
        /// 检查卡片 c 的连接标记数量是否是 link1[, 或者为 link2...]
        /// </summary>
        public bool IsLink(ClientCard c, int link, params int[] ex_link)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsLink, c, link, ex_link);
        }

        /// <summary>
        /// 检查卡片 c 的攻击力是否是 atk1[, 或者为 atk2...]，如果c不是怪兽卡，或者不在 LOCATION_MZONE 则都返回false
        /// </summary>
        public bool IsAttack(ClientCard c, int attack, params int[] ex_attack)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsAttack, c, attack, ex_attack);
        }

        /// <summary>
        /// 检查卡片 c 的守备力是否是 def1[, 或者为 def2...]，如果c不是怪兽卡，或者不在 LOCATION_MZONE 则都返回false
        /// </summary>
        public bool IsDefense(ClientCard c, int defense, params int[] ex_defense)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsDefense, c, defense, ex_defense);
        }

        /// <summary>
        /// 检查卡片 c [由 player 连接召唤时]用作连接素材时是否属于种族 race
        /// </summary>
        public bool IsLinkRace(ClientCard c, int race, int player = PLAYER_NONE)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsLinkRace, c, race, player);
        }

        /// <summary>
        /// 检查卡片 c [由 player 融合召唤时]用作融合素材是否属于属性 attribute
        /// </summary>
        public bool IsFusionAttribute(ClientCard c, int attribute, int player = PLAYER_NONE)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsFusionAttribute, c, attribute, player);
        }

        /// <summary>
        /// 检查卡片 c [由 player 连接召唤时]用作连接素材是否属于属性 attribute
        /// </summary>
        public bool IsLinkAttribute(ClientCard c, int attribute, int player = PLAYER_NONE)
        {
            return HandleCardIntIntExToBool(FunctionCode.Card.IsLinkAttribute, c, attribute, player);
        }

        /// <summary>
        /// Return to the co-ordination of c with the call monster sc with the level
        /// 返回卡片 c 的对于同调怪兽 sc 的同调用等级此函数除了某些特定卡如调节支援士，返回值与Card.GetLevel(c) 相同
        /// </summary>
        public int GetSynchroLevel(ClientCard c, ClientCard sc)
        {
            int? result =  HandleDoubleCardToInt(FunctionCode.Card.GetSynchroLevel, c, sc);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the ritual liberation level for rc ritual monsters
        /// 返回卡片 c 的对于仪式怪兽 rc 仪式解放等级此函数除了某些特定卡如仪式供物，返回值与Card.GetLevel(c) 相同
        /// </summary>
        public int GetRitualLevel(ClientCard c, ClientCard sc)
        {
            int? result = HandleDoubleCardToInt(FunctionCode.Card.GetRitualLevel, c, sc);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// 检查卡片 c 是否是连接标记为 dir 的卡
        /// </summary>
        public bool IsLinkMarker(ClientCard c, int dir)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsLinkMarker, c, dir);
        }

        /// <summary>
        /// Check if c is of type type
        /// 检查卡片 c 是否属于类型 type
        /// </summary>
        public bool IsType(ClientCard c, int type)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsType, c, type);
        }

        /// <summary>
        /// 检查卡片 c 用作融合素材时是否属于类型 type （与IsType的区别在于对于魔陷区的怪兽卡，用其原本类型作判断）
        /// </summary>
        public bool IsFusionType(ClientCard c, int type)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsFusionType, c, type);
        }

        /// <summary>
        /// 检查卡片 c 用作同调素材时是否属于类型 type （与IsType的区别在于对于魔陷区的怪兽卡，用其原本类型作判断）
        /// </summary>
        public bool IsSynchroType(ClientCard c, int type)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsSynchroType, c, type);
        }

        /// <summary>
        /// 检查卡片 c 用作超量素材时是否属于类型 type （与IsType的区别在于对于魔陷区的怪兽卡，用其原本类型作判断）
        /// </summary>
        public bool IsXyzType(ClientCard c, int type)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsXyzType, c, type);
        }

        /// <summary>
        /// 检查卡片 c 用作连接素材时是否属于类型 type （与IsType的区别在于对于魔陷区的怪兽卡，用其原本类型作判断）
        /// </summary>
        public bool IsLinkType(ClientCard c, int type)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsLinkType, c, type);
        }

        /// <summary>
        /// Check if c is race race
        /// 检查卡片 c 是否属于种族 race
        /// </summary>
        public bool IsRace(ClientCard c, int race)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsRace, c, race);
        }

        /// <summary>
        /// Check whether c belongs to attribute attribute
        /// 检查卡片 c 是否属于属性 attribute
        /// </summary>
        public bool IsAttribute(ClientCard c, int attribute)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsAttribute, c, attribute);
        }

        /// <summary>
        /// Check if c contains the reason
        /// 检查卡片 c 是否包含原因 reason
        /// </summary>
        public bool IsReason(ClientCard c, int reason)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsReason, c, reason);
        }

        /// <summary>
        /// 检查卡片 c 的召唤类型是否是 sumtype
        /// </summary>
        public bool IsSummonType(ClientCard c, int sumtype)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsSummonType, c, sumtype);
        }

        /// <summary>
        /// 检查卡片 c 的召唤区域是否是 tloc
        /// </summary>
        public bool IsSummonLocation(ClientCard c, int tloc)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsSummonLocation, c, tloc);
        }

        /// <summary>
        ///检查卡片 c 的召唤玩家是否是 sumplayer
        /// </summary>
        public bool IsSummonPlayer(ClientCard c, int sumplayer)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsSummonPlayer, c, sumplayer);
        }

        /// <summary>
        /// Check whether c contains a status code
        ///检查卡片 c 是否包含某个状态码 status
        /// </summary>
        public bool IsStatus(ClientCard c, int status)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsStatus, c, status);
        }

        /// <summary>
        /// Check if c is linked to chained chainc
        ///检查卡片 c 是否和连锁 chainc 有联系。 chainc==0 表示当前连锁
        /// </summary>
        public bool IsRelateToChain(ClientCard c, int chainc)
        {
            //不随意调用
            return HandleCardIntToBool(FunctionCode.Card.IsRelateToChain, c, chainc);
        }

        /// <summary>
        /// Check whether c is the representation pos
        ///检查卡片 c 是否是表示形式 pos
        /// </summary>
        public bool IsPosition(ClientCard c, int pos)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsPosition, c, pos);
        }

        /// <summary>
        /// Check whether the c position before the change is represented by pos
        ///检查卡片 c 位置变化之前是否是表示形式 pos
        /// </summary>
        public bool IsPreviousPosition(ClientCard c, int pos)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsPreviousPosition, c, pos);
        }

        /// <summary>
        /// Check whether the current control of c is a controler
        ///检查卡片 c 的当前控制着是否是 controler
        /// </summary>
        public bool IsControler(ClientCard c, int controler)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsControler, c, controler);
        }

        /// <summary>
        ///检查卡片 c 的当前控制着是否是 controler
        /// </summary>
        public bool IsPreviousControler(ClientCard c, int controler)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsPreviousControler, c, controler);
        }

        /// <summary>
        /// Check if c is the current location
        ///检查卡片 c 当前位置是否是 location
        /// </summary>
        public bool IsLocation(ClientCard c, int location)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsLocation, c, location);
        }

        /// <summary>
        /// Check if the location before c is location
        ///检查卡片 c 之前的位置是否是 location
        /// </summary>
        public bool IsPreviousLocation(ClientCard c, int location)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsPreviousLocation, c, location);
        }

        /// <summary>
        /// Check whether c is below the level level (at least 1)
        ///检查卡片 c 是否是等级 level 以下（至少为1）
        /// </summary>
        public bool IsLevelBelow(ClientCard c, int level)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsLevelBelow, c, level);
        }

        /// <summary>
        /// Check whether c is above level level
        ///检查卡片 c 是否是等级 level 以上
        /// </summary>
        public bool IsLevelAbove(ClientCard c, int level)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsLevelAbove, c, level);
        }

        /// <summary>
        /// Check whether c is below the rank rank (at least 1)
        ///检查卡片 c 是否是阶级 rank 以下（至少为1）
        /// </summary>
        public bool IsRankBelow(ClientCard c, int rank)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsRankBelow, c, rank);
        }

        /// <summary>
        ///Check whether c is above the class rank
        ///检查卡片 c 是否是阶级 rank 以上
        /// </summary>
        public bool IsRankAbove(ClientCard c, int rank)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsRankAbove, c, rank);
        }

        /// <summary>
        ///检查卡片 c 是否连接标记数量是 link 以下（至少为1）
        /// </summary>
        public bool IsLinkBelow(ClientCard c, int link)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsLinkBelow, c, link);
        }

        /// <summary>
        ///检查卡片 c 是否连接标记数量是 link 以上
        /// </summary>
        public bool IsLinkAbove(ClientCard c, int link)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsLinkAbove, c, link);
        }

        /// <summary>
        /// Check whether c is attack power atk below (at least 0)
        ///检查卡片 c 是否是攻击力 atk 以下（至少为0）
        /// </summary>
        public bool IsAttackBelow(ClientCard c, int atk)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsAttackBelow, c, atk);
        }

        /// <summary>
        ///Check whether c is more than attack power atk
        ///检查卡片 c 是否是攻击力 atk 以上
        /// </summary>
        public bool IsAttackAbove(ClientCard c, int atk)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsAttackAbove, c, atk);
        }

        /// <summary>
        /// Check whether c is defensive def below (at least 0)
        ///检查卡片 c 是否是守备力 def 以下（至少为0）
        /// </summary>
        public bool IsDefenseBelow(ClientCard c, int def)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsDefenseBelow, c, def);
        }

        /// <summary>
        ///Check c is defensive def above
        ///检查卡片 c 是否是守备力 def 以上
        /// </summary>
        public bool IsDefenseAbove(ClientCard c, int def)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsDefenseAbove, c, def);
        }

        /// <summary>
        ///检查卡片 c 是否可以放置 countertype 类型的指示物
        /// </summary>
        public bool IsCanHaveCounter(ClientCard c, int countertype)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsDefenseAbove, c, countertype);
        }

        /// <summary>
        ///检查卡片 c 是否是可[用 sum_type 方式]融合召唤的卡
        /// </summary>
        public bool IsFusionSummonableCard(ClientCard c, int sum_type = 0)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsFusionSummonableCard, c, sum_type);
        }

        /// <summary>
        /// Check if you can make a special call to c
        ///检查是否可以对卡片 c [用 sum_type 方式]进行特殊召唤手续
        /// </summary>
        public bool IsSpecialSummonable(ClientCard c, int sum_type = 0)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsSpecialSummonable, c, sum_type);
        }

        /// <summary>
        /// Check whether c can hand
        ///检查卡片 c 是否可以送去[玩家 player 的]手牌
        /// </summary>
        public bool IsAbleToHand(ClientCard c, int player)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsAbleToHand, c, player);
        }

        /// <summary>
        ///Check whether c can be excluded as a cost
        ///检查卡片 c 是否可以[以 pos 表示形式]作为cost除外
        /// </summary>
        public bool IsAbleToRemoveAsCost(ClientCard c, int pos = (int)CardPosition.FaceUp)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsAbleToRemoveAsCost, c, pos);
        }

        /// <summary>
        /// Check whether c can be discarded
        ///检查卡片 c 是否可以以 reason 原因丢弃
        /// </summary>
        public bool IsDiscardable(ClientCard c, int reason = (int)CardReason.Cost)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsDiscardable, c, reason);
        }

        /// <summary>
        ///检查卡片 c 是否可以被[玩家 player]叠放
        /// </summary>
        public bool IsCanOverlay(ClientCard c, int player)
        {
            return HandleCardIntToBool(FunctionCode.Card.IsCanOverlay, c, player);
        }

        /// <summary>
        ///返回卡片 c 的连接区的卡片组（目前只限怪兽区）
        /// </summary>
        public IList<ClientCard> GetLinkedGroup(ClientCard c)
        {
            return HandleCardToGroup(FunctionCode.Card.GetLinkedGroup, c);
        }

        /// <summary>
        ///返回和卡片 c 互相连接状态的卡片组
        /// </summary>
        public IList<ClientCard> GetMutualLinkedGroup(ClientCard c)
        {
            return HandleCardToGroup(FunctionCode.Card.GetMutualLinkedGroup, c);
        }

        /// <summary>
        ///返回与卡片 c 同一纵列的 c 以外的卡片组
        /// </summary>
        public IList<ClientCard> GetColumnGroup(ClientCard c)
        {
            return HandleCardToGroup(FunctionCode.Card.GetColumnGroup, c);
        }

        /// <summary>
        ///Returns the material used for the appearance of c)
        ///返回卡片 c 出场使用的素材
        /// </summary>
        public IList<ClientCard> GetMaterial(ClientCard c)
        {
            return HandleCardToGroup(FunctionCode.Card.GetMaterial, c);
        }

        /// <summary>
        /// Returns the current set of cards
        ///返回卡片 c 当前装备着的卡片组
        /// </summary>
        public IList<ClientCard> GetEquipGroup(ClientCard c)
        {
            return HandleCardToGroup(FunctionCode.Card.GetEquipGroup, c);
        }

        /// <summary>
        ///Returns the card set attacked by this turn
        ///返回卡片 c 本回合攻击过的卡片组
        /// </summary>
        public IList<ClientCard> GetAttackedGroup(ClientCard c)
        {
            return HandleCardToGroup(FunctionCode.Card.GetAttackedGroup, c);
        }

        /// <summary>
        /// Returns the deck of the card that fought this turn
        ///返回本回合与卡片 c 进行过战斗的卡片组 进行过战斗指发生过伤害的计算，用于剑斗兽等卡的判定
        /// </summary>
        public IList<ClientCard> GetBattledGroup(ClientCard c)
        {
            return HandleCardToGroup(FunctionCode.Card.GetBattledGroup, c);
        }

        /// <summary>
        /// Returns all currently persistent objects
        ///返回卡片 c 当前所有的永续对象
        /// </summary>
        public IList<ClientCard> GetCardTarget(ClientCard c)
        {
            return HandleCardToGroup(FunctionCode.Card.GetCardTarget, c);
        }

        /// <summary>
        /// Returns all cards with c as the persistent object
        ///返回取卡片 c 作为永续对象的所有卡
        /// </summary>
        public IList<ClientCard> GetOwnerTarget(ClientCard c)
        {
            return HandleCardToGroup(FunctionCode.Card.GetOwnerTarget, c);
        }

        /// <summary>
        ///检查卡片 c 是否是连接状态
        /// </summary>
        public bool IsLinkState(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsLinkState, c);
        }

        /// <summary>
        ///检查卡片 c 是否是额外连接状态
        /// </summary>
        public bool IsExtraLinkState(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsExtraLinkState, c);
        }

        /// <summary>
        ///检查与卡片 c 同一纵列的区域是否全都有卡
        /// </summary>
        public bool IsAllColumn(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAllColumn, c);
        }

        /// <summary>
        ///检查卡片 c 是否属于额外卡组的怪兽(融合·同调·超量·连接)
        /// </summary>
        public bool IsExtraDeckMonster(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsExtraDeckMonster, c);
        }

        /// <summary>
        /// Check whether c is in the re-call state
        ///检查卡片 c 属否处于再召唤状态
        /// </summary>
        public bool IsDualState(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsDualState, c);
        }

        /// <summary>
        /// Check whether c direct attack
        ///检查卡片 c 是否直接攻击过
        /// </summary>
        public bool IsDirectAttacked(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsDirectAttacked, c);
        }

        /// <summary>
        /// Check whether c is associated with this battle
        ///检查卡片 c 是否和本次战斗关联 注：此效果通常用于伤害计算后伤害阶段结束前，用于检查战斗的卡是否离场过
        /// </summary>
        public bool IsRelateToBattle(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsRelateToBattle, c);
        }

        /// <summary>
        /// Check whether c is in an invalid state
        ///检查卡片 c 是否处于无效状态
        /// </summary>
        public bool IsDisabled(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsDisabled, c);
        }

        /// <summary>
        /// Check whether c is a card that can be called normally
        ///检查卡片 c 是否是可通常召唤的卡
        /// </summary>
        public bool IsSummonableCard(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsSummonableCard, c);
        }

        /// <summary>
        /// Check whether c can be sent to the card group
        ///检查卡片 c 是否可以送去卡组
        /// </summary>
        public bool IsAbleToDeck(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAbleToDeck, c);
        }

        /// <summary>
        /// Check if c can send extra cards
        ///检查卡片 c 是否可以送去额外卡组 对于非融合、同调等额外怪兽或者非灵摆怪兽此函数均返回false
        /// </summary>
        public bool IsAbleToExtra(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAbleToExtra, c);
        }

        /// <summary>
        /// Check whether c can be sent to the cemetery
        ///检查卡片 c 是否可以送去墓地
        /// </summary>
        public bool IsAbleToGrave(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAbleToGrave, c);
        }

        /// <summary>
        /// Check whether c can be sent as the cost of hand cards
        ///检查卡片 c 是否可以作为cost送去手牌
        /// </summary>
        public bool IsAbleToHandAsCost(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAbleToHandAsCost, c);
        }

        /// <summary>
        /// Check whether c can be sent to the card as a cost group
        ///检查卡片 c 是否可以作为cost送去卡组
        /// </summary>
        public bool IsAbleToDeckAsCost(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAbleToDeckAsCost, c);
        }

        /// <summary>
        /// Check whether c can be sent as an additional cost card group
        ///检查卡片 c 是否可以作为cost送去额外卡组，主卡组的灵摆卡会返回false
        /// </summary>
        public bool IsAbleToExtraAsCost(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAbleToExtraAsCost, c);
        }

        /// <summary>
        /// Check whether c can be sent as a cost card group or additional card group (for the new Yu-xia, sword fighting beast fusion call monster detection procedures)
        ///检查卡片 c 是否可以作为cost送去卡组或额外卡组（用于新宇侠、剑斗兽融合怪兽的召唤手续检测）
        /// </summary>


        public bool IsAbleToDeckOrExtraAsCost(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAbleToDeckOrExtraAsCost, c);
        }

        /// <summary>
        ///Check whether c can be sent to the cemetery as a cost
        ///检查卡片 c 是否可以作为cost送去墓地
        /// </summary>
        public bool IsAbleToGraveAsCost(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAbleToGraveAsCost, c);
        }

        /// <summary>
        /// Check whether c can be liberated (non-superior call)
        ///检查卡片 c 是否可以被解放（非上级召唤用）
        /// </summary>
        public bool IsReleasable(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsReleasable, c);
        }

        /// <summary>
        ///Check whether c can be liberated by the effect
        ///检查卡片 c 是否可以被效果解放
        /// </summary>
        public bool IsReleasableByEffect(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsReleasableByEffect, c);
        }

        /// <summary>
        /// Check whether c can attack
        ///检查卡片 c 是否可以攻击
        /// </summary>
        public bool IsAttackable(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAttackable, c);
        }

        /// <summary>
        /// Check whether c is a surface-side representation
        ///检查卡片 c 是否是表侧表示
        /// </summary>
        public bool IsFaceup(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsFaceup, c);
        }

        /// <summary>
        ///Check whether c is an attack
        ///检查卡片 c 是否是攻击表示
        /// </summary>
        public bool IsAttackPos(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAttackPos, c);
        }

        /// <summary>
        /// Check whether c is the backside representation
        ///检查卡片 c 是否是里侧表示
        /// </summary>
        public bool IsFacedown(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsFacedown, c);
        }

        /// <summary>
        /// Check whether c is a defensive representation
        ///检查卡片 c 是否是守备表示
        /// </summary>
        public bool IsDefensePos(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsDefensePos, c);
        }

        /// <summary>
        /// Check if c is present
        ///检查卡片 c 是否在场
        /// </summary>
        public bool IsOnField(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsOnField, c);
        }

        /// <summary>
        /// Check if c is open
        ///检查卡片 c 是否处于公开状态
        /// </summary>
        public bool IsPublic(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsPublic, c);
        }

        /// <summary>
        ///Check whether c is in the declaration prohibition state
        ///检查卡片 c 是否处于被宣言禁止状态
        /// </summary>
        public bool IsForbidden(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsForbidden, c);
        }

        /// <summary>
        /// Check whether c can change the control
        ///检查卡片 c 是否可以改变控制权
        /// </summary>
        public bool IsAbleToChangeControler(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsAbleToChangeControler, c);
        }

        /// <summary>
        ///检查卡片 c 是否可以用效果改变表示形式
        /// </summary>
        public bool IsCanChangePosition(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsCanChangePosition, c);
        }

        /// <summary>
        /// Check whether c can turn into the inside of that
        ///检查卡片 c 是否可以转成里侧表示
        /// </summary>
        public bool IsCanTurnSet(ClientCard c)
        {
            return HandleCardToBool(FunctionCode.Card.IsCanTurnSet, c);
        }

        /// <summary>
        ///返回以玩家 player 来看的所有连接区域
        /// </summary>
        public int GetLinkedZone(ClientCard c,int player)
        {
            int? result =  HandleIntToInt(FunctionCode.Card.GetLinkedZone, c, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///返回[以 player 来看的]与卡片 c 互相连接的卡 所在的区域 
        /// </summary>
        public int GetMutualLinkedZone(ClientCard c, int player)
        {
            int? result = HandleIntToInt(FunctionCode.Card.GetMutualLinkedZone, c, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///返回卡片 c [由 player 融合召唤时]用作融合素材时的属性
        /// </summary>
        public int GetFusionAttribute(ClientCard c, int player = PLAYER_NONE)
        {
            int? result = HandleIntToInt(FunctionCode.Card.GetFusionAttribute, c, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///返回卡片 c [由 player 连接召唤时]用作连接素材时的属性
        /// </summary>
        public int GetLinkAttribute(ClientCard c, int player = PLAYER_NONE)
        {
            int? result = HandleIntToInt(FunctionCode.Card.GetLinkAttribute, c, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///返回卡片 c [由 player 连接召唤时]作为连接素材时的种族
        /// </summary>
        public int GetLinkRace(ClientCard c, int player = PLAYER_NONE)
        {
            int? result = HandleIntToInt(FunctionCode.Card.GetLinkRace, c, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the card that caused the position of c to change
        ///返回导致卡片 c 的位置变化的卡
        /// </summary>
        public ClientCard GetReasonCard(ClientCard c)
        {
            return HandleCardToCard(FunctionCode.Card.GetReasonCard, c);
        }

        /// <summary>
        /// Returns the current artefact object
        ///返回卡片 c 当前的装备对象
        /// </summary>
        public ClientCard GetEquipTarget(ClientCard c)
        {
            return HandleCardToCard(FunctionCode.Card.GetEquipTarget, c);
        }

        /// <summary>
        ///Returns the device object before c
        ///返回卡片 c 之前的装备对象
        /// </summary>
        public ClientCard GetPreviousEquipTarget(ClientCard c)
        {
            return HandleCardToCard(FunctionCode.Card.GetPreviousEquipTarget, c);
        }

        /// <summary>
        ///Returns the card with c as the excess material
        ///返回以卡片 c 为超量素材的卡
        /// </summary>
        public ClientCard GetOverlayTarget(ClientCard c)
        {
            return HandleCardToCard(FunctionCode.Card.GetOverlayTarget, c);
        }

        /// <summary>
        /// Returns c the current first persistent object
        ///返回卡片 c 当前第一个永续对象，没有则返回 nil
        /// </summary>
        public ClientCard GetFirstCardTarget(ClientCard c)
        {
            return HandleCardToCard(FunctionCode.Card.GetFirstCardTarget, c);
        }

        /// <summary>
        ///Returns the card that fought with c
        ///返回与卡片 c 进行战斗的卡，没有则返回nil
        /// </summary>
        public ClientCard GetBattleTarget(ClientCard c)
        {
            return HandleCardToCard(FunctionCode.Card.GetBattleTarget, c);
        }

        /// <summary>
        ///Check whether c can be used as a non-adjustment
        ///检查卡片 c 是否可以作为同调怪兽 sc 的调整以外的怪兽
        /// </summary>
        public bool IsNotTuner(ClientCard c, ClientCard sc)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.IsNotTuner, c, sc);
        }

        /// <summary>
        /// Check whether c2 is the correct equipment object for c1
        ///检查卡片 c2是否是卡片 c1 的正确的装备对象
        /// </summary>
        public bool CheckEquipTarget(ClientCard c1, ClientCard c2)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.CheckEquipTarget, c1, c2);
        }

        /// <summary>
        ///检查卡片 c2 是否是卡片 c1 的正确的同盟对象
        /// </summary>
        public bool CheckUnionTarget(ClientCard c1, ClientCard c2)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.CheckUnionTarget, c1, c2);
        }

        /// <summary>
        /// Check whether c1 takes c2 as a persistent object
        ///检查卡片 c2 是否取卡片 c1 为永续对象
        /// </summary>
        public bool IsHasCardTarget(ClientCard c1, ClientCard c2)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.IsHasCardTarget, c1, c2);
        }

        /// <summary>
        /// Check whether c1 and c2 are linked
        ///检查卡片 c1 是否和卡片 c2 有联系
        /// </summary>
        public bool IsRelateToCard(ClientCard c1, ClientCard c2)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.IsRelateToCard, c1, c2);
        }

        /// <summary>
        /// Check whether the c can replace the material description card name of the fusion monster fc
        ///检查卡片 c 能否代替融合怪兽 fc 的记述卡名的素材
        /// </summary>
        public bool CheckFusionSubstitute(ClientCard c, ClientCard fc)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.CheckFusionSubstitute, c, fc);
        }

        /// <summary>
        /// Check whether c1 can become a target of c2
        ///检查卡片 c1 是否可以成为卡片 c2 的攻击目标
        /// </summary>
        public bool IsCanBeBattleTarget(ClientCard c1, ClientCard c2)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.IsCanBeBattleTarget, c1, c2);
        }

        /// <summary>
        ///Returns the number of effects that c is affected by the type of code
        ///返回卡片 c 受到影响的种类是code的效果的数量
        /// </summary>
        public int GetEffectCount(ClientCard c, int code)
        {
            int? result = HandleCardIntToInt(FunctionCode.Card.GetEffectCount, c, code);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///The type of returned c is the number of identifying effects of the code
        ///返回卡片 c 的种类是 code 的标识效果的数量
        /// </summary>
        public int GetFlagEffect(ClientCard c, int code)
        {
            int? result = HandleCardIntToInt(FunctionCode.Card.GetFlagEffect, c, code);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///Returns the number of countertype types on c
        ///返回卡片 c 上的 countertype 类型的指示物的数量， countertype==0 则返回卡片 c 上所有类型的指示物数量之和
        /// </summary>
        public int GetCounter(ClientCard c, int countertype)
        {
            int? result = HandleCardIntToInt(FunctionCode.Card.GetCounter, c, countertype);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Check whether c is usually called (not including the normally called set)
        ///检查卡片 c 是否可以进行通常召唤（不包含通常召唤的set)，ignore_count==true 则不检查召唤次数限制,minc 表示至少需要的祭品数（用于区分妥协召唤与上级召唤）,zone 表示必须要召唤到的区域
        /// </summary>
        public bool IsSummonable(ClientCard c, bool ignore_count = false, int minc = 0, int zone = 0x1f)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsSummonable, c, ignore_count, minc, zone);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check whether c can be normally called set
        ///检查卡片 c 是否可进行通常召唤的set，ignore_count==true 则不检查召唤次数限制 minc 表示至少需要的祭品数（用于区分妥协召唤set与上级召唤set）,zone 表示必须要放置到的区域
        /// </summary>
        public bool IsMSetable(ClientCard c, bool ignore_count = false, int minc = 0, int zone = 0x1f)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsMSetable, c, ignore_count, minc, zone);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check whether mg can be selected in the [min-max months] excessive material on the c to call the excess procedure
        ///检查是否可以在场上的卡[中选出 minc-maxc 个超量素材]对卡片 c 进行超量召唤手续 c 如果不是超量会返回 false
        /// </summary>
        public bool IsXyzSummonable(ClientCard c, int minc = 0, int maxc = 0)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsXyzSummonable, c, minc, maxc);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///检查是否可以用[包含卡 lcard 的]场上的卡[选出 minc-maxc 个连接素材]对卡片 c 进行连接召唤手续 c 如果不是连接会返回 false
        /// </summary>
        public bool IsLinkSummonable(ClientCard c, ClientCard lcard = null, int minc = 0, int maxc = 0)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsLinkSummonable, c, lcard, minc, maxc);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// The type of c is the label that identifies the effect of code, and returns nil if it does not
        ///返回卡片 c 的种类为 code 的标识效果的Label(数量可能不止1个)
        /// </summary>
        public IList<int> GetFlagEffectLabel(ClientCard c, int code)
        {
            object result = HandleCardSpecial(FunctionCode.Card.GetFlagEffectLabel, c, code);
            if (result == null) return new List<int>();
            return (IList<int>)result;
        }

        /// <summary>
        ///返回[以 player 来看的] location 范围内与卡片 c 同一纵列的区域(c 所在的位置排除)，
        /// </summary>
        public int GetColumnZone(ClientCard c, int location, int player)
        {
            object result = HandleCardSpecial(FunctionCode.Card.GetColumnZone, c, location, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///Returns all stacked cards at a specified location
        ///返回卡片 c 当前叠放着的卡片组
        /// </summary>
        public IList<ClientCard> GetOverlayGroup(ClientCard c)
        {
            object result = HandleCardSpecial(FunctionCode.Card.GetOverlayGroup, c);
            if (result == null) return new List<ClientCard>();
            return (IList<ClientCard>)result;
        }

        /// <summary>
        /// Check if the player can remove at least count cards for the specified reason
        ///检查玩家 player 能否以 reason 为原因，至少取除卡片 c 下面叠放的 count 张卡
        /// </summary>
        public bool CheckRemoveOverlayCard(ClientCard c, int player, int count, int reason)
        {
            object result = HandleCardSpecial(FunctionCode.Card.CheckRemoveOverlayCard, c, player, count, reason);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check if c is affected by the effect type
        ///检查卡片 c 是否受到效果种类是code的效果的影响
        /// </summary>
        public bool IsHasEffect(ClientCard c, int code, int check_player = PLAYER_NONE)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsHasEffect, c, code, check_player);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Returns the attackable card set and whether it can attack directly
        ///返回卡片 c 可攻击的卡片组以及能否直接攻击，[0]是否可以攻击，[1]卡片组
        /// </summary>
        public object[] GetAttackableTarget(ClientCard c)
        {
            object result = HandleCardSpecial(FunctionCode.Card.GetAttackableTarget, c);
            if (result == null) return new object[0];
            return (object[])result;
        }

        /// <summary>
        /// Check c for excess monster xyzc with the level of whether the excess is lv
        ///检查卡片 c 对于超量怪兽 xyzc 的超量用等级是否是 lv
        /// </summary>
        public bool IsXyzLevel(ClientCard c, ClientCard xyzc, int lv)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsXyzLevel, c, xyzc, lv);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check whether c can be the object of sc
        /// 检查卡片 c 是否可以成为卡片sc的效果对象
        /// </summary>
        public bool IsCanBeCardTarget(ClientCard c, ClientCard sc)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.IsCanBeCardTarget, c, sc);
        }

        /// <summary>
        /// 检查卡片 c 是否可以被sc的效果无效
        /// </summary>
        public bool IsCanBeDisabledByCard(ClientCard c, ClientCard sc)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.IsCanBeDisabledByCard, c, sc);
        }

        /// <summary>
        /// Check whether c immune sc
        /// 检查卡片 c 是否可以免疫sc的效果
        /// </summary>
        public bool IsImmuneToCard(ClientCard c, ClientCard sc)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.IsImmuneToCard, c, sc);
        }

        /// <summary>
        /// Check whether c can be destoryed by sc
        /// 检查卡片 c 是否可以被sc的效果破坏
        /// </summary>
        public bool IsDestructable(ClientCard c, ClientCard sc)
        {
            return HandleDoubleCardToBool(FunctionCode.Card.IsDestructable, c, sc);
        }

        /// <summary>
        /// Check whether c can be set to the magic trap area, ignore_field = true is disregard of the trap area trap trap
        /// 检查卡片 c 是否可以set到魔法陷阱区，ignore_field==true 则无视魔陷区格子是否能使用的限制
        /// </summary>
        public bool IsSSetable(ClientCard c, bool ignore_field = false)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsSSetable, c, ignore_field);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check whether c can be player except player
        /// 检查卡片 c 是否可以被[玩家 player 以 pos 的表示形式,reason 原因]除外
        /// </summary>
        public bool IsAbleToRemove(ClientCard c, int player, int pos = (int)CardPosition.FaceUp, int reason = (int)CardReason.Effect)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsAbleToRemove, c, player,pos,reason);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check whether c can be a continuous attack, c the number of attacks declared> = ac return false
        /// 检查卡片 c 是否可以连续攻击，c 的攻击宣言次数>=ac 则返回false monsteronly==true 则表示只能对怪兽攻击
        /// </summary>
        public bool IsChainAttackable(ClientCard c, int ac = 2, bool monsteronly = false)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsChainAttackable, c, ac, monsteronly);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check whether the control of c can be changed
        /// 检查卡片 c 的控制权是否可以改变。 ignore_mzone==true 会忽视转移控制权后的玩家场上是否有空格位， zone 表示必须要使用的位置
        /// </summary>
        public bool IsControlerCanBeChanged(ClientCard c, bool ignore_mzone = false, int zone = 0xff)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsControlerCanBeChanged, c, ignore_mzone, zone);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check if c can place count counters of type countertype [singly = true]
        /// 检查是否可以给[在 location 区域存在的]卡片 c [逐个(singly==true)]放置 count 个 countertype 类型的指示物
        /// </summary>
        public bool IsCanAddCounter(ClientCard c, int countertype, int count, int location, bool singly = false)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsCanAddCounter, c, countertype, count, singly,location);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///Check if the player player can remove count counters of type c on c for reason
        /// 检查玩家 player 是否可以以原因 reason 移除卡片 c 上的 count 个 countertype 类型的指示物
        /// </summary>
        public bool IsCanRemoveCounter(ClientCard c, int player, int countertype, int count, int reason)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsCanRemoveCounter, c, player, countertype, count, reason);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// 检查卡片 c 是否可以成为[融合怪兽 fc 的 summon_type 方式的]融合素材
        /// </summary>
        public bool IsCanBeFusionMaterial(ClientCard c, ClientCard fc = null, int summon_type = (int)SummonType.Fusion)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsCanBeFusionMaterial, c, fc, summon_type);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// 检查卡片 c 是否可以成为[以 tuner 为调整的同调怪兽 sc 的]同调素材
        /// </summary>
        public bool IsCanBeSynchroMaterial(ClientCard c, ClientCard sc = null, ClientCard tuner = null)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsCanBeSynchroMaterial, c, sc, tuner);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// 检查卡片 c 是否可以作为[仪式怪兽 sc 的]仪式素材,没有指定sc的场合，必须填nil
        /// </summary>
        public bool IsCanBeRitualMaterial(ClientCard c, ClientCard sc = null)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsCanBeRitualMaterial, c, sc);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// 检查卡片 c 是否可以成为[超量怪兽 sc 的]超量素材,没有指定sc的场合，必须填nil
        /// </summary>
        public bool IsCanBeXyzMaterial(ClientCard c, ClientCard sc = null)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsCanBeXyzMaterial, c, sc);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// 检查卡片 c 是否可以成为[连接怪兽 sc 的]连接素材,没有指定sc的场合，必须填nil
        /// </summary>
        public bool IsCanBeLinkMaterial(ClientCard c, ClientCard sc = null)
        {
            object result = HandleCardSpecial(FunctionCode.Card.IsCanBeLinkMaterial, c, sc);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// 检查卡片 c 在 check_player 场上[check_location 区域]的唯一性[, 忽略卡片 icard 的唯一性影响]
        /// </summary>
        public bool CheckUniqueOnField(ClientCard c, int check_player, int check_location = (int)CardLocation.Onfield, ClientCard icard = null)
        {
            object result = HandleCardSpecial(FunctionCode.Card.CheckUniqueOnField, c, check_player, check_location, icard);
            if (result == null) return false;
            return (bool)result;
        }

        //=================== Duel ===================

        /// <summary>
        /// Returns the current LP of the player's player
        /// 返回玩家 player 的当前LP
        /// </summary>
        public int GetLP(int player)
        {
            object result = HandleDuel(FunctionCode.Duel.GetLP, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// 返回[玩家 player 所经过的]当前的回合数
        /// </summary>
        public int GetTurnCount(int player = PLAYER_NONE)
        {
            object result = HandleDuel(FunctionCode.Duel.GetTurnCount, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the player's number of rules drawn per turn
        /// 返回玩家 player 每回合的规则抽卡数量
        /// </summary>
        public int GetDrawCount(int player)
        {
            object result = HandleDuel(FunctionCode.Duel.GetDrawCount, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the player's player's damage during this battle
        /// 返回玩家 player 在本次战斗中受到的伤害
        /// </summary>
        public int GetBattleDamage(int player)
        {
            object result = HandleDuel(FunctionCode.Duel.GetBattleDamage, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// 返回以玩家 player 来看的所有连接区域
        /// </summary>
        public int GetLinkedZone(int player)
        {
            object result = HandleDuel(FunctionCode.Duel.GetLinkedZone, player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the player's deck of cards available for ritual summoning material
        /// 返回玩家 player 可用的用于仪式召唤素材的卡片组
        /// </summary>
        public IList<ClientCard> GetRitualMaterial(int player)
        {
            object result = HandleDuel(FunctionCode.Duel.GetRitualMaterial, player);
            if (result == null) return new List<ClientCard>();
            return (IList<ClientCard>)result;
        }

        /// <summary>
        /// Returns the current round of players
        /// 返回当前的回合玩家
        /// </summary>
        public int GetTurnPlayer()
        {
            object result = HandleDuel(FunctionCode.Duel.GetTurnPlayer);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the chain number currently being processed
        /// 返回当前正在处理的连锁序号
        /// </summary>
        public int GetCurrentChain()
        {
            object result = HandleDuel(FunctionCode.Duel.GetCurrentChain);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Returns the current stage
        /// 返回当前的阶段
        /// </summary>
        public int GetCurrentPhase()
        {
            object result = HandleDuel(FunctionCode.Duel.GetCurrentPhase);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Used to check if damage has been calculated during the damage phase
        /// 用于在伤害阶段检查是否已经计算了战斗伤害
        /// </summary>
        public bool IsDamageCalculated()
        {
            object result = HandleDuel(FunctionCode.Duel.IsDamageCalculated);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Return the card for this combat attack
        /// 返回此次战斗攻击的卡
        /// </summary>
        public ClientCard GetAttacker()
        {
           return (ClientCard)HandleDuel(FunctionCode.Duel.GetAttacker);
        }

        /// <summary>
        /// Return the card for this combat attack
        /// 返回此次战斗被攻击的卡，如果返回null表示是直接攻击
        /// </summary>
        public ClientCard GetAttackTarget()
        {
            return (ClientCard)HandleDuel(FunctionCode.Duel.GetAttackTarget);
        }

        /// <summary>
        /// heck the player at the current stage whether the operation
        /// 检查玩家在当前阶段是否有操作
        /// </summary>
        public bool CheckPhaseActivity()
        {
            object result = HandleDuel(FunctionCode.Duel.CheckPhaseActivity);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check whether the player can enter the combat phase of the round
        /// 检查回合玩家能否进入战斗阶段
        /// </summary>
        public bool IsAbleToEnterBP()
        {
            object result = HandleDuel(FunctionCode.Duel.IsAbleToEnterBP);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Returns the count card at the top of the player's deck
        /// 返回玩家 player 的卡组最上方的 count 张卡
        /// </summary>
        public IList<ClientCard> GetDecktopGroup(int player, int count)
        {
            object result = HandleDuel(FunctionCode.Duel.GetDecktopGroup, player,count);
            if (result == null) return new List<ClientCard>();
            return (IList<ClientCard>)result;
        }

        /// <summary>
        /// Return to the player player can be liberated (non-superior summoned use) of the deck, use_hand is true, including the hand card
        /// 返回玩家 player 可解放（非上级召唤用）的卡片组， use_hand==true 则包括手卡
        /// </summary>
        public IList<ClientCard> GetReleaseGroup(int player, bool use_hand = false)
        {
            object result = HandleDuel(FunctionCode.Duel.GetReleaseGroup, player, use_hand);
            if (result == null) return new List<ClientCard>();
            return (IList<ClientCard>)result;
        }

        /// <summary>
        /// Return player player can be liberated (non-superior summoned with) the number of cards, use_hand is true, including the hand card
        /// 返回玩家 player 可解放（非上级召唤用）的卡片数量， use_hand==true 则包括手卡
        /// </summary>
        public int GetReleaseGroupCount(int player, bool use_hand = false)
        {
            object result = HandleDuel(FunctionCode.Duel.GetReleaseGroupCount, player, use_hand);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// 第一个返回值是 玩家 player  [在区域 location 内的]可用的用于融合召唤素材的卡片组(包含受 EFFECT_EXTRA_FUSION_MATERIAL 效果影响的卡)
        /// 第二个返回值是 只包含 手卡、怪兽区[、除外、墓地、卡组、额外卡组、P区·魔陷区原本种类是] 怪兽卡的卡片组(也即是没有包含受 EFFECT_EXTRA_FUSION_MATERIAL 效果影响的卡)
        /// </summary>
        public IList<IList<ClientCard>> GetFusionMaterial(int player, int location = (int)CardLocation.Hand | (int)CardLocation.MonsterZone)
        {
            object result = HandleDuel(FunctionCode.Duel.GetFusionMaterial, player, location);
            if (result == null) return new List<IList<ClientCard>>();
            return (IList<IList<ClientCard>>)result;
        }

        /// <summary>
        /// Check if the player is affected by the effect of the code type
        /// 检查玩家 player 是否受到种类为 code 的效果影响
        /// </summary>
        public bool IsPlayerAffectedByEffect(int player, int code)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerAffectedByEffect, player, code);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check the player player can effect pumping [count Zhang] card
        /// 检查玩家 player 是否可以效果抽[count 张]卡
        /// </summary>
        public bool IsPlayerCanDraw(int player, int count = 0)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanDraw, player, count);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check player player can put the card group top count cards sent to the cemetery
        /// 检查玩家 player 是否可以把卡组顶端 count 张卡送去墓地
        /// </summary>
        public bool IsPlayerCanDiscardDeck(int player, int count = 0)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanDiscardDeck, player, count);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check the player player can put the card group top count cards sent to the cemetery as cost
        /// 检查玩家 player 能否把卡组顶端 count 张卡送去墓地作为cost
        /// </summary>
        public bool IsPlayerCanDiscardDeckAsCost(int player, int count = 0)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanDiscardDeckAsCost, player, count);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// 检查玩家 player 是否可以把卡片[ c ]盖放到魔陷区
        /// </summary>
        public bool IsPlayerCanSSet(int player, ClientCard c = null)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanSSet, player, c);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///Check whether the player player special call count times
        ///检查玩家 player 能否特殊召唤 count 次
        /// </summary>
        public bool IsPlayerCanSpecialSummonCount(int player, int count)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanSpecialSummonCount, player, count);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///Check whether the player can liberate the player c
        ///检查玩家 player 是否能解放[卡片 c]
        /// </summary>
        public bool IsPlayerCanRelease(int player, ClientCard c = null)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanRelease, player, c);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///Check whether the player can send c to hand
        ///检查玩家是否能把卡片[c]送去手牌
        /// </summary>
        public bool IsPlayerCanSendtoHand(int player, ClientCard c = null)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanSendtoHand, player, c);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///Check whether the player can send c to the cemetery
        ///检查玩家是否能把卡片[c]送去墓地
        /// </summary>
        public bool IsPlayerCanSendtoGrave(int player, ClientCard c = null)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanSendtoGrave, player, c);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///Check whether the player can send c to the card group
        ///检查玩家是否能把卡片[c]送去卡组
        /// </summary>
        public bool IsPlayerCanSendtoDeck(int player, ClientCard c = null)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanSendtoDeck, player, c);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// 返回玩家 player 的额外卡组表侧表示的卡中最上方的 count 张卡
        /// </summary>
        public IList<ClientCard> GetExtraTopGroup(int player, int count)
        {
            object result = HandleDuel(FunctionCode.Duel.GetExtraTopGroup, player, count);
            if (result == null) return new List<ClientCard>();
            return (IList<ClientCard>)result;
        }

        /// <summary>
        /// Return the card for this combat attack
        /// 返回本次进行战斗的怪兽。以玩家 player 来看，第一个是自己的怪兽，第二个是对方的怪兽，没有进行战斗的怪兽的话，则为null
        /// </summary>
        public IList<ClientCard> GetBattleMonster(int player)
        {
            object result = HandleDuel(FunctionCode.Duel.GetBattleMonster,player);
            if (result == null)
            {
                return type == ReceiveType.Success ? null : new List<ClientCard>();
            }
            return (IList<ClientCard>)result;
        }

        /// <summary>
        /// Returns the number of specific marker effects for the player's player
        /// 返回玩家 player 的 code 标识效果的数量
        /// </summary>
        public int GetFlagEffect(int player, int code)
        {
            object result = HandleDuel(FunctionCode.Duel.GetFlagEffect, player,code);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///返回玩家 player 的种类为 code 的标识效果的全部Label
        /// </summary>
        public IList<int> GetFlagEffectLabel(int player, int code)
        {
            object result = HandleDuel(FunctionCode.Duel.GetFlagEffectLabel, player, code);
            if (result == null) return new List<int>();
            return (IList<int>)result;
        }

        /// <summary>
        ///检查玩家 player 能否[向卡片 c]添加[count 个 countertype 类型的]指示物，如果 player 不是 0或者1，则返回false 额外参数如果要用，必须全写
        /// </summary>
        public bool IsCanAddCounter(int player, int countertype = -1, int count = -1, ClientCard c = null)
        {
            object result = HandleDuel(FunctionCode.Duel.IsCanAddCounter, player, countertype, count, c);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check if the player player can remove count counters of type c on c for reason
        ///检查玩家 player 以 reason 为原因是否能移除场上的 countertype 类型的 count 个指示物
        /// </summary>
        public bool IsCanRemoveCounter(int player, int s, int o, int countertype, int count, int reason)
        {
            object result = HandleDuel(FunctionCode.Duel.IsCanRemoveCounter, player, s, o, countertype, count, reason);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Returns the number of counterstype types that exist on the farm
        ///返回场上存在的 countertype 类型的指示物的数量
        /// </summary>
        public int GetCounter(int player, int s, int o, int countertype)
        {
            object result = HandleDuel(FunctionCode.Duel.GetCounter, player, s, o, countertype);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///检查场地代号是否是code [，来源玩家是否是 player][，生效区域是否在 loc 内]
        /// </summary>
        public bool IsEnvironment(int code, int player = PLAYER_ALL, int loc = (int)CardLocation.FieldZone | (int)CardLocation.Onfield)
        {
            object result = HandleDuel(FunctionCode.Duel.IsEnvironment, code, player, loc);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///Check player player can pay cost point lp
        ///检查玩家 player 是否能支付cost点lp
        /// </summary>
        public bool CheckLPCost(int player, int cost)
        {
            object result = HandleDuel(FunctionCode.Duel.CheckLPCost, player, cost);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///Check whether the round player can still summon this turn [Card c]
        ///检查回合玩家本回合是否还能通常召唤[卡片 c]
        /// </summary>
        public bool CheckSummonedCount(ClientCard c = null)
        {
            object result = HandleDuel(FunctionCode.Duel.CheckSummonedCount, c);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///Returns the number of spaces available in the player's field
        ///返回玩家 player 的场上区域 location 内的可用的[区域 zone 里的]空格数
        /// </summary>
        public int GetLocationCount(int player, int location, int use_player = PLAYER_NULL, int reason = LOCATION_REASON_TOFIELD, int zone = 0xff)
        {
            object result = HandleDuel(FunctionCode.Duel.GetLocationCount, player, location, use_player, reason, zone);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///返回玩家 player 场上[对于 use_player 来说]可用的怪兽区数量（？）
        /// </summary>
        public int GetUsableMZoneCount(int player, int use_player = PLAYER_NULL)
        {
            object result = HandleDuel(FunctionCode.Duel.GetUsableMZoneCount, player, use_player);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///返回以玩家 player 来看的 s_range 和 o_range 区域的处于连接状态的卡片组
        /// </summary>
        public IList<ClientCard> GetLinkedGroup(int player, int s_range, int o_range)
        {
            object result = HandleDuel(FunctionCode.Duel.GetLinkedGroup, player, s_range, o_range);
            if (result == null) return new List<ClientCard>();
            return (IList<ClientCard>)result;
        }

        /// <summary>
        ///返回以玩家 player 来看的 s_range 和 o_range 区域的处于连接状态的卡片的数量
        /// </summary>
        public int GetLinkedGroupCount(int player, int s_range, int o_range)
        {
            object result = HandleDuel(FunctionCode.Duel.GetLinkedGroupCount, player, s_range, o_range);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        /// Back to player The player's field is located in the location number seq card
        ///返回玩家 player 的场上位于区域 location 序号为 seq 的卡，常用于获得场地区域·灵摆区域的卡
        /// </summary>
        public ClientCard GetFieldCard(int player, int location, int seq)
        {
            return (ClientCard)HandleDuel(FunctionCode.Duel.GetFieldCard, player, location, seq);
        }

        /// <summary>
        /// Check if the player's player field is seq in the location of the space is availabl
        ///检查玩家 player 的场上位于区域 location 序号为 seq 的空格是否可用
        /// </summary>
        public bool CheckLocation(int player, int location, int seq)
        {
            object result = HandleDuel(FunctionCode.Duel.CheckLocation, player, location, seq);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Returns the card at the specified location
        ///返回以 player 来看的指定位置的卡，s 代表以 player 来看的自己的位置，o 代表以 player 来看的对方的位置 
        /// </summary>
        public IList<ClientCard> GetFieldGroup(int player, int s, int o)
        {
            object result = HandleDuel(FunctionCode.Duel.GetFieldGroup, player, s, o);
            if (result == null) return new List<ClientCard>();
            return (IList<ClientCard>)result;
        }

        /// <summary>
        ///Returns the number of cards in the specified location
        ///同 Duel.GetFieldGroup ，只是返回的是卡的数量
        /// </summary>
        public int GetFieldGroupCount(int player, int s, int o)
        {
            object result = HandleDuel(FunctionCode.Duel.GetFieldGroupCount, player, s, o);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///Returns all stacked cards at a specified location
        ///返回以 player 来看的指定位置的所有叠放的卡
        /// </summary>
        public IList<ClientCard> GetOverlayGroup(int player, int s, int o)
        {
            object result = HandleDuel(FunctionCode.Duel.GetOverlayGroup, player, s, o);
            if (result == null) return new List<ClientCard>();
            return (IList<ClientCard>)result;
        }

        /// <summary>
        ///Returns the number of all stacked cards in the specified location
        ///返回以 player 来看的指定位置的所有叠放的卡的数量
        /// </summary>
        public int GetOverlayCount(int player, int s, int o)
        {
            object result = HandleDuel(FunctionCode.Duel.GetOverlayCount, player, s, o);
            if (result == null) return -1;
            return (int)result;
        }

        /// <summary>
        ///Returns the deck of cards used for the normal summon c to be released (for higher-level summons)
        ///返回场上用于通常召唤卡片 c 可解放（上级召唤用）的卡片组
        /// </summary>
        public IList<ClientCard> GetTributeGroup(ClientCard c)
        {
            object result = HandleDuel(FunctionCode.Duel.GetTributeGroup, c);
            if (result == null) return new List<ClientCard>();
            return (IList<ClientCard>)result;
        }

        /// <summary>
        ///Check if the player can remove at least count cards for the specified reason
        ///检查 player 能否以原因 reason 移除以 player 来看的指定位置至少 count 张卡
        /// </summary>
        public bool CheckRemoveOverlayCard(int player, int s, int o, int count, int reason)
        {
            object result = HandleDuel(FunctionCode.Duel.CheckRemoveOverlayCard, player, s, o,count,reason);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check player player can usually call [c, to sumtype]
        ///检查玩家 player 是否可以通常召唤[c，以 sumtype 方式]
        /// </summary>
        public bool IsPlayerCanSummon(int player, int sumtype = 0, ClientCard c = null)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanSummon, player, sumtype, c);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///检查玩家 player 是否可以盖放怪兽[c，以sumtype方式]
        /// </summary>
        public bool IsPlayerCanMSet(int player, int sumtype = 0, ClientCard c = null)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanMSet, player, sumtype, c);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check player player can special summon [c to target_player field to sumtype summation, sumpos form]
        ///检查玩家 player 能否特殊召唤[卡片 c 到 target_player 场上，以 sumtype 召唤方式，sumpos 表示形式]
        /// </summary>
        public bool IsPlayerCanSpecialSummon(int player, int sumtype = 0, int sumpos  = 0, int target_player = PLAYER_NULL, ClientCard c = null)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanSpecialSummon, player, sumtype, sumpos, target_player, c);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///Check player player can be pos to the form of special summon the parameters of the monster to the target_player field
        ///检查玩家 player 是否可以[以 sumtype 方式][以 pos 表示形式]特殊召唤 给定参数的怪兽到 target_player 场上
        /// </summary>
        public bool IsPlayerCanSpecialSummonMonster(int player, int code, int target_player, int sumtype = 0,int pos = (int)CardPosition.FaceUp,int ? setcode = null, int? type = null,int? atk = null,int? def = null,
            int? level = null,int? race = null,int? attribute = null)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanSpecialSummonMonster, player, code, setcode, type, atk, def, level, race,
                attribute, pos, target_player, sumtype);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Check whether the player player except c
        ///检查玩家 player 是否能[以 reason 原因]除外[卡片 c]
        /// </summary>
        public bool IsPlayerCanRemove(int player, ClientCard c = null,int reason = (int)CardReason.Effect)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanRemove, player, c, reason);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        ///检查玩家是否可以额外的增加通常召唤次数。
        /// </summary>
        public bool IsPlayerCanAdditionalSummon(int player)
        {
            object result = HandleDuel(FunctionCode.Duel.IsPlayerCanAdditionalSummon, player);
            if (result == null) return false;
            return (bool)result;
        }

        /// <summary>
        /// Returns the number of times a player fought this turn
        ///返回玩家 player 这回合战斗过的次数 
        /// </summary>
        public int GetBattledCount(int player)
        {
            object result = HandleDuel(FunctionCode.Duel.GetBattledCount, player);
            if (result == null) return -1;
            return (int)result;
        }


    }
}
