using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace server
{
    public class Server
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

        // this method accepts new players
        void AcceptPlayer(Socket listener)
        {
            Socket playerSock = listener.Accept();

            Thread thread = new Thread(new ParameterizedThreadStart(HandlePlayerCmds));

            thread.Start(playerSock);
        }

        void SendResponse(StreamWriter sw, object resp)
        {
            string json = JsonSerializer.Serialize(resp);

            sw.WriteLine(json);
            sw.Flush();
        }

        void ReadCmd(StreamReader sr)
        {

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

            ResponseServerState rss = new ResponseServerState()
            {
                Code = 220,
                Description = "READY",
                Name = name
            };

            SendResponse(writer, rss);

            while (true)
            {
                string request = reader.ReadLine();
                RequestCmd rc = JsonSerializer.Deserialize<RequestCmd>(request);

                // cmd: ER = enter room
                if (rc.CMD == Cmd.ER)
                {
                    // cmd: ER (marker,color,roomCode), where: ER = ENTER ROOM

                    RequestCmdEnterRoom rcer = JsonSerializer.Deserialize<RequestCmdEnterRoom>(request);

                    // must check if the arguments are correct

                    // checking the existence of a room with a given room code
                    (bool isRoomExist, Room room) t = IsRoomExist(rcer.RoomCode);

                    // if the room exists
                    if (t.isRoomExist)
                    {
                        // must connect this user to specified room

                        // bind this player to specified room
                        room = t.room;

                        lock (locker)
                        {
                            // create player instance for this client
                            player = new Player(playerSock, room.NumPlayers, rcer.Marker, rcer.Color);

                            // add player instance to specified room
                            room.AddPlayer(player);
                        }

                        ResponseEnterRoom rer = new ResponseEnterRoom()
                        {
                            Code = 245,
                            Description = "successful connection to the room",
                            VisitorPlayerID = player.ID,
                            OwnerPlayerID = room.OwnerID,
                            Map = room.Map,
                            MaxPlayers = room.MaxPlayers,
                            PlayersInfo = new PlayerInfo[room.NumPlayers]
                        };

                        Player cp = null; // current player

                        for (int i = 0; i < room.NumPlayers; i++)
                        {
                            cp = room.GetPlayerByIndex(i);
                            rer.PlayersInfo[i] = new PlayerInfo { ID = cp.ID, Color = cp.Color, Marker = cp.Marker };
                        }

                        // send a response to the client
                        SendResponse(writer, rer);
                    }
                    else
                    {
                        Response resp = new Response()
                        {
                            Code = 425,
                            Description = "the room with the given code doesn't exist"
                        };

                        // send a response to the client
                        SendResponse(writer, resp);
                    }
                }
                else if (rc.CMD == Cmd.CR) // cmd: CR = create room
                {
                    // cmd: CR (marker,color,map,maxplayers), where: CR = CREATE ROOM

                    RequestCmdCreateRoom rccr = JsonSerializer.Deserialize<RequestCmdCreateRoom>(request);

                    // must check if the arguments are correct

                    // must create new game room

                    player = new Player(playerSock, 0, rccr.Marker, rccr.Color);

                    room = new Room(player, rccr.Map, rccr.MaxPlayers);
                    room.AddPlayer(player);

                    rooms.Add(room);

                    ResponseCreateRoom rcr = new ResponseCreateRoom()
                    {
                        Code = 255,
                        Description = "new room created successfully",
                        RoomCode = room.Code
                    };

                    // send a response to the client
                    SendResponse(writer, rcr);
                }
                else if (rc.CMD == Cmd.LPIR) // cmd: LPIR = listing players in a room
                {
                    // check: if a room has been created for this user

                    if (room == null)
                    {
                        Response resp = new Response()
                        {
                            Code = 435,
                            Description = "you are not connected to any room"
                        };

                        // send a response to the client
                        SendResponse(writer, resp);
                    }
                    else
                    {
                        Player cp = null; // current player

                        ResponseListingPlayersInRoom rlpir = new ResponseListingPlayersInRoom()
                        {
                            Code = 265,
                            Description = "successful listing",
                            PlayersInfo = new PlayerInfo[room.NumPlayers]
                        };

                        for (int i = 0; i < room.NumPlayers; i++)
                        {
                            cp = room.GetPlayerByIndex(i);
                            rlpir.PlayersInfo[i] = new PlayerInfo { ID = cp.ID, Color = cp.Color, Marker = cp.Marker };
                        }

                        // send a response to the client
                        SendResponse(writer, rlpir);
                    }
                }
            }

            reader.Close();
            writer.Close();
            stream.Close();
        }

        // start the server
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
