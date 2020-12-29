using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace client
{
    class Game
    {
        Map map;
        List<Player> players;
        IPAddress serverIp;
        Socket playerSock;
        int serverPort;

        bool connectDone;

        public Game(string _serverIp, int _serverPort)
        {
            serverIp = IPAddress.Parse(_serverIp);
            serverPort = _serverPort;
            map = null;
            players = null;
        }

        public void Start(char playerMarker, ConsoleColor playerColor)
        {
            Console.CursorVisible = false;
            Console.Title = "Maze Game Online";

            ConnectToServer();

            //string path = "/home/archie/Desktop/MainDir/CodeProjs/Langs/CSharp/Own/MazeGameOnline/src/client/maps/map3.txt";

            //map = new Map(path);
            //player = new Player((0, 0), (map.X0, map.Y0), '@', ConsoleColor.Green);

            //RunGameLoop();
        }

        public void ConnectToServer()
        {
            // Создаём локальную конечную точку, на которой будет запущен FTP сервер
            IPEndPoint remoteEP = new IPEndPoint(serverIp, serverPort);

            // Создаём потоковый сокет TCP
            playerSock = new Socket(serverIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            connectDone = false;
            playerSock.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), playerSock);

            string messWait = "Connecting to the server";
            int posX = ((Console.WindowWidth - messWait.Length) / 2);
            int posY = Console.WindowHeight / 2;

            Console.CursorVisible = false;
            Console.Title = messWait;
            Console.Clear();
            Console.SetCursorPosition(posX, posY);
            Console.Write(messWait);

            posX += messWait.Length;

            while (connectDone)
            {
                EraseFromConsole(posX, posY, 3);

                for (int i = 0; i < 3; i++)
                {
                    Console.SetCursorPosition(posX + i, posY);
                    Console.Write(".");
                    Thread.Sleep(1000);
                }
            }
        }

        // Along the OX axis
        void EraseFromConsole(int x, int y, int num)
        {
            Console.ResetColor();

            for (int i = 0; i < num; i++)
            {
                Console.SetCursorPosition(x + i, y);
                Console.Write(" ");
            }
        }

        void ConnectCallback(IAsyncResult ar)
        {
            playerSock.EndConnect(ar);

            // Signal that the connection had been made
            connectDone = true;
        }

        void HandlePlayerInput(Player p)
        {
            ConsoleKeyInfo keyInfo;
            ConsoleKey key;

            do
            {
                keyInfo = Console.ReadKey(true);
                key = keyInfo.Key;
            }
            while (Console.KeyAvailable);

            int xShift = map.X0;
            int yShift = map.Y0;

            (int, int) playerPos = p.CurrPlayerPos;

            int playerX = playerPos.Item1;
            int playerY = playerPos.Item2;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (map.isPosWalkable(playerX, playerY - 1))
                        p.CurrPlayerPos = (playerX, playerY - 1);
                    break;
                case ConsoleKey.DownArrow:
                    if (map.isPosWalkable(playerX, playerY + 1))
                        p.CurrPlayerPos = (playerX, playerY + 1);
                    break;
                case ConsoleKey.LeftArrow:
                    if (map.isPosWalkable(playerX - 1, playerY))
                        p.CurrPlayerPos = (playerX - 1, playerY);
                    break;
                case ConsoleKey.RightArrow:
                    if (map.isPosWalkable(playerX + 1, playerY))
                        p.CurrPlayerPos = (playerX + 1, playerY);
                    break;
            }
        }

        void DrawPlayerPosition()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.SetCursorPosition(map.X0 + 17, map.Y0 - 1);

            // Need to change statement: i < 20
            for (int i = 0; i < 20; i++)
                Console.Write(" ");

            Console.SetCursorPosition(map.X0 + 17, map.Y0 - 1);
            //Console.Write($"x: {player.CurrPlayerPos.Item1} y: {player.CurrPlayerPos.Item2}");
        }

        void RunGameLoop()
        {
            map.Draw();
            Console.SetCursorPosition(map.X0, map.Y0 - 1);
            Console.Write("Player position: ");


            while (true)
            {
                DrawPlayerPosition();

                //player.Draw();

                //HandlePlayerInput(player);
            }
        }
    }
}