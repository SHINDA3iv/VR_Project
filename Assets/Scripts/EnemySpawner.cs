using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // ������� ������ (Weak, Normal, Strong)
    public float spawnInterval = 5f; // �������� ������ ������

    private float timeSinceLastSpawn;
    private List<Vector3> spawnPoints = new List<Vector3>(); // ������ ����� ��� ������

    private void Start()
    {
        // ��������� ����� ������ ������ ��� �������� �������
        GenerateSpawnPoints();
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnEnemy();
            timeSinceLastSpawn = 0;

            // ��������� �������� ������ ��� ���������� ���������
            spawnInterval = Mathf.Max(1f, spawnInterval - 0.1f);
        }
    }

    private void GenerateSpawnPoints()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("������ ������ ����� Collider!");
            return;
        }

        Bounds bounds = collider.bounds; // ������� �������
        float step = 0.5f; // ��� ����� �������, ��������� ��� ������� �����������

        for (float x = bounds.min.x; x <= bounds.max.x; x += step)
        {
            for (float z = bounds.min.z; z <= bounds.max.z; z += step)
            {
                Vector3 rayOrigin = new Vector3(x, bounds.max.y + 1f, z); // ����� ���� ��� ��������

                // ��������� ����������� ������� ����� Raycast
                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == this.gameObject) // ��������, ��� ����� ����������� �������� �������
                    {
                        // ���������, ����������� �� ����� NavMesh
                        NavMeshHit navMeshHit;
                        if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1f, NavMesh.AllAreas))
                        {
                            spawnPoints.Add(navMeshHit.position);
                        }
                    }
                }
            }
        }

        Debug.Log($"������������� ����� ������: {spawnPoints.Count}");
    }

    private void SpawnEnemy()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("��� ��������� ����� ��� ������!");
            return;
        }

        // �������� ��������� ����� ������
        Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // ���������� ��� ����� � ������ ����������� ���������
        int enemyIndex = GetEnemyTypeBasedOnProbability();

        // ������ �����
        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPosition, Quaternion.identity);

        // ��������� ����� �� �������� ��� ������
        float heightOffset = GetEnemyHeight(enemy) / 2f;
        enemy.transform.position += Vector3.up * heightOffset;
    }

    private float GetEnemyHeight(GameObject enemy)
    {
        // �������� �������� ������ ����� Collider
        Collider col = enemy.GetComponent<Collider>();
        if (col != null)
        {
            return col.bounds.size.y;
        }

        // ���� Collider �����������, ������� ����� Renderer
        Renderer rend = enemy.GetComponent<Renderer>();
        if (rend != null)
        {
            return rend.bounds.size.y;
        }

        // ���� ������ �� �������, ���������� �������� �� ���������
        Debug.LogWarning("�� ������� ���������� ������ �����. ������������ �������� �� ���������: 2.");
        return 2f; // �������� �� ���������
    }

    private int GetEnemyTypeBasedOnProbability()
    {
        float randomValue = Random.value;
        if (randomValue < 0.5f) // 50% �� ������� �����
        {
            return 0; // Weak
        }
        else if (randomValue < 0.85f) // 35% �� �����������
        {
            return 1; // Normal
        }
        else // 15% �� ��������
        {
            return 2; // Strong
        }
    }
}
