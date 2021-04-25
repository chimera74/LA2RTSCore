using LA2RTS.LA2Entities;
using System;
using System.Collections.Generic;
using System.Timers;
using static LA2RTS.LA2Entities.LA2UserChar;

namespace LA2RTS
{
    public class OnlineRTSClientState : RTSClientState
    {

        internal Timer requestStateTimer;
        private bool _wasFirstPositionPacket;

        public OnlineRTSClientState(RTSClient parentClient) : base(parentClient)
        {
        }

        public override void OnEnter()
        {
            _wasFirstPositionPacket = false;

            requestStateTimer = new Timer(3000);
            requestStateTimer.Elapsed += RequestStateTask;
            requestStateTimer.AutoReset = true;
            requestStateTimer.Enabled = true;

            client.SendQuickSelfInfoTransmitEnable(true);
            client.SendSelfInfoRequest();

            Random r = new Random();
            Timer delayTimer = new Timer(r.Next(400, 1400));
            delayTimer.Elapsed += (s, e) =>
            {
                client.SendFullNPCInfoRequest();
                client.SendQuickNPCInfoTransmitEnable(true);
                client.SendFullPlayerInfoRequest();
                client.SendQuickPlayerInfoTransmitEnable(true);
            };
            delayTimer.AutoReset = false;
            delayTimer.Enabled = true;

        }

        public override void OnExit()
        {
            base.OnExit();
            client.SendQuickSelfInfoTransmitEnable(false);
            client.SendQuickNPCInfoTransmitEnable(false);
            client.SendQuickPlayerInfoTransmitEnable(false);
        }

        public override void CleanUp()
        {
            requestStateTimer.Stop();
            requestStateTimer.Enabled = false;
            requestStateTimer.Dispose();
        }

        public override bool ProcessRequest(byte[] header, byte[] message)
        {
            switch (header[0])
            {
                case 0x03:
                    ProcessSelfInfoPacket(message);
                    break;
                case 0x04:
                    ProcessQuickSelfInfoPacket(message);
                    break;
                case 0x05:
                    ProcessStatusPacket(message);
                    break;
                case 0x06:
                    ProcessNPCFullInfoPacket(message);
                    break;
                case 0x07:
                    ProcessNPCQuickInfoPacket(message);
                    break;
                case 0x08:
                    ProcessPlayerFullInfoPacket(message);
                    break;
                case 0x09:
                    ProcessPlayerQuickInfoPacket(message);
                    break;
                default:
                    return false;
            }
            return true;
        }

        internal void RequestStateTask(Object source, ElapsedEventArgs e)
        {
            client.SendStatusRequest();
        }

        internal void ChangeStateToEnteringWorld()
        {
            OnExit();
            client.clientState = new EnteringWorldRTSClientState(client);
            client.clientState.OnEnter();
        }

        private void ProcessStatusPacket(byte[] packet)
        {

            if (client.UserChar == null)
                client.UserChar = new LA2UserChar(client);

            int currentOffeset = 0;
            client.UserChar.Status = (ClientStatus)Utils.ReadByteFromBuff(packet, ref currentOffeset);
            if (client.UserChar.Status != ClientStatus.InGame)
                ChangeStateToEnteringWorld();
            client.RaiseStatusPacketEvent(client);
        }

        private void ProcessSelfInfoPacket(byte[] packet)
        {

            if (client.UserChar == null)
                client.UserChar = new LA2UserChar(client);

            int currentOffeset = 0;
            //LA2Object
            client.UserChar.OID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Name = Utils.ReadStrFromBuff(packet, ref currentOffeset);
            client.UserChar.Valid = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

            //LA2Spawn
            client.UserChar.X = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Y = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Z = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.SpawnTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            //LA2Live
            client.UserChar.AbnormalID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.AllyID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.AtkOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.AtkTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ClanID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.CurHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.CurMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Fishing = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.HP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Karma = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Level = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Load = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.MaxHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.MaxMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.MP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.MyAtkTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.SP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.TargetOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Team = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.TeleportDist = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.TeleportTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ToX = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ToY = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ToZ = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            client.UserChar.Speed = Utils.ReadDoubleFromBuff(packet, ref currentOffeset);

            client.UserChar.Exp = Utils.ReadLongFromBuff(packet, ref currentOffeset);
            client.UserChar.EXP2 = Utils.ReadLongFromBuff(packet, ref currentOffeset);

            client.UserChar.Attackable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Dead = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Dropped = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Fly = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.InCombat = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.IsMember = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Moved = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.PK = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.PvP = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Running = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Sitting = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Sweepable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

            client.UserChar.Ally = Utils.ReadStrFromBuff(packet, ref currentOffeset);
            client.UserChar.Clan = Utils.ReadStrFromBuff(packet, ref currentOffeset);
            client.UserChar.Title = Utils.ReadStrFromBuff(packet, ref currentOffeset);

            //LA2Char
            client.UserChar.CP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.CurCP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.MaxCP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ClassID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.MainClass = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Sex = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Race = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.CubicCount = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Recom = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            client.UserChar.MountType = Utils.ReadByteFromBuff(packet, ref currentOffeset);
            client.UserChar.StoreType = Utils.ReadByteFromBuff(packet, ref currentOffeset);

            client.UserChar.Hero = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Noble = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Premium = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

            //LA2UserChar
            client.UserChar.CanCryst = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Charges = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.WeightPenalty = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.WeapPenalty = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ArmorPenalty = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.DeathPenalty = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Souls = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            client.RaiseSelfInfoPacketEvent(client);
        }

