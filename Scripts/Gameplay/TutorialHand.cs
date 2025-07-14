using DG.Tweening;
using UnityEngine;

public class TutorialHand : MonoBehaviour
{
    private Tween rotationTween;
    private Tween scaleTween;

    void OnEnable()
    {
        // Reset scale to zero immediately (start small)
        transform.localScale = Vector3.zero;

        // Scale up to normal size (1,1,1) in 0.2s, then start rotation tween
        scaleTween = transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            rotationTween = transform.DORotate(new Vector3(30, 0, 10), 0.2f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        });
    }

    void OnDisable()
    {
        // Kill tweens if active
        scaleTween?.Kill();
        rotationTween?.Kill();

        // Reset rotation and scale
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = Vector3.zero; // or Vector3.one if you want to reset to normal scale
    }

}
