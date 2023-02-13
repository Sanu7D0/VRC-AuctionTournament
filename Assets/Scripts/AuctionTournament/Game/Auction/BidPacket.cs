using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace AuctionTournament.Game.Auction
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BidPacket : UdonSharpBehaviour
    {
        [SerializeField] private AuctionManager auctionManager;
        
        /// <summary>
        /// [0]: Sender ID
        /// [1]: Desired price
        /// [2]: Round at moment sent
        /// </summary>
        [UdonSynced] private int[] packet = new int[3];

        public override void OnDeserialization()
        {
            if (!Networking.IsMaster)
                return;
            
            auctionManager.HandleBidRequest(packet[0], packet[1], packet[2]);
        }

        public void Init(VRCPlayerApi player)
        {
            Networking.SetOwner(player, gameObject);
        }

        public void Send(int senderId)
        {
            int desiredPrice = auctionManager.CurrentPrice + AuctionManager.BidRaiseAmount;

            packet[0] = senderId;
            packet[1] = desiredPrice;
            packet[2] = auctionManager.CurrentRound;
            
            RequestSerialization();
            OnDeserialization();
        }
    }
}
