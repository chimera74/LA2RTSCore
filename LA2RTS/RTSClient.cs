using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using LA2RTS.LA2Entities;

namespace LA2RTS
{
    public class RTSClient
    {
        public Thread WorkingTread = null;
        public TcpClient TcpClient;
        public LA2UserChar UserChar;
        public RTSClientState clientState;

        public long clientID;

        internal RTSServer server;
        private bool isThreadClosing = false;

        const int BUFFER_SIZE = 1024;

        public void ClientThread()
        {

            clientState.OnEnter();

            // Читаем из потока клиента до тех пор, пока от него поступают данные
            while (server.IsRunning && !isThreadClosing)
            {
                try
                {
                    if (TcpClient.GetStream().DataAvailable)
                    {
                        byte[] header = new byte[3];
                        int headerCount = TcpClient.GetStream().Read(header, 0, 3);
                        if (headerCount != 3)
                            throw new PacketFormatException("Wrong header size");
                        int dataSizePos = 1;
                        ushort dataSize = Utils.ReadUInt16FromBuff(header, ref dataSizePos);

                        byte[] buffer = new byte[dataSize];
                        int pos = 0;
                        while (pos < dataSize)
                        {
                            pos += TcpClient.GetStream().Read(buffer, pos, dataSize - pos);
                        }
                        if (pos != dataSize)
                            throw new PacketFormatException("Data count does not match header");

                        ProcessRequest(header, buffer);
                    }
                }
                catch (IOException ioex)
                {
                    //disconnect client
                    server.RaiseExceptionEvent(this, ioex.ToString());
                    Disconnect();
                }
                catch (ObjectDisposedException)
                {
                    // consume exception;
                }
                catch (InvalidOperationException)
                {
                    // consume exception;
                }
            }
        }

        private void ProcessRequest(byte[] header, byte[] buffer)
        {

            try
            {
                bool isConsumed = clientState.ProcessRequest(header, buffer);

                if (!isConsumed)
                {
                    switch (header[0])
                    {

                        case 0x00:
                            Disconnect();
                            break;

                        case 0x01: // Client is pinging us, do nothing
                            break;

                        default:

                            break;
                    }
                }
            }
            catch (PacketFormatException ex)
            {
                server.RaiseExceptionEvent(this, ex.ToString());
                Disconnect();
            }
        }

        internal void SendPacket(byte header, byte[] message, ushort length)
        {
            try
            {
                if (length > ushort.MaxValue)
                    throw new Exception("Packet size is greater than UInt16 capacity");
                int fullLength = 3 + length;
                byte[] fullPacket = new byte[fullLength];
                fullPacket[0] = header;
                BitConverter.GetBytes(length).CopyTo(fullPacket, 1);
                if (length > 0)
                    message.CopyTo(fullPacket, 3);

                TcpClient.GetStream().Write(fullPacket, 0, fullLength);

            }
            catch (IOException)
            {
                // client closed connection
                Disconnect();
            }
        }

        public void SendEnterCredentialsCommand(string login, string password)
        {
            ushort messageLength = (ushort)((login.Length + password.Length) * 2 + 8);
            byte[] message = new byte[messageLength];
            int currentOffset = 0;
            Utils.PutStrInBuff(message, login, ref currentOffset);
            Utils.PutStrInBuff(message, password, ref currentOffset);

            SendPacket(0x02, message, (ushort)message.Length);
        }

        public void SendEnterWorldCommand()
        {
            SendPacket(0x03, null, 0);
        }

        public void SendStatusRequest()
        {
            SendPacket(0x04, null, 0);
        }

        public void SendSelfInfoRequest()
        {
            SendPacket(0x05, null, 0);
        }

        public void SendQuickSelfInfoTransmitEnable(bool isEnabled)
        {
            byte[] message = new byte[1];
            message[0] = (byte)(isEnabled ? 1 : 0);
            SendPacket(0x06, message, (ushort)message.Length);
        }

        public void SendMoveToCommand(int x, int y, int z)
        {
            ushort messageLength = 12;
            byte[] message = new byte[messageLength];
            int currentOffset = 0;
            Utils.PutIntInBuff(message, x, ref currentOffset);
            Utils.PutIntInBuff(message, y, ref currentOffset);
            Utils.PutIntInBuff(message, z, ref currentOffset);

            SendPacket(0x07, message, (ushort)message.Length);
        }

        internal void SendFullNPCInfoRequest()
        {
            SendPacket(0x08, null, 0);
        }

