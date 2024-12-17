using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Префабы врагов (Weak, Normal, Strong)
    public float spawnInterval = 5f; // Интервал спавна врагов

    private float timeSinceLastSpawn;
    private List<Vector3> spawnPoints = new List<Vector3>(); // Список точек для спавна

    private int enemiesPerSpawn = 3; // Количество врагов за раз
    private int totalEnemiesInWave = 9; // Всего врагов в одной волне
    public float waveDelay = 1f; // Задержка перед появлением меню после волны
    private float healthMultiplier = 1f; // Множитель здоровья врагов
    private float damageMultiplier = 1f; // Множитель урона врагов
    private int lastWaveNum = 0;

    private void Start()
    {
        GenerateSpawnPoints();
        StartCoroutine(WaveController());
    }

    private void GenerateSpawnPoints()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("Объект должен иметь Collider!");
            return;
        }

        Bounds bounds = collider.bounds; // Границы объекта
        float step = 0.5f; // Шаг между точками, уменьшаем для большей детализации

        for (float x = bounds.min.x; x <= bounds.max.x; x += step)
        {
            for (float z = bounds.min.z; z <= bounds.max.z; z += step)
            {
                Vector3 rayOrigin = new Vector3(x, bounds.max.y + 1f, z); // Старт луча над объектом

                // Проверяем поверхность объекта через Raycast
                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider.gameObject == this.gameObject) // Убедимся, что точка принадлежит текущему объекту
                    {
                        // Проверяем, принадлежит ли точка NavMesh
                        NavMeshHit navMeshHit;
                        if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1f, NavMesh.AllAreas))
                        {
                            spawnPoints.Add(navMeshHit.position);
                        }
                    }
                }
            }
        }

        Debug.Log($"Сгенерировано точек спавна: {spawnPoints.Count}");
    }

    private IEnumerator WaveController()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWave());
            yield return new WaitForSeconds(waveDelay);
            yield return StartCoroutine(WaitForPlayerDecision());
            healthMultiplier *= 1.05f;
            damageMultiplier *= 1.05f;
        }
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < totalEnemiesInWave; i += enemiesPerSpawn)
        {
            SpawnEnemies(enemiesPerSpawn);
            yield return new WaitForSeconds(spawnInterval);
        }

        while (EnemyManager.Instance.EnemiesAlive > 0)
        {
            yield return null; // Ждём уничтожения всех врагов
        }
    }

    private void SpawnEnemies(int count)
    {
        List<Vector3> availableSpawnPoints = new List<Vector3>(spawnPoints);

        for (int i = 0; i < count; i++)
        {
            if (availableSpawnPoints.Count == 0)
            {
                Debug.LogWarning("Недостаточно точек для спавна врагов!");
                break;
            }

            // Выбираем случайную точку спавна из доступных
            Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Count)];

            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            Vector3 spawnPosition = availableSpawnPoints[randomIndex];

            availableSpawnPoints.RemoveAt(randomIndex);

            int enemyIndex = GetEnemyTypeBasedOnProbability();
            GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPosition, Quaternion.identity);

            // Поднимаем врага на половину его высоты
            float heightOffset = GetEnemyHeight(enemy) / 2f;
            enemy.transform.position += Vector3.up * heightOffset;


            if (enemy.TryGetComponent<EnemyBehaviour>(out var behaviour))
            {
                behaviour.SetStats(healthMultiplier, damageMultiplier);
            }

            EnemyManager.Instance.RegisterEnemy(enemy);
        }
    }

    private float GetEnemyHeight(GameObject enemy)
    {
        // Пытаемся получить высоту через Collider
        Collider col = enemy.GetComponent<Collider>();
        if (col != null)
        {
            return col.bounds.size.y;
        }

        // Если Collider отсутствует, пробуем через Renderer
        Renderer rend = enemy.GetComponent<Renderer>();
        if (rend != null)
        {
            return rend.bounds.size.y;
        }

        // Если ничего не найдено, возвращаем значение по умолчанию
        Debug.LogWarning("Не удалось определить высоту врага. Используется значение по умолчанию: 2.");
        return 2f; // Значение по умолчанию
    }

    private int GetEnemyTypeBasedOnProbability()
    {
        float randomValue = Random.value;
        if (randomValue < 0.5f) return 0; // Weak
        if (randomValue < 0.85f) return 1; // Normal
        return 2; // Strong
    }

    private IEnumerator WaitForPlayerDecision()
    {
        bool playerReady = false;
        UIManager.Instance.ShowNextWaveMenu(() => playerReady = true);

        while (!playerReady)
        {
            yield return null;
        }

        UIManager.Instance.HideNextWaveMenu();
    }
}
