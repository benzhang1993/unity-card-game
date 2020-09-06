using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReceiveCardPlay : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("received card");
        Debug.Log(eventData.currentInputModule.name);
        GameObject droppedCard = eventData.pointerDrag;
        Destroy(droppedCard);
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<HealthBar>().takeDamage(20);
        GameObject.FindGameObjectWithTag("Enemy").GetComponentInChildren<HealthBar>().takeDamage(20);
    }
}
