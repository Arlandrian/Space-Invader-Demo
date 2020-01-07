using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public bool isEnemy = true;
    public float moveSpeed = 5f;


    bool fired = false;

    void Start()
    {
        moveSpeed = isEnemy ? -moveSpeed:moveSpeed;
    }


    void Update()
    {
        if (fired) {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
    }

    public void Fire()
    {
        fired = true;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (isEnemy) {
            //player
            ;
            //Barrier

            //Bottom
        }
        else {
            if (other.CompareTag("Enemy")) {
                other.GetComponent<InvaderBehaviour>().Explode();
                Destroy(gameObject);
            }

            //Barrier

            //UfO
            //TOP
        }





    }


}
