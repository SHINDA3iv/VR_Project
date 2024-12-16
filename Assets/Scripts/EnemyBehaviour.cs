using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType { Weak, Normal, Strong }

public class EnemyBehaviour : MonoBehaviour
{
    public EnemyType enemyType;

    private float health;
    private float damage;
    private float speed;

    private Transform target; // Ссылка на ёлку
    private NavMeshAgent agent;

    private bool isNearTree = false; // Флаг нахождения рядом с ёлкой
    private float damageInterval = 1f; // Интервал нанесения урона
    private float damageTimer = 0f; // Таймер для урона

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject tree = GameObject.FindWithTag("Tree");
        if (tree != null)
        {
            target = tree.transform;
            Vector3 globalTargetPosition = target.position; // Позиция уже в глобальных координатах
            SetEnemyStats();

            if (agent != null)
            {
                agent.SetDestination(globalTargetPosition); // Направляем врага к цели
            }
        }
    }

    private void Update()
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

    private void SetEnemyStats()
    {
        // Задаём характеристики в зависимости от типа врага
        switch (enemyType)
        {
            case EnemyType.Weak:
                health = 50f;
                damage = 5f;
                speed = 3.5f;
                break;

            case EnemyType.Normal:
                health = 100f;
                damage = 10f;
                speed = 2.5f;
                break;

            case EnemyType.Strong:
                health = 150f;
                damage = 20f;
                speed = 1.5f;
                break;
        }

        agent.speed = speed; // Устанавливаем скорость движения
    }

    private void DealDamageToTree()
    {
        TreeBehaviour tree = target.GetComponent<TreeBehaviour>();
        if (tree != null)
        {
            tree.TakeDamage(10); // Наносим ёлке 10 урона
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            isNearTree = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tree"))
        {
            isNearTree = false;
        }
    }
}
