using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Not needed. delete later
/// </summary>
public abstract class SpaceShip { 
    public enum SpaceShipType { Speedy, Tanky}

    public SpaceShipType shipType;
    public Sprite sprite;

    public float moveSpeed;
    public float fireWaitTime;

    public abstract void Fire();

}
