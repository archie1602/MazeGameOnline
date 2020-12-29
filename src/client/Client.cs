using System;
using System.IO;
using Figgle;
using Pastel;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace client
{
    class Client
    {
        Game game;
        public Client()
        {

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
                DisplayClientVersion(10);


                Menu main = new Menu(mainMenuOptions, Menu.Type.Button, 10);

                int selectedButton = main.Run().Value.Item1;

                if (selectedButton == 0)
                    EnterRoom();
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

        void EnterRoom()
        {
            Console.Write("Enter the room");
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
                "Quit",
                "Create"
            };

            Menu createRoom = new Menu(createRoomMenuOptions, Menu.Type.Input, 10);

            try
            {
                // selectedIndex, marker, color, ip, port, maxPlayers
                (int, char, string, string, int, int) roomSettings = createRoom.Run().Value;

                if (roomSettings.Item1 == 5) // Quit
                    return false;
                if (roomSettings.Item1 == 6) // Create
                {
                    CreateRoom((roomSettings.Item2, roomSettings.Item3, roomSettings.Item4, roomSettings.Item5, roomSettings.Item6));
                }
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

        void CreateRoom((char, string, string, int, int) roomSettings)
        {
            // marker, color, ip, port, maxPlayers

            IPAddress ip = IPAddress.Parse(roomSettings.Item3);
            int port = roomSettings.Item4;
            char marker = roomSettings.Item1;
            string color = roomSettings.Item2;
            int map = 1;
            int maxPlayers = roomSettings.Item5;

            IPEndPoint localEP = new IPEndPoint(ip, port);

            Socket playerSock = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                playerSock.Connect(localEP);

                NetworkStream stream = new NetworkStream(playerSock);

                StreamReader reader = new StreamReader(stream);

                string response = reader.ReadLine();
                string request = string.Empty;
                string serverName = string.Empty;

                if (response.Contains("READY"))
                {
                    StreamWriter writer = new StreamWriter(stream);

                    request = $"CR ({marker},{color},{map},{roomSettings.Item3})"; // CREATE ROOM (marker, color, map, maxplayers)
                    serverName = response.Split(' ')[1];

                    // Add only to the streamwriter buffer
                    writer.WriteLine(request);

                    // Push and send to server and clear streamwriter buffer
                    writer.Flush();

                    // Read server answer
                    response = reader.ReadLine();

                    // If the server is ready to create a room for us
                    if (response == "RC") // ROOM CREATED
                    {

                    } // Else, server isn't ready to create a room for us :(
                    else
                    {

                    }

                    // Proccess server response

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

            // game = new Game("", 11000);
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

        void DisplayClientVersion(int positionY)
        {
            string hintText = "Client v1.0";
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
