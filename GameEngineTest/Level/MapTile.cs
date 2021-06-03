using GameEngineTest.Engine;
using GameEngineTest.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

// Represents a map tile in a Map's tile map
namespace GameEngineTest.Level
{
    public class MapTile : MapEntity
    {
        // this determines a tile's properties, like if it's passable or not
        public TileType TileType { get; private set; }

        private int tileIndex;

        public MapTile(float x, float y, Dictionary<string, Frame[]> animations, string startingAnimation, int tileIndex, TileType tileType)
            : base(x, y, animations, startingAnimation)
        {
            TileType = tileType;
            this.tileIndex = tileIndex;
        }

        public MapTile(float x, float y, SpriteSheet spriteSheet, string startingAnimation, TileType tileType)
            : base(x, y, spriteSheet, startingAnimation)
        {
            TileType = tileType;
        }

        public MapTile(float x, float y, Dictionary<string, Frame[]> animations, string startingAnimation, TileType tileType)
            : base(x, y, animations, startingAnimation)
        {
            TileType = tileType;
        }

        public MapTile(Texture2D image, float x, float y, string startingAnimation, TileType tileType)
            : base(image, x, y, startingAnimation)
        {
            TileType = tileType;
        }

        public MapTile(Texture2D image, float x, float y, TileType tileType)
            : base(image, x, y)
        {
            TileType = tileType;
        }

        public MapTile(Texture2D image, float x, float y, float scale, TileType tileType)
            : base(image, x, y, scale)
        {
            TileType = tileType;
        }

        public MapTile(Texture2D image, float x, float y, float scale, SpriteEffects spriteEffect, TileType tileType)
            : base(image, x, y, scale, spriteEffect)
        {
            TileType = tileType;
        }

        public MapTile(Texture2D image, float x, float y, float scale, SpriteEffects spriteEffect, RectangleGraphic bounds, TileType tileType)
            : base(image, x, y, scale, spriteEffect, bounds)
        {
            TileType = tileType;
        }

        public int getTileIndex()
        {
            return tileIndex;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            base.Draw(graphicsHandler);
            //drawBounds(graphicsHandler, new Color(0, 0, 255, 100));
        }
    }
}
