using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    #region Public Variables

    [Header("Prefab References")]
    public GameObject bulletPrefab;

    [Header("References")]
    public Transform barrelPlace;
    public Sprite SpaceShipSprite;

    [Header("Settings")]
    public bool useRawInput = false;
    public float moveSpeed = 5f;
    public float fireWaitTime = 1.0f;
    float xEdge => GameManager.Instance.xEdge;

    [Range(0, 1)]
    public int shipType;

    [HideInInspector]
    public bool usingDoubleBarrel = false;

    #endregion

    #region Private Variables

    float lastFireTime = 0f;
    bool loaded = true;

    List<PowerUp> powerUps;

    #endregion

    void Start()
    {
        
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.playerShipPrefabs[shipType].GetComponent<SpriteRenderer>().sprite;
        InitPowerUps();
        LoadBullet();
    }

    void InitPowerUps()
    {
        powerUps = new List<PowerUp>(GameManager.Instance.powerUpDatas.Count);

        PowerUp pUp = new PowerUp_FastMovement();
        pUp.Init(GameManager.Instance.powerUpDatas[0], this,GameManager.Instance.fastMovementTimeT);
        powerUps.Add(pUp);

        pUp = new PowerUp_DoubleBarrel();
        pUp.Init(GameManager.Instance.powerUpDatas[1], this, GameManager.Instance.doubleBarrelTimeT);
        powerUps.Add(pUp);

        pUp = new PowerUp_FastShooting();
        pUp.Init(GameManager.Instance.powerUpDatas[2], this, GameManager.Instance.fastShootTimeT);
        powerUps.Add(pUp);
    }

    void Update()
    {
        if (GameManager.Instance.isGamePaused)
            return;

        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Q))
            SceneManager.LoadScene(0);

        // User input
        float horizontal = useRawInput ? Input.GetAxisRaw("Horizontal"): Input.GetAxis("Horizontal");
        bool fire = Input.GetButton("Fire1");

        // Movement
        Vector3 newPos = transform.position + Vector3.right * horizontal * moveSpeed * Time.deltaTime;
        if (newPos.x > -xEdge && newPos.x < xEdge)
            transform.position = newPos;

        if (!loaded) {
            if(Time.time - lastFireTime >= fireWaitTime) {
                LoadBullet();
            }
        }


        if (fire && loaded) {
            Fire();
        }
    }

    private void FixedUpdate()
    {
        foreach(var pw in powerUps) {
            pw.Update(Time.fixedDeltaTime);
        }
    }

    void Fire()
    {
        ProjectileBehaviour[] bullets = barrelPlace.GetComponentsInChildren<ProjectileBehaviour>();

        foreach(var bullet in bullets) {
            if (bullet.isStarted) {
                lastFireTime = Time.time;
                bullet.Fire();
                AudioManager.Instance.spaceShipFire.Play();
                loaded = false;
            }
        }
        
    }

    void LoadBullet()
    {
        loaded = true;
        if (usingDoubleBarrel) {
            Instantiate(bulletPrefab, barrelPlace.position - Vector3.right * 0.2f, Quaternion.identity, barrelPlace);
            Instantiate(bulletPrefab, barrelPlace.position + Vector3.right * 0.2f, Quaternion.identity, barrelPlace);
        }
        else {
            Instantiate(bulletPrefab, barrelPlace.position, Quaternion.identity, barrelPlace);
        }

    }


    public void AddPowerUp(PowerUpData powerUp)
    {
        
        var pwUp = powerUps.Find(i => i.Id == powerUp.id);
        if(pwUp != null) {
            pwUp.StartOrAddCooldownTime();
        }
        else {
            Debug.LogError("No Power Up with id: "+powerUp.id);
        }
    }

    public void ExplodeShip()
    {
        // Pause the game except the player ship animation
        GetComponent<Animator>().enabled = true;
        AudioManager.Instance.spaceShipExplode.Play();

        // Wait for 2 seconds if there is life
        GameManager.Instance.ShipExloded();
        // load space ship resume the game 
        // if there isnt game over

    }

    public void ReloadShip()
    {
        GetComponent<Animator>().enabled = false;
        transform.position = GameManager.Instance.shipStartingPoint.position;
        GetComponent<SpriteRenderer>().sprite = GameManager.Instance.playerShipPrefabs[shipType].GetComponent<SpriteRenderer>().sprite;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 limitPos = new Vector3(0,-4f,0);
        Gizmos.DrawSphere(limitPos + Vector3.right * xEdge, 0.2f);
        Gizmos.DrawSphere(limitPos - Vector3.right * xEdge, 0.2f);
    }

}
