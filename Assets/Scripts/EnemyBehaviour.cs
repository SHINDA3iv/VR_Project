using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Weak, Normal, Strong }

public class EnemyBehaviour : MonoBehaviour
{
    public Action OnDeath;

    public EnemyType enemyType;

    private float health;
    private float damage;
    private float speed;

    private Transform treeTarget;
    private NavMeshAgent agent;

    private bool isNearTree = false; // Флаг нахождения рядом с ёлкой

    private Coroutine damageCoroutine;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        treeTarget = GameObject.FindWithTag("Tree")?.transform;
        if (treeTarget != null) agent.SetDestination(treeTarget.position);
    }

    void Update()
    {
        // Если враг находится рядом с ёлкой, наносим ей урон
        if (isNearTree)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                DealDamageToTree();
                damageTimer = 0f;
            }
        }
    }

    public void SetStats(float healthMultiplier, float damageMultiplier)
    {
        switch (enemyType)
        {
            case EnemyType.Weak:
                health = 50f * healthMultiplier;
                damage = 5f * damageMultiplier;
                speed = 3.5f;
                break;

            case EnemyType.Normal:
                health = 100f * healthMultiplier;
                damage = 10f * damageMultiplier;
                speed = 2.5f;
                break;

            case EnemyType.Strong:
                health = 150f * healthMultiplier;
                damage = 20f * damageMultiplier;
                speed = 1.5f;
                break;
        }

        agent.speed = speed;
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            isNearTree = true;
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(DamageTreeOverTime());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            isNearTree = false;
            if (damageCoroutine != null) 
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }


    private IEnumerator DamageTreeOverTime()
    {
        while (isNearTree)
        {
            TreeBehaviour tree = treeTarget.GetComponent<TreeBehaviour>();
            if (tree != null)
            {
                tree.TakeDamage((int)damage);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
