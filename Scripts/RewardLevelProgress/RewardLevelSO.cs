using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RewardLevel
{
    [CreateAssetMenu(fileName = "RewardLevelData", menuName = "Data/RewardLevelData")]
    public class RewardLevelSO : SerializedScriptableObject
    {
        public int id;
        [ShowInInspector]
        public Dictionary<Sprite, int> rewards = new();
        public KeyValuePair<Sprite, int> GetRandomReward()
        {
            int randomValue = UnityEngine.Random.Range(0, rewards.Count);
            return rewards.ElementAt(randomValue);
        }
    }
}