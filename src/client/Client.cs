using System;
using Figgle;
using Pastel;
using System.IO;
using System.Net;
using System.Threading;
using System.Text.Json;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace client
{
    class Client
    {
        Room room;
        PlayerType playerType;
        Socket playerSock;
        NetworkStream stream;
        StreamWriter writer;
        StreamReader reader;

        bool isListing = false;

        public enum PlayerType
        {
            owner,
            visitor
        }

        public Client()
        {
            room = null;
            playerSock = null;
            stream = null;
            writer = null;
            reader = null;
        }

        public void Start()
        {
            while (true)
            {
                Console.CursorVisible = false;
                Console.Clear();

                DisplayIntro();

                string[] mainMenuOptions = new string[]
                {
                    "Enter the room",
                    "Create a room",
                    "About",
                    "Quit"
                };

                DisplayTitle("Welcome to the Maze Game. To start the game you need to connect to the server", ConsoleColor.DarkGreen, 8);
                DisplayTitleRight("Client v1.0", 10);

                Menu main = new Menu(mainMenuOptions, Menu.Type.Main, 10);

                int selectedButton = main.Run().Value.selectedIndex;

                if (selectedButton == 0)
                    if (DisplayEnterRoom())
                        break;
                    else
                        continue;
                if (selectedButton == 1)
                {
                    if (DisplayCreateRoom())
                        break;
                    else
                        continue;
                }
                if (selectedButton == 2)
                    DisplayAbout();
                else if (selectedButton == 3)
                    Quit();

            }
        }

        // Menu methods

        bool DisplayEnterRoom()
        {
            Console.Clear();
            Console.Title = "Maze Game Online: enter the room";

            DisplayIntro();

            DisplayTitle("To connect to a room, enter its details and click 'Connect'", ConsoleColor.DarkGreen, 8);

            string[] erOpts = new string[]
            {
                "Marker:",
                "Color:",
                "Ip:",
                "Port:",
                "Room code:",
                "Connect",
                "Quit"
            };

            Menu enterRoom = new Menu(erOpts, Menu.Type.EnterRoom, 10);

            try
            {
                (int selectedIndex, char marker, string color, string ip, int port, int maxPlayers, string roomCode) roomInfo = enterRoom.Run().Value;

                if (roomInfo.selectedIndex == 5) // Enter
                    EnterRoom((roomInfo.marker, roomInfo.color, roomInfo.ip, roomInfo.port, roomInfo.roomCode));
                else if (roomInfo.selectedIndex == 6) // Quit
                    return false;
            }
            catch (FormatException ioExc)
            {
                Console.Clear();
                Console.WriteLine(ioExc.ToString());
                Console.ReadKey();
            }
            catch (OverflowException overflowExc)
            {
                Console.Clear();
                Console.WriteLine(overflowExc.ToString());
                Console.ReadKey();
            }
            catch (SocketException sockExc)
            {

            }

            return true;
        }

        bool DisplayCreateRoom()
        {
            Console.Clear();
            Console.Title = "Maze Game Online: create a room";

            DisplayIntro();

            DisplayTitle("To create a room, specify its settings and click 'Create'", ConsoleColor.DarkGreen, 8);

            string[] createRoomMenuOptions = new string[]
            {
                "Marker:",
                "Color:",
                "Ip:",
                "Port:",
                "Max players:",
                "Create",
                "Quit"
            };

            Menu createRoom = new Menu(createRoomMenuOptions, Menu.Type.CreateRoom, 10);

            try
            {
                // selectedIndex, marker, color, ip, port, maxPlayers, roomCode
                (int selectedIndex, char marker, string color, string ip, int port, int maxPlayers, string roomCode) roomSettings = createRoom.Run().Value;

                if (roomSettings.selectedIndex == 5) // Quit
                {
                    // marker, color, ip, port, maxPlayers
                    CreateRoom((roomSettings.marker, roomSettings.color, roomSettings.ip, roomSettings.port, roomSettings.maxPlayers));
                }
                else if (roomSettings.selectedIndex == 6) // Create
                    return false;
            }
            catch (FormatException ioExc)
            {
                Console.Clear();
                Console.WriteLine(ioExc.ToString());
                Console.ReadKey();
            }
            catch (OverflowException overflowExc)
            {
                Console.Clear();
                Console.WriteLine(overflowExc.ToString());
                Console.ReadKey();
            }
            catch (SocketException sockExc)
            {

            }

            return true;
        }

        bool ValidateIPAddress(string ipAddress)
        {
            // Set the default return value to false
            bool isIPAddress = false;

            // Set the regular expression to match IP addresses
            string ipPattern = @"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";

            // Create an instance of System.Text.RegularExpressions.Regex
            Regex regex = new Regex(ipPattern);

            // Validate the IP address
            isIPAddress = regex.IsMatch(ipAddress);

            return isIPAddress;
        }

        void SendCmd(object requestCmd)
        {
            string json = JsonSerializer.Serialize(requestCmd);

            writer.WriteLine(json);
            writer.Flush();
        }

        void EnterRoom((char marker, string color, string ip, int port, string roomCode) roomInfo)
        {
            IPAddress ip = IPAddress.Parse(roomInfo.ip);
            int port = roomInfo.port;
            char marker = roomInfo.marker;
            string color = roomInfo.color;
            string roomCode = roomInfo.roomCode;

            IPEndPoint remoteEP = new IPEndPoint(ip, port);

            playerSock = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                playerSock.Connect(remoteEP);

                stream = new NetworkStream(playerSock);
                reader = new StreamReader(stream);

                string response = reader.ReadLine();
                ResponseServerState rss = JsonSerializer.Deserialize<ResponseServerState>(response);

                string serverName = rss.Name;

                if (rss.Code == 220)
                {
                    writer = new StreamWriter(stream);

                    // ENTER ROOM (Cmd.ER, marker, color, roomCode)
                    RequestCmdEnterRoom rcer = new RequestCmdEnterRoom()
                    {
                        CMD = Cmd.ER,
                        Marker = marker,
                        Color = color,
                        RoomCode = roomCode
                    };

                    // send command to the server
                    SendCmd(rcer);

                    // read server answer
                    response = reader.ReadLine();
                    ResponseEnterRoom rer = JsonSerializer.Deserialize<ResponseEnterRoom>(response);

                    // proccess server response

                    // if the connection to the room is established
                    if (rer.Code == 245)
                    {
                        playerType = PlayerType.visitor;

                        //Player visitor = new Player(rer.VisitorPlayerID, marker, color);
                        room = new Room(roomCode, rer.OwnerPlayerID, rer.Map, rer.MaxPlayers);
                        //room.AddPlayer(visitor);

                        for (int i = 0; i < rer.PlayersInfo.Length; i++)
                            room.AddPlayer(new Player(rer.PlayersInfo[i].ID, rer.PlayersInfo[i].Marker, rer.PlayersInfo[i].Color));

                        WaitingRoom();

                    } // Else, the connection to the room isn't established :(
                    else if (rer.Code == 425)
                    {
                        // => the room with the given code does not exist
                    }

                    writer.Close();
                }
                else
                {
                    // Server busy... Please try again later...
                }

                reader.Close();
                stream.Close();
            }
            catch (SocketException sockExc)
            {

            }
        }

        void CreateRoom((char marker, string color, string ip, int port, int maxPlayers) roomSettings)
        {
            IPAddress ip = IPAddress.Parse(roomSettings.ip);
            int port = roomSettings.port;
            char marker = roomSettings.marker;
            string color = roomSettings.color;
            int map = 1;
            int maxPlayers = roomSettings.maxPlayers;

            IPEndPoint remotEP = new IPEndPoint(ip, port);

            playerSock = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                playerSock.Connect(remotEP);

                stream = new NetworkStream(playerSock);
                reader = new StreamReader(stream);

                string response = reader.ReadLine();
                ResponseServerState rss = JsonSerializer.Deserialize<ResponseServerState>(response);

                string serverName = rss.Name;

                if (rss.Code == 220)
                {
                    writer = new StreamWriter(stream);

                    // CREATE ROOM (Cmd.CR, marker, color, map, maxplayers)
                    RequestCmdCreateRoom rccr = new RequestCmdCreateRoom()
                    {
                        CMD = Cmd.CR,
                        Marker = marker,
                        Color = color,
                        Map = map,
                        MaxPlayers = maxPlayers
                    };

                    // send command to the server
                    SendCmd(rccr);

                    // read server answer
                    response = reader.ReadLine();
                    ResponseCreateRoom rcr = JsonSerializer.Deserialize<ResponseCreateRoom>(response);

                    // proccess server response

                    // if the server is ready to create a room for us
                    if (rcr.Code == 255) // => ROOM CREATED
                    {
                        playerType = PlayerType.owner;

                        Player owner = new Player(0, marker, color);
                        room = new Room(rcr.RoomCode, 0, map, maxPlayers);
                        room.AddPlayer(owner);

                        WaitingRoom();
                    } // Else, server isn't ready to create a room for us :(
                    else
                    {

                    }

                    writer.Close();
                }
                else
                {
                    // Server busy... Please try again later...
                }

                reader.Close();
                stream.Close();
            }
            catch (SocketException sockExc)
            {
                DisplayTitle(sockExc.ToString().Substring(0, 50), ConsoleColor.Red, 8);
            }

            Console.ReadKey();
        }

        // listing players in a room with a time interval
        void ListingPlayers(object obj)
        {
            int timeout = (int)obj;

            isListing = true;

            int maxLength = 2;
            bool isErase = false;

            // LISTING PLAYERS IN THE ROOM
            RequestCmd requestCmd = new RequestCmd()
            {
                CMD = Cmd.LPIR,
            };

            while (isListing)
            {
                // send command to the server
                SendCmd(requestCmd);

                // read server response
                string response = reader.ReadLine();
                ResponseListingPlayersInRoom rlpir = JsonSerializer.Deserialize<ResponseListingPlayersInRoom>(response);

                if (rlpir.Code == 265)
                {
                    // STOPPED HERE
                    // TEST THIS CODE
                    // AND FIX WaitingRoom() method

                    // Erase
                    if (isErase)
                    {
                        Console.SetCursorPosition((Console.WindowWidth - (maxLength + 1 + 16)) / 2, 10);
                        for (int i = 0; i < rlpir.PlayersInfo.Length; i++)
                            for (int j = 0; j < maxLength + 1 + 16; j++)
                                Console.Write(" ");
                    }
                    else
                        isErase = true;

                    for (int i = 0; i < rlpir.PlayersInfo.Length; i++)
                    {
                        Console.SetCursorPosition((Console.WindowWidth - (maxLength + 1 + 16)) / 2, 10 + i);
                        Console.Write($"{rlpir.PlayersInfo[i].Marker}".Pastel(rlpir.PlayersInfo[i].Color));

                        Console.Write("                ");
                        Console.Write($"{rlpir.PlayersInfo[i].ID}");
                    }

                    /*
                    // Erase
                    if (t != null)
                    {
                        Console.SetCursorPosition((Console.WindowWidth - (maxLength + 1 + 16)) / 2, 10);
                        for (int i = 0; i < t.Length; i++)
                        {
                            for (int j = 0; j < maxLength + 1 + 16; j++)
                            {
                                Console.Write(" ");
                            }
                        }
                    }

                    string[] players = listing.Split(';');
                    t = new (int, ConsoleColor, char)[players.Length];


                    for (int i = 0; i < players.Length; i++)
                    {
                        string[] playerInfo = players[i].Substring(1, players[i].Length - 2).Split(',');

                        if (playerInfo[0].Length > maxLength)
                            maxLength = playerInfo[0].Length;

                        t[i].id = int.Parse(playerInfo[0]);
                        t[i].color = ConsoleColor.Green;
                        t[i].marker = Char.Parse(playerInfo[2]);
                    }

                    Console.SetCursorPosition((Console.WindowWidth - (maxLength + 1 + 16)) / 2, 10);

                    foreach ((int id, ConsoleColor color, char marker) p in t)
                    {
                        Console.ForegroundColor = p.color;
                        Console.Write($"{p.marker}");
                        Console.ResetColor();

                        Console.Write("                ");
                        Console.WriteLine($"{p.id}");
                    }
                    */
                }
                else if (rlpir.Code == 435)
                {
                    // => BAD: listing players => current player is not connected to any room
                }

                Thread.Sleep(timeout);
            }
        }

        void WaitingRoom()
        {
            Console.Clear();
            // TODO: add title
            DisplayIntro();
            DisplayTitle("Player list", ConsoleColor.DarkGreen, 8);

            DisplayTitleLeft("Owner: ", 8);
            Console.Write($"{room.GetPlayerByID(room.OwnerID).Marker}".Pastel(room.GetPlayerByID(room.OwnerID).Color));

            DisplayTitleRight("Room code: " + room.Code, 8);

            Thread thread = new Thread(new ParameterizedThreadStart(ListingPlayers));
            thread.Start(5000);

            if (playerType == PlayerType.owner)
            {
                string[] WaitingRoomMenyOptions = new string[]
                {
                    "Start",
                    "Quit"
                };

                Menu waitingRoom = new Menu(WaitingRoomMenyOptions, Menu.Type.WaitingRoomOwner, 10);

                int selectedButton = waitingRoom.Run().Value.selectedIndex;
            }
            else if (playerType == PlayerType.visitor)
            {
                string[] WaitingRoomMenyOptions = new string[]
                {
                    "Disconnect"
                };

                Menu waitingRoom = new Menu(WaitingRoomMenyOptions, Menu.Type.WaitingRoomVisitor, 10);

                int selectedButton = waitingRoom.Run().Value.selectedIndex;
            }
        }

        void DisplayAbout()
        {
            Console.Write("Some about information...");
        }

        void Quit()
        {
            // reset console settings
            Console.CursorVisible = true;
            Console.Clear();
            Environment.Exit(0);
        }

        void DisplayTitle(string hintText, ConsoleColor color, int positionY)
        {
            Console.SetCursorPosition(0, positionY);

            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(" ");

            Console.SetCursorPosition((Console.WindowWidth - hintText.Length) / 2, positionY);
            Console.ForegroundColor = color;
            Console.Write(hintText);
            Console.ResetColor();
        }

        void DisplayTitleLeft(string hintText, int positionY)
        {
            Console.SetCursorPosition(0, positionY);
            Console.Write(hintText.Pastel("#CF2856"));
        }

        void DisplayTitleRight(string hintText, int positionY)
        {
            Console.SetCursorPosition((Console.WindowWidth - hintText.Length), positionY);
            Console.Write(hintText.Pastel("#CF2856"));
        }

        void DisplayIntro()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(FiggleFonts.Larry3d.Render("Maze Game").Pastel("#47C925"));
        }
    }
}
