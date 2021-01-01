using System;

namespace client
{
    public class RequestCmdCreateRoom : RequestCmd
    {
        public char Marker { get; set; }
        public string Color { get; set; }
        public int Map { get; set; }
        public int MaxPlayers { get; set; }
    }
}
