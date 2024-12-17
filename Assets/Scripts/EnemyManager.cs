using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    public int EnemiesAlive => enemies.Count;

    private List<GameObject> enemies = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
        enemy.GetComponent<EnemyBehaviour>().OnDeath += () => enemies.Remove(enemy);
    }
}
