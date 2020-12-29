using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace server
{
    class GamePlayer
    {
        Socket playerSock;
        (int, int) currPlayerPos;
        (int, int) prevPlayerPos;
        char marker;
        ConsoleColor color;

        public (int, int) CurrPlayerPos
        {
            get { return currPlayerPos; }
            set
            {
                prevPlayerPos = currPlayerPos;
                currPlayerPos = value;
            }
        }

        public char Marker
        {
            get { return marker; }
            set { marker = value; }
        }

        public ConsoleColor Color
        {
            get { return color; }
            set { color = value; }
        }

        // , (int, int) _currPlayerPos, char _marker, ConsoleColor _color
        public GamePlayer(Socket _playerSock)
        {
            playerSock = _playerSock;

            currPlayerPos = (0, 0);
            prevPlayerPos = currPlayerPos;
            marker = '@';
            color = ConsoleColor.Green;
        }
    }
}