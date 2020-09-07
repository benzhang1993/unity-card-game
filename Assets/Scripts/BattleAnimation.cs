using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
public class BattleAnimation : MonoBehaviour
{
    private GameObject attacker;
    private GameObject target;
    private State state;
    private enum State { Idle, Dashing, Attack }
    private Action onDashComplete;

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
                Debug.Log("performing dash");
                float dashSpeed = 10f;
                float reachedDistance = 1f;
                attacker.GetComponent<Animator>().SetBool("Dashing", true);
                attacker.transform.position += (target.transform.position - attacker.transform.position) * dashSpeed * Time.deltaTime;
                if(Vector3.Distance(attacker.transform.position, target.transform.position) < reachedDistance)
                {
                    onDashComplete();
                }
                break;
            case State.Attack:
                attacker.GetComponent<Animator>().SetBool("Attack", true);
                break;
        }
    }

    public void performDashAttack(GameObject attacker, GameObject target, Action onAttackComplete)
    {
        this.attacker = attacker;
        this.target = target;
        dashToPosition(() => {
            state = State.Attack;
            onAttackComplete();
        });
    }
 
    private void dashToPosition(Action onDashComplete)
    {
        state = State.Dashing;
        this.onDashComplete = onDashComplete;
        Debug.Log("State: " + state);
        Debug.Log(attacker.GetInstanceID());
    }
}
