using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleHandler : MonoBehaviour
{
    public GameObject card1;
    public GameObject handArea;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Transform playerBattleStation;
    public Transform enemyBattleStation;
    private UnitInfo playerUnit;
    private UnitInfo enemyUnit;
    public BattleState state;

    void Start()
    {
        state = BattleState.START;
        setUpBattle();
        
    }

    void setUpBattle()
    {
        initializeUnit(playerPrefab, playerBattleStation, playerUnit);
        initializeUnit(enemyPrefab, enemyBattleStation, enemyUnit);

        for(int i = 0; i < 5; i++)
        {
            GameObject playerCard = Instantiate(card1, new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(handArea.transform, false);
        }
    }

    void initializeUnit(GameObject unitPrefab, Transform unitBattleStation, UnitInfo unitInfo)
    {
        GameObject unitGO = Instantiate(unitPrefab, unitBattleStation);
        unitInfo = unitPrefab.GetComponent<UnitInfo>();
        unitGO.transform.Find("UnitName").gameObject.GetComponent<Text>().text = unitInfo.unitName;
        unitGO.GetComponentInChildren<HealthBar>().initialize(unitInfo.unitMaxHP);
    }
}
