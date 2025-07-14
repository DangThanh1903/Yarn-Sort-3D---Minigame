using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

namespace ThanhScrollController.Grid
{
    public class ThanhRowCellView : MonoBehaviour
    {
        [Header("Main UI Elements")]
        public GameObject container;
        public Image image;
        public TextMeshProUGUI levelName;
        public Button button;
        public Image medalImage;

        [Header("Button States")]
        [SerializeField] private GameObject playGroup;
        [SerializeField] private Image playImage;
        [SerializeField] private TextMeshProUGUI playText;

        [SerializeField] private GameObject lockGroup;
        [SerializeField] private Image lockImage;
        [SerializeField] private TextMeshProUGUI lockText;

        [Header("Sprites")]
        [SerializeField] private List<Sprite> buttonSprites; // 0: Play, 1: Replay, 2: Lock
        [SerializeField] private List<Sprite> medalSprites;
        public void SetData(ThanhData data)
        {
            container.SetActive(data != null);
            if (data == null) return;

            // image
            image.sprite = data.image;
            levelName.text = data.name;


            // medal
            int medal = PlayerPrefs.GetInt($"medal_{data.id}", 0);
            medalImage.gameObject.SetActive(medal > 0);
            if (medal > 0 && medal - 1 < medalSprites.Count)
                medalImage.sprite = medalSprites[medal - 1];

            // Button
            button.onClick.RemoveAllListeners();
            // Lock case
            if (data.id > MiniGameController.Ins.currentFinishLv)
            {
                playGroup.SetActive(false);
                lockGroup.SetActive(true);
                lockText.text = $"LOCKED";
                return;
            }

            // Play/Replay case
            lockGroup.SetActive(false);
            playGroup.SetActive(true);

            button.onClick.AddListener(() =>
            {
                PlayMiniGame(data);
                GameManager.Ins.SoundManager.PlaySound(TypeSound.BUTTON);
                button.onClick.RemoveAllListeners();
            });

            if (data.id == MiniGameController.Ins.currentFinishLv)
            {
                playImage.sprite = buttonSprites[0];
                playText.text = "PLAY";
            }
            else
            {
                playImage.sprite = buttonSprites[1];
                playText.text = "REPLAY";
            }
        }

        void PlayMiniGame(ThanhData data)
        {
            MiniGameController.Ins.SetLevel(data);
            GameManager.Ins.SceneLoader.LoadScene(TypeScene.Minigame);
        }
    }
}