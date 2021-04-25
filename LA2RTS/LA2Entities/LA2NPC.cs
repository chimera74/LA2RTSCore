using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LA2RTS.LA2Entities
{
    public class LA2NPC : LA2Live
    {
        public bool IsPet;
        public int PetType;

        public event Action<LA2NPC> UpdateEvent;
        public event Action<LA2NPC> QuickUpdateEvent;
        public event Action<LA2NPC> ExpiredEvent;

        public LA2NPC() : base() { }
        public LA2NPC(int oid) : base(oid) { }

        internal void UpdateFull(LA2NPC obj)
        {
            base.UpdateFull(obj);

            IsPet = obj.IsPet;
            PetType = obj.PetType;

            if (UpdateEvent != null)
                UpdateEvent(this);
        }

        internal void UpdateQuick(LA2NPC obj)
        {
            base.UpdateQuick(obj);

            if (QuickUpdateEvent != null)
                QuickUpdateEvent(this);
        }

        internal override void ObjectExpired(object source, ElapsedEventArgs e)
        {
            lock(_lock)
            {
                if (ExpiredEvent != null)
                    ExpiredEvent(this);
            }
        }
    }
}
