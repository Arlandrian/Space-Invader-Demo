using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject bulletPrefab;

    [Header("References")]
    public Transform barrelPlace;

    [Header("Settings")]

    public bool useRawInput = false;
    public float moveSpeed = 5f;
    public float fireWaitTime = 1.0f;

#region Private Variables

    float lastFireTime = 0f;
    bool loaded = true;
    GameObject loadedBulletRef;
#endregion

    void Start()
    {
        LoadBullet();
    }

    void Update()
    {
        // User input
        float horizontal = useRawInput ? Input.GetAxisRaw("Horizontal"): Input.GetAxis("Horizontal");
        bool fire = Input.GetButton("Jump");
        // Movement
        transform.position += Vector3.right * horizontal * moveSpeed * Time.deltaTime;


        if (!loaded) {
            if(Time.time - lastFireTime >= fireWaitTime) {
                LoadBullet();
            }
        }



        if (loaded && fire) {
            Fire();
        }
    }

    void Fire()
    {
        lastFireTime = Time.time;

        loadedBulletRef.transform.parent = null;
        loadedBulletRef.GetComponent<ProjectileBehaviour>().Fire();
        loadedBulletRef = null;

        loaded = false;
    }

    void LoadBullet()
    {
        loaded = true;
        loadedBulletRef = Instantiate(bulletPrefab,barrelPlace.position,Quaternion.identity,barrelPlace);
    }
}
