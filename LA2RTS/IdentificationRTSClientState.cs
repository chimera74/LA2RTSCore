using System;

namespace LA2RTS
{
    public class IdentificationRTSClientState : RTSClientState
    {
        public IdentificationRTSClientState(RTSClient parentClient) : base(parentClient)
        {
        }

        public override void OnEnter()
        {
            // send id request
            // start id request timer
        }

        public override void OnExit()
        {
            //stop timer
            throw new NotImplementedException();
        }

        public override bool ProcessRequest(byte[] header, byte[] buffer)
        {

            // if ClientType packet 
            // change state
            return false;
        }

        public void RequestIdTask()
        {
            //send ClientTypeRequest
        }

    }
}
