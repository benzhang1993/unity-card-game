using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, VICTORY, DEFEATED }

public class BattleHandler : MonoBehaviour
{
    public GameObject cardPrefab;
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject handArea;
    public GameObject battleAnnoucement;
    public Transform playerBattleStation;
    public Transform enemyBattleStationsGrid;
    public GameObject enemyBattleStationPrefab;
    public Text playerMana;

    private GameObject playerGO;
    private List<GameObject> enemyGOs;
    private BattleState state;
    private GameObject currentEnemyTarget;
    private Deck deck;
    List<GameObject> hand;
    private int maxMana;
    private int currentMana;

    void Start()
    {
        state = BattleState.START;
        deck = new Deck();
        hand = new List<GameObject>();
        StartCoroutine(setUpBattle());
        currentMana = maxMana = 3;
        playerMana.text = currentMana.ToString();
    }

    IEnumerator setUpBattle()
    {
        // TODO - selective load monster instead of all
        System.Object[] loadedMonsters = Resources.LoadAll("Monsters", typeof(Monster));
        Monster[] monstersList = Array.ConvertAll(loadedMonsters, item => (Monster) item);
        playerGO = initializePlayer(playerPrefab, playerBattleStation);
        enemyGOs = initializeMonsters(enemyPrefab, enemyBattleStationPrefab, monstersList, enemyBattleStationsGrid);
        
        // Defaults to first enemy as target
        selectTarget(enemyGOs[0]);

        setUpDeck();

        yield return new WaitForSeconds(1f);

        state = BattleState.PLAYERTURN;
        playerTurn();
    }

    GameObject initializePlayer(GameObject unitPrefab, Transform unitBattleStation)
    {
        GameObject unitGO = Instantiate(unitPrefab, unitBattleStation);
        return unitGO;
    }

    List<GameObject> initializeMonsters(GameObject unitPrefab, GameObject enemyBattleStationPrefab, Monster[] enemySOs, Transform enemyBattleStationsGrid)
    {
        List<GameObject> enemyGOs = new List<GameObject>();
        for(int i = 0; i < enemySOs.Length; i++)
        {
            GameObject enemyBattleStation = Instantiate(enemyBattleStationPrefab, enemyBattleStationsGrid);
            GameObject unitGO = Instantiate(unitPrefab, enemyBattleStation.transform);
            unitGO.GetComponent<MonsterInfo>().monster = enemySOs[i];
            unitGO.GetComponent<OnTarget>().setBattleHandler(this);
            enemyGOs.Add(unitGO);
        }
        return enemyGOs;
    }

    public void setUpDeck()
    {
        // Resource.LoadAll only looks for folders under the "Resources" folder
        System.Object[] loadedCards = Resources.LoadAll("Cards", typeof(Card));
        Card[] cardSOs = Array.ConvertAll(loadedCards, item => (Card) item);
        List<GameObject> cardGOs = initializeCardGOs(cardSOs);
        deck.setDeck(cardGOs);
        deck.shuffle();
    }

    private List<GameObject> initializeCardGOs(Card[] cardSOs)
    {
        List<GameObject> cardGOs = new List<GameObject>();
        foreach(Card cardSO in cardSOs)
        {
            GameObject playerCard = Instantiate(cardPrefab, handArea.transform);
            playerCard.GetComponent<CardDisplay>().card = cardSO;
            playerCard.GetComponent<CardEffect>().card = cardSO;
            cardGOs.Add(playerCard);
        }
        return cardGOs;
    }

    public void playerTurn()
    {
        battleAnnoucement.GetComponent<Text>().text = "Your Turn";
        setMana(maxMana);
        hand = deck.draw(5);
        foreach(GameObject card in hand)
        {
            card.SetActive(true);
        }
    }

