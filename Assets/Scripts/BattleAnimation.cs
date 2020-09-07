using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

enum State { Idle, Dashing, Attack}
public class BattleAnimation : MonoBehaviour
{
    public GameObject attacker;
    private GameObject target;
    [SerializeField] private State state;
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

    public void performDashAttack(GameObject target, Action onAttackComplete)
    {
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
    }
}
