using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
namespace RewardLevel
{
    public class RewardUIHandle : MonoBehaviour
    {
        [Header("Dont change this!!!!!!!!!!!!")]
        [SerializeField] private List<RewardLevel> _rewardImage;
        [SerializeField] private Image progressBar;
        [Header("Change this!!!!!!!!!!!!!!!!!!")]
        [InfoBox("The thresholds need to in order and start with 0 (0, 5, 10, ...)!!!!!!!!!!!!!!")]
        [SerializeField] int[] thresholds = { 0, 10, 20, 30, 40 };
        int routineNumber;
        private int progressReward;
        void Awake()
        {
            progressReward = GameData.LevelShow.Value;
            routineNumber = thresholds[thresholds.Length - 1] + thresholds[1];
        }
        void Start()
        {
            SetUp();
        }
        void UpdateProgressUI(int progress)
        {
            progressBar.fillAmount = (float)progress % routineNumber / thresholds[thresholds.Length - 1];
            

        }
        void SetUp()
        {
            int progress = progressReward % routineNumber;
            if (progress == 1)
            {
                for (int i = 0; i < thresholds.Length; i++)
                {
                    PlayerPrefs.SetInt($"LevelReward{i}", 0);
                    _rewardImage[i].SetRewardImage(0);
                }
            }
            int tempProgress =
            (progress > thresholds[thresholds.Length - 1])
            ? (progressReward / routineNumber + 1) * routineNumber 
            : progressReward;
            for (int i = 0; i < thresholds.Length; i++)
            {
                if (progress >= thresholds[i] && progressReward != 1)
                {
                    if (PlayerPrefs.GetInt($"LevelReward{i}", 0) == 0)
                    {
                        PlayerPrefs.SetInt($"LevelReward{i}", 1);
                        _rewardImage[i].receiveReward();
                    }
                    if (tempProgress != routineNumber)
                    {
                        _rewardImage[i].SetRewardImage(1);
                    }
                }
                if (i == 0 && progressReward <= thresholds[thresholds.Length - 1])
                {
                    _rewardImage[i].SetRewardText(1);
                }
                else
                {
                    _rewardImage[i].SetRewardText(tempProgress / routineNumber * routineNumber + thresholds[i]);
                }
            }
            UpdateProgressUI(tempProgress);
        }
    }
}

