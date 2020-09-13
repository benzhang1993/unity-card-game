using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : MonoBehaviour
{
    public Card card;

    private int damage;
    private int healing;
    private int shielding;
    private int manaCost;

    void Start()
    {
        this.damage = card.damage;
        this.healing = card.healing;
        this.shielding = card.shielding;
        this.manaCost = card.manaCost;
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

    public int getManaCost()
    {
        return manaCost;
    }
}
