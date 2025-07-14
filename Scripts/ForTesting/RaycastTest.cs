using UnityEngine;
using DG.Tweening;

namespace MiniGameThanh
{
    public class RaycastTest : MonoBehaviour
    {
        void Start()
        {
            transform.DORotate(new Vector3(30, 0, 6), 0.2f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo); // -1 means infinite loops
        }
    }
}
