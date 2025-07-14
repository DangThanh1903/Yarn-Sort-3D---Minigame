using UnityEngine.UI;
using UnityEngine;
using UniRx;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using Dragon.SDK;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif
namespace MiniGameThanh
{
    public class GameplayController : MonoBehaviour
    {
        public static GameplayController Ins;
        // Const
        private const int width = 15;
        private const int height = 15;
        //private
        private List<Color> mapList = new List<Color>(225);
        private ReactiveProperty<int> currencyThread = new ReactiveProperty<int>(-1); // win == 1; lose by time == -1; lose hy HP == -2
        private ReactiveProperty<int> isWinOrLose = new ReactiveProperty<int>(0);
        private int playerHP = 100;
        private float duration;
        private int lastSecond = -1;
        private bool isPlay = false;
        private HashSet<Color> colorOfLevel = new HashSet<Color>();
        private Tween timerTween;
        private Tween delayHint;
        private bool isTutorialFirstLine = true;

        [Header("Dont touch this")]
        // serializeField
        [SerializeField] private LoomBodyManager _loomBodyManager;
        [SerializeField] private LoomHeadManager _loomHeadManager;
        [SerializeField] private Image _mapView;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private GameObject _hpField;
        [SerializeField] private Image _imageProgress;
        [SerializeField] private List<Image> _medalUI;
        [SerializeField] private GameObject _hpChangerPrefab;
        [SerializeField] private CameraManager _cameraManager;
        [Header("Setting")]
        [SerializeField] private int hintDuration = 7;
        // public
        [HideInInspector]
        public int numberOfButton;
        [HideInInspector]
        public int currentLine = 0;

        #region  CheatSheet
#if UNITY_EDITOR
        [Button]
        void Win()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("Win() can only be used while playing!");
                return;
            }
            isWinOrLose.Value = 1;
            foreach (GameObject loomThreads in _loomBodyManager.threadOnLoom)
            {
                loomThreads.SetActive(true);
            }
        }
        [Button]
        void Lose()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("Lose() can only be used while playing!");
                return;
            }
            isWinOrLose.Value = -1;
        }
