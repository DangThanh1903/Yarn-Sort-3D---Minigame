using MiniGameThanh;
using UnityEngine;
using UnityEngine.UI;

public class PopUpOutOfHP : PopUpBase
{
    [SerializeField] Button _continueWithCoin;
    [SerializeField] Button _continueWithAds;

    public override void Init()
    {
        base.Init();
        _continueWithCoin.onClick.AddListener(OnClickContinueWithCoin);
        _continueWithAds.onClick.AddListener(OnClickContinueWithAds);
        if (!SoundManager.Ins.IsSoundLose)
            SoundManager.Ins.PlaySound(TypeSound.LOSE);
    }
    private void OnClickContinueWithCoin()
    {
        GameplayController.Ins.ResetHP();
        GameplayController.Ins.ResumeTimer();
        SoundManager.Ins.IsSoundLose = false;
        Hide();
    }
    private void OnClickContinueWithAds()
    {
        GameplayController.Ins.ResetHP();
        GameplayController.Ins.ResumeTimer();
        SoundManager.Ins.IsSoundLose = false;
        Hide();
    }
}
