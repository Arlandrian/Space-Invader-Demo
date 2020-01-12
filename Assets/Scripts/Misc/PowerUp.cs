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

public class PowerUp_FastMovement : PowerUp
{
    public float speedMultiplier = 2f;

    protected override void Load()
    {
        // Multiply player's speed with speedMultiplier;
        playerController.moveSpeed *= speedMultiplier;
    }

    protected override void Unload()
    {
        // Multiply player's speed with 1/speedMultiplier;
        playerController.moveSpeed /= speedMultiplier;
        
    }
}

public class PowerUp_DoubleBarrel : PowerUp
{
    protected override void Load()
    {
        playerController.usingDoubleBarrel = true;
    }

    protected override void Unload()
    {
        playerController.usingDoubleBarrel = false;

    }
}

public class PowerUp_FastShooting : PowerUp
{
    float fireSpeedMultiplier = 1.6f;
    protected override void Load()
    {
        playerController.fireWaitTime /= fireSpeedMultiplier;
    }

    protected override void Unload()
    {
        playerController.fireWaitTime *= fireSpeedMultiplier;
    }
}