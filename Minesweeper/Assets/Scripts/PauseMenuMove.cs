using UnityEngine;
using System.Collections;

public class PauseMenuMove : MonoBehaviour {
    private Camera mainCamera;
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
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (objectToDisableOnTimeTrial != null)
            if (gm.timeLimit != Mathf.Infinity)
                objectToDisableOnTimeTrial.SetActive(false);
        
        targetActive = mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight / 2, 10));

        targetRest = mainCamera.ScreenToWorldPoint(new Vector3(mainCamera.pixelWidth / 2, mainCamera.pixelHeight * 2, 10));

        float scaleModifier = mainCamera.orthographicSize / 10.5f;
        speed *= scaleModifier;
        this.transform.localScale = new Vector3(scaleModifier, scaleModifier, scaleModifier);
        this.transform.position = targetRest;
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
