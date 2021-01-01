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
        public int MaxPlayers { get; set; }
        public int NumPlayers { get; set; }
        public int Map { get; set; }
        public string Code { get; set; }
        public int OwnerID { get; set; } // owner player id

        public Room(string _code, int _ownerID, int _map, int _maxPlayers)
        {
            Code = _code;
            players = new Player[_maxPlayers];
            NumPlayers = 0;
            OwnerID = _ownerID;
            Map = _map;
            MaxPlayers = _maxPlayers;
            game = null;
        }

        // add a new player to the room
        public void AddPlayer(Player player)
        {
            if (MaxPlayers == NumPlayers)
                throw new Exception($"Max players are {MaxPlayers}");
            else
                players[NumPlayers++] = player;
        }

        public Player GetPlayerByID(int id)
        {
            foreach (Player p in players)
                if (p.ID == id)
                    return p;

            return null;
        }

        public Player GetPlayerByIndex(int index)
        {
            // TODO: add check
            return players[index];
        }
    }
}