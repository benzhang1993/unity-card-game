using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    public Card card;

    private int damage;
    void Start()
    {
        this.damage = card.damage;
    }

    public int getDamage()
    {
        return damage;
    }
}
