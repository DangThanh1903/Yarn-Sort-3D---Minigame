using Sirenix.OdinInspector;
using UnityEngine;
using ThanhScrollController.Grid;

public class MiniGameController : MonoBehaviour
{

    public static MiniGameController Ins;
    public int currentFinishLv;
    public ThanhData currentLevel;
    public bool isFirstTimeMinigame;

    // For firebase
    public int NumberOfMove = 0;
    public int IsLoseByHp = 0;
    public int IsLoseByTime = 0;

    public void ResetFirebaseVar()
    {
        NumberOfMove = 0;
        IsLoseByHp = 0;
        IsLoseByTime = 0;
    }
    void Awake()
    {
        if (Ins == null)
        {
            Ins = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        currentFinishLv = PlayerPrefs.GetInt("currentFinishLv", 0);
    }
    public void SetTutorial()
    {
        isFirstTimeMinigame = (PlayerPrefs.GetInt("IsFirstTimeMinigame", 0) == 0) ? true : false;
    }
    public void SetLevel(ThanhData thanhData)
    {
        currentLevel = thanhData;
    }
    public void TurnOffTutorial()
    {
        PlayerPrefs.SetInt("IsFirstTimeMinigame", 1);
    }
    public void UnlockNextLv()
    {
        int index = currentLevel.id;
        currentFinishLv = (index >= currentFinishLv) ? index + 1 : currentFinishLv;
        PlayerPrefs.SetInt("currentFinishLv", currentFinishLv);
    }
}
