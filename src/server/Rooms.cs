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
        List<Player> players;
        Game game;
        string code;
        int owner; // owner player id
        int maxPlayers;
        int map;
        public string Code => code;

        public Room(Player _ownerPlayer, int _map, int _maxPlayers)
        {
            players = new List<Player>() { _ownerPlayer };
            owner = _ownerPlayer.Id;
            map = _map;
            maxPlayers = _maxPlayers;
            game = null;

            code = GenerateCode(6, true);
        }

        // Generates a random string with a given size.    
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

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }
    }
}