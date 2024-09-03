using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PieChart : MonoBehaviour
{
    public Image[] imagesPieChart;
    public TextMeshProUGUI[] labelsPieChart;
    public string[] labelSuffix;
    public float[] values;
    
    

    // Start is called before the first frame update
    void Start()
    {
        SetValues(values);
        HideSuffixes();
    }

    /*void Update()
    {
        SetValues(values);
    }*/

    public void SetValues(float[] newValues)
    {
        float totalAmount = 0f;
        foreach (float newValue in newValues)
        {
            totalAmount += newValue;
        }

        float totalPercent = 0f;

        for(int i = 0; i < imagesPieChart.Length; i++)
        {
            float newPercent = totalAmount; 
            if (i < newValues.Length)
            {
                newPercent = newValues[i] / totalAmount;
                totalPercent += newPercent;
                labelsPieChart[i].text = newValues[i].ToString("#,#");                
            }                
            else
            {
                labelsPieChart[i].text = "";
            }

            imagesPieChart[i].fillAmount = totalPercent;
            
            // Labels
            float radius = imagesPieChart[i].rectTransform.rect.width / 4;
            float labelPercent = totalPercent - (newPercent / 2);
            float theta = Mathf.Deg2Rad * (labelPercent * 360f * -1);
            // x = r*Cos(theta)
            // y = r*Sin(theta)
            labelsPieChart[i].transform.localPosition = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0);
            labelsPieChart[i].GetComponent<IdleJiggle>().SetNewStartingValues();
        }

        values = newValues;
    }

    public void ShowSuffix(int i)
    {
        if(i < labelSuffix.Length && i < values.Length)
            if (values[i] > 0 && labelSuffix[i] != "")
                labelsPieChart[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = labelSuffix[i];
    }

    public void HideSuffixes()
    {
        for(int i = 0; i < labelsPieChart.Length; i++)
        {
            labelsPieChart[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }
    }


}
