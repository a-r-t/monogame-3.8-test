using GameEngineTest.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.Level
{
    public class MapEntity : GameObject
    {
        public MapEntityStatus MapEntityStatus { get; set; } = MapEntityStatus.ACTIVE;

        // if true, if entity goes out of the camera's update range, and then ends up back in range, the entity will "respawn" back to its starting parameters
        public bool IsRespawnable { get; set; } = true;

        // if true, enemy cannot go out of camera's update range
        public bool IsUpdateOffScreen { get; set; }

        public MapEntity(float x, float y, SpriteSheet spriteSheet, string startingAnimation)
            : base(spriteSheet, x, y, startingAnimation)
        {
        }

        public MapEntity(float x, float y, Dictionary<string, Frame[]> animations, string startingAnimation)
            : base(x, y, animations, startingAnimation)
        {
        }

        public MapEntity(Texture2D image, float x, float y, string startingAnimation)
            : base(image, x, y, startingAnimation)
        {
        }

        public MapEntity(Texture2D image, float x, float y)
            : base(image, x, y)
        {
        }

        public MapEntity(Texture2D image, float x, float y, float scale)
            : base(image, x, y, scale)
        {
        }

        public MapEntity(Texture2D image, float x, float y, float scale, SpriteEffects spriteEffect)
            : base(image, x, y, scale, spriteEffect)
        {
        }

        public MapEntity(Texture2D image, float x, float y, float scale, SpriteEffects spriteEffect, RectangleGraphic bounds)
            : base(image, x, y, scale, spriteEffect, bounds)
        {
        }

        public override void Initialize()
        {
            this.x = startPositionX;
            this.y = startPositionY;
            this.amountMovedX = 0;
            this.amountMovedY = 0;
            this.previousX = startPositionX;
            this.previousY = startPositionY;
            UpdateCurrentFrame();
        }
    }
}
