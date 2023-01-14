using UnityEngine;
using System.Collections;

public class PauseMenuMove : MonoBehaviour {

    public float offsetX;
    public float offsetY;
    public Transform targetRest;
    public Transform targetActive;
    public float speed = 6f;
    public bool isActive = false;
    private void Start() {
        
    }

    void Update()
    {
        if (isActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetActive.position, speed);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetRest.position, speed);
        }
    }

}
