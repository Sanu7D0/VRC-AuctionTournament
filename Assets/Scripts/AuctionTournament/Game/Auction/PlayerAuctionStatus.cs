using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AuctionTournament.Game.Auction
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerAuctionStatus : UdonSharpBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerBalanceText;
        [SerializeField] private TextMeshProUGUI playerScoreText;

        public void SetBalance(int balance)
        {
            playerBalanceText.text = balance.ToString();
        }

        public void SetPlayer(string playerName, int playerScore)
        {
            playerNameText.text = playerName;
            playerScoreText.text = playerScore.ToString();
        }

    }
}
