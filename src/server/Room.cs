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
        string code;
        int ownerID; // owner player id
        int maxPlayers;
        int numPlayers;
        int map;
        public string Code => code;
        public int NumPlayers => numPlayers;

        public Room(Player _owner, int _map, int _maxPlayers)
        {
            players = new Player[_maxPlayers];
            numPlayers = 0;
            ownerID = _owner.ID;
            map = _map;
            maxPlayers = _maxPlayers;
            game = null;

            code = GenerateCode(6, true);
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

        public Player GetPlayer(int index)
        {
            return players[index];
        }

        // add a new player to the room
        public void AddPlayer(Player player)
        {
            if (maxPlayers == numPlayers)
                throw new Exception($"Max players are {maxPlayers}");
            else
                players[numPlayers++] = player;
        }
    }
}