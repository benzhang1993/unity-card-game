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

    Action onPlayCardComplete;
    private GameObject playerGO;
    private List<GameObject> enemyGOs;
    private BattleState state;
    private int movesLeft = 5;
    private GameObject currentEnemyTarget;
    private Deck deck;
    private Card[] cardSOs;

    void Start()
    {
        state = BattleState.START;
        deck = new Deck();
        StartCoroutine(setUpBattle());
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
            Debug.Log("making an enemy battlestation");
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
        cardSOs = Array.ConvertAll(loadedCards, item => (Card) item);
        deck.loadCardArray(cardSOs);
        deck.shuffle();
    }

    void playerTurn()
    {
        battleAnnoucement.GetComponent<Text>().text = "Your Turn";
        movesLeft = 5;
        List<Card> hand = deck.draw(5); 
        // TODO - refactor drawing method
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
    }

    public void selectTarget(GameObject target)
    {
        if(currentEnemyTarget != null)
        {
            currentEnemyTarget.GetComponent<OnTarget>().hideTargetArrow();
        }
        this.currentEnemyTarget = target;
        target.GetComponent<OnTarget>().setTargetArrow();
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
        Debug.Log(movesLeft);
        if(isAllEnemiesDefeated(enemyGOs))
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
