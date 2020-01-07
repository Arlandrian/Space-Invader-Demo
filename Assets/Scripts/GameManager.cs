using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public int score = 0;
    public int hiScore = 0;
    public int remainingLife = 3;

    internal void AddScore(int add)
    {
        score += add;
        if(score > hiScore) {
            SetHiScore(score);
        }

        //Change HUD
    }

    void SetHiScore(int newHiScore)
    {
        hiScore = newHiScore;

        //Change HUD
        
        //Change Player prefs
    }
}
