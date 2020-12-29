using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace server
{
    class Player
    {
        int id;
        Socket playerSock;
        (int, int) currPlayerPos;
        public (int, int) prevPlayerPos;
        char marker;
        ConsoleColor color;

        public int Id => id;

        public (int, int) CurrPlayerPos
        {
            get { return currPlayerPos; }
            set
            {
                PrevPlayerPos = currPlayerPos;
                currPlayerPos = value;
            }
        }

        public (int, int) PrevPlayerPos
        {
            get { return currPlayerPos; }
            set { prevPlayerPos = value; }
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

        public Player(int _id, Socket _playerSock, char _marker, ConsoleColor _color)
        {
            id = _id;
            playerSock = _playerSock;
            marker = _marker;
            color = _color;

            currPlayerPos = (0, 0);
            prevPlayerPos = currPlayerPos;
        }
    }
}