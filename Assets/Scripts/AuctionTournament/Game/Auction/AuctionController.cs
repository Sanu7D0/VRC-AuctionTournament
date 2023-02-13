using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AuctionTournament.Game.Auction
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class AuctionController : UdonSharpBehaviour
    {
        [UdonSynced, FieldChangeCallback(nameof(PlayerId))]
        private int playerId;
        [UdonSynced, FieldChangeCallback(nameof(Online))]
        private bool online = false;
        [UdonSynced, FieldChangeCallback(nameof(Recall))]
        private bool recall;

        [SerializeField] private AuctionManager auctionManager;
        [SerializeField] private BidPacket bidPacket;
        
        [SerializeField] private MeshRenderer materialTarget;
        [SerializeField] private Material offlineMaterial;
        [SerializeField] private Material onlineMaterial;

        public int PlayerId
        {
            get => playerId;
            set
            {
                playerId = value;

                Player = playerId == -1 ? null : VRCPlayerApi.GetPlayerById(playerId);
            }
        }

        public bool Online
        {
            get => online;
            set
            {
                online = value;

                materialTarget.material = online ? onlineMaterial : offlineMaterial;
            }
        }

        public bool Recall
        {
            get => recall;
            set
            {
                recall = value;

                if (Player == null)
                    return;
                
                VRCPlayerApi localPlayer = Networking.LocalPlayer;
                if (localPlayer.playerId == Player.playerId)
                    localPlayer.TeleportTo(transform.position, transform.rotation);
                recall = false;
            }
        }

        public VRCPlayerApi Player { get; private set; }

        public void Bid()
        {
            if (Player == null)
                return;
            
            bidPacket.Send(playerId);
        }

        public void RecallPlayer()
        {
            SetProgramVariable(nameof(recall), true);
            RequestSerialization();
        }
        
        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (!Networking.IsMaster || Player != null) return;
            
            // TODO: 토너먼트 간에 텔레포트 해도 주인 풀리지 않도록 락인
            SetProgramVariable(nameof(online), true);
            SetProgramVariable(nameof(playerId), player.playerId);
            RequestSerialization();
            
            bidPacket.Init(player);
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (!Networking.IsMaster || Player != player) return;
            
            SetProgramVariable(nameof(online), false);
            SetProgramVariable(nameof(playerId), -1);
            RequestSerialization();
        }
    }
}
