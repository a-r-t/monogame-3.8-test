using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameEngineTest.Utils
{
    public class Stopwatch
    {
        private long beforeTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        private int millisecondsToWait = 0;

        // tell stopwatch how many milliseconds to "time"
        public void SetWaitTime(int millisecondsToWait)
        {
            this.millisecondsToWait = millisecondsToWait;
            beforeTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /*
        public void Tick(double millisecondsPassed)
        {
            currentTime += millisecondsPassed;
        }
        */

        // will return true or false based on if the "time" is up (a specified number of milliseconds have passed)
        public bool IsTimeUp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - beforeTime > millisecondsToWait;
        }

        // reset timer to wait again for specified number of milliseconds
        public void Reset()
        {
            SetWaitTime(millisecondsToWait);
        }
    }
}
