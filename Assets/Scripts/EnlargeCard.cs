using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnlargeCard : EventTrigger
{
    Vector3 originalPosition;
    float enlargeScale = 1.2f;
    public override void OnPointerEnter(PointerEventData data)
    {
        transform.localScale += new Vector3(enlargeScale, enlargeScale, enlargeScale);
        originalPosition = transform.position;
        transform.position -= new Vector3(0, transform.position.y * enlargeScale/2f, 0);
        gameObject.GetComponent<Canvas>().sortingOrder = 2;
    }
    
    public override void OnPointerExit(PointerEventData data)
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = originalPosition;
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
    }

    public void onClick()
    {
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = originalPosition;
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
    }
}
