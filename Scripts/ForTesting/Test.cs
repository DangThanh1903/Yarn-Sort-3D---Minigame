using DG.Tweening;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private List<GameObject> threadOnLoom;
    public float followSpeed = 20f;
    public float minSpacing = 1f; // Minimum allowed distance between objects
    private bool isEndAnim = false;
    public Transform[] waypoints;
    void Start()
    {
        PlayEndAnim();
    }
    void Update()
    {
        if (isEndAnim)
        {
            FollowTheHead();
        }
    }
    public void PlayEndAnim()
    {
        isEndAnim = true;
        Vector3[] pathPoints = waypoints.Select(w => w.position).ToArray();

        threadOnLoom[0].transform.DOPath(pathPoints, 3f, PathType.CatmullRom)
            .SetOptions(false) // false = do not orient to path
            .SetEase(Ease.OutSine)
            .OnComplete(() => {
                DOVirtual.DelayedCall(0.2f, () => {
                    // Get target z after the delay
                    float targetZ = threadOnLoom[0].transform.position.z;
                    float duration = 0.5f; // duration of the z lerp

                    foreach (var item in threadOnLoom)
                    {
                        // Tween only the z position smoothly
                        item.transform.DOMoveZ(targetZ, duration).SetEase(Ease.OutSine);
                    }
                }).OnComplete(() => {
                    isEndAnim = false;
                });
        });
    }

    void FollowTheHead()
    {
        for (int i = 1; i < threadOnLoom.Count; i++)
        {
            Transform target = threadOnLoom[i - 1].transform;
            Transform follower = threadOnLoom[i].transform;

            Vector3 direction = target.position - follower.position;
            float distance = direction.magnitude;

            if (distance > minSpacing)
            {
                float speedFactor = Mathf.Clamp01((distance - minSpacing) / distance); // slow down when near
                float step = followSpeed * speedFactor * Time.deltaTime;

                follower.position = Vector3.Lerp(follower.position, target.position, step);
            }
        }
    }
}
