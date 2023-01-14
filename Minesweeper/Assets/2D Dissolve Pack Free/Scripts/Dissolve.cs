using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    Animator anim;

     void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DissolveOut();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            DissolveIn();
        }
    }

    private void DissolveOut()
    {
        anim.SetTrigger("DissolveOut");
    }

    private void DissolveIn()
    {
        anim.SetTrigger("DissolveIn");
    }
}
