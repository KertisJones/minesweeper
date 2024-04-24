using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabSelection : MonoBehaviour
{
    private int currentTab = 0;
    public GameObject[] TabButtons;
    public GameObject[] TabMenus;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < TabButtons.Length; i++)
        {
            int i2 = i;
            TabButtons[i].GetComponent<Button>().onClick.AddListener(delegate { SetTab(i2); });
        }
        SetTab(currentTab);
    }

    public void SetTab(int newTab)
    {
        currentTab = newTab;
        if (currentTab >= TabMenus.Length)
            currentTab = TabMenus.Length - 1;
        RefreshTabs();
    }
    
    public void RefreshTabs()
    {
        if (TabMenus.Length == 0)
            return;
        
        for (int i = 0; i < TabMenus.Length; i++)
        {
            if (i != currentTab)
            {
                TabMenus[i].SetActive(false);
                TabButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }            
        }

        if (currentTab >= 0)
        {
            TabMenus[currentTab].SetActive(true);
            TabButtons[currentTab].GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }        
    }

    public void HideTabs()
    {
        for (int i = 0; i < TabMenus.Length; i++)
        {
            TabMenus[i].SetActive(false);
            TabButtons[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);          
        }
    }
}
