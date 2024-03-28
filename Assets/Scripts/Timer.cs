using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer
{
    public float maxTime { get; private set; } //The timer's max time. Used for the reset function.
    public float timeLeft { get; private set; } //How much time the timer has left before it calls the event and returns true on IsDone
    public bool isPaused { get; private set; } //When true, using Tick() does nothing

    public UnityEvent timerComplete = new UnityEvent(); //Event is called after a tick lowers the timer to 0

    //Constructor
    public Timer(float time)
    {
        maxTime = time;
        timeLeft = maxTime;
    }


    //--Methods--\\

    //Reduces time left by tickAmount.
    //By default you should use Time.deltaTime for tickAmount.
    public void Tick(float tickAmount)
    {
        if (!isPaused && timeLeft > 0f)
        {
            timeLeft = (float)Math.Round(Mathf.Clamp(timeLeft - tickAmount, 0, maxTime), 4);
        }
        //Trigger event on complete
        if(!isPaused && timeLeft <= 0f)
        {
            timerComplete.Invoke();
        }
    }

    public bool IsDone()
    {
        return timeLeft <= 0f;
    }

    public float GetTime()
    {
        return timeLeft;
    }

    //Sets timeLeft equal to time
    public void SetTime(float time)
    {
        timeLeft = time;
    }

    public void SetMaxTime(float time)
    {
        maxTime = time;
    }

    public void Reset()
    {
        timeLeft = maxTime;
    }

    #region Pause and Unpause

    public void Pause()
    {
        isPaused = true;
    }

    public void Unpause()
    {
        isPaused = false;
    }

    //If the timer is paused, it unpauses, if the timer is unpaused, it pauses
    public void TogglePause()
    {
        isPaused = !isPaused;
    }

    //Manually set whether the timer is paused or not
    public void SetPaused(bool val)
    {
        isPaused = val;
    }

    #endregion
}
