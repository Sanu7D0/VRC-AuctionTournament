using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AuctionTournament.Game.Auction
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class AuctionManager : UdonSharpBehaviour
    {
        public const int BidRaiseAmount = 5;
        private const int BidRoundTimeoutSeconds = 5;
        
        [UdonSynced] private int currentPrice;
        [UdonSynced] private int currentRound;
        [UdonSynced] private int maxRound;
        [UdonSynced] private int lastBidPlayerId = -1;
        [UdonSynced] private float roundStartTime; // TODO: time sync...
        [UdonSynced] private int[] playerBalances;
        [UdonSynced] private bool isAuctionInProgress = false;
        

        [SerializeField] private GameManager gameManager;
        [SerializeField] private AuctionScreen auctionScreen;

        public bool IsAuctionInProgress => isAuctionInProgress;
        public int[] PlayerBalances => playerBalances;

        public int CurrentPrice => currentPrice;
        public int CurrentRound => currentRound;
        public int MaxRound => maxRound;

        public string BidPlayerName => lastBidPlayerId == -1 ? "" : VRCPlayerApi.GetPlayerById(lastBidPlayerId).displayName;

        public float RoundRemainTime => BidRoundTimeoutSeconds - (Time.time - roundStartTime);


        private void Start()
        {
            Debug.Log($"Bidrounditimeout: {BidRoundTimeoutSeconds}");
        }

        private void Update()
        {
            if (!Networking.IsMaster)
                return;
            
            if (!IsAuctionInProgress || RoundRemainTime > 0)
                return;
            
            EndAuctionRound();
        }

        public void StartAuction(VRCPlayerApi[] players)
        {
            playerBalances = new int[players.Length];
            for (int i = 0; i < playerBalances.Length; i++)
                playerBalances[i] = 50;
            currentRound = 0;
            maxRound = 5;
            isAuctionInProgress = true;

            StartAuctionRound();
        }

        public void HandleBidRequest(int senderId, int desiredPrice, int roundSent)
        {
            if (desiredPrice <= currentPrice)
            {
                Debug.Log("HandleBidPacket: Price desync");
                return;
            }
            if (currentRound != roundSent)
            {
                Debug.Log("HandleBidPacket: Round desync");
                return;
            }

            if (!CheckBalanceEnough(gameManager.GetPlayerIndex(senderId), desiredPrice))
            {
                Debug.Log("HandleBidPacket: Not enough balance");
                return;
            }

            lastBidPlayerId = senderId;
            currentPrice = desiredPrice;
            roundStartTime = Time.time;
            RequestSerialization();
        }
        
        private void EndAuction()
        {
            isAuctionInProgress = false;
            RequestSerialization();
        }

        private void StartAuctionRound()
        {
            lastBidPlayerId = -1;
            roundStartTime = Time.time;
            currentPrice = 0;
            currentRound++;
            RequestSerialization();
        }

        private void EndAuctionRound()
        {
            HandleBidOff();
            
            if (currentRound >= maxRound)
                EndAuction();
            else
                StartAuctionRound();
        }

        private void HandleBidOff()
        {
            if (lastBidPlayerId == -1)
                return;
            
            playerBalances[gameManager.GetPlayerIndex(lastBidPlayerId)] -= currentPrice;
            RequestSerialization();
        }

        private bool CheckBalanceEnough(int playerIndex, int amount)
        {
            return playerBalances[playerIndex] >= amount;
        }
    }
}
