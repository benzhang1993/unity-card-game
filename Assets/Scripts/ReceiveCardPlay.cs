using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReceiveCardPlay : MonoBehaviour, IDropHandler
{
    public HealthBar healthBar;
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("received card");
        Debug.Log(eventData.currentInputModule.name);
        GameObject droppedCard = eventData.pointerDrag;
        Destroy(droppedCard);
        healthBar.takeDamage(20);
    }
}
