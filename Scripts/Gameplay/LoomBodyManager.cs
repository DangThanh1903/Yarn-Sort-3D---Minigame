using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace MiniGameThanh
{
    public class LoomBodyManager : MonoBehaviour
    {
        private bool isEndAnim = false;

        [Header("Body")]
        [SerializeField] private GameObject loom;
        public List<GameObject> threadOnLoom;
        public float radius = 5f;
        public float moveSpeed = 50f;
        public float targetAngle = 0f; 
        private float[] currentAngles;
        private Vector3 center;

        [Header("End animation")]
        public List<GameObject> threadForAnim;
        [SerializeField] private float followSpeed = 50f;
        [SerializeField] private float minSpacing = 3.6f;
        [SerializeField] private EndAnimSelecter endAnimSelecter;
        void Start()
        {
            center = loom.transform.position;
            EndAnimBodySetUp();
        }
        void Update()
        {
            if (isEndAnim)
            {
                FollowTheHead();
            }
        }
        public void PlayLoomAnimation()
        {
            // Spin the loom's body
            DOVirtual.DelayedCall(0.3f, () =>
            {
                loom.transform.DOLocalRotate(
                    new Vector3(24, 0, 0), // initial step
                    0.3f,
                    RotateMode.LocalAxisAdd // <-- key: additive rotation!
                )
                .SetEase(Ease.OutBack, 5f);
                threadOnLoom[GameplayController.Ins.currentLine].SetActive(true);
                GameplayController.Ins.currentLine++;
            });
        }

        public void PlayEndAnim()
        {
            isEndAnim = true;

            // For flying obj
            // Set path points
            Vector3[] pathPoints = endAnimSelecter.waypoints.Select(w => w.position).ToArray();

            // PlayAnim
            threadForAnim[0].transform.DOPath(pathPoints, 3f, PathType.CatmullRom)
                .SetOptions(false) // false = do not orient to path
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(0.1f, () =>
                    {
                        // Get target z after the delay
                        float targetZ = threadForAnim[0].transform.position.z;
                        float duration = 0.5f; // duration of the z lerp

                        foreach (var item in threadForAnim)
                        {
                            // Tween only the z position smoothly
                            item.transform.DOMoveZ(targetZ, duration).SetEase(Ease.OutSine);
                        }
                    }).OnComplete(() =>
                    {
                        isEndAnim = false;
                    });
                });
        }

        void EndAnimBodySetUp()
        {
            // For body
            currentAngles = new float[threadOnLoom.Count];
            float angleStep = 360f / threadOnLoom.Count;

            // Set initial angles around the circle
            for (int i = 0; i < threadOnLoom.Count; i++)
            {
                currentAngles[i] = i * angleStep;
            }
        }

        void FollowTheHead()
        {
            // For fly obj
            for (int i = 1; i < threadForAnim.Count; i++)
            {
                Transform target = threadForAnim[i - 1].transform;
                Transform follower = threadForAnim[i].transform;

                float distance = Vector3.Distance(target.position, follower.position);

                if (distance >= minSpacing)
                {
                    float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
                    Vector3 direction = (target.position - follower.position).normalized;
                    Vector3 desiredPos = target.position - direction * minSpacing;
                    follower.position = Vector3.Lerp(follower.position, desiredPos, t);
                }
            }


            // For body obj
            for (int i = 0; i < threadOnLoom.Count; i++)
            {
                // Normalize angles to [0,360)
                currentAngles[i] = currentAngles[i] % 360f;
                float normalizedTarget = targetAngle % 360f;

                // Going clockwise
                float distClockwise = (normalizedTarget - currentAngles[i] + 360f) % 360f;
                // Going counter clockwise
                float distCounterClockwise = (currentAngles[i] - normalizedTarget + 360f) % 360f;

                // If distance is very small, snap to target angle
                if (distClockwise < 0.1f)
                {
                    currentAngles[i] = normalizedTarget;
                }
                else
                {
                    // Move clockwise by moveSpeed * deltaTime, but do not overshoot target
                    float step = moveSpeed * Time.deltaTime;
                    currentAngles[i] += Mathf.Min(step, distClockwise);
                }

                float rad = currentAngles[i] * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(0, Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
                threadOnLoom[i].transform.position = center + offset;

                // Face outward from center
                threadOnLoom[i].transform.LookAt(2 * threadOnLoom[i].transform.position - center);
            }
        }

    }

}