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
        }
    }
}
