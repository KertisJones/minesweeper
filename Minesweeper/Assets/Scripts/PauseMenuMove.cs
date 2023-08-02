using UnityEngine;
using System.Collections;

public class PauseMenuMove : MonoBehaviour {

    public float offsetX;
    public float offsetY;
    public Vector3 targetRest;
    public Vector3 targetActive;
    public float speed = 6f;
    public bool isActive = false;
    GameManager gm;
    public GameObject objectToDisableOnTimeTrial;
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        if (objectToDisableOnTimeTrial != null)
            if (gm.timeLimit != Mathf.Infinity)
                objectToDisableOnTimeTrial.SetActive(false);
    }
    public void SetActive(bool isActiveNew)
    {
        isActive = isActiveNew;
    }

    void Update()
    {
        if (isActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetActive, speed);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetRest, speed);
        }
        if (gm != null)
        {
            if (!gm.isPaused && !gm.isGameOver && !gm.isTitleMenu)
            {
                isActive = false;
            }
        }        
    }

}
