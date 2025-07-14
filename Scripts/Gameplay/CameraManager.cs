using DG.Tweening;
using UnityEngine;
namespace MiniGameThanh
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private float shakeStrength = 0.6f;
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float randomness = 90f;
        void Start()
        {
            CameraSetUp();   
        }
        public void Shake()
        {
            transform.DOShakePosition(
                duration: shakeDuration,
                strength: shakeStrength,
                vibrato: vibrato,
                randomness: randomness,
                snapping: false,
                fadeOut: true
            );
        }
        void CameraSetUp()
        {
            float aspect = (float)Screen.width / Screen.height;

            // Example logic: adjust z-position for wider screens
            if (aspect >= 0.7f) // Ultra wide
            {
                gameObject.transform.position = new Vector3(0, 1, -8f);
            }
            else if (aspect >= 0.49f) // 9:18
            {
                gameObject.transform.position = new Vector3(0, 1, -10f);
            }
            else if (aspect >= 0.45f) // 10:22
            {
                gameObject.transform.position = new Vector3(0, 1, -20f);
            }
            else // 10:25
            {
                gameObject.transform.position = new Vector3(0, 1, -35f);
            }
        }
    }
}

