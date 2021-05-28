using GameEngineTest.Engine;
using GameEngineTest.GameObject;
using GameEngineTest.Level;
using GameEngineTest.Tilesets;
using GameEngineTest.Utils;
using System;
using System.Collections.Generic;
using System.Text;

// Represents a test map to be used in a level
namespace GameEngineTest.Maps
{
    public class TestMap : Map
    {
        public TestMap() : base("test_map.txt", new CommonTileset(), new Point(1, 11))
        {
        }

        public override List<Enemy> LoadEnemies()
        {
            List<Enemy> enemies = new List<Enemy>();
            enemies.Add(new BugEnemy(GetPositionByTileIndex(15, 9), Direction.LEFT));
            enemies.Add(new DinosaurEnemy(GetPositionByTileIndex(19, 1).AddY(2), GetPositionByTileIndex(22, 1).AddY(2), Direction.RIGHT));
            return enemies;
        }

        public override List<EnhancedMapTile> LoadEnhancedMapTiles()
        {
            List<EnhancedMapTile> enhancedMapTiles = new List<EnhancedMapTile>();

            enhancedMapTiles.Add(new HorizontalMovingPlatform(
                Screen.ContentManager.LoadTexture("GreenPlatform.png"),
                GetPositionByTileIndex(24, 6),
                GetPositionByTileIndex(27, 6),
                TileType.JUMP_THROUGH_PLATFORM,
                3,
                new Rectangle(0, 6, 16, 4),
                Direction.RIGHT
            ));

            enhancedMapTiles.Add(new EndLevelBox(
                GetPositionByTileIndex(32, 7)
            ));

            return enhancedMapTiles;
        }

        public override List<NPC> LoadNPCs()
        {
            List<NPC> npcs = new List<NPC>();

            npcs.Add(new Walrus(GetPositionByTileIndex(30, 10).Subtract(new Point(0, 13)), this));

            return npcs;
        }
    }
}
