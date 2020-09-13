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
        GameObject playedCard = eventData.pointerDrag;
        if(cardIsPlayable(playedCard))
        {
            playedCard.SetActive(false);
            battleHandler.GetComponent<BattleHandler>().playCard(playedCard);     
        }
    }

    private bool cardIsPlayable(GameObject card)
    {
        if(card.GetComponent<CardEffect>().getManaCost() <= battleHandler.getCurrenMana())
        {
            return true;
        }
        else {
            return false;
        }
    }
}