#endif
        #endregion

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
            GetLevelDataFromData();
            AddListonerThread();
            numberOfButton = CheckMaxUniqueRow();
            MiniGameController.Ins.SetTutorial();
            MiniGameController.Ins.ResetFirebaseVar();
        }
        void Start()
        {
            SimplePool.Preload(_hpChangerPrefab, 3);
            // ApplyColorsToMap();
            ApplyColorsToLoom();
            _loomHeadManager.SetButtonColor(numberOfButton, colorOfLevel.ToList(), true);
            _loomHeadManager.SetButtonColor(numberOfButton, GetUniqueRow(14));
            _loomHeadManager.SetRopeColors(GetRow(14));
            UpdateHpUI();
            UpdateTimerUI(duration);
            GameManager.Ins.GameStateController.TypeStateGame = TypeStateGame.NONE;
            StartCoroutine(TutorialSetUp(MiniGameController.Ins.isFirstTimeMinigame));
        }
        IEnumerator TutorialSetUp(bool isFirstTime)
        {
            if (isFirstTime)
            {
                LoadStartTutorialToFirebase();
                yield return new WaitForSeconds(0.1f);
                _loomHeadManager.SetHand(GetCurrentColor(currencyThread.Value + 1));
            }
        }
        private void GetLevelDataFromData()
        {
            Color[] flatColorsArray = MiniGameController.Ins.currentLevel.texture2D.GetPixels();
            Color[,] colorGrid = new Color[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorGrid[x, y] = flatColorsArray[y * width + x];
                }
            }
            SetColorMap(colorGrid);
            duration = MiniGameController.Ins.currentLevel.duration;
            _mapView.sprite = MiniGameController.Ins.currentLevel.image;
            LoadStartLevelToFirebase();
        }
        // Set from a 2D array
        public void SetColorMap(Color[,] map)
        {
            mapList.Clear();

            for (int y = height - 1; y >= 0; y--)
                for (int x = 0; x < width; x++)
                    if (map[x, y].a == 0)
                    {
                        mapList.Add(Color.black);
                    }
                    else
                    {
                        mapList.Add(map[x, y]);
                    }
        }

        // Convert to 2D array if needed at runtime
        public Color[,] GetColorMap()
        {
            Color[,] map = new Color[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    map[x, y] = mapList[y * width + x];

            return map;
        }
        private List<Color> GetUniqueRow(int rowIndex)
        {
            HashSet<Color> uniqueColors = new HashSet<Color>();
            Color[,] colors = GetColorMap();
            for (int x = 0; x < 15; x++)
            {
                uniqueColors.Add(colors[x, rowIndex]);
                colorOfLevel.Add(colors[x, rowIndex]);
            }

            return uniqueColors.ToList();
        }
        private List<Color> GetRow(int rowIndex)
        {
            List<Color> colors = new List<Color>();
            Color[,] temp = GetColorMap();
            for (int x = 0; x < 15; x++)
            {
                colors.Add(temp[x, rowIndex]);
            }

            return colors;
        }

        public int CheckMaxUniqueRow()
        {
            int max = 0;
            for (int i = 0; i < 15; i++)
            {
                int temp = GetUniqueRow(i).Count;
                max = (temp > max) ? temp : max;
            }
            return max;
        }

        // private void ApplyColorsToMap()
        // {
        //     Color[,] map = GetColorMap();
        //     int width = map.GetLength(0);
        //     int height = map.GetLength(1);
        //     int expectedChildCount = width * height;

        //     if (_mapView.transform.childCount < expectedChildCount)
        //     {
        //         Debug.LogError($"Expected at least {expectedChildCount} children, but got {_mapView.transform.childCount}.");
        //         return;
        //     }

        //     for (int y = 0; y < height; y++)     // Rows
        //     {
        //         for (int x = 0; x < width; x++)  // Columns
        //         {
        //             int index = y * width + x;   // Row-major order
        //             Transform child = _mapView.transform.GetChild(index);
        //             Image img = child.GetComponent<Image>();

        //             if (img != null)
        //             {
        //                 img.color = map[x, y];
        //             }
        //             else
        //             {
        //                 Debug.LogWarning($"Child at index {index} is missing an Image component.");
        //             }
        //         }
        //     }
        //     Debug.Log("Color map applied to children.");
        // }

        void ApplyColorsToLoom()
        {
            // Ensure there are exactly 225 colors in the list
            if (mapList.Count != 225)
            {
                Debug.LogError("mapList must contain exactly 225 colors.");
                return;
            }

            int index = 0; // Index for mapList

            for (int row = 14; row >= 0; row--)
            {
                for (int col = 0; col < 15; col++)
                {
                    // Find the child object
                    Transform childRow = _loomBodyManager.threadOnLoom[row].transform;
                    Transform childRowOfAnimLoom = _loomBodyManager.threadForAnim[15 - (row + 1)].transform;
                    Transform child = childRow.GetChild(col);
                    Transform childOfAnimLoom = childRowOfAnimLoom.GetChild(15 - (col + 1));

                    Renderer renderer = child.GetComponent<Renderer>();
                    Renderer rendererOfAnimLoom = childOfAnimLoom.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = mapList[index];
                        rendererOfAnimLoom.material.color = mapList[index];
                    }
                    index++;
                }
            }
        }

        void RemoveGrayMap(int index)
        {
            for (int i = (index + 1) * width; i < (index + 2) * width; i++)
            {
                _mapView.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        void AddListonerThread()
        {
            currencyThread
            .Subscribe(newValue =>
            {
                // Every thread
                if (newValue >= 0)
                {
                    _loomHeadManager.AddNextThread(newValue);
                    _loomHeadManager.PlayRopeAnim(newValue);
                    _loomHeadManager.SetUpTargetToMoveButton(newValue);
                    if (MiniGameController.Ins.isFirstTimeMinigame && isTutorialFirstLine)
                    {
                        _loomHeadManager.SetHand(GetCurrentColor(currencyThread.Value + 1));
                    }
                    MiniGameController.Ins.NumberOfMove++;
                }

                // Full line
                if (newValue == 14)
                {
                    if (MiniGameController.Ins.isFirstTimeMinigame)
                    {
                        isTutorialFirstLine = false;
                        _loomHeadManager.UnSetHand();
                        // Turn of tutorial by set = 1
                        MiniGameController.Ins.TurnOffTutorial();
                        LoadEndTutorialToFirebase();
                    }
                    _loomHeadManager.PlayFullThreadLineAnim();
                    _loomBodyManager.PlayLoomAnimation();
                    int currentIndexLine = 15 - currentLine - 2;
                    if (currentLine < 14)
                    {
                        List<Color> nextColorList = GetUniqueRow(currentIndexLine);
                        _loomHeadManager.SetButtonColor(numberOfButton, nextColorList);
                        _loomHeadManager.SetRopeColors(GetRow(currentIndexLine));
                    }
                    else
                    {
                        isWinOrLose.Value = 1; // Win
                    }
                    RemoveGrayMap(currentIndexLine);
                    currencyThread.Value = -1; // Reset
                }
            })
            .AddTo(this);

            isWinOrLose
            .Subscribe(newValue =>
            {
                _loomHeadManager.UnSetButton();
                PauseTimer();
                if (newValue == -1)
                {
                    MiniGameController.Ins.IsLoseByTime++;
                    //Lose by time
                    Debug.Log("THUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA!!!!!!!!!!!");
                    // Remove hint
                    delayHint.Kill();
                    _loomHeadManager.UnSetHand();
                    StartCoroutine(IE_delayShowPopUp(-1, 0.8f));
                }
                else if (newValue == -2)
                {
                    MiniGameController.Ins.IsLoseByHp++;
                    //Lose by HP
                    Debug.Log("THUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA!!!!!!!!!!!");
                    delayHint.Kill();
                    _loomHeadManager.UnSetHand();
                    StartCoroutine(IE_delayShowPopUp(-1, 0.8f));
                }
                else if (newValue == 1)
                {
                    // Remove hint
                    LoadWinToFirebase();
                    delayHint.Kill();
                    _loomHeadManager.UnSetHand();
                    // Set medal menu
                    int levelDuration = (int)duration;
                    int temp = MiniGameController.Ins.currentLevel.id;
                    int winMedal = ((lastSecond < levelDuration / 3) ? 1 : ((lastSecond > levelDuration * 2 / 3) ? 3 : 2));
                    if (PlayerPrefs.GetInt($"medal_{temp}", 0) < winMedal)
                    {
                        PlayerPrefs.SetInt($"medal_{temp}", winMedal);
                    }
                    // Increase lv 
                    MiniGameController.Ins.UnlockNextLv();
                    //Win
                    Debug.Log("WINNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNNN!!!!!!!!!!!");
                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        gameObject.transform.GetChild(2).gameObject.SetActive(false);
                        gameObject.transform.GetChild(3).gameObject.SetActive(false);
                        _loomBodyManager.PlayEndAnim();
                    });
                    StartCoroutine(IE_delayShowPopUp(newValue, 5f));
                }
            }).AddTo(this);
        }

        void WaitForHint()
        {
            if (MiniGameController.Ins.isFirstTimeMinigame && isTutorialFirstLine && currencyThread.Value != 13)
            {
                return;
            }
            _loomHeadManager.UnSetHand();
            // Kill previous tween if it's still running
            if (delayHint != null && delayHint.IsActive())
            {
                delayHint.Kill();
            }
            delayHint = DOVirtual.DelayedCall(hintDuration, () =>
            {
                Debug.Log("Hint!!!!!!!!!!!!!");
                _loomHeadManager.SetHand(GetCurrentColor(currencyThread.Value + 1));
            }, false);
        }
        private void NextThread()
        {
            currencyThread.Value += 1;
        }
        public bool KnitButtonPress(Color buttonColor)
        {
            if (!isPlay)
            {
                StartDOTweenTimer(duration);
                isPlay = true;
            }
            // Hint
            WaitForHint();
            if (buttonColor == GetCurrentColor(currencyThread.Value + 1))
            {
                NextThread();
                return true;
            }
            else
            {
                PlayerTakeDamage();
                return false;
            }
        }
        private void PlayerTakeDamage()
        {
            playerHP -= 10;
            _cameraManager.Shake();
            GameObject hpChanger = SimplePool.Spawn(_hpChangerPrefab, Vector3.zero, Quaternion.identity);
            hpChanger.transform.SetParent(_hpField.transform, false);
            if (playerHP <= 0)
            {
                isWinOrLose.Value = -1; // Lose
            }
            UpdateHpUI();
        }
        public void ResetHP()
        {
            playerHP = 100;
            UpdateHpUI();
            isWinOrLose.Value = 0;
        }
        private void UpdateHpUI()
        {
            int temp = (playerHP <= 0) ? 0 : playerHP;
            _hpField.transform.GetChild(0).GetComponent<Image>().fillAmount = (float)playerHP / 100;
            _hpField.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{temp}";
        }
        private void UpdateTimerUI(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            _timerText.text = $"{minutes:D2}:{seconds:D2}";
            _imageProgress.fillAmount = (float)time / duration;
            if (time >= duration / 3 && time < duration * 2 / 3)
            {
                _medalUI[2].DOColor(new Color(0.5f, 0.5f, 0.5f, 1f), 0.5f);
            }
            else if (time < duration / 3)
            {
                _medalUI[1].DOColor(new Color(0.5f, 0.5f, 0.5f, 1f), 0.5f);
            }
        }
        public void PauseTimer()
        {
            if (timerTween != null && timerTween.IsPlaying())
                timerTween.Pause();
        }

        public void ResumeTimer()
        {
            if (timerTween != null && !timerTween.IsPlaying())
                timerTween.Play();
        }
        public void IncreaseTimer(float addDuration)
        {
            duration += addDuration;
            StartDOTweenTimer(lastSecond + addDuration);
        }
        private void StartDOTweenTimer(float Currencyduration)
        {
            // Kill any existing timer
            if (timerTween != null && timerTween.IsActive())
            {
                timerTween.Kill();
            }
            timerTween = DOTween.To(() => Currencyduration, x =>
            {
                Currencyduration = x;
                int seconds = Mathf.FloorToInt(Currencyduration);
                if (seconds != lastSecond)
                {
                    lastSecond = seconds;
                    UpdateTimerUI(seconds);
                }
            }, 0f, Currencyduration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isWinOrLose.Value = -2;
            });
        }
        public Color GetCurrentColor()
        {
            if (currencyThread.Value == -1)
                return Color.white;
            return mapList[224 - currentLine * 15 - currencyThread.Value];
        }
        public Color GetCurrentColor(int index)
        {
            return mapList[224 - currentLine * 15 - index];
        }

        IEnumerator IE_delayShowPopUp(int i, float delayDuration)
        {
            yield return new WaitForSeconds(delayDuration);
            if (i == -1)
            {
                OnLose();
            }
            else if (i == -2)
            {
                OnLoseByHP();
            }
            else if (i == 1)
            {
                OnWin();
            }

        }
        private async void OnLose()
        {
            await PopUpHandler.Ins.ShowPopupAsync("PopUpLoseMinigame");
        }
        private async void OnLoseByHP()
        {
            await PopUpHandler.Ins.ShowPopupAsync("PopUpOutOfHP");
        }
        private async void OnWin()
        {
            await PopUpHandler.Ins.ShowPopupAsync("PopUpWinMinigame");
        }

        public void SetUpButton()
        {
            _loomHeadManager.SetUpButton();
        }
        public int GetRemainTime()
        {
            return lastSecond;
        }

        #region Firebase
        public void LoadLoseToFirebase()
        {
            if (!IsNewLevel())
                return;

            if (MiniGameController.Ins.IsLoseByHp > 0)
            {
                FirebaseManager.LogLevelLose(
                MiniGameController.Ins.currentLevel.id + 1,
                duration - lastSecond,
                GameConst.EV_KEY_REASON_4,
                MiniGameController.Ins.NumberOfMove,
                0,
                MiniGameController.Ins.IsLoseByHp,
                GameConst.EV_MODE_GAMEPLAY_CHALLENGE
                );
            }

            if (MiniGameController.Ins.IsLoseByTime > 0)
            {
                FirebaseManager.LogLevelLose(
                MiniGameController.Ins.currentLevel.id + 1,
                duration - lastSecond,
                GameConst.EV_KEY_REASON_5,
                MiniGameController.Ins.NumberOfMove,
                0,
                MiniGameController.Ins.IsLoseByTime,
                GameConst.EV_MODE_GAMEPLAY_CHALLENGE
                );
            }

        }

        public void LoadWinToFirebase()
        {
            if (!IsNewLevel())
                return;


            FirebaseManager.LogLevelWin(
                MiniGameController.Ins.currentLevel.id + 1,
                duration - lastSecond,
                MiniGameController.Ins.NumberOfMove,
                0,
                MiniGameController.Ins.IsLoseByHp + MiniGameController.Ins.IsLoseByTime,
                GameConst.EV_MODE_GAMEPLAY_CHALLENGE);
        }
        public void LoadStartTutorialToFirebase()
        {
            FirebaseManager.LogStartTutorial(GameConst.EV_MODE_GAMEPLAY_CHALLENGE);
        }
        public void LoadEndTutorialToFirebase()
        {
            FirebaseManager.LogTutorialComplete(GameConst.EV_MODE_GAMEPLAY_CHALLENGE, TimeSpan.FromSeconds(duration - lastSecond));
        }
        public void LoadStartLevelToFirebase()
        {
            if (!IsNewLevel())
                return;

            FirebaseManager.LogLevelStart(MiniGameController.Ins.currentLevel.id + 1, GameConst.EV_MODE_GAMEPLAY_CHALLENGE);
        }
        public bool IsNewLevel()
        {
            int temp = MiniGameController.Ins.currentLevel.id;
            return PlayerPrefs.GetInt($"medal_{temp}", 0) == 0;
        }
        #endregion
    }
}