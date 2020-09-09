using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, VICTORY, DEFEATED }

public class BattleHandler : MonoBehaviour
{
    public Card card;
    public Deck deck;
    public Card[] cardSOs;
    public GameObject cardPrefab;
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
        state = BattleState.START;
        deck = new Deck();
        StartCoroutine(setUpBattle());
    }

    IEnumerator setUpBattle()
    {
        playerGO = initializeUnit(playerPrefab, playerBattleStation, playerUnit);
        enemyGO = initializeUnit(enemyPrefab, enemyBattleStation, enemyUnit);
        playerBattleAnimation = playerGO.GetComponent<BattleAnimation>();
        playerBattleAnimation.setAttacker(playerGO);

        setUpDeck();

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

    public void setUpDeck()
    {
        // Resource.LoadAll only looks for folders under the "Resources" folder
        System.Object[] loadedCards = Resources.LoadAll("Cards", typeof(Card));
        cardSOs = Array.ConvertAll(loadedCards, item => (Card) item);
        deck.loadCardArray(cardSOs);
    }

    void playerTurn()
    {
        battleState.GetComponent<Text>().text = "Your Turn";
        movesLeft = 5;
        List<Card> hand = deck.draw(5); 
        for(int i = 0; i < hand.Count; i++)
        {
            GameObject playerCard = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            playerCard.GetComponent<CardDisplay>().card = hand[i];
            playerCard.transform.SetParent(handArea.transform, false);
        }
    }

    public void playCard(GameObject cardPlayed)
    {
        playerBattleAnimation.performDashAttack(enemyGO, ()=> {
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
