using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image _iconButton;
    [SerializeField] Button _commandButton;
    [SerializeField] float _scaler = 0.9f;

    [SerializeField] bool _blockAnim;
    public UnityEvent onDown;
    public UnityEvent onUp;

    private void Start()
    {
        _iconButton.transform.localScale = Vector3.one;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        onDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onUp?.Invoke();
    }
    public void OnClick()
    {
        if (_commandButton.interactable == true)
            IconButtonScaler(Vector3.one * _scaler);
    }
    public void OnUp()
    {
        if (_commandButton.interactable == true)
            IconButtonScaler(Vector3.one);
    }
    private void IconButtonScaler(Vector3 value)
    {
        if (_blockAnim) return;
        _iconButton.transform.DOScale(value, 0.15f).SetEase(Ease.OutSine);
    }
}
