using System.Collections;
using System.Collections.Generic;
using MiniGameThanh;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpWinMinigame : PopUpBase
{
    [SerializeField] Button _nextButton;
    [SerializeField] Image _medalImage;
    [SerializeField] Image _levelImage;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] List<Sprite> medals;
    public override void Init()
    {
        base.Init();
        _nextButton.onClick.AddListener(OnClickNextButton);
        SoundManager.Ins.PlaySound(TypeSound.WIN);
    }
    void Awake()
    {
        GetCurrnencyMedal();
    }
    private void OnClickNextButton()
    {
        GameManager.Ins.SceneLoader.LoadScene(TypeScene.Lobby);
        Hide();
    }
    private void GetCurrnencyMedal()
    {
        int temp = PlayerPrefs.GetInt($"medal_{MiniGameController.Ins.currentLevel.id}", 0);
        _medalImage.sprite = medals[temp - 1];
        // switch (temp)
        // {
        //     case 1:
        //         _text.text = "You receive a bronze medal";
        //         break;
        //     case 2:
        //         _text.text = "You receive a silver medal";
        //         break;
        //     case 3:
        //         _text.text = "You receive a gold medal";
        //         break;
        //     default:
        //         _text.text = "Something wrong happened";
        //         break;
        // }
        _levelImage.sprite = MiniGameController.Ins.currentLevel.image;
        _text.text = $"Finished with {GameplayController.Ins.GetRemainTime()}s remain!";
    }
}