        private void ProcessQuickSelfInfoPacket(byte[] packet)
        {

            if (client.UserChar == null)
                client.UserChar = new LA2UserChar(client);

            int currentOffeset = 0;

            //LA2Spawn
            client.UserChar.X = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Y = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.Z = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            //LA2Live
            client.UserChar.AbnormalID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.AtkOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.AtkTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.CurHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.CurMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.HP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.MaxHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.MaxMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.MP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.TargetOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ToX = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ToY = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ToZ = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            client.UserChar.Speed = Utils.ReadDoubleFromBuff(packet, ref currentOffeset);

            client.UserChar.Attackable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Dead = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Dropped = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.InCombat = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Moved = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.PK = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.PvP = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Running = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Sitting = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
            client.UserChar.Sweepable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

            //LA2Char
            client.UserChar.CP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.CurCP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.MaxCP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.CubicCount = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            //LA2UserChar            
            client.UserChar.Charges = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.WeightPenalty = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.WeapPenalty = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.ArmorPenalty = Utils.ReadIntFromBuff(packet, ref currentOffeset);
            client.UserChar.DeathPenalty = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            if (!_wasFirstPositionPacket)
            {
                _wasFirstPositionPacket = true;
                client.RaiseFirstPositionPacketEvent(client);
            }
            client.RaiseQuickUpdatePacketEvent(client);
        }

        private void ProcessNPCFullInfoPacket(byte[] packet)
        {
            LinkedList<LA2NPC> npcs = new LinkedList<LA2NPC>();

            int currentOffeset = 0;
            int npcCount = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            for (int n = 0; n < npcCount; n++)
            {
                LA2NPC npc = new LA2NPC(Utils.ReadIntFromBuff(packet, ref currentOffeset));

                //LA2Object
                npc.ID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.Name = Utils.ReadStrFromBuff(packet, ref currentOffeset);
                npc.Valid = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

                //LA2Spawn
                npc.X = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.Y = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.Z = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.SpawnTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                //LA2Live
                npc.AbnormalID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.AllyID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.AtkOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.AtkTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.ClanID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.CurHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.CurMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.Fishing = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.HP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.Karma = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.Level = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.Load = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.MaxHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.MaxMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.MP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.MyAtkTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.SP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.TargetOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.Team = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.TeleportDist = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.TeleportTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.ToX = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.ToY = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.ToZ = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                npc.Speed = Utils.ReadDoubleFromBuff(packet, ref currentOffeset);

                npc.Exp = Utils.ReadLongFromBuff(packet, ref currentOffeset);
                npc.EXP2 = Utils.ReadLongFromBuff(packet, ref currentOffeset);

                npc.Attackable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Dead = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Dropped = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Fly = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.InCombat = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.IsMember = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Moved = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.PK = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.PvP = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Running = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Sitting = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Sweepable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

                npc.Ally = Utils.ReadStrFromBuff(packet, ref currentOffeset);
                npc.Clan = Utils.ReadStrFromBuff(packet, ref currentOffeset);
                npc.Title = Utils.ReadStrFromBuff(packet, ref currentOffeset);

                //LA2NPC
                npc.IsPet = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.PetType = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                npcs.AddLast(npc);
            }

            client.server.npcList.UpdateList(npcs);
        }

