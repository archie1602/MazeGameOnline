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
        Socket playerSock;
        (int, int) currPlayerPos;

        public int ID { get; set; } // player id in a specific room
        public (int, int) PrevPlayerPos { get; set; }
        public char Marker { get; set; }
        public string Color { get; set; }
        public (int, int) CurrPlayerPos
        {
            get { return currPlayerPos; }
            set
            {
                PrevPlayerPos = currPlayerPos;
                currPlayerPos = value;
            }
        }

        public Player(Socket _playerSock, int _id, char _marker, string _color)
        {
            ID = _id;
            playerSock = _playerSock;
            Marker = _marker;
            Color = _color;

            currPlayerPos = (0, 0);
            PrevPlayerPos = currPlayerPos;
        }
    }
}