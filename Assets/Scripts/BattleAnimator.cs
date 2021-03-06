﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class BattleAnimator : MonoBehaviour
{
    private GameObject actor;
    private GameObject target;
    private State state;
    private enum State { Idle, Dashing, Death }
    private enum ActionType { Attack, Healing, Shielding }
    private Action onDashComplete;
    private Vector3 targetLocation;
    
    // for death animation
    private Color alphaColor;
    private float timeToFade = 1.5f; 

    private void Awake()
    {
        this.state = State.Idle;
        actor = gameObject;
        alphaColor = gameObject.GetComponent<Image>().color;
        alphaColor.a = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        switch(state)
        {
            case State.Idle:
                break;
            case State.Dashing:
                Vector3 baseMovement = (targetLocation.x - actor.transform.position.x > 0) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
                float dashSpeed = 25f;
                float reachedDistance = 1.5f;
                actor.GetComponent<Animator>().SetBool("Walking", true);
                actor.transform.position += baseMovement * dashSpeed * Time.deltaTime;
                if(Vector3.Distance(actor.transform.position, targetLocation) < reachedDistance)
                {
                    actor.transform.position = targetLocation;
                    // needed so update doesn't keep firing off onDashCompletes;
                    state = State.Idle;
                    actor.GetComponent<Animator>().SetBool("Walking", false);
                    onDashComplete();
                }
                break;
            case State.Death:
                gameObject.GetComponent<Image>().color = Color.Lerp(gameObject.GetComponent<Image>().color, alphaColor, timeToFade * Time.deltaTime);
                if(gameObject.GetComponent<Image>().color.a < 0.1)
                {
                    gameObject.SetActive(false);
                }
                break;
        }
    }

    public void performDashAttackAnimation(GameObject target, int damage, Action onHitAnimationPlayed, Action onAttackAnimationComplete)
    {
        this.target = target;
        Vector3 enemyLocation = target.transform.position;
        enemyLocation.x -= 3;
        Vector3 attackerOriginalLocation = actor.transform.position;

        dashToPosition(enemyLocation, () => {
            actor.GetComponent<Animator>().SetTrigger("Attack");
            target.GetComponent<Animator>().SetTrigger("Hurt");
            // TODO - animate damage taken
            displayActionResult(ActionType.Attack, damage, target);
            onHitAnimationPlayed();
            StartCoroutine(WaitForAnimation(actor.GetComponent<Animator>(), ()=>
            {
                hideActionResult(target);
                target.GetComponent<Animator>().SetTrigger("Idle");
                dashToPosition(attackerOriginalLocation, () => {
                    actor.GetComponent<Animator>().SetTrigger("Idle");
                    onAttackAnimationComplete();
                });
            }));
        });
    }

    public void performHealingAnimation(GameObject targetGO, int healing, Action onHealAnimationComplete)
    {
        this.target = targetGO;
        actor.GetComponent<Animator>().SetTrigger("Heal");
        displayActionResult(ActionType.Healing, healing, target);
        StartCoroutine(WaitForAnimation(actor.GetComponent<Animator>(), ()=>
        {
            actor.GetComponent<Animator>().SetTrigger("Idle");
            hideActionResult(targetGO);
            onHealAnimationComplete();
        }));
    }

    public void playDeathAnimation()
    {
        state = State.Death;
    }

    private void dashToPosition(Vector3 location, Action onDashComplete)
    {
        this.targetLocation = location;
        state = State.Dashing;
        this.onDashComplete = onDashComplete;
    }

    IEnumerator WaitForAnimation (Animator anim, Action action) {
        yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo(0).length);
        action();
    }

    private void displayActionResult(ActionType actionType, int value, GameObject unitGO)
    {
        Text actionResultText = unitGO.transform.Find("ActionResult").gameObject.GetComponent<Text>();
        switch(actionType)
        {
            case ActionType.Attack:
                actionResultText.color = Color.red;
                actionResultText.text = " - " + value;
                break;
            case ActionType.Healing:
                actionResultText.color = Color.green;
                actionResultText.text = " + " + value;
                break;
            case ActionType.Shielding:
                actionResultText.color = Color.yellow;
                actionResultText.text = " + " + value;
                break;
        }
        actionResultText.gameObject.SetActive(true);
    }

    private void hideActionResult(GameObject unitGO)
    {
        Text actionResultText = unitGO.transform.Find("ActionResult").gameObject.GetComponent<Text>();
        actionResultText.gameObject.SetActive(false);
    }
}
