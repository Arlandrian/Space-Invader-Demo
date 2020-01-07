using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderBehaviour : MonoBehaviour
{

    public InvaderType invaderType;

    // Start is called before the first frame update
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
        }
    }

    public void Explode()
    {
        GetComponent<Animator>().SetTrigger("explode");
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f);

        GameManager.Instance.AddScore(100);

        //Decrement current invader count
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
