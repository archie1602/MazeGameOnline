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

            // Create game server
            Server gameServer = new Server("127.0.0.1", 11000, "Archie server");

            gameServer.Start();
        }
    }
}
