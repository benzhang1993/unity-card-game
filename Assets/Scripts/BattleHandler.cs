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
    public Transform enemyBattleStationsGrid;
    public GameObject enemyBattleStationPrefab;
    private PlayerInfo playerInfo;
    private MonsterInfo enemyInfo;
    private GameObject playerGO;
    private List<GameObject> enemyGOs;
    public BattleState state;
    public BattleAnimator playerBattleAnimator;
    public BattleAnimator enemyBattleAnimator;
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
        // TODO - selective load monster instead of all
        System.Object[] loadedMonsters = Resources.LoadAll("Monsters", typeof(Monster));
        Monster[] monstersList = Array.ConvertAll(loadedMonsters, item => (Monster) item);
        playerGO = initializePlayer(playerPrefab, playerBattleStation);
        enemyGOs = initializeMonsters(enemyPrefab, enemyBattleStationPrefab, monstersList, enemyBattleStationsGrid);
        playerBattleAnimator = playerGO.GetComponent<BattleAnimator>();
        playerBattleAnimator.setActor(playerGO);
        enemyBattleAnimator = enemyGOs[0].GetComponent<BattleAnimator>();
        enemyBattleAnimator.setActor(enemyGOs[0]);

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
            processDashAttackEffect(playerGO, enemyGOs[0], playerBattleAnimator, enemyBattleAnimator, cardPlayed.GetComponent<CardEffect>().getDamage(), postPlayerTurn);
        }
        else if(cardEffect.getHealing() != 0)
        {
            processHealingEffect(playerGO, playerGO, playerBattleAnimator, enemyBattleAnimator, cardPlayed.GetComponent<CardEffect>().getHealing(), postPlayerTurn);
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
        processDashAttackEffect(enemyGOs[0], playerGO, enemyBattleAnimator, playerBattleAnimator, enemyAttackDamage, postEnemyTurn);
    }

    private void processHealingEffect(GameObject healerGO, GameObject targetGO, BattleAnimator healerAnimator, 
        BattleAnimator targetAnimator, int healing, Action postAnimation)
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

    private void processDashAttackEffect(GameObject attackerGO, GameObject targetGO, BattleAnimator attackerAnimator, 
        BattleAnimator targetAnimator, int damage, Action postAnimation)
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
        if(enemyGOs[0].GetComponentInChildren<HealthBar>().getCurrentHealth() <= 0)
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
