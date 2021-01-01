using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace client
{
    class Player
    {
        public int ID { get; set; } // player id in a specific room
        public (int, int) PrevPlayerPos { get; set; }
        public char Marker { get; set; }
        public string Color { get; set; }
        (int, int) currPlayerPos;

        public (int, int) CurrPlayerPos
        {
            get { return currPlayerPos; }
            set
            {
                PrevPlayerPos = currPlayerPos;
                currPlayerPos = value;
            }
        }

        public Player(int _id, char _marker, string _color)
        {
            ID = _id;
            Marker = _marker;
            Color = _color;

            currPlayerPos = (0, 0);
            PrevPlayerPos = CurrPlayerPos;
        }
    }
}