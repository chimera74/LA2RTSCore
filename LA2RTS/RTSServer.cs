using LA2RTS.LA2Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace LA2RTS
{
    public class RTSServer
    {

        public Action<RTSClient, string> LoggerDelegate;
        public Action<RTSClient, string> ClientConnectedDelegate;
        public Action<RTSClient, string> ClientDisconnectedDelegate;

        public const int PORT = 30512;

        private int port;
        private TcpListener listener; // Объект, принимающий TCP-клиентов
        private Thread mainServerThread = null;

        public bool IsRunning = false;
        public List<RTSClient> clients = new List<RTSClient>();

        public RTSNpcList npcList;
        public RTSPlayerList playerList;

        public RTSServer(int Port)
        {
            this.port = Port;
            npcList = new RTSNpcList(this);
            playerList = new RTSPlayerList(this);
        }

        ~RTSServer()
        {
            Stop();
        }

        public void Start()
        {
            if (!IsRunning)
            {
                mainServerThread = new Thread(() =>
                {
                    // Создаем "слушателя" для указанного порта
                    listener = new TcpListener(IPAddress.Any, port);
                    Random rng = new Random();
                    listener.Start(); // Запускаем его

                    // В бесконечном цикле
                    while (IsRunning)
                    {

                        //check if there is a pending client
                        if (!listener.Pending())
                        {
                            Thread.Sleep(200);
                            if (!IsRunning)
                            {
                                break;
                            }
                            continue;
                        }

                        //accept new client
                        TcpClient client = listener.AcceptTcpClient();

                        //TODO: try catch
                        RTSClient newClient = new RTSClient(this, client);
                        clients.Add(newClient);
                        newClient.WorkingTread = new Thread(newClient.ClientThread);

                        newClient.WorkingTread.Start();

                        RaiseClientConnectedEvent(newClient);
                    }
                });

                mainServerThread.Start();

                IsRunning = true;
            }
        }

        public void Stop()
        {   
            if (IsRunning)
            {
                if (listener != null)
                {

                    IsRunning = false;
                    foreach (RTSClient cl in new List<RTSClient>(clients))
                    {
                        cl.Disconnect();
                    }
                    listener.Stop();
                }
                IsRunning = false;
            }
        }

        public void DisconnectClient(RTSClient client)
        {
            client.Disconnect();
            clients.Remove(client);
        }

        public LA2Live FindSpawnByOID(int oid)
        {
            LA2Live res = null;

            var client = clients.FirstOrDefault(c => c.UserChar.OID == oid);
            if (client != null)
            {
                res = client.UserChar;
                return res;
            }

            res = playerList.PlayerList.FirstOrDefault(p => p.OID == oid);
            if (res != null)
                return res;

            res = npcList.NPCList.FirstOrDefault(n => n.OID == oid);

            return res;
        }

        #region Events

        public void RegisterOnAllEvents(Action<RTSClient, string> deleagate)
        {
            ExceptionEvent += deleagate;

            //DisconnectEvent += deleagate;
            //SelfInfoPacketEvent += deleagate;
            //PingPongEvent += deleagate;            
        }
        //logger events

        public event Action<RTSClient> ClientDisconnectedEvent;
        internal void RaiseClientDisconnectEvent(RTSClient cl)
        {
            ClientDisconnectedEvent?.Invoke(cl);
        }

        public event Action<RTSClient> ClientConnectedEvent;
        internal void RaiseClientConnectedEvent(RTSClient cl)
        {
            ClientConnectedEvent?.Invoke(cl);
        }

        public event Action<RTSClient, string> PingPongEvent;
        internal void RaisePingPongEvent(RTSClient cl, string mes)
        {
            PingPongEvent?.Invoke(cl, mes);
        }

        public event Action<RTSClient, string> ExceptionEvent;
        internal void RaiseExceptionEvent(RTSClient cl, string mes)
        {
            ExceptionEvent?.Invoke(cl, mes);
        }

        public event Action<LA2NPC> NewNpcEvent;
        internal void RaiseNewNpcEvent(LA2NPC npc)
        {
            NewNpcEvent?.Invoke(npc);
        }


        public event Action<LA2Char> NewPlayerEvent;
        internal void RaiseNewPlayerEvent(LA2Char pl)
        {
            NewPlayerEvent?.Invoke(pl);
        }

        #endregion
    }
}
