using VRC.SDKBase;

namespace AuctionTournament.Util
{
    public static class ArrayExtensions
    {
        public static int[] GetPlayerIds(this VRCPlayerApi[] players)
        {
            int[] ids = new int[players.Length];
            for (int i = 0; i < players.Length; i++)
                ids[i] = players[i].playerId;

            return ids;
        }

        public static VRCPlayerApi[] GetPlayers(this int[] playerIds)
        {
            var players = new VRCPlayerApi[playerIds.Length];
            for (int i = 0; i < playerIds.Length; i++)
                players[i] = VRCPlayerApi.GetPlayerById(playerIds[i]);

            return players;
        }
    }
}