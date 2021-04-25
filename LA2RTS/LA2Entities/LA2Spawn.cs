using System;

namespace LA2RTS.LA2Entities
{
    public class LA2Spawn : LA2Object
    {
        public int X;
        public int Y;
        public int Z;
        public int SpawnTime;

        public LA2Spawn() : base()
        {
        }

        public LA2Spawn(int oid) : base(oid)
        {   
        }

        internal void UpdateFull(LA2Spawn obj)
        {
            base.UpdateFull(obj);

            X = obj.X;
            Y = obj.Y;
            Z = obj.Z;
            SpawnTime = obj.SpawnTime;
        }

        internal void UpdateQuick(LA2Spawn obj)
        {
            base.UpdateQuick(obj);

            X = obj.X;
            Y = obj.Y;
            Z = obj.Z;
        }
    }
}