        private void ProcessNPCQuickInfoPacket(byte[] packet)
        {
            LinkedList<LA2NPC> npcs = new LinkedList<LA2NPC>();

            int currentOffeset = 0;
            int npcCount = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            for (int n = 0; n < npcCount; n++)
            {
                LA2NPC npc = new LA2NPC(Utils.ReadIntFromBuff(packet, ref currentOffeset));

                //LA2Object
                npc.Valid = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

                //LA2Spawn
                npc.X = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.Y = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.Z = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                //LA2Live
                npc.AbnormalID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.AtkOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.AtkTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.CurHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.CurMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.HP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.MaxHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.MaxMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.MP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.TargetOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.ToX = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.ToY = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                npc.ToZ = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                npc.Speed = Utils.ReadDoubleFromBuff(packet, ref currentOffeset);

                npc.Attackable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Dead = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Dropped = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.InCombat = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Moved = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.PK = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.PvP = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Running = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Sitting = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                npc.Sweepable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

                npcs.AddLast(npc);
            }

            LinkedList<LA2NPC> unknownNPCs = client.server.npcList.QuickUpdateList(npcs);
            if (unknownNPCs.Count > 0)
                client.SendFullNPCInfoRequestByOIDs(unknownNPCs);
        }

        private void ProcessPlayerFullInfoPacket(byte[] packet)
        {
            LinkedList<LA2Char> players = new LinkedList<LA2Char>();

            int currentOffeset = 0;
            int playerCount = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            for (int n = 0; n < playerCount; n++)
            {
                LA2Char pl = new LA2Char(Utils.ReadIntFromBuff(packet, ref currentOffeset));

                //LA2Object
                pl.ID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Name = Utils.ReadStrFromBuff(packet, ref currentOffeset);
                pl.Valid = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

                //LA2Spawn
                pl.X = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Y = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Z = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.SpawnTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                //LA2Live
                pl.AbnormalID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.AllyID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.AtkOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.AtkTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.ClanID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.CurHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.CurMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Fishing = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.HP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Karma = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Level = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Load = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.MaxHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.MaxMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.MP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.MyAtkTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.SP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.TargetOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Team = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.TeleportDist = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.TeleportTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.ToX = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.ToY = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.ToZ = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                pl.Speed = Utils.ReadDoubleFromBuff(packet, ref currentOffeset);

                pl.Exp = Utils.ReadLongFromBuff(packet, ref currentOffeset);
                pl.EXP2 = Utils.ReadLongFromBuff(packet, ref currentOffeset);

                pl.Attackable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Dead = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Dropped = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Fly = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.InCombat = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.IsMember = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Moved = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.PK = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.PvP = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Running = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Sitting = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Sweepable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

                pl.Ally = Utils.ReadStrFromBuff(packet, ref currentOffeset);
                pl.Clan = Utils.ReadStrFromBuff(packet, ref currentOffeset);
                pl.Title = Utils.ReadStrFromBuff(packet, ref currentOffeset);

                //LA2Char
                pl.CP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.CurCP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.MaxCP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.ClassID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.MainClass = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Sex = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Race = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.CubicCount = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Recom = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                pl.MountType = Utils.ReadByteFromBuff(packet, ref currentOffeset);
                pl.StoreType = Utils.ReadByteFromBuff(packet, ref currentOffeset);

                pl.Hero = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Noble = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Premium = Utils.ReadBoolFromBuff(packet, ref currentOffeset);


                players.AddLast(pl);
            }

            client.server.playerList.UpdateList(players);
        }

        private void ProcessPlayerQuickInfoPacket(byte[] packet)
        {
            LinkedList<LA2Char> players = new LinkedList<LA2Char>();

            int currentOffeset = 0;
            int playerCount = Utils.ReadIntFromBuff(packet, ref currentOffeset);

            for (int n = 0; n < playerCount; n++)
            {
                LA2Char pl = new LA2Char(Utils.ReadIntFromBuff(packet, ref currentOffeset));

                //LA2Object
                pl.Valid = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

                //LA2Spawn
                pl.X = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Y = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.Z = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                //LA2Live
                pl.AbnormalID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.AtkOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.AtkTime = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.CurHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.CurMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.HP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.MaxHP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.MaxMP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.MP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.TargetOID = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.ToX = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.ToY = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.ToZ = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                pl.Speed = Utils.ReadDoubleFromBuff(packet, ref currentOffeset);

                pl.Attackable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Dead = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Dropped = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.InCombat = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Moved = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.PK = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.PvP = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Running = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Sitting = Utils.ReadBoolFromBuff(packet, ref currentOffeset);
                pl.Sweepable = Utils.ReadBoolFromBuff(packet, ref currentOffeset);

                //LA2Char
                pl.CP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.CurCP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.MaxCP = Utils.ReadIntFromBuff(packet, ref currentOffeset);
                pl.CubicCount = Utils.ReadIntFromBuff(packet, ref currentOffeset);

                players.AddLast(pl);
            }

            LinkedList<LA2Char> unknownPlayers = client.server.playerList.QuickUpdateList(players);
            if (unknownPlayers.Count > 0)
                client.SendFullPlayerInfoRequestByOIDs(unknownPlayers);
        }
    }
}
