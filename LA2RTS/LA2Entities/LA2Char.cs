using System;
using System.Timers;

namespace LA2RTS.LA2Entities
{
    public class LA2Char : LA2Live
    {
        public int CP;
        public int CurCP;
        public int MaxCP;
        public bool Hero;
        public bool Noble;
        public int ClassID;
        public int MainClass;
        public byte MountType;
        public byte StoreType;
        public int Sex;
        public int Race;
        public int CubicCount;
        public int Recom;
        public bool Premium;

        public event Action<LA2Char> UpdateEvent;
        public event Action<LA2Char> QuickUpdateEvent;
        public event Action<LA2Char> ExpiredEvent;

        public LA2Char() : base() { }
        public LA2Char(int oid) : base(oid) { }

        internal void UpdateFull(LA2Char obj)
        {
            base.UpdateFull(obj);

            CP = obj.CP;
            CurCP = obj.CurCP;
            MaxCP = obj.MaxCP;
            Hero = obj.Hero;
            Noble = obj.Noble;
            ClassID = obj.ClassID;
            MainClass = obj.MainClass;
            MountType = obj.MountType;
            StoreType = obj.StoreType;
            Sex = obj.Sex;
            Race = obj.Race;
            CubicCount = obj.CubicCount;
            Recom = obj.Recom;
            Premium = obj.Premium;

            if (UpdateEvent != null)
                UpdateEvent(this);
        }

        internal void UpdateQuick(LA2Char obj)
        {
            base.UpdateQuick(obj);

            CP = obj.CP;
            CurCP = obj.CurCP;
            MaxCP = obj.MaxCP;

            CubicCount = obj.CubicCount;

            if (QuickUpdateEvent != null)
                QuickUpdateEvent(this);
        }


        internal override void ObjectExpired(object source, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (ExpiredEvent != null)
                    ExpiredEvent(this);
            }
        }
    }
}
