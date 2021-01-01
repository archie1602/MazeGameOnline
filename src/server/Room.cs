using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace server
{
    class Room
    {
        Player[] players;
        Game game;
        public int MaxPlayers { get; set; }
        public int Map { get; set; }
        public string Code { get; set; }
        public int OwnerID { get; set; } // owner player id
        public int NumPlayers { get; set; }

        public Room(Player _owner, int _map, int _maxPlayers)
        {
            players = new Player[_maxPlayers];
            NumPlayers = 0;
            OwnerID = _owner.ID;
            Map = _map;
            MaxPlayers = _maxPlayers;
            game = null;

            Code = GenerateCode(6, true);
        }

        // generates a random string with a given size
        string GenerateCode(int size, bool lowerCase = false)
        {
            Random _random = new Random();
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):   
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length = 26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
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

        // add a new player to the room
        public void AddPlayer(Player player)
        {
            if (MaxPlayers == NumPlayers)
                throw new Exception($"Max players are {MaxPlayers}");
            else
                players[NumPlayers++] = player;
        }
    }
}