using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, VICTORY, DEFEATED }

public class BattleHandler : MonoBehaviour
{
    public GameObject card1;
    public GameObject handArea;
    public GameObject battleState;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public Transform playerBattleStation;
    public Transform enemyBattleStation;
    private UnitInfo playerUnit;
    private UnitInfo enemyUnit;
    private GameObject playerGO;
    private GameObject enemyGO;
    public BattleState state;
    public BattleAnimation playerBattleAnimation;
    private int movesLeft = 5;

    void Start()
    {
        playerBattleAnimation = playerPrefab.GetComponent<BattleAnimation>();
        state = BattleState.START;
        StartCoroutine(setUpBattle());   
    }

    IEnumerator setUpBattle()
    {
        playerGO = initializeUnit(playerPrefab, playerBattleStation, playerUnit);
        enemyGO = initializeUnit(enemyPrefab, enemyBattleStation, enemyUnit);

        yield return new WaitForSeconds(1f);

        state = BattleState.PLAYERTURN;
        playerTurn();
    }

    GameObject initializeUnit(GameObject unitPrefab, Transform unitBattleStation, UnitInfo unitInfo)
    {
        GameObject unitGO = Instantiate(unitPrefab, unitBattleStation);
        unitInfo = unitPrefab.GetComponent<UnitInfo>();
        unitGO.transform.Find("UnitName").gameObject.GetComponent<Text>().text = unitInfo.unitName;
        unitGO.GetComponentInChildren<HealthBar>().initialize(unitInfo.unitMaxHP);
        return unitGO;
    }

    void playerTurn()
    {
        battleState.GetComponent<Text>().text = "Your Turn";
        movesLeft = 5;
        for(int i = 0; i < 5; i++)
        {
            GameObject playerCard = Instantiate(card1, new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.transform.SetParent(handArea.transform, false);
        }
    }

    public void playCard(GameObject cardPlayed)
    {
        playerBattleAnimation.performDashAttack(enemyGO, ()=> {
            Debug.Log("callback from dashattack");
            HealthBar enemyHealthBar = GameObject.FindGameObjectWithTag("Enemy").GetComponentInChildren<HealthBar>();
            enemyHealthBar.takeDamage(20);
            if(enemyGO.GetComponentInChildren<HealthBar>().getCurrentHealth() <= 0)
            {
                state = BattleState.VICTORY;
                endBattle();
            } 
            else if(--movesLeft == 0)
            { 
                StartCoroutine(enemyTurn());
            }
        });
    }

    IEnumerator enemyTurn()
    {
        battleState.GetComponent<Text>().text = "Enemy Turn";
        HealthBar playerHealthBar = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<HealthBar>();
        playerHealthBar.takeDamage(100);
        yield return new WaitForSeconds(.5f);
        if(playerHealthBar.getCurrentHealth() <= 0)
        {
            state = BattleState.DEFEATED;
            endBattle();
        }
        else
        {
            playerTurn();
        }
    }

    void endBattle()
    {
        if(state == BattleState.VICTORY)
        {
            battleState.GetComponent<Text>().text = "Victory";
        }
        else if (state == BattleState.DEFEATED)
        {
            battleState.GetComponent<Text>().text = "Defeat";
        }
    }
}
