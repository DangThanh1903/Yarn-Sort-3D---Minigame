using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RewardLevel
{
    public class PopUpLevelReward : PopUpBase
    {
        [SerializeField] Transform _parentRw;
        [SerializeField] UIRewElement _uiRwElement;

        public void ShowReward(KeyValuePair<Sprite, int> listRewardData)
        {
            SoundManager.Ins.PlaySound(TypeSound.REWARD);
            StartCoroutine(IE_delayShowRw(listRewardData));
        }
        IEnumerator IE_delayShowRw(KeyValuePair<Sprite, int> listRewardData)
        {
            UIRewElement uiRwElement = Instantiate(_uiRwElement, _parentRw);
            uiRwElement.Init(listRewardData.Key, listRewardData.Value);
            yield return new WaitForSeconds(2f);
            
        }
    }
}