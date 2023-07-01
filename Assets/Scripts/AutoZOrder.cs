using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class AutoZOrder : MonoBehaviour
{
    private void Update()
    {
        Debug.Log("Update");
        var childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var currentChild = transform.GetChild(i);
            var zOrder = transform.childCount - i;
            currentChild.GetComponent<SpriteRenderer>().sortingOrder = zOrder;
        }
        /*var childs = new List<Transform>();
        for (int i = 0; i < childCount; i++)
        {
            childs.Add(transform.GetChild(i));
        }
        
        childs = childs.OrderBy(e => e.position.y).ToList();
        
        for (int i = 0; i < childCount; i++)
        {
            var zOrder = childCount - i;
            childs[i].GetComponent<SpriteRenderer>().sortingOrder = zOrder;
        }*/
    }
}
