using GameEngineTest.GameObjects;
using GameEngineTest.Level;
using System;
using System.Collections.Generic;
using System.Text;

// Builder class to instantiate a MapTile class
namespace GameEngineTest.Builders
{
    public class MapTileBuilder : GameObjectBuilder
    {
        private TileType tileType = TileType.PASSABLE;
        private int tileIndex = -1;

        public MapTileBuilder(Frame frame) : base(frame) { }

        public MapTileBuilder(Frame[] frames) : base(frames) { }

        public MapTileBuilder WithTileType(TileType tileType)
        {
            this.tileType = tileType;
            return this;
        }

        public MapTileBuilder WithTileIndex(int tileIndex)
        {
            this.tileIndex = tileIndex;
            return this;
        }

        public new MapTile Build(float x, float y)
        {
            return new MapTile(x, y, CloneAnimations(), startingAnimationName, tileIndex, tileType);
        }
    }
}
