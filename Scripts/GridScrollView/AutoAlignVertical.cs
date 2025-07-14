using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class AutoAlignVertical : MonoBehaviour
{
    public float spacing = 0.5f;
    [Button]
    public void AlignChildrenVertical()
    {
        List<Transform> activeChildren = new List<Transform>();

        // Collect all active child objects
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeInHierarchy)
            {
                activeChildren.Add(child);
            }
        }

        int activeCount = activeChildren.Count;
        if (activeCount == 0) return;

        float totalHeight = 0f;

        // Calculate total height of active objects
        foreach (var child in activeChildren)
        {
            Renderer renderer = child.GetComponentInChildren<Renderer>();
            float height = renderer ? renderer.bounds.size.y : 1f;
            totalHeight += height;
        }

        totalHeight += spacing * (activeCount - 1);
        float startY = totalHeight / 2f;
        float currentY = -startY;

        // Position each active object
        foreach (var child in activeChildren)
        {
            Renderer renderer = child.GetComponentInChildren<Renderer>();
            float height = renderer ? renderer.bounds.size.y : 1f;

            Vector3 pos = child.localPosition;
            pos.y = currentY + height / 2f;
            child.localPosition = pos;

            currentY += height + spacing;
        }
    }

}
