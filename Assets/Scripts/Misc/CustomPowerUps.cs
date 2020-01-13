using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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