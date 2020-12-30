using System;
using System.IO;
using Figgle;
using Pastel;
using System.Net;
using System.Threading;
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

                DisplayTitle("Welcome to the Maze Game. To start the game you need to connect to the server.", ConsoleColor.DarkGreen, 8);
                DisplayTitleRight("Client v1.0", 10);

                Menu main = new Menu(mainMenuOptions, Menu.Type.Main, 10);

                int selectedButton = main.Run().Value.selectedIndex;

                if (selectedButton == 0)
                    DisplayEnterRoom();
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
                // selectedIndex, marker, color, ip, port, maxPlayers, roomCode
                (int selectedIndex, char marker, string color, string ip, int port, int maxPlayers, string roomCode) roomInfo = enterRoom.Run().Value;

                if (roomInfo.selectedIndex == 5) // Enter
                {
                    // marker, color, ip, port, roomCode

                    EnterRoom((roomInfo.marker, roomInfo.color, roomInfo.ip, roomInfo.port, roomInfo.roomCode));
                }
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

        void CreateRoom((char marker, string color, string ip, int port, int maxPlayers) roomSettings)
        {
            // marker, color, ip, port, maxPlayers

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
                string request = string.Empty;
                string serverName = string.Empty;

                if (response.Contains("READY"))
                {
                    writer = new StreamWriter(stream);

                    request = $"CR ({marker},{color},{map},{maxPlayers})"; // CREATE ROOM (marker, color, map, maxplayers)
                    serverName = response.Substring(10);

                    // Add only to the streamwriter buffer
                    writer.WriteLine(request);

                    // Push and send to server and clear streamwriter buffer
                    writer.Flush();

                    // Read server answer
                    response = reader.ReadLine();

                    // Proccess server response

                    // If the server is ready to create a room for us
                    if (response.Contains("255")) // => ROOM CREATED
                    {
                        playerType = PlayerType.owner;

                        string roomCode = response.Split(' ')[1];

                        Player owner = new Player(0, marker, ConsoleColor.Green);
                        room = new Room(roomCode, 0, map, maxPlayers);
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

        void EnterRoom((char marker, string color, string ip, int port, string roomCode) roomInfo)
        {
            // marker, color, ip, port, roomCode

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
                string request = string.Empty;
                string serverName = string.Empty;

                if (response.Contains("READY"))
                {
                    writer = new StreamWriter(stream);

                    request = $"ER ({marker},{color},{roomCode})"; // ENTER ROOM (marker, color, roomCode)
                    serverName = response.Substring(10);

                    // send command to the server
                    SendCmd(request);

                    // read server answer
                    response = reader.ReadLine();

                    // proccess server response

                    // If the connetion to the room is established
                    if (response.Contains("245")) // => connection to the room is established
                    {
                        playerType = PlayerType.visitor;

                        int playerID = int.Parse(response.Split(' ')[1]);

                        Player visitor = new Player(playerID, marker, ConsoleColor.Yellow);
                        room = new Room(roomCode, 0, 1, 5);
                        room.AddPlayer(visitor);

                        WaitingRoom();

                    } // Else, the connection to the room isn't established :(
                    else if (response.Contains("425"))
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

        void SendCmd(string cmd)
        {
            writer.WriteLine(cmd);
            writer.Flush();
        }

        // listing players in a room with a time interval
        void ListingPlayers(object obj)
        {
            int timeout = (int)obj;

            isListing = true;

            int maxLength = 0;
            (int id, ConsoleColor color, char marker)[] t = null;

            while (isListing)
            {
                // send command
                SendCmd("LPIR");


                // read server response
                string response = reader.ReadLine();


                if (response.Contains("265"))
                {
                    string listing = reader.ReadLine();

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
                }
                else
                {
                    // => BAD: listing players
                }

                Thread.Sleep(timeout);
            }
        }

        void WaitingRoom()
        {
            Console.Clear();

            DisplayIntro();
            DisplayTitle("Player list", ConsoleColor.DarkGreen, 8);

            DisplayTitleLeft("Owner: ", 8);
            Console.ForegroundColor = room.GetPlayer(room.OwnerID).Color;
            Console.Write($"{room.GetPlayer(room.OwnerID).Marker}");
            Console.ResetColor();

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
