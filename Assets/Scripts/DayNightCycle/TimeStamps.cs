using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class TimeStamps
{
    public delegate void UpdateTime(int value);
    public static UpdateTime hourTime; //Event every hour
    public static UpdateTime minuteTime; //Event every minute
    public static UpdateTime secondTime; //Event every second
    public static UpdateTime dayTime; //Event every day
    public static float wholeDayTime; //Factor of the day, being 0 the start of the day and 1 the end of the day

    public static bool timeRunning {get; private set;} //set via the function

    public static int currentHours{get; private set;} //current Hour
    public static int currentMinutes {get; private set;} //current Minute
    public static int currentDays {get; private set;}//current Day
    public static int currentSeconds {get; private set;}//current Seconds
    private static float decimalT;

    public static float dayToMinute = 1; //How many minutes is one day, 3600 being 1:1
    
    /// <summary>
    /// Updates the clock
    /// </summary>
    public static void ClockUpdate(float fixedDeltaTime)
    {
        if(!timeRunning)
            return;
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

    /// <summary>
    ///Event on every second
    /// <summary>
    private static void SecondCall()
    {
        if(secondTime != null)
            secondTime(currentSeconds);
    }
    /// <summary>
    ///Event every minute
    /// <summary>
    private static void MinuteCall()
    {
        if(minuteTime != null)
            minuteTime(currentMinutes);
    }
    /// <summary>
    ///Evenet every hour
    /// <summary>
    private static void HourCall()
    {
        if(hourTime != null)
            hourTime(currentHours);
    }
    /// <summary>
    ///Event every day
    /// <summary>
    private static void DayCall()
    {
        if(dayTime != null)
            dayTime(currentDays);
    }
    /// <summary>
    ///Start the timer
    /// <summary>
    public static void startTime()
    {
        timeRunning = true;
    }
}
