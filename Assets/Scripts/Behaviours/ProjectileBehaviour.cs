using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public bool isEnemy = true;
    public float moveSpeed = 5f;


    bool fired;
    public bool isStarted = false;

    void Start()
    {
        isStarted = true;
        if (isEnemy) {
            moveSpeed = -moveSpeed;
            Fire();

        }
        else {
            moveSpeed *= 2f;
            fired = false;
        }
    }

    public void Fire()
    {
        if (isStarted) {
            fired = true;
            transform.parent = null;
            GetComponent<Collider2D>().enabled = true;
            GetComponent<Rigidbody2D>().velocity = Vector3.up * moveSpeed;
        }

    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (isEnemy) {
            //player
            if (other.CompareTag("Player")) {
                other.GetComponent<PlayerController>().ExplodeShip();
                Destroy(gameObject);

            }
            //Barrier

            //Projectile
            if (other.CompareTag("Bullet")) {
                Destroy(gameObject);
            }

            //Bottom
            if (other.CompareTag("Wall")) {
                ExplodeWall();
            }
        }
        else {
            if (other.CompareTag("Enemy")) {
                other.GetComponent<InvaderBehaviour>().Explode();

                Destroy(gameObject);

            }

            //TOP
            if (other.CompareTag("Wall")) {
                ExplodeWall();
            }
        }

    }

    private void ExplodeWall()
    {
        fired = false;

        // Stop animator if exist
        if(isEnemy)
            GetComponent<Animator>().enabled = false;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = GameManager.Instance.Sprite_BulletExplodeLittle;

        // Stop movement
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        GetComponent<Collider2D>().enabled = false;

        //Destroy Game object
        Destroy(gameObject,0.5f);

    }


}
