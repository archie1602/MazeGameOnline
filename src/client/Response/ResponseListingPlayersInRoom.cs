using System;

namespace client
{
    public class ResponseListingPlayersInRoom : Response
    {
        public PlayerInfo[] PlayersInfo { get; set; }
    }
}
