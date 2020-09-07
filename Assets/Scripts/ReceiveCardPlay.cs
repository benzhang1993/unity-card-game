using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReceiveCardPlay : MonoBehaviour, IDropHandler
{
    public BattleHandler battleHandler;
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("received card");
        GameObject droppedCard = eventData.pointerDrag;
        battleHandler.GetComponent<BattleHandler>().playCard(droppedCard);
        Destroy(droppedCard);
    }
}
