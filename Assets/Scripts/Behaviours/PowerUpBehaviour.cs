using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehaviour : MonoBehaviour
{
    [HideInInspector]
    public PowerUpData powerUpData;

    public float fallSpeed = 6f;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.down * fallSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            collision.GetComponent<PlayerController>().AddPowerUp(powerUpData);
            AudioManager.Instance.powerUp.Play();
            Destroy(gameObject,Time.fixedDeltaTime*2f);

        }

        if (collision.CompareTag("Wall")) {
            Destroy(gameObject, 3f);
        }
    }

    internal void Init(PowerUpData powerUpData)
    {
        this.powerUpData = powerUpData;
        GetComponent<SpriteRenderer>().sprite = powerUpData.sprite;
    }
}

