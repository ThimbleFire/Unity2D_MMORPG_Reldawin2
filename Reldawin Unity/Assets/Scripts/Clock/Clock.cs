using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public int ServerTime = int.MinValue;
    public int FrameTime = int.MinValue;
    public int TimePerFrame = int.MinValue;
    public Sprite[] timesOfDayInOrder;
    public Image image;
    public int frame = 0;

    private const int DaysInGamePerRealDays = 4;
    private const int SecondsInADay = 86400; /* / DaysInGamePerRealDays; */
    private const float Interval = 1.0f;
    private float Timer = Interval;

    public Text debug;

    public void Setup(int gameTime)
    {
        ServerTime = gameTime;

        TimePerFrame = SecondsInADay / timesOfDayInOrder.Length;
        frame = Mathf.FloorToInt((ServerTime / TimePerFrame ) );
        FrameTime = ServerTime % TimePerFrame;

        UpdateImage();
    }

    void Update()
    {
        Timer -= Time.deltaTime;

        //if a second has expired
        if ( Timer <= 0.0f )
        {
            Timer = Interval + Timer;

            //add a minute to the game clock
            FrameTime++;
            if ( FrameTime >= TimePerFrame )
            {
                frame++;
                FrameTime = 0;
            }

            // if a day has expired...
            ServerTime++;
            if ( ServerTime >= SecondsInADay )
            {
                ServerTime = 0;

                if ( frame >= timesOfDayInOrder.Length )
                    frame = 0;
            }

            UpdateImage();
        }

        debug.text = string.Format("ServerTime {1} out of {2}\nFrameTime: {3} out of {4}",
            Timer, ServerTime, SecondsInADay, FrameTime, TimePerFrame
            );
    }

    void UpdateImage()
    {
        image.sprite = timesOfDayInOrder[frame];
    }
}
