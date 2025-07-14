using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HPChanger : MonoBehaviour
{
    [SerializeField] float lifeTime = 2f;
    [SerializeField] float moveDistance = 3f;
    [SerializeField] CanvasGroup canvasGroup;
    private Sequence s;

    void OnEnable()
    {
        PlayAnimation();
        Invoke(nameof(DestroySelf), lifeTime);
    }

    void PlayAnimation()
    {
        // Kill existing tween if still active
        if (s != null && s.IsActive())
            s.Kill();

        s = DOTween.Sequence();
        s.Join(transform.DOMoveY(moveDistance, lifeTime).SetRelative())
         .Join(canvasGroup.DOFade(0f, lifeTime));

        // Set alpha back to full at the start (optional but clean)
        canvasGroup.alpha = 1f;
    }

    void DestroySelf()
    {
        transform.SetParent(null, false);
        SimplePool.Despawn(gameObject);
    }

    void OnDisable()
    {
        CancelInvoke();
        if (s != null && s.IsActive())
            s.Kill();
    }
}
