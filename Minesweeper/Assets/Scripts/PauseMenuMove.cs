using UnityEngine;
using System.Collections;

public class PauseMenuMove : MonoBehaviour {
    private Camera mainCamera;
    public float offsetX;
    public float offsetY;
    public Vector3 targetRest;
    public Vector3 targetActive;
    public float speed = 6f;
    private bool isActive = false;
    GameManager gm;
    TabSelection tabs;
    public GameObject[] objectsToDisableWhileActive;

    void OnEnable()
    {
        GameManager.OnResetStartingPositionsEvent += SetScale;
    }
    void OnDisable()
    {
        GameManager.OnResetStartingPositionsEvent -= SetScale;
    }

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        tabs = GetComponent<TabSelection>();

        SetScale();
    }
    public void SetScale()
    {
        if (mainCamera == null)
            return;

        targetActive = mainCamera.ScreenToWorldPoint(new Vector3((float)mainCamera.pixelWidth / 2f, (float)mainCamera.pixelHeight / 2f, 10));
        targetRest = mainCamera.ScreenToWorldPoint(new Vector3((float)mainCamera.pixelWidth / 2f, (float)mainCamera.pixelHeight * 2f, 10));

        float scaleModifier = mainCamera.orthographicSize / 10.5f;
        speed *= scaleModifier;
        this.transform.localScale = new Vector3(scaleModifier, scaleModifier, scaleModifier);

        if (isActive)
            this.transform.position = targetActive;
        else
            this.transform.position = targetRest;
    }

    public bool GetIsActive()
    {
        return isActive;
    }
    public void SetActive(bool isActiveNew)
    {
        isActive = isActiveNew;

        for (int i = 0; i < objectsToDisableWhileActive.Length; i++)
        {
            objectsToDisableWhileActive[i].SetActive(!isActiveNew);
        }

        if (tabs != null)
        {
            if (isActive)
                tabs.RefreshTabs();
            else
                tabs.HideTabs();
        }
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
