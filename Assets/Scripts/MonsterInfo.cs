using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class acts as both display and info - similar to CardEffect + CardDisplay
public class MonsterInfo : MonoBehaviour
{
    public Monster monster;
    public HealthBar healthBar;

    public Image monsterSprite;
    public Text monsterNameText;
    private int unitMaxHP;

    void Start()
    {
        this.unitMaxHP = monster.unitMaxHP;
        monsterSprite.sprite = monster.artwork;
        monsterNameText.text = monster.unitName;
        healthBar.initialize(monster.unitMaxHP);
    }

    public int getUnitMaxHP()
    {
        return this.unitMaxHP;
    }
}
