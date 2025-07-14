using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
public class AlignChildrenOnX : MonoBehaviour
{
    [SerializeField] private float alignRange = 5f; // Total width to align across
    [SerializeField] private bool onlyActive = true;
    void Start()
    {
       StartCoroutine(AlignOnStart());
    }
    private IEnumerator AlignOnStart()
    {
        yield return new WaitForSeconds(0.2f);
         AlignNow();
    }
    [Button]
    public void AlignNow()
    {
        int count = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (!onlyActive || child.gameObject.activeInHierarchy)
            {
                count++;
            }
        }

        if (count == 0) return;

        float spacing = (count > 1) ? alignRange / (count - 1) : 0f;
        float startX = -alignRange / 2f;

        int aligned = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (!onlyActive || child.gameObject.activeInHierarchy)
            {
                Vector3 pos = child.localPosition;
                pos.x = startX + spacing * aligned;
                child.localPosition = pos;
                aligned++;
            }
        }
    }
}
