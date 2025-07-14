using System.Collections.Generic;
using UnityEngine;
using GogoGaga.OptimizedRopesAndCables;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;
namespace MiniGameThanh
{
    public class LoomHeadManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> buttons;
        [SerializeField] private List<GameObject> threads;
        [SerializeField] private List<Rope> ropes;
        [SerializeField] private GameObject _plank;
        [SerializeField] private Transform _plankAnimPos;
        [SerializeField] private RectTransform _tutorialHand;

        void Start()
        {
            SetUpButton();
        }
        public void SetHand(Color currentColor)
        {
            foreach (var button in buttons)
            {
                if (button.GetComponent<KnitButton>().buttonColor.Value == currentColor)
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _tutorialHand.parent as RectTransform,             // Parent canvas/rect
                        RectTransformUtility.WorldToScreenPoint(null,  button.transform.position), // null uses default camera
                        null,                                              // null = screen space overlay; use camera if needed
                        out Vector2 localPoint
                    );
                    _tutorialHand.localPosition = new Vector3(localPoint.x + 50f, localPoint.y - 60f, 0f);
                    _tutorialHand.gameObject.SetActive(true);
                    return;
                }
            }
        }
        public void UnSetHand()
        {
            _tutorialHand.gameObject.SetActive(false);
        }
        public void SetButtonColor(int numberOfColor, List<Color> colors, bool isFirstTime = false)
        {
            if (isFirstTime)
            {
                for (int i = 0; i < numberOfColor; i++)
                {
                    // Change button and rope color
                    buttons[i].GetComponent<KnitButton>().buttonColor.Value = colors[i];
                }
            }
            else
            {
                List<Color> buttonColors = buttons.Select(b => b.GetComponent<KnitButton>().buttonColor.Value).ToList();
                if (colors.All(color => buttonColors.Contains(color)))
                {
                    return;
                }
                List<Color> result = MergeListsWithLimit(buttonColors, colors, numberOfColor);
                Debug.Log("Colors:");
                foreach (var color in colors)
                {
                    Debug.Log(color);
                }
                Debug.Log("Result:");
                foreach (var color in result)
                {
                    Debug.Log(color);
                }
                for (int i = 0; i < result.Count; i++)
                {
                    // Change button and rope color
                    buttons[i].GetComponent<KnitButton>().buttonColor.Value = result[i];
                }
            }

        }
        public List<Color> MergeListsWithLimit(List<Color> currentList, List<Color> newList, int requiredCount)
        {
            HashSet<Color> seenColors = new HashSet<Color>(); // To track already added colors
            List<Color> resultList = new List<Color>(requiredCount);
            foreach (var color in newList)
            {
                if (seenColors.Add(color)) // Add returns false if already in set
                {
                    resultList.Add(color);
                    if (resultList.Count == requiredCount) return resultList;
                }
            }
            // Add unique colors from currentList if needed
            foreach (var color in currentList)
            {
                if (seenColors.Add(color)) // Add returns false if already in set
                {
                    resultList.Add(color);
                    if (resultList.Count == requiredCount) return resultList;
                }
            }
            return resultList;
        }
        public void SetUpButton()
        {
            for (int i = 0; i < GameplayController.Ins.numberOfButton; i++)
            {
                buttons[i].SetActive(true);
            }
        }
        public void UnSetButton()
        {
            for (int i = 0; i < GameplayController.Ins.numberOfButton; i++)
            {
                buttons[i].SetActive(false);
            }
        }
        public void SetRopeColors(List<Color> colors)
        {
            DOVirtual.DelayedCall(0.3f, () => {
                for (int i = 0; i < ropes.Count; i++)
                {
                    Renderer renderer = ropes[i].GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Material newMaterial = new Material(renderer.material);
                        newMaterial.color = colors[i];
                        renderer.material = newMaterial;
                    }
                }
            });
        }
        public void AddNextThread(int currencyThread)
        {
            if (currencyThread == -1) 
                return;
            // from right to left
            int inWorldThread = 15 - currencyThread - 1;
            // change thread color
            Transform child = threads[inWorldThread].transform.GetChild(0);
            Renderer renderer = child.GetComponent<Renderer>(); 
            if (renderer != null)
            {
                renderer.material.color = GameplayController.Ins.GetCurrentColor();
            }
            // Spawn thread
            threads[inWorldThread].transform.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack);
        }
        public void PlayRopeAnim(int currencyThread)
        {
            if (currencyThread == -1) 
                return;
            // from right to left
            int inWorldThread = 15 - currencyThread - 1;
            // Move down start of the rope
            ropes[inWorldThread].transform
            .DOMove(ropes[inWorldThread].transform.position + new Vector3(0.3f, -0.2f, 0), 0.1f)
            .SetLoops(2, LoopType.Yoyo);
            // Move up end of the rope
            ropes[inWorldThread].EndPoint.transform
            .DOMove(ropes[inWorldThread].EndPoint.transform.position + new Vector3(-0.2f, 0, 0), 0.1f)
            .SetLoops(2, LoopType.Yoyo);
        }
        public void PlayFullThreadLineAnim()
        {
            Vector3 originPos = _plank.transform.position;
            // Move the plank 
            DOVirtual.DelayedCall(0.1f, () => {
                _plank.transform.DOMove(_plankAnimPos.position, 0.2f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    foreach (var thread in threads)
                    {
                        thread.transform.localScale = Vector3.zero;
                    }
                    _plank.transform.DOMove(originPos, 0.2f)
                    .SetEase(Ease.InOutQuad);
                });
            });
        }
        public void SetUpTargetToMoveButton(int currencyThread)
        {
            if (currencyThread == -1) 
                return;
            // from right to left
            int inWorldThread = 15 - currencyThread - 1;
            for (int i = 0; i < GameplayController.Ins.numberOfButton; i++)
            {
                buttons[i].GetComponent<KnitButton>().UpdateTargetPosition(ropes[inWorldThread].EndPoint.transform.position);
            }
        }
    }
}