using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

public class Name : MonoBehaviour
{
    public static Name instance;

    public TMP_Text text;
    public string playerName = "";
    public bool done = false;

    void Awake()
    {
        instance = this;
    }

    public void EnterName(string letter)
    {     
        playerName = playerName + letter;
        text.text = playerName;
    }

    public void Delete()
    {
        text.text = "";
    }

    public void ExitName()
    {
        text.text = "";
        done = true;
    }
}
