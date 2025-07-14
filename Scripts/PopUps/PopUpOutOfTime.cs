using UnityEngine;
using UnityEngine.UI;

public class PopUpOutOfTime : PopUpBase
{
    [SerializeField] Button _home;
    [SerializeField] Button _replay;
    public override void Init()
    {
        base.Init();
        _home.onClick.AddListener(OnHome);
        _replay.onClick.AddListener(OnReplay);
        if (!SoundManager.Ins.IsSoundLose)
            SoundManager.Ins.PlaySound(TypeSound.LOSE);
    }
    private void OnHome()
    {
        GameManager.Ins.SceneLoader.LoadScene(TypeScene.Lobby);
        SoundManager.Ins.IsSoundLose = false;

    }
    private void OnReplay()
    {
        GameManager.Ins.SceneLoader.LoadScene(TypeScene.Minigame);
        SoundManager.Ins.IsSoundLose = false;

    }
}
