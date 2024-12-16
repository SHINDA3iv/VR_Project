using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager gameManager;

    GameOverScript gameOverScript;

    void Awake()
    {
        gameManager = this;
    }

    void Start()
    {
        gameOverScript = FindObjectOfType<GameOverScript>();
    }



    private void EndGame()
    {
    }

}
