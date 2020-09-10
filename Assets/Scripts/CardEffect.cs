using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    public Card card;

    private int damage;
    private int healing;
    private int shielding;

    void Start()
    {
        this.damage = card.damage;
        this.healing = card.healing;
        this.shielding = card.shielding;
    }

    public int getDamage()
    {
        return damage;
    }

    public int getHealing()
    {
        return healing;
    }

    public int getShielding()
    {
        return shielding;
    }
}
