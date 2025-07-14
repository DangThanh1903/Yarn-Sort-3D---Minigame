using System.Collections;
using DG.Tweening;
using GogoGaga.OptimizedRopesAndCables;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
namespace MiniGameThanh
{
    public class KnitButton : MonoBehaviour
    {
        public Rope ropeOfButton;
        private Vector3 targetPosition;
        private Animator animator;
        public ReactiveProperty<Color> buttonColor = new ReactiveProperty<Color>(Color.black); 

        void Awake()
        {
            AddOnChangeColorListoner();
            animator = GetComponent<Animator>();
        }

        void OnEnable()
        {
            ropeOfButton.gameObject.SetActive(true);
            AddButtonListoner();
        }
        void OnDisable()
        {
            ropeOfButton.gameObject.SetActive(false);
            RemoveButtonFunc();
        }
        public void RemoveButtonFunc()
        {
            gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        void AddButtonListoner()
        {
            gameObject.GetComponent<Button>().onClick.AddListener(() => {
                GameManager.Ins.SoundManager.PlaySound(TypeSound.CLICK_WOOL);
                if (GameplayController.Ins.KnitButtonPress(buttonColor.Value)) // This check and handle function too
                {
                    MoveObject();
                    // PressedAnim();
                    if (Random.value > 0.5f)
                    {
                        animator.SetTrigger("Play");
                    }
                    else
                    {
                        animator.SetTrigger("Play1");
                    }
                }
            });
        }

        void AddOnChangeColorListoner()
        {
            buttonColor
            .Subscribe(newValue =>
            {
                SetRopeColor(newValue);
                SetButtonColor(newValue);
            })
            .AddTo(this);
        }
        void MoveObject()
        {
            ropeOfButton.transform.position = new Vector3(
                targetPosition.x, 
                ropeOfButton.transform.position.y, 
                ropeOfButton.transform.position.z
                );
        }
        private void PressedAnim()
        {
            Transform target = gameObject.transform;

            // Kill any ongoing rotation
            target.DOKill();

            // Reset rotation if mid-spin
            target.localRotation = Quaternion.identity;

            // Start new spin
            target.DOLocalRotate(new Vector3(0, 0, transform.localEulerAngles.z + 360), 0.8f, RotateMode.FastBeyond360)
                .SetEase(Ease.OutBack);
        }

        public void UpdateTargetPosition(Vector3 newPosition)
        {
            targetPosition = newPosition;
        }

        public void SetRopeColor(Color color)
        {
            Renderer renderer = ropeOfButton.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
            float aspect = (float)Screen.width / Screen.height;
            ropeOfButton.ropeLength = (aspect >= 0.48f) ? 90 : 110;
        }
        public void SetButtonColor(Color color)
        {
            gameObject.GetComponent<Image>().color = color;; 
        }
    }

}