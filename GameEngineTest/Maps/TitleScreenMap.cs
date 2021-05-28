using GameEngineTest.Level;
using GameEngineTest.Tilesets;
using GameEngineTest.Utils;
using System;
using System.Collections.Generic;
using System.Text;

// Represents the map that is used as a background for the main menu and credits menu screen
namespace GameEngineTest.Maps
{
    public class TitleScreenMap : Map
    {
        public TitleScreenMap() : base("title_screen_map.txt", new CommonTileset(), new Point(1, 9))
        {
        }

    }
}
