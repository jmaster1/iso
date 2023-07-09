using UnityEngine;

[ExecuteInEditMode]
public class AutoZOrder : MonoBehaviour
{
    private int zOrder;
    private void Update()
    {
        zOrder = 0;
        SetZOrder(transform);
    }

    private void SetZOrder(Transform obj)
    {
        if (obj.TryGetComponent(out Renderer sr))
        {
            sr.sortingOrder = zOrder--;
        }

        for (var i = 0; i < obj.childCount; i++)
        {
            SetZOrder(obj.GetChild(i));
        }
    }
}
