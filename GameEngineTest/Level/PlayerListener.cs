using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.Level
{
    public interface PlayerListener
    {
        void OnLevelCompleted();
        void OnDeath();
    }
}
