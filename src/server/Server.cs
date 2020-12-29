using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace server
{
    class Server
    {
        IPAddress ip;
        int port;
        string name;
        List<Room> rooms;
        public bool IsRunning { get; set; }

        public Server(string _ip, int _port, string _name)
        {
            ip = IPAddress.Parse(_ip);
            port = _port;
            name = _name;
            rooms = new List<Room>();
            IsRunning = false;
        }

        /*
        void ServerGreeting(Socket clientSock)
        {
            NetworkStream stream = new NetworkStream(clientSock);
            StreamWriter writer = new StreamWriter(stream);

            writer.WriteLine("READY " + name); // Cmd: READY [game server name]
            writer.Flush();

            writer.Close();
            stream.Close();
        }
        */

        // this method accepts new players
        void AcceptPlayer(Socket listener)
        {
            Socket playerSock = listener.Accept();

            Thread thread = new Thread(new ParameterizedThreadStart(HandlePlayerCmds));

            thread.Start(playerSock);
        }

        void SendResponse(StreamWriter sw, string response)
        {
            sw.WriteLine(response);
            sw.Flush();
        }

        void HandlePlayerCmds(object obj)
        {
            Socket playerSock = (Socket)obj;

            NetworkStream stream = new NetworkStream(playerSock);
            StreamWriter writer = new StreamWriter(stream);
            StreamReader reader = new StreamReader(stream);

            // State variables
            Room room = null; // room that corresponds current player socket

            SendResponse(writer, "220 READY " + name);

            while (true)
            {
                string request = reader.ReadLine();

                // split the command by spaces
                string[] requestSplit = request.Split(' ');

                string cmd = requestSplit[0].ToUpperInvariant();

                // Cmd: CR (marker,color,map,maxplayers), where: CR = CR CREATE ROOM
                if (cmd == "CR")
                {
                    string[] argSplit = requestSplit[1].Trim(new char[] { '(', ')' }).Split(',');

                    // get CR args
                    char marker = Char.Parse(argSplit[0]);
                    string color = argSplit[1];
                    int map = Int32.Parse(argSplit[2]);
                    int maxPlayers = Int32.Parse(argSplit[3]);

                    // must check if the arguments are correct

                    // must create new game room

                    Player ownerPlayer = new Player(0, playerSock, marker, ConsoleColor.Green);

                    room = new Room(ownerPlayer, map, maxPlayers);

                    SendResponse(writer, "255 " + room.Code);
                }

                // Также еще необходимо следующие команды: вернуть список игроков в комнате текущего плеера (если он подключен к какой-то комнате)
                // то есть команда выглядит так 'LPIR' (listing players in room)

                // Maeybe there is more than 3 command; maybe we need to process rooms commands
            }

            // Should to understand what this player really wants to do: [create room] or [enter a room]

            // Need to proccess client command in loop

            reader.Close();
            writer.Close();
            stream.Close();
        }

        // Start the server
        public void Start()
        {
            // set flag 'IsRunning' to value: true
            IsRunning = true;

            // create a local end point on which server will run
            IPEndPoint localEP = new IPEndPoint(ip, port);

            // create a tcp stream socket
            Socket listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // bind listener-socket to the local endpoint
                listener.Bind(localEP);

                // listening for a connections to the server
                listener.Listen();

                // while flag 'IsRunning' equals to true, accepts new players
                while (IsRunning)
                {
                    AcceptPlayer(listener);
                }

                listener.Close();
            }
            catch (Exception exc)
            {

            }
        }
    }
}
