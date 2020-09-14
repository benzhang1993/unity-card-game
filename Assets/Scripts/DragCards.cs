using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCards : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private float enlargeScale = 1.1f;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("OnPointerDown");

        // transform.localScale = new Vector3(1, 1, 1);
        // rectTransform.anchoredPosition = originalPosition;
        // gameObject.GetComponent<Canvas>().sortingOrder = 0;

        // originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Debug.Log("OnBeginDrag");
    }
    public void OnDrag(PointerEventData eventData) {
        rectTransform.anchoredPosition += eventData.delta;
    }
    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("OnEndDrag");
        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.anchoredPosition = originalPosition;
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        rectTransform.localScale += new Vector3(enlargeScale, enlargeScale, enlargeScale);
        transform.position -= new Vector3(0, transform.position.y * enlargeScale/2f, 0);

        // TODO - figure out why changing sortingOrder makes the card drop not detectable by playArea
        // gameObject.GetComponent<Canvas>().sortingOrder = 1;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.anchoredPosition = originalPosition;
        gameObject.GetComponent<Canvas>().sortingOrder = 0;
    }
}
