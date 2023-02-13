using AuctionTournament.Game;
using AuctionTournament.Game.Auction;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AuctionTournament.Lobby
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LobbyManager : UdonSharpBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AuctionController[] auctionControllers;
        
        public void StartGame()
        {
            if (!Networking.IsMaster)
            {
                Debug.LogError("Only master can start a game");
                return;
            }
            
            int playerCount = 0;
            foreach (AuctionController controller in auctionControllers)
                if (controller.Player != null)
                    playerCount++;

            var players = new VRCPlayerApi[playerCount];
            int i = 0;
            foreach (AuctionController controller in auctionControllers)
                if (controller.Player != null)
                    players[i++] = controller.Player;
            
            Debug.Log($"LobbyManager: Start game with {playerCount} players");
            gameManager.StartGame(players);
        }
    }
}
