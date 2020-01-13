using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PowerUp
{
    public byte Id => powerUpData.id;

    protected PowerUpData powerUpData;
    protected PlayerController playerController;

    private float timeLeft;
    private bool isRunning = false;

    private Transform hudTransform;
    private Text hudText;

    public void Init(PowerUpData powerUpData,PlayerController playerC,Transform hudT)
    {
        this.powerUpData = powerUpData;
        this.playerController = playerC;
        this.hudTransform = hudT;
        hudText = hudT.GetComponentInChildren<Text>();
    }
    
    // Use in Fixed Update
    public void Update(float _deltaTime)
    {
        if (isRunning) {
            timeLeft -= _deltaTime;
            // Update UI text
            hudText.text = PowerUpTimeFormat(timeLeft);
            if (timeLeft <= 0) {
                Unload();
                isRunning = false;
                // Make UI invisible
                hudTransform.gameObject.SetActive(false);
            }
        }
        
    }

    void Start()
    {
        timeLeft = powerUpData.cooldownTime;
        isRunning = true;
        Load();
        //Make UI visible
        hudTransform.gameObject.SetActive(true);
    }

    public void StartOrAddCooldownTime()
    {
        if (isRunning) {
            timeLeft += powerUpData.cooldownTime;
        }
        else {
            Start();
        }

    }

    protected abstract void Load();
    protected abstract void Unload();

    string PowerUpTimeFormat(float time)
    {
        return string.Format("{0:00.0}", time);
    }
}
