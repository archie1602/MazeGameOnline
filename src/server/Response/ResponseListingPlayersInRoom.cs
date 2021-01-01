using System;

namespace server
{
    public class ResponseListingPlayersInRoom : Response
    {
        public PlayerInfo[] PlayersInfo { get; set; }
    }
}
