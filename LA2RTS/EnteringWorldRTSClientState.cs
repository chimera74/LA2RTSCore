using LA2RTS.LA2Entities;
using System;
using System.Timers;
using static LA2RTS.LA2Entities.LA2UserChar;

namespace LA2RTS
{
    public class EnteringWorldRTSClientState : RTSClientState
    {
        internal Timer requestStateTimer;

        public EnteringWorldRTSClientState(RTSClient parentClient) : base(parentClient)
        {
        }

        public override void OnEnter()
        {
            client.SendStatusRequest();

            requestStateTimer = new Timer(3000);
            requestStateTimer.Elapsed += RequestStateTask;
            requestStateTimer.AutoReset = true;
            requestStateTimer.Enabled = true;
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
                case 0x05:
                    ProcessStatusPacket(message);
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

        internal void ChangeStateToOnline()
        {
            OnExit();
            client.clientState = new OnlineRTSClientState(client);
            client.clientState.OnEnter();
        }

        private void ProcessStatusPacket(byte[] packet)
        {

            if (client.UserChar == null)
                client.UserChar = new LA2UserChar(client);

            int currentOffeset = 0;
            client.UserChar.Status = (ClientStatus)Utils.ReadByteFromBuff(packet, ref currentOffeset);
            if (client.UserChar.Status == ClientStatus.InGame)
                ChangeStateToOnline();
            client.RaiseStatusPacketEvent(client);
        }
    }
}
