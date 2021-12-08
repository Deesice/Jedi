using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Critter : MonoBehaviour, IPool
{
    Animator animator;
    NavMeshAgent navMeshAgent;
    public float activationDistance;
    bool active;
    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        animator.SetFloat("speed", navMeshAgent.velocity.magnitude);
        if (!active && (transform.position.z - PlayerController.instance.transform.position.z) <= activationDistance)
        {
            active = true;
            var toPlayer = PlayerController.instance.transform.position - transform.position;
            toPlayer.y = 0;
            toPlayer.z = 0;
            toPlayer = transform.position + toPlayer.normalized * 100;
            toPlayer.x = Mathf.Clamp(toPlayer.x, -Ground.instance.tileSize / 2, Ground.instance.tileSize / 2);
            navMeshAgent.SetDestination(toPlayer);
        }
    }
    public void OnTakeFromPool()
    {
        navMeshAgent.SetDestination(transform.position);
        active = false;
    }
}
