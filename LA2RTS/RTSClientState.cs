namespace LA2RTS
{
    public abstract class RTSClientState
    {

        protected RTSClient client;

        public RTSClientState(RTSClient parentClient)
        {
            this.client = parentClient;
        }

        public abstract void OnEnter();
        public virtual void OnExit()
        {
            CleanUp();
        }
        public abstract bool ProcessRequest(byte[] header, byte[] message);
        public virtual void CleanUp()
        { }

    }
}
