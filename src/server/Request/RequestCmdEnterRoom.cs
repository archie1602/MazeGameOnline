using System;

namespace server
{
    // class - requested command enter room
    public class RequestCmdEnterRoom : RequestCmd
    {
        public char Marker { get; set; }
        public string Color { get; set; }
        public string RoomCode { get; set; }
    }
}
