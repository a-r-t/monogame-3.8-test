using GameEngineTest.GameObject;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

// This class is a base class for all enemies in the game -- all enemies should extend from it
namespace GameEngineTest.Level
{
    public class Enemy : MapEntity
    {
        public Enemy(float x, float y, SpriteSheet spriteSheet, string startingAnimation)
            : base(x, y, spriteSheet, startingAnimation)
        {
        }

        public Enemy(float x, float y, Dictionary<string, Frame[]> animations, string startingAnimation)
            : base(x, y, animations, startingAnimation)
        {
        }

        public Enemy(Texture2D image, float x, float y, string startingAnimation)
            : base(image, x, y, startingAnimation)
        {
        }

        public Enemy(Texture2D image, float x, float y)
            : base(image, x, y)
        {
        }

        public Enemy(Texture2D image, float x, float y, float scale)
            : base(image, x, y, scale)
        {
        }

        public Enemy(Texture2D image, float x, float y, float scale, SpriteEffects spriteEffect)
            : base(image, x, y, scale, spriteEffect)
        {
        }

        public Enemy(Texture2D image, float x, float y, float scale, SpriteEffects spriteEffect, Rectangle bounds)
            : base(image, x, y, scale, spriteEffect, bounds)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void Update(Player player)
        {
            base.Update();
            if (Intersects(player))
            {
                TouchedPlayer(player);
            }
        }

        // A subclass can override this method to specify what it does when it touches the player
        public void TouchedPlayer(Player player)
        {
            player.HurtPlayer(this);
        }
    }
}
