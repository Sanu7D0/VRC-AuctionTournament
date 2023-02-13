using AuctionTournament.Game.Auction;
using AuctionTournament.Util;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AuctionTournament.Game
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class GameManager : UdonSharpBehaviour
    {
        public const int MaxGamePlayers = 8;

        [SerializeField] private AuctionManager auctionManager;

        [UdonSynced] private int[] playerIds;

        private VRCPlayerApi[] _players;

        public int[] PlayerIds => playerIds;
        
        public VRCPlayerApi[] Players
        {
            get
            {
                if (_players != null)
                    return _players;

                if (playerIds == null)
                    return null;

                _players = playerIds.GetPlayers();
                return _players;
            }
        }

        public void StartGame(VRCPlayerApi[] players)
        {
            playerIds = players.GetPlayerIds();
            RequestSerialization();

            auctionManager.StartAuction(players);
        }

        public int GetPlayerIndex(int playerId)
        {
            for (int i = 0; i < playerIds.Length; i++)
                if (playerIds[i] == playerId)
                    return i;

            return -1;
        }
    }
}
