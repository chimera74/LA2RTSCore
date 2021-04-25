using LA2RTS.LA2Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LA2RTS
{
    public class RTSPlayerList
    {
        HashSet<LA2Char> _playerList;
        private object _lock = new object();
        private RTSServer server;

        public HashSet<LA2Char> PlayerList
        {
            get { return _playerList; }
        }

        public RTSPlayerList(RTSServer server)
        {
            this.server = server;
            _playerList = new HashSet<LA2Char>();
        }

        public void UpdateList(IEnumerable<LA2Char> players)
        {
            foreach (var pl in players)
            {
                if (IsUserCharCheck(pl))
                    continue;

                LA2Char existingPlayer = null;
                lock (_lock)
                {
                    existingPlayer = _playerList.FirstOrDefault(f => f.OID == pl.OID);
                    if (existingPlayer == null)
                    {
                        _playerList.Add(pl);
                        pl.ExpiredEvent += RemovePlayer;
                        pl.ResetExpirationTimer();
                        server.RaiseNewPlayerEvent(pl);
                    }
                }

                if (existingPlayer != null)
                {
                    lock (existingPlayer._lock)
                    {
                        existingPlayer.UpdateFull(pl);
                    }
                }
            }
        }

        private bool IsUserCharCheck(LA2Char pl)
        {
            bool res = false;
            foreach (var cl in server.clients)
            {
                if (cl.UserChar.OID == pl.OID)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        private void RemovePlayer(LA2Char pl)
        {
            lock (_lock)
            {
                _playerList.Remove(pl);
            }
        }

        public LinkedList<LA2Char> QuickUpdateList(IEnumerable<LA2Char> players)
        {
            LinkedList<LA2Char> unknownPlayers = new LinkedList<LA2Char>();
            foreach (var pl in players)
            {
                if (IsUserCharCheck(pl))
                    continue;

                LA2Char existingPlayer = null;
                lock (_lock)
                {
                    existingPlayer = _playerList.FirstOrDefault(f => f.OID == pl.OID);
                }

                if (existingPlayer == null)
                {
                    unknownPlayers.AddLast(pl);
                }

                if (existingPlayer != null)
                {
                    lock (existingPlayer._lock)
                    {
                        existingPlayer.UpdateQuick(pl);
                    }
                }
            }
            return unknownPlayers;
        }
    }
}
