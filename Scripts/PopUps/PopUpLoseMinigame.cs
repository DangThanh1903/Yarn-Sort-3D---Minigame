using Dragon.SDK;
using MiniGameThanh;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpLoseMinigame : PopUpBase
{
    [SerializeField] Button _continueWithCoin;
    [SerializeField] Button _continueWithAds;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] int timeDuration = 30;
    [SerializeField] int _coin = 300;
    private void ReturnLobby()
    {
        GameplayController.Ins.LoadLoseToFirebase();
        GameManager.Ins.SceneLoader.LoadScene(TypeScene.Lobby);
        Hide();
    }
    public override void Init()
    {
        base.Init();
        _closeButton.onClick.AddListener(ReturnLobby);
        _continueWithCoin.onClick.AddListener(OnClickContinueWithCoin);
        _continueWithAds.onClick.AddListener(OnClickContinueWithAds);
        _timerText.text = $"+{timeDuration}s";
        _continueWithCoin.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{_coin}";
        if (!SoundManager.Ins.IsSoundLose)
            SoundManager.Ins.PlaySound(TypeSound.LOSE);
    }
    private void OnClickContinueWithCoin()
    {
        if (GameData.Coin.Value < _coin)
        {
            gameObject.SetActive(false);
            PopUpShop popup = UIMinigameHandle.Ins.SpawnShop();
            popup.MoveToCoin();
            popup.A_CloseShop += () =>
            {
                gameObject.SetActive(true);
            };
            return;
        }
        GameplayController.Ins.ResetHP();
        GameplayController.Ins.IncreaseTimer(timeDuration);
        GameplayController.Ins.SetUpButton();
        SoundManager.Ins.IsSoundLose = false;
        GameData.Coin.Value -= _coin;
        Hide();
    }
    private void OnClickContinueWithAds()
    {
        GameplayController.Ins.ResetHP();
        GameplayController.Ins.IncreaseTimer(timeDuration);
        GameplayController.Ins.SetUpButton();
        SoundManager.Ins.IsSoundLose = false;
        Hide();
    }
}
