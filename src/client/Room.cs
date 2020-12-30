using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace client
{
    class Room
    {
        Player[] players;
        Game game;
        string code;
        int ownerID; // owner player id
        int maxPlayers;
        int numPlayers;
        int map;
        public string Code => code;
        public int OwnerID => ownerID;

        public Room(string _code, int _ownerID, int _map, int _maxPlayers)
        {
            code = _code;
            players = new Player[_maxPlayers];
            numPlayers = 0;
            ownerID = _ownerID;
            map = _map;
            maxPlayers = _maxPlayers;
            game = null;
        }

        // add a new player to the room
        public void AddPlayer(Player player)
        {
            if (maxPlayers == numPlayers)
                throw new Exception($"Max players are {maxPlayers}");
            else
                players[numPlayers++] = player;
        }

        public Player GetPlayer(int index)
        {
            return players[index];
        }
    }
}