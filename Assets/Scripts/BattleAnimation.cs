using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class BattleAnimation : MonoBehaviour
{
    private GameObject attacker;
    private GameObject target;
    private State state;
    private enum State { Idle, Dashing }
    private Action onDashComplete;
    private Vector3 targetLocation;

    private void Awake()
    {
        this.state = State.Idle;
    }

    // Update is called once per frame
    private void Update()
    {
        switch(state)
        {
            case State.Idle:
                break;
            case State.Dashing:
                Debug.Log(targetLocation.x + " " + attacker.transform.position.x);
                Vector3 baseMovement = (targetLocation.x - attacker.transform.position.x > 0) ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
                float dashSpeed = 25f;
                float reachedDistance = 1.5f;
                attacker.GetComponent<Animator>().SetBool("Walking", true);
                attacker.transform.position += baseMovement * dashSpeed * Time.deltaTime;
                if(Vector3.Distance(attacker.transform.position, targetLocation) < reachedDistance)
                {
                    attacker.transform.position = targetLocation;
                    // needed so update doesn't keep firing off onDashCompletes;
                    state = State.Idle;
                    attacker.GetComponent<Animator>().SetBool("Walking", false);
                    onDashComplete();
                }
                break;
        }
    }

    public void performDashAttack(GameObject target, Action onAttackComplete)
    {
        this.target = target;
        Vector3 enemyLocation = target.transform.position;
        enemyLocation.x -= 3;
        Vector3 attackerOriginalLocation = attacker.transform.position;

        dashToPosition(enemyLocation, () => {
            attacker.GetComponent<Animator>().SetTrigger("Attack");
            StartCoroutine(WaitForAnimation(attacker.GetComponent<Animator>(), ()=>
            {
                dashToPosition(attackerOriginalLocation, () => {
                    attacker.GetComponent<Animator>().SetTrigger("Idle");
                    onAttackComplete();
                });
            }));
        });
    }
 
    private void dashToPosition(Vector3 location, Action onDashComplete)
    {
        this.targetLocation = location;
        state = State.Dashing;
        this.onDashComplete = onDashComplete;
    }

    public void setAttacker(GameObject attacker)
    {
        this.attacker = attacker;
    }

    public void setTarget(GameObject target)
    {
        this.target = target;
    }

    IEnumerator WaitForAnimation (Animator anim, Action action) {
        yield return new WaitForSeconds (anim.GetCurrentAnimatorStateInfo(0).length);
        action();
    }
}
