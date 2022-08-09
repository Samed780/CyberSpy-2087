using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent agent;
    public LayerMask groundLayer, playerLayer;
    public Transform player;

    //guarding
    public Vector3 destination;
    bool destinationSet;
    public float range;

    //chasing
    public float chaseRange;
    bool playerInCahseRange;

    //attacking
    public float attackRange, attackRate;
    private bool playerInAttackRange, readyToAttack = true;
    public GameObject attackProjectile;
    public Transform firePos;

    //melee attacking
    public bool meleeAttacker;
    public int meleeDamage = 2;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>().transform;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInCahseRange = Physics.CheckSphere(transform.position, chaseRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInCahseRange)
            Guard();
        if (playerInCahseRange && playerInAttackRange)
            Chase();
        if (playerInAttackRange)
            Attack();
    }

    void Guard()
    {
        if (!destinationSet)
            SearchForDestination();
        else
            agent.SetDestination(destination);

        Vector3 distanceToDestination = transform.position - destination;

        if (distanceToDestination.magnitude < 1f)
            destinationSet = false;
    }

    private void SearchForDestination()
    {
        //set random destination for agent to go to
        float randomZPos = UnityEngine.Random.Range(-range, range);
        float randomXPos = UnityEngine.Random.Range(-range, range);

        //set destination
        destination = new Vector3(transform.position.x + randomXPos, transform.position.y, transform.position.z + randomZPos);

        if (Physics.Raycast(destination, -transform.up, 2f, groundLayer))
            destinationSet = true;

    }

    void Chase()
    {
        agent.SetDestination(player.position);
    }

    void Attack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (readyToAttack && !meleeAttacker)
        {
            firePos.LookAt(player);
            animator.SetTrigger("Attack");
            Instantiate(attackProjectile, firePos.position, firePos.rotation);
            readyToAttack = false;
            StartCoroutine(ResetAttack());
        }
        else if(readyToAttack && meleeAttacker)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void MeleeDamage()
    {
        if (playerInAttackRange)
        {
            player.GetComponent<PlayerHealthSystem>().TakeDamage(meleeDamage);
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackRate);
        readyToAttack = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

    }
}
