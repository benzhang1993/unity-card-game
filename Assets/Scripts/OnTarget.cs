using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnTarget : MonoBehaviour, IPointerDownHandler
{
    private BattleHandler battleHandler;
    public Image targetArrow;

    public void OnPointerDown(PointerEventData eventData) {
        setTargetArrow();
        battleHandler.selectTarget(gameObject);
    }

    public void setBattleHandler(BattleHandler battleHandler)
    {
        this.battleHandler = battleHandler;
    }
    public void setTargetArrow()
    {
        targetArrow.gameObject.SetActive(true);
    }
    public void hideTargetArrow()
    {
        targetArrow.gameObject.SetActive(false);
    }
}