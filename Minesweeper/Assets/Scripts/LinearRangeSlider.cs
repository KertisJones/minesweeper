using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinearRangeSlider : MonoBehaviour
{
    public Slider slider;
    public TMPro.TextMeshProUGUI valueText;
    public bool autoUpdatePercentage = true;
    public bool invertPercentage = false;
    public bool maxIsInfinity = false;
    public float percentMultiplier = 100f;
    public string suffix = "%";
    // Start is called before the first frame update
    void Start()
    {
        if (autoUpdatePercentage)
        {
            slider.onValueChanged.AddListener(delegate { UpdateTextPercentage(); });
            UpdateTextPercentage();
        }
        else
        {
            slider.onValueChanged.AddListener(delegate { UpdateTextRaw(); });
            UpdateTextRaw();
        }
    }

    public void UpdateTextPercentage()
    {
        float decimalPercent = (slider.minValue + slider.value) / (slider.maxValue - slider.minValue);
        if (invertPercentage)
            decimalPercent = 1 - decimalPercent;
        float percent = Mathf.Round(decimalPercent * percentMultiplier);
        valueText.text = percent + suffix;
    }

    public void UpdateTextRaw() 
    {
        float value = slider.value;
        if (invertPercentage)
            value = slider.maxValue - value;
        
        string valueStr = value + suffix;
        if (maxIsInfinity && value == slider.maxValue)
            valueStr = "∞" + suffix;

        valueText.text = valueStr;
    }

    public void SetAdjustedValue(float newValue)
    {
        float value = newValue;
        if (invertPercentage)
            value = slider.maxValue - value;
        
        slider.value = value;
    }

    public float GetAdjustedValue()
    {
        float value = slider.value;
        if (invertPercentage)
            value = slider.maxValue - value;
        
        return value;
    }
}
