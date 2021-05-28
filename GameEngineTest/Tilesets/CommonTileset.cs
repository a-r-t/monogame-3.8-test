using GameEngineTest.Builders;
using GameEngineTest.Builders;
using GameEngineTest.Engine;
using GameEngineTest.GameObject;
using GameEngineTest.Level;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

// This class represents a "common" tileset of standard tiles defined in the CommonTileset.png file
namespace GameEngineTest.Tilesets
{
    public class CommonTileset : Tileset
    {
        public CommonTileset() : base(Screen.ContentManager.LoadTexture("CommonTileset.png"), 16, 16, 3)
        {
        }

        public override List<MapTileBuilder> DefineTiles()
        {
            List<MapTileBuilder> mapTiles = new List<MapTileBuilder>();

            // grass
            Frame grassFrame = new FrameBuilder(GetSubImage(0, 0), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder grassTile = new MapTileBuilder(grassFrame)
                    .WithTileType(TileType.NOT_PASSABLE);

            mapTiles.Add(grassTile);

            // sky
            Frame skyFrame = new FrameBuilder(GetSubImage(0, 1), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder skyTile = new MapTileBuilder(skyFrame);

            mapTiles.Add(skyTile);

            // dirt
            Frame dirtFrame = new FrameBuilder(GetSubImage(0, 2), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder dirtTile = new MapTileBuilder(dirtFrame)
                    .WithTileType(TileType.NOT_PASSABLE);

            mapTiles.Add(dirtTile);

            // sun
            Frame[] sunFrames = new Frame[] {
                new FrameBuilder(GetSubImage(2, 0), 400)
                        .WithScale(tileScale)
                        .Build(),
                new FrameBuilder(GetSubImage(2, 1), 400)
                        .WithScale(tileScale)
                        .Build()
        };

            MapTileBuilder sunTile = new MapTileBuilder(sunFrames);

            mapTiles.Add(sunTile);

            // tree trunk with full hole
            Frame treeTrunkWithFullHoleFrame = new FrameBuilder(GetSubImage(2, 2), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder treeTrunkWithFullHoleTile = new MapTileBuilder(treeTrunkWithFullHoleFrame)
                    .WithTileType(TileType.NOT_PASSABLE);

            mapTiles.Add(treeTrunkWithFullHoleTile);

            // left end branch
            Frame leftEndBranchFrame = new FrameBuilder(GetSubImage(1, 5), 0)
                    .WithScale(tileScale)
                    .WithBounds(0, 6, 16, 4)
                    .Build();

            MapTileBuilder leftEndBranchTile = new MapTileBuilder(leftEndBranchFrame)
                    .WithTileType(TileType.JUMP_THROUGH_PLATFORM);

            mapTiles.Add(leftEndBranchTile);

            // right end branch
            Frame rightEndBranchFrame = new FrameBuilder(GetSubImage(1, 5), 0)
                    .WithScale(tileScale)
                    .WithBounds(0, 6, 16, 4)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .Build();

            MapTileBuilder rightEndBranchTile = new MapTileBuilder(rightEndBranchFrame)
                    .WithTileType(TileType.JUMP_THROUGH_PLATFORM);

            mapTiles.Add(rightEndBranchTile);

            // tree trunk
            Frame treeTrunkFrame = new FrameBuilder(GetSubImage(1, 0), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder treeTrunkTile = new MapTileBuilder(treeTrunkFrame)
                    .WithTileType(TileType.NOT_PASSABLE);

            mapTiles.Add(treeTrunkTile);

            // tree top leaves
            Frame treeTopLeavesFrame = new FrameBuilder(GetSubImage(1, 1), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder treeTopLeavesTile = new MapTileBuilder(treeTopLeavesFrame)
                    .WithTileType(TileType.NOT_PASSABLE);

            mapTiles.Add(treeTopLeavesTile);

            // yellow flower
            Frame[] yellowFlowerFrames = new Frame[] {
                new FrameBuilder(GetSubImage(1, 2), 500)
                    .WithScale(tileScale)
                    .Build(),
                new FrameBuilder(GetSubImage(1, 3), 500)
                        .WithScale(tileScale)
                        .Build(),
                new FrameBuilder(GetSubImage(1, 2), 500)
                        .WithScale(tileScale)
                        .Build(),
                new FrameBuilder(GetSubImage(1, 4), 500)
                        .WithScale(tileScale)
                        .Build()
        };

            MapTileBuilder yellowFlowerTile = new MapTileBuilder(yellowFlowerFrames);

            mapTiles.Add(yellowFlowerTile);

            // purple flower
            Frame[] purpleFlowerFrames = new Frame[] {
                new FrameBuilder(GetSubImage(0, 3), 500)
                        .WithScale(tileScale)
                        .Build(),
                new FrameBuilder(GetSubImage(0, 4), 500)
                        .WithScale(tileScale)
                        .Build(),
                new FrameBuilder(GetSubImage(0, 3), 500)
                        .WithScale(tileScale)
                        .Build(),
                new FrameBuilder(GetSubImage(0, 5), 500)
                        .WithScale(tileScale)
                        .Build()
        };

            MapTileBuilder purpleFlowerTile = new MapTileBuilder(purpleFlowerFrames);

            mapTiles.Add(purpleFlowerTile);

            // middle branch
            Frame middleBranchFrame = new FrameBuilder(GetSubImage(2, 3), 0)
                    .WithScale(tileScale)
                    .WithBounds(0, 6, 16, 4)
                    .Build();

            MapTileBuilder middleBranchTile = new MapTileBuilder(middleBranchFrame)
                    .WithTileType(TileType.JUMP_THROUGH_PLATFORM);

            mapTiles.Add(middleBranchTile);

            // tree trunk hole top
            Frame treeTrunkHoleTopFrame = new FrameBuilder(GetSubImage(2, 4), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder treeTrunkHoleTopTile = new MapTileBuilder(treeTrunkHoleTopFrame)
                    .WithTileType(TileType.NOT_PASSABLE);

            mapTiles.Add(treeTrunkHoleTopTile);

            // tree trunk hole bottom
            Frame treeTrunkHoleBottomFrame = new FrameBuilder(GetSubImage(2, 5), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder treeTrunkHoleBottomTile = new MapTileBuilder(treeTrunkHoleBottomFrame)
                    .WithTileType(TileType.NOT_PASSABLE);

            mapTiles.Add(treeTrunkHoleBottomTile);

            // top water
            Frame topWaterFrame = new FrameBuilder(GetSubImage(3, 0), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder topWaterTile = new MapTileBuilder(topWaterFrame);

            mapTiles.Add(topWaterTile);

            // water
            Frame waterFrame = new FrameBuilder(GetSubImage(3, 1), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder waterTile = new MapTileBuilder(waterFrame);

            mapTiles.Add(waterTile);

            // grey rock
            Frame greyRockFrame = new FrameBuilder(GetSubImage(3, 2), 0)
                    .WithScale(tileScale)
                    .Build();

            MapTileBuilder greyRockTile = new MapTileBuilder(greyRockFrame)
                    .WithTileType(TileType.NOT_PASSABLE);

            mapTiles.Add(greyRockTile);

            return mapTiles;
        }
    }
}
