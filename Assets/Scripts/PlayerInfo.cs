using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public Text playerNameText;
    public HealthBar healthBar;
    public string unitName;
    public int unitMaxHP;

    void Start()
    {
        playerNameText.text = unitName;
        healthBar.initialize(unitMaxHP);
    }
}
