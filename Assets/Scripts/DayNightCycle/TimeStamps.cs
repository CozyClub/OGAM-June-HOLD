using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class TimeStamps
{
    public delegate void UpdateTime(int value);
    public static UpdateTime hourTime;
    public static UpdateTime minuteTime;
    public static UpdateTime secondTime;
    public static UpdateTime dayTime;
    public static float wholeDayTime;

    private static int currentHours;
    private static int currentMinutes;
    private static int currentDays;
    private static int currentSeconds;
    private static float decimalT;

    public static float dayToMinute = 1; //15 minutes is one day
    
    public static void ClockUpdate(float fixedDeltaTime)
    {
        decimalT += 3600/dayToMinute*fixedDeltaTime;

        if(decimalT > 1)
        {
            currentSeconds += Mathf.FloorToInt(decimalT);
            decimalT = decimalT%1;
            SecondCall();
        }

        if(currentSeconds/60 >= 1)
        {
            currentMinutes += currentSeconds/60;
            currentSeconds = currentSeconds%60;
            SecondCall();
            MinuteCall();
        }

        if(currentMinutes/60 >= 1)
        {
            currentHours += currentMinutes/60;
            currentMinutes = currentMinutes%60;
            MinuteCall();
            HourCall();
        }

        if(currentHours/24 >= 1)
        {
            currentDays += currentHours/24;
            currentHours = currentHours%24; 
            HourCall();
            DayCall();
        }
        wholeDayTime = (currentSeconds+currentMinutes*60+currentHours*60*60)/86400f;
    }
    private static void SecondCall()
    {
        if(secondTime != null)
            secondTime(currentSeconds);
    }
    private static void MinuteCall()
    {
        if(minuteTime != null)
            minuteTime(currentMinutes);
    }
    private static void HourCall()
    {
        if(hourTime != null)
            hourTime(currentHours);
    }
    private static void DayCall()
    {
        if(dayTime != null)
            dayTime(currentDays);
    }

    public static int getHours()
    {return currentHours;}

    public static int getMinutes()
    {return currentMinutes;}

    public static float getSeconds()
    {return currentSeconds;}
    public static float getDecimalT()
    {return decimalT;}
}
