using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Monster")]
public class Monster : ScriptableObject
{
    public string unitName;
    public int unitMaxHP;
    public Sprite artwork;
}
