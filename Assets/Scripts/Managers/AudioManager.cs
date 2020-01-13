using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public float startingPeriod = 1.5f;
    public float period = 1.5f;
    public AudioSource [] mainMusic;
    public AudioSource spaceShipFire { get; private set; }
    public AudioSource spaceShipExplode { get; private set; }
    public AudioSource invaderExplode { get; private set; }
    public AudioSource ufoSpawn { get; private set; }
    public AudioSource powerUp { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        spaceShipFire = transform.GetChild(0).GetComponent<AudioSource>() ;
        spaceShipExplode = transform.GetChild(1).GetComponent<AudioSource>() ;
        invaderExplode = transform.GetChild(2).GetComponent<AudioSource>() ;
        ufoSpawn = transform.GetChild(3).GetComponent<AudioSource>(); 
        powerUp = transform.GetChild(4).GetComponent<AudioSource>();

        ResetPeriod();
    }

    int counter = 0;
    float lastTimePlayed = 0;
    private void FixedUpdate()
    {
        if (GameManager.Instance.isGamePaused)
            return;

        float elapsedTime = Time.time - lastTimePlayed;
        if(elapsedTime > period) {
            lastTimePlayed = Time.time;
            mainMusic[counter].Play();
            counter++;
            if (counter == 4) {
                counter = 0;
                if (period > 0.5f)
                    period -= 0.04f;
            }
        }
    }

    public void ResetPeriod()
    {
        period = startingPeriod;
    }
}