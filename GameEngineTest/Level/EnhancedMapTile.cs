using GameEngineTest.Engine;
using GameEngineTest.GameObject;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

// This class is a base class for all enhanced map tiles in the game -- all enhanced map tiles should extend from it
namespace GameEngineTest.Level
{
    public class EnhancedMapTile : MapTile
    {
        public EnhancedMapTile(float x, float y, SpriteSheet spriteSheet, string startingAnimation, TileType tileType)
            : base(x, y, spriteSheet, startingAnimation, tileType)
        {
        }

        public EnhancedMapTile(float x, float y, Dictionary<string, Frame[]> animations, string startingAnimation, TileType tileType)
            : base(x, y, animations, startingAnimation, tileType)
        {
        }

        public EnhancedMapTile(Texture2D image, float x, float y, String startingAnimation, TileType tileType)
            : base(image, x, y, startingAnimation, tileType)
        {
        }

        public EnhancedMapTile(Texture2D image, float x, float y, TileType tileType)
            : base(image, x, y, tileType)
        {
        }

        public EnhancedMapTile(Texture2D image, float x, float y, TileType tileType, float scale)
            : base(image, x, y, scale, tileType)
        {
        }

        public EnhancedMapTile(Texture2D image, float x, float y, TileType tileType, float scale, SpriteEffects spriteEffect)
            : base(image, x, y, scale, spriteEffect, tileType)
        {
        }

        public EnhancedMapTile(Texture2D image, float x, float y, TileType tileType, float scale, SpriteEffects spriteEffect, Rectangle bounds)
            : base(image, x, y, scale, spriteEffect, bounds, tileType)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }


        public virtual void Update(Player player)
        {
            base.Update();
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            base.Draw(graphicsHandler);
        }
    }
}
