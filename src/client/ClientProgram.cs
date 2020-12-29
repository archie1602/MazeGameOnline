using System;
using System.IO;
using Figgle;
using Pastel;
using System.Drawing;

using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace client
{
    class ClientProgram
    {
        static void Main(string[] args)
        {
            Client client = new Client();

            client.Start();

            /*

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 11000;

            IPEndPoint localEP = new IPEndPoint(ip, port);

            Socket playerSock = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            playerSock.Connect(localEP);

            NetworkStream stream = new NetworkStream(playerSock);

            StreamReader reader = new StreamReader(stream);

            string response1 = reader.ReadLine();
            Console.WriteLine(response1);

            string response2 = reader.ReadLine();
            Console.WriteLine(response2);

            string response3 = reader.ReadLine();
            Console.WriteLine(response3);

            reader.Close();
            stream.Close();

            */

            /*

            byte[] buffer1 = new byte[1024];

            playerSock.Receive(buffer1);

            string response1 = Encoding.ASCII.GetString(buffer1);

            byte[] buffer2 = new byte[1024];

            playerSock.Receive(buffer2);

            string response2 = Encoding.ASCII.GetString(buffer2);

            */
        }
    }
}
