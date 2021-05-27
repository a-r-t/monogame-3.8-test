using System;
using System.Collections.Generic;
using System.Text;

// Represents different tile types a MapTile or EnhancedMapTile can have, which affects how entities interact with it collision wise
namespace GameEngineTest.Level
{
    public enum TileType
    {
        PASSABLE, NOT_PASSABLE, JUMP_THROUGH_PLATFORM
    }
}
