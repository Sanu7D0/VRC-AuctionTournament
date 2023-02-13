using System;
using AuctionTournament.Util;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AuctionTournament.Game.Auction
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AuctionScreen : UdonSharpBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AuctionManager auctionManager;
        
        // UI - Auction status
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private TextMeshProUGUI timeOutText;
        [SerializeField] private TextMeshProUGUI bidPlayerText;
        [SerializeField] private TextMeshProUGUI currentPriceText;
        
        // UI - Player status
        [SerializeField] private PlayerAuctionStatus[] playerStatuses;

        private bool _isPlayerNameUpdated = false;

        private void Update()
        {
            if (!auctionManager.IsAuctionInProgress)
                return;
            
            // Auction status group
            roundText.text = $"{auctionManager.CurrentRound}/{auctionManager.MaxRound} Auction";
            timeOutText.text = $"{auctionManager.RoundRemainTime:F0}";
            bidPlayerText.text = auctionManager.BidPlayerName;
            currentPriceText.text = auctionManager.CurrentPrice.ToString();
            
            // Player status group
            if (!_isPlayerNameUpdated)
                UpdatePlayers();
            
            int[] playerBalances = auctionManager.PlayerBalances;
            if (playerBalances == null)
                return;

            for (int i = 0; i < playerBalances.Length; i++)
                playerStatuses[i].SetBalance(playerBalances[i]);
        }

        private void UpdatePlayers()
        {
            if (gameManager.Players == null)
                return;

            VRCPlayerApi[] players = gameManager.Players;

            for (int i = 0; i < players.Length; i++)
            {
                playerStatuses[i].gameObject.SetActive(true);
                // TODO: Score
                playerStatuses[i].SetPlayer(players[i].displayName, 0);
            }

            for (int i = players.Length; i < GameManager.MaxGamePlayers; i++)
            {
                playerStatuses[i].gameObject.SetActive(false);
            }
            
            _isPlayerNameUpdated = true;
        }
    }
}
