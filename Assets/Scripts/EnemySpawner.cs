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

    private int enemiesPerSpawn = 5; // Количество врагов за раз
    private int totalEnemiesInWave = 15; // Всего врагов в одной волне
    private float healthMultiplier = 1f; // Множитель здоровья врагов
    private float damageMultiplier = 1f; // Множитель урона врагов
    private int lastWaveNum = 0;

    private void Start()
    {
        // Генерация точек спавна только для текущего объекта
        GenerateSpawnPoints();
        StartCoroutine(SpawnWave());
    }

    private void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnEnemy();
            timeSinceLastSpawn = 0;

            // Уменьшаем интервал спавна для увеличения сложности
            spawnInterval = Mathf.Max(1f, spawnInterval - 0.1f);
        }
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

    private IEnumerator SpawnWave()
    {
        while (true)
        {
            for (int i = 0; i < totalEnemiesInWave; i += enemiesPerSpawn)
            {
                SpawnEnemies(enemiesPerSpawn);
                yield return new WaitForSeconds(spawnInterval);
            }

            yield return StartCoroutine(WaitForNextWave());

            // Усиливаем врагов с каждой волной
            healthMultiplier *= 1.05f;
            damageMultiplier *= 1.05f;
        }
    }

    private void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (spawnPoints.Count == 0)
            {
                Debug.LogWarning("Нет доступных точек для спавна!");
                return;
            }

            // Выбираем случайную точку спавна
            Vector3 spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Count)];

            // Определяем тип врага с учетом вероятности появления
            int enemyIndex = GetEnemyTypeBasedOnProbability();

            // Создаём врага
            GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPosition, Quaternion.identity);

            // Поднимаем врага на половину его высоты
            float heightOffset = GetEnemyHeight(enemy) / 2f;
            enemy.transform.position += Vector3.up * heightOffset;


            EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
            if (enemyBehaviour != null)
            {
                enemyBehaviour.SetStats(healthMultiplier, damageMultiplier);
            }
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
        if (randomValue < 0.5f) // 50% на слабого врага
        {
            return 0; // Weak
        }
        else if (randomValue < 0.85f) // 35% на нормального
        {
            return 1; // Normal
        }
        else // 15% на сильного
        {
            return 2; // Strong
        }
    }

    private IEnumerator WaitForNextWave()
    {
        bool playerReady = false;

        // Показываем меню и ждём, пока игрок не выберет "Продолжить"
        UIManager.Instance.ShowNextWaveMenu(() => playerReady = true);

        while (!playerReady)
        {
            yield return null;
        }

        UIManager.Instance.HideNextWaveMenu();
    }
}
