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

    private Transform target; // ������ �� ����
    private NavMeshAgent agent;

    private bool isNearTree = false; // ���� ���������� ����� � �����
    private float damageInterval = 1f; // �������� ��������� �����
    private float damageTimer = 0f; // ������ ��� �����

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject tree = GameObject.FindWithTag("Tree");
        if (tree != null)
        {
            target = tree.transform;
            Vector3 globalTargetPosition = target.position; // ������� ��� � ���������� �����������
            SetEnemyStats();

            if (agent != null)
            {
                agent.SetDestination(globalTargetPosition); // ���������� ����� � ����
            }
        }
    }

    private void Update()
    {
        // ���� ���� ��������� ����� � �����, ������� �� ����
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
        // ����� �������������� � ����������� �� ���� �����
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

        agent.speed = speed; // ������������� �������� ��������
    }

    private void DealDamageToTree()
    {
        TreeBehaviour tree = target.GetComponent<TreeBehaviour>();
        if (tree != null)
        {
            tree.TakeDamage(10); // ������� ���� 10 �����
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
