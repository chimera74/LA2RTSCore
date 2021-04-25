using LA2RTS.LA2Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LA2RTS
{
    public class RTSNpcList
    {
        HashSet<LA2NPC> _npcList;
        private object _lock = new object();
        private RTSServer server;

        public HashSet<LA2NPC> NPCList
        {
            get { return _npcList; }
        }

        public RTSNpcList(RTSServer server)
        {
            this.server = server;
            _npcList = new HashSet<LA2NPC>();
        }

        public void UpdateList(IEnumerable<LA2NPC> npcs)
        {
            foreach (var npc in npcs)
            {
                LA2NPC existingNpc = null;
                lock (_lock)
                {
                    existingNpc = _npcList.FirstOrDefault(f => f.OID == npc.OID);
                    if (existingNpc == null)
                    {
                        _npcList.Add(npc);
                        npc.ExpiredEvent += RemoveNPC;
                        npc.ResetExpirationTimer();
                        server.RaiseNewNpcEvent(npc);
                    }
                }
                
                if (existingNpc != null)
                {
                    lock (existingNpc._lock)
                    {
                        existingNpc.UpdateFull(npc);
                    }
                }
            }
        }

        private void RemoveNPC(LA2NPC npc)
        {
            lock (_lock)
            {
                _npcList.Remove(npc);
            }
        }

        public LinkedList<LA2NPC> QuickUpdateList(IEnumerable<LA2NPC> npcs)
        {
            LinkedList<LA2NPC> unknownNPCs = new LinkedList<LA2NPC>();
            foreach (var npc in npcs)
            {   
                LA2NPC existingNpc = null;
                lock (_lock)
                {
                    existingNpc = _npcList.FirstOrDefault(f => f.OID == npc.OID);
                }

                if (existingNpc == null)
                {
                    unknownNPCs.AddLast(npc);
                }

                if (existingNpc != null)
                {
                    lock (existingNpc._lock)
                    {
                        existingNpc.UpdateQuick(npc);
                    }
                }
            }
            return unknownNPCs;
        }
    }
}
