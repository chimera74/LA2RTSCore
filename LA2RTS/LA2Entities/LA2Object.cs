using System;
using System.Timers;

namespace LA2RTS.LA2Entities
{
    public class LA2Object
    {
        public int ID;
        public int OID;
        public string Name;
        public bool Valid;

        public object _lock = new object();
        
        private Timer expirationTimer;

        const int OBJECT_EXPIRATION_TIME_MS = 6000;

        public LA2Object()
        {
        }

        public LA2Object(int oid)
        {
            OID = oid;
        }

        internal void UpdateFull(LA2Object obj)
        {
            ResetExpirationTimer();
            ID = obj.ID;
            Name = obj.Name;
            Valid = obj.Valid;
        }

        internal void UpdateQuick(LA2Object obj)
        {
            ResetExpirationTimer();
            Valid = obj.Valid;
        }

        internal void ResetExpirationTimer()
        {
            if (expirationTimer != null)
            {
                expirationTimer.Stop();
                expirationTimer.Enabled = false;
                expirationTimer.Dispose();
            }

            expirationTimer = new Timer(OBJECT_EXPIRATION_TIME_MS);
            expirationTimer.Elapsed += ObjectExpired;
            expirationTimer.AutoReset = false;
            expirationTimer.Enabled = true;
        }

        internal virtual void ObjectExpired(Object source, ElapsedEventArgs e)
        {
        }
    }

    
}
