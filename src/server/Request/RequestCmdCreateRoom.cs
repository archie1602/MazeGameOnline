using System;

namespace server
{
    public class RequestCmdCreateRoom : RequestCmd
    {
        public char Marker { get; set; }
        public string Color { get; set; }
        public int Map { get; set; }
        public int MaxPlayers { get; set; }
    }
}
