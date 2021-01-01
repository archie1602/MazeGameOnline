using System;

namespace client
{
    public class ResponseEnterRoom : Response
    {
        public int VisitorPlayerID { get; set; }
        public int OwnerPlayerID { get; set; }
        public int Map { get; set; }
        public int MaxPlayers { get; set; }
        public PlayerInfo[] PlayersInfo { get; set; }
        //public char OwnerPlayerMarker { get; set; }
        //public string OwnerPlayerColor { get; set; }
    }
}
