using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderBehaviour : MonoBehaviour
{
    [Range(0, 1)]
    public float powerUpSpawnChance = 0.7f;

    public InvaderType invaderType;

    public int xIndex { get; private set; }
    public int yIndex { get; private set; }

    bool isExploded = false;

    void Start()
    {
        Animator anim = GetComponent<Animator>();
        switch (invaderType) {
            case InvaderType.Basic:
                anim.runtimeAnimatorController = InvaderManager.Instance.basicInvaderAC;
                break;
            case InvaderType.Strong:
                anim.runtimeAnimatorController = InvaderManager.Instance.strongInvaderAC;
                break;
            case InvaderType.Alpha:
                anim.runtimeAnimatorController = InvaderManager.Instance.alphaInvaderAC;
                break;
            case InvaderType.Ufo:
                AudioManager.Instance.ufoSpawn.Play();
                break;
        }
    }

    public void Init(InvaderType type, int xIndex,int yIndex)
    {
        this.invaderType = type;
        this.xIndex = xIndex;
        this.yIndex = yIndex;
    }

    public void Explode()
    {
        if (isExploded)
            return;

        GetComponent<Animator>().SetTrigger("explode");
        GetComponent<Collider2D>().enabled = false;
        
        if (invaderType == InvaderType.Ufo) {
            isExploded = true;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GameManager.Instance.KilledUfo();
            AudioManager.Instance.spaceShipExplode.Play();
            AudioManager.Instance.ufoSpawn.Stop();
            Destroy(gameObject, 1f);
            return;
        }
        AudioManager.Instance.invaderExplode.Play();

        InvaderManager.Instance.InvederExploded(xIndex, yIndex);

        SpawnPowerUp();

        transform.parent = null;
        isExploded = true;

        Destroy(gameObject, 1f);
    }

    void SpawnPowerUp()
    {
        float rnd = UnityEngine.Random.value;

        switch (invaderType) {
            case InvaderType.Basic:
                GameManager.Instance.AddScore(10);

                PowerUpData pwData = GameManager.Instance.powerUpDatas[0];
                if (rnd <= pwData.spawnChance) {
                    GameObject pwUp = Instantiate(InvaderManager.Instance.powerUpPrefab, transform.position, Quaternion.identity);
                    pwUp.GetComponent<PowerUpBehaviour>().Init(pwData);
                }
                break;
            case InvaderType.Strong:
                GameManager.Instance.AddScore(20);

                pwData = GameManager.Instance.powerUpDatas[1];
                if (rnd <= pwData.spawnChance) {
                    GameObject pwUp = Instantiate(InvaderManager.Instance.powerUpPrefab, transform.position, Quaternion.identity);
                    pwUp.GetComponent<PowerUpBehaviour>().Init(pwData);

                }
                break;
            case InvaderType.Alpha:
                GameManager.Instance.AddScore(50);

                pwData = GameManager.Instance.powerUpDatas[2];
                if (rnd <= pwData.spawnChance) {
                    GameObject pwUp = Instantiate(InvaderManager.Instance.powerUpPrefab, transform.position, Quaternion.identity);
                    pwUp.GetComponent<PowerUpBehaviour>().Init(pwData);
                }
                break;
        }

    }
}
