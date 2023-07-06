using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class AutoZOrder : MonoBehaviour
{
    private int zOrder;
    private void Update()
    {
        zOrder = 0;
        var childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            SetZOrder(child);
        }
    }

    private void SetZOrder(Transform obj)
    {
        if (obj.TryGetComponent(out SpriteRenderer sr))
        {
            sr.sortingOrder = zOrder--;
        }

        for (int i = 0; i < obj.childCount; i++)
        {
            SetZOrder(obj.GetChild(i));
        }
    }
}