        public void SendQuickNPCInfoTransmitEnable(bool isEnabled)
        {
            byte[] message = new byte[1];
            message[0] = (byte)(isEnabled ? 1 : 0);
            SendPacket(0x09, message, (ushort)message.Length);
        }


        public void SendFullNPCInfoRequestByOIDs(LinkedList<LA2NPC> npcs)
        {
            int messageLength = npcs.Count * 4 + 4;
            byte[] message = new byte[messageLength];
            int currentOffset = 0;
            Utils.PutIntInBuff(message, npcs.Count, ref currentOffset);
            foreach (var npc in npcs)
            {
                Utils.PutIntInBuff(message, npc.OID, ref currentOffset);
            }

            SendPacket(0x0A, message, (ushort)message.Length);
        }

        internal void SendFullPlayerInfoRequest()
        {
            SendPacket(0x0B, null, 0);
        }

        public void SendQuickPlayerInfoTransmitEnable(bool isEnabled)
        {
            byte[] message = new byte[1];
            message[0] = (byte)(isEnabled ? 1 : 0);
            SendPacket(0x0C, message, (ushort)message.Length);
        }

        public void SendFullPlayerInfoRequestByOIDs(LinkedList<LA2Char> chars)
        {
            int messageLength = chars.Count * 4 + 4;
            byte[] message = new byte[messageLength];
            int currentOffset = 0;
            Utils.PutIntInBuff(message, chars.Count, ref currentOffset);
            foreach (var pl in chars)
            {
                Utils.PutIntInBuff(message, pl.OID, ref currentOffset);
            }

            SendPacket(0x0D, message, (ushort)message.Length);
        }

        public void SendTargetCommand(int OID)
        {
            byte[] message = new byte[4];
            int currentOffset = 0;
            Utils.PutIntInBuff(message, OID, ref currentOffset);
            SendPacket(0x0E, message, (ushort)message.Length);
        }

        public void SendUseActionCommand(int actionID, bool ctrl)
        {
            byte[] message = new byte[5];
            int currentOffset = 0;
            Utils.PutIntInBuff(message, actionID, ref currentOffset);
            Utils.PutBoolInBuff(message, ctrl, ref currentOffset);
            SendPacket(0x0F, message, (ushort)message.Length);
        }

        public void SendUseSkillCommand(int skillID, bool ctrl)
        {
            byte[] message = new byte[5];
            int currentOffset = 0;
            Utils.PutIntInBuff(message, skillID, ref currentOffset);
            Utils.PutBoolInBuff(message, ctrl, ref currentOffset);
            SendPacket(0x10, message, (ushort)message.Length);
        }

        public void SendMoveToCommand(int x, int y)
        {
            SendMoveToCommand(x, y, UserChar.Z);
        }

        public void SendEnableBotCommand(bool isEnabled)
        {
            byte[] message = new byte[1];
            message[0] = (byte)(isEnabled ? 1 : 0);
            SendPacket(0x11, message, (ushort)message.Length);
        }

        public void SendStopCommand()
        {
            SendPacket(0x12, null, 0);
        }

        public RTSClient(RTSServer server, TcpClient tcpClient)
        {
            clientID = Utils.GenerateClientID();
            clientState = new EnteringWorldRTSClientState(this);
            this.server = server;
            this.TcpClient = tcpClient;
        }

        public void Disconnect()
        {
            clientState.CleanUp();
            TcpClient.Close();
            isThreadClosing = true;
            server.clients.Remove(this);
            server.RaiseClientDisconnectEvent(this);
        }

        public event Action<RTSClient> QuickUpdatePacketEvent;
        internal void RaiseQuickUpdatePacketEvent(RTSClient cl)
        {
            QuickUpdatePacketEvent?.Invoke(cl);
        }

        public event Action<RTSClient> StatusPacketEvent;
        internal void RaiseStatusPacketEvent(RTSClient cl)
        {
            StatusPacketEvent?.Invoke(cl);
        }

        public event Action<RTSClient> SelfInfoPacketEvent;
        internal void RaiseSelfInfoPacketEvent(RTSClient cl)
        {
            SelfInfoPacketEvent?.Invoke(cl);
        }

        public event Action<RTSClient> FirstPositionPacketEvent;
        internal void RaiseFirstPositionPacketEvent(RTSClient cl)
        {
            FirstPositionPacketEvent?.Invoke(cl);
        }

        //public override bool Equals(object obj)
        //{
        //    var otherClient = obj as RTSClient;
        //    if (otherClient == null)
        //        return false;

        //    return clientID.Equals(otherClient.clientId);
        //}

        //public override int GetHashCode()
        //{
        //    return clientID.GetHashCode();
        //}
    }
}
