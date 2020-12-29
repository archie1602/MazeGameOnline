using System;

using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace server
{
    class ServerProgram
    {
        static void Main(string[] args)
        {

            // Create game server // 127.0.0.1
            Server gameServer = new Server("192.168.0.244", 11000, "Archie server");

            gameServer.Start();


            /*
            // Create GameServer
            GameServer gs = new GameServer("127.0.0.1", 11000);

            // Create GameMap
            GameMap gm = new GameMap("/home/archie/Desktop/MainDir/CodeProjs/Langs/CSharp/Own/MazeGameOnline/src/client/maps/map3.txt");

            // Number of players
            gs.NumPlayers = 2;

            // Load Game Map
            gs.LoadMap(gm);
            */

            /*

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 11000;

            IPEndPoint localEP = new IPEndPoint(ip, port);

            Socket listener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEP);
            listener.Listen();

            Socket client = listener.Accept();

            NetworkStream stream = new NetworkStream(client);

            StreamWriter writer = new StreamWriter(stream);

            Console.WriteLine("Press any key to send 1 message");

            Console.ReadKey();

            writer.WriteLine("Hello it's me - server !!!!!");

            Console.WriteLine("Press any key to writer.Flush()");

            Console.ReadKey();

            writer.Flush();

            Console.WriteLine("Press any key to send 2 message");

            Console.ReadKey();

            writer.WriteLine("How are you, client ?");

            Console.WriteLine("Press any key to writer.Flush()");

            Console.ReadKey();

            writer.Flush();

            Console.WriteLine("Press any key to writer.Close()");

            Console.ReadKey();

            writer.Close();

            */

            /*
            byte[] buffer1 = Encoding.ASCII.GetBytes("Hello it's me - server!!!!!");
            byte[] buffer2 = Encoding.ASCII.GetBytes("How are you, client ?\r\n");

            client.Send(buffer1);

            client.Send(buffer2);
            */

            /*

            NetworkStream stream = new NetworkStream(listener);

            StreamWriter writer = new StreamWriter(stream);

            //string response = writer.ReadLine();

            //Console.WriteLine(response);

            writer.Close();
            stream.Close();
            */
        }
    }
}
