using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, VICTORY, DEFEATED }

public class BattleHandler : MonoBehaviour
{
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
    public BattleAnimation enemyBattleAnimation;
    Action onPlayCardComplete;
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
        playerBattleAnimation.setActor(playerGO);
        enemyBattleAnimation = enemyGO.GetComponent<BattleAnimation>();
        enemyBattleAnimation.setActor(enemyGO);

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
        deck.shuffle();
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
            playerCard.GetComponent<CardEffect>().card = hand[i];
            playerCard.transform.SetParent(handArea.transform, false);
        }
    }

    public void playCard(GameObject cardPlayed, Action onPlayCardComplete)
    {
        this.onPlayCardComplete = onPlayCardComplete;
        CardEffect cardEffect = cardPlayed.GetComponent<CardEffect>();
        if(cardEffect.getDamage() != 0)
        {
            processDashAttackEffect(playerGO, enemyGO, playerBattleAnimation, enemyBattleAnimation, cardPlayed.GetComponent<CardEffect>().getDamage(), postPlayerTurn);
        }
        else if(cardEffect.getHealing() != 0)
        {
            processHealingEffect(playerGO, playerGO, playerBattleAnimation, enemyBattleAnimation, cardPlayed.GetComponent<CardEffect>().getHealing(), postPlayerTurn);
        }
        else if(cardEffect.getShielding() != 0)
        {
            postPlayerTurn();
        }
        else{
            postPlayerTurn();
        }
    }

    IEnumerator enemyTurn()
    {
        yield return new WaitForSeconds(.5f);
        battleState.GetComponent<Text>().text = "Enemy Turn";
        int enemyAttackDamage = 50;
        processDashAttackEffect(enemyGO, playerGO, enemyBattleAnimation, playerBattleAnimation, enemyAttackDamage, postEnemyTurn);
    }

    private void processHealingEffect(GameObject healerGO, GameObject targetGO, BattleAnimation healerAnimator, 
        BattleAnimation targetAnimator, int healing, Action postAnimation)
    {
        healerAnimator.performHealingAnimation(targetGO, healing,
        // on attack hit callback
        ()=>
        {
            HealthBar targetHealthBar = targetGO.GetComponentInChildren<HealthBar>();
            targetHealthBar.heal(healing);
            postAnimation();
        });
    }

    private void processDashAttackEffect(GameObject attackerGO, GameObject targetGO, BattleAnimation attackerAnimator, 
        BattleAnimation targetAnimator, int damage, Action postAnimation)
    {
        attackerAnimator.performDashAttackAnimation(targetGO, damage,
        // on attack hit callback
        ()=>
        {
            HealthBar targetHealthBar = targetGO.GetComponentInChildren<HealthBar>();
            targetHealthBar.takeDamage(damage);
        }
        // on attack complete callback
        , 
        ()=> {
            postAnimation();
        });
    }

    private void postEnemyTurn()
    {
        if(playerGO.GetComponentInChildren<HealthBar>().getCurrentHealth() <= 0)
        {
            state = BattleState.DEFEATED;
            endBattle();
        }
        else
        {
            playerTurn();
        }
    }

    private void postPlayerTurn()
    {
        Debug.Log(movesLeft);
        if(enemyGO.GetComponentInChildren<HealthBar>().getCurrentHealth() <= 0)
        {
            state = BattleState.VICTORY;
            endBattle();
        } 
        else if(--movesLeft == 0)
        { 
            StartCoroutine(enemyTurn());
        }
        onPlayCardComplete();
    }

    private void endBattle()
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
