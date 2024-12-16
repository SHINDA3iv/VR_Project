using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreDisplay : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text scoreText;

    public void DisplayHighScore(string name, float score)
    {
        nameText.text = name;
        float minutes = Mathf.FloorToInt(score / 60);
        float seconds = Mathf.FloorToInt(score % 60);
        scoreText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void HideEntryDisplay()
    {
        nameText.text = "";
        scoreText.text = "";
    }
}