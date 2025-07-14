using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RewardLevel
{
    public class RewardLevel : MonoBehaviour
    {
        [SerializeField] RewardLevelSO _rewardLevelSO;
        [SerializeField] Sprite[] rewardIcon = new Sprite[2];
        [SerializeField] Image rewardImage;
        public TextMeshProUGUI RewardText;
        public void receiveReward()
        {
            GetRandomReward(_rewardLevelSO.GetRandomReward());
        }
        public void SetRewardText(int levelIndex)
        {
            RewardText.text = $"Level {levelIndex}";
        }
        public void SetRewardImage(int index)
        {
            rewardImage.sprite = rewardIcon[index];
        }
        async void GetRandomReward(KeyValuePair<Sprite, int> reward)
        {
            PopUpLevelReward popUpLevelReward = await PopUpHandler.Ins.ShowPopupAsync("PopUpLevelReward") as PopUpLevelReward;
            popUpLevelReward.ShowReward(reward);
        }
        public void SetButtonToast()
        {
            if (rewardImage.sprite == rewardIcon[1])
                return;
            UIToast.Ins.SpawnToast($"Reach {RewardText.text} to get rewards!");
        }
    }
}