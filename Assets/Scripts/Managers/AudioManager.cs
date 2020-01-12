using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
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
    }

}