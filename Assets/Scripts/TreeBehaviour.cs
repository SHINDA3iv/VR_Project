using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour
{
    public int maxHealth = 500;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UIManager.Instance.UpdateTreeHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UIManager.Instance.UpdateTreeHealth(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("Ёлка уничтожена! Игра окончена.");
            UIManager.Instance.ShowGameOverMenu();
        }
    }
}
