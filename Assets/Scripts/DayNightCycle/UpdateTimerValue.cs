using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateTimerValue : MonoBehaviour
{
    public enum TimeStamp{Hours, Minutes, Seconds, Days}
    public TimeStamp currentReading;
    private TextMeshProUGUI textField;
    public void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        switch(currentReading)
        {
            case TimeStamp.Hours:
                TimeStamps.hourTime += UpdateValue;
                break;
            case TimeStamp.Minutes:
                TimeStamps.minuteTime += UpdateValue;
                break;
            case TimeStamp.Seconds:
                TimeStamps.secondTime += UpdateValue;
                break;
            case TimeStamp.Days:
                TimeStamps.dayTime += UpdateValue;
            break;
        }
    }
    public void UpdateValue(int value)
    {
        string valueShown = value.ToString();
        if(valueShown.Length < 2)
            valueShown = "0" + valueShown;
        textField.text = valueShown;
    }
    void OnEnable()
    {
        Start();
    }
}
