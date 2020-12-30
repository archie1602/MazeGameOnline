using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
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
        object locker = new object();
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

        // checking the existence of a room with a given room code
        (bool isRoomExist, Room room) IsRoomExist(string roomCode)
        {
            foreach (Room r in rooms)
                if (r.Code == roomCode)
                    return (true, r);

            return (false, null);
        }

        void HandlePlayerCmds(object obj)
        {
            Socket playerSock = (Socket)obj;

            NetworkStream stream = new NetworkStream(playerSock);
            StreamWriter writer = new StreamWriter(stream);
            StreamReader reader = new StreamReader(stream);

            // state variables
            Player player = null; // client player
            Room room = null; // room that corresponds current player socket

            SendResponse(writer, "220 READY " + name);

            while (true)
            {
                string request = reader.ReadLine();

                // split the command by spaces
                string[] requestSplit = request.Split(' ');

                string cmd = requestSplit[0].ToUpperInvariant();

                if (cmd == "ER")
                {
                    // cmd: ER (marker,color,roomCode), where: ER = ENTER ROOM

                    string[] argArr = requestSplit[1].Substring(1, requestSplit[1].Length - 2).Split(',');

                    // get ER args
                    char marker = Char.Parse(argArr[0]);
                    string color = argArr[1];
                    string roomCode = argArr[2];

                    // must check if the arguments are correct

                    // checking the existence of a room with a given room code
                    (bool isRoomExist, Room room) t = IsRoomExist(roomCode);

                    // if the room exists
                    if (t.isRoomExist)
                    {
                        // must connect this user to specified room

                        // bind this player to specified room
                        room = t.room;

                        lock (locker)
                        {
                            // create player instance for this client
                            player = new Player(playerSock, room.NumPlayers, marker, ConsoleColor.Green);

                            // add player instance to specified room
                            room.AddPlayer(player);
                        }

                        // send a response to the client
                        SendResponse(writer, "245 " + player.ID);
                    }
                    else
                    {
                        SendResponse(writer, "425");
                    }
                }
                else if (cmd == "CR")
                {
                    // cmd: CR (marker,color,map,maxplayers), where: CR = CREATE ROOM

                    string[] argSplit = requestSplit[1].Trim(new char[] { '(', ')' }).Split(',');

                    // get CR args
                    char marker = Char.Parse(argSplit[0]);
                    string color = argSplit[1];
                    int map = Int32.Parse(argSplit[2]);
                    int maxPlayers = Int32.Parse(argSplit[3]);

                    // must check if the arguments are correct

                    // must create new game room

                    player = new Player(playerSock, 0, marker, ConsoleColor.Green);

                    room = new Room(player, map, maxPlayers);
                    room.AddPlayer(player);

                    rooms.Add(room);

                    SendResponse(writer, "255 " + room.Code);
                }
                else if (cmd == "LPIR")
                {
                    // check: if a room has been created for this user

                    if (room == null)
                    {
                        // => FAIL
                    }
                    else
                    {
                        SendResponse(writer, "265 successful listing");

                        string listing = string.Empty;
                        Player cp = null; // current player

                        for (int i = 0; i < room.NumPlayers; i++)
                        {
                            cp = room.GetPlayer(i);
                            listing += $"({cp.ID},{cp.Color.ToString()},{cp.Marker})";

                            if (i + 1 != room.NumPlayers)
                                listing += ";";
                        }

                        SendResponse(writer, listing);
                    }

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