    public void playCard(GameObject cardPlayed)
    {
        setMana(currentMana - cardPlayed.GetComponent<CardEffect>().getManaCost());
        CardEffect cardEffect = cardPlayed.GetComponent<CardEffect>();
        if(cardEffect.getDamage() != 0)
        {
            processDashAttackEffect(playerGO, currentEnemyTarget, cardPlayed.GetComponent<CardEffect>().getDamage(), postPlayerTurn);
        }
        else if(cardEffect.getHealing() != 0)
        {
            processHealingEffect(playerGO, playerGO, cardPlayed.GetComponent<CardEffect>().getHealing(), postPlayerTurn);
        }
        else if(cardEffect.getShielding() != 0)
        {
            postPlayerTurn();
        }
        else{
            postPlayerTurn();
        }
        deck.sendToDiscard(cardPlayed);
    }

    public void selectTarget(GameObject target)
    {
        if(currentEnemyTarget != null)
        {
            currentEnemyTarget.GetComponent<OnTarget>().hideTargetArrow();
        }
        currentEnemyTarget = target;
        target.GetComponent<OnTarget>().setTargetArrow();
    }

    public int getCurrenMana()
    {
        return currentMana;
    }

    IEnumerator enemyTurn()
    {
        yield return new WaitForSeconds(.5f);
        battleAnnoucement.GetComponent<Text>().text = "Enemy Turn";
        int enemyAttackDamage = 50;
        processDashAttackEffect(currentEnemyTarget, playerGO, enemyAttackDamage, postEnemyTurn);
    }

    private void processHealingEffect(GameObject healerGO, GameObject targetGO, int healing, Action postAnimation)
    {
        healerGO.GetComponent<BattleAnimator>().performHealingAnimation(targetGO, healing,
        // On heal callback
        ()=>
        {
            HealthBar targetHealthBar = targetGO.GetComponentInChildren<HealthBar>();
            targetHealthBar.heal(healing);
            postAnimation();
        });
    }

    private void processDashAttackEffect(GameObject attackerGO, GameObject targetGO, int damage, Action postAnimation)
    {
        attackerGO.GetComponent<BattleAnimator>().performDashAttackAnimation(targetGO, damage,
        // On attack hit callback
        ()=>
        {
            processAttackHit(targetGO, damage);
        }
        // On attack complete callback
        , 
        ()=> {
            postAnimation();
        });
    }

    private void processAttackHit(GameObject targetGO, int damage)
    {
        HealthBar targetHealthBar = targetGO.GetComponentInChildren<HealthBar>();
        targetHealthBar.takeDamage(damage);
        if(targetHealthBar.getCurrentHealth() <= 0)
        {
            targetGO.GetComponent<BattleAnimator>().playDeathAnimation();
            removeDeadGOAndSetNextAliveAsTarget(targetGO);
        }
    }

    private void removeDeadGOAndSetNextAliveAsTarget(GameObject targetGO)
    {
        enemyGOs.Remove(targetGO);
        if(enemyGOs.Count > 0) selectTarget(enemyGOs[0]);
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
        if(isAllEnemiesDefeated(enemyGOs))
        {
            state = BattleState.VICTORY;
            endBattle();
        } 
        else if(currentMana <= 0)
        { 
            discardHand();
            StartCoroutine(enemyTurn());
        }
    }

    private void discardHand()
    {
        foreach(GameObject card in hand)
        {
            deck.sendToDiscard(card);
        }
        hand.Clear();
    }

    private void setMana(int mana)
    {
        currentMana = mana;
        playerMana.text = currentMana.ToString();
    }

    private bool isAllEnemiesDefeated(List<GameObject> enemyGOs)
    {
        foreach(GameObject enemy in enemyGOs)
        {
            if(enemy.GetComponentInChildren<HealthBar>().getCurrentHealth() > 0) return false;
        }
        return true;
    }

    private void endBattle()
    {
        if(state == BattleState.VICTORY)
        {
            battleAnnoucement.GetComponent<Text>().text = "Victory";
        }
        else if (state == BattleState.DEFEATED)
        {
            battleAnnoucement.GetComponent<Text>().text = "Defeat";
        }
    }
}
