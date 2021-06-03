using GameEngineTest.Builders;
using GameEngineTest.Engine;
using GameEngineTest.GameObjects;
using GameEngineTest.Level;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

// This is the class for the Cat player character
// basically just sets some values for physics and then defines animations
namespace GameEngineTest.Players
{
    public class Cat : Player
    {
        public Cat(float x, float y)
            : base(new SpriteSheet(Screen.ContentManager.LoadTexture("Images/Cat"), 24, 24), x, y, "STAND_RIGHT")
        {
            gravity = .5f;
            terminalVelocityY = 6f;
            jumpHeight = 14.5f;
            jumpDegrade = .5f;
            walkSpeed = 3.1f;
            momentumYIncrease = .5f;
            JUMP_KEY = Keys.Up;
            MOVE_LEFT_KEY = Keys.Left;
            MOVE_RIGHT_KEY = Keys.Right;
            CROUCH_KEY = Keys.Down;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            base.Draw(graphicsHandler);
            //drawBounds(graphicsHandler, new Color(255, 0, 0, 170));
        }

        public override Dictionary<string, Frame[]> GetAnimations(SpriteSheet spriteSheet)
        {
            Dictionary<string, Frame[]> animations = new Dictionary<string, Frame[]>();
            animations.Add("STAND_RIGHT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(0, 0), 0)
                    .WithScale(3)
                    .WithBounds(8, 9, 8, 9)
                    .Build()
            });

            animations.Add("STAND_LEFT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(0, 0), 0)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(8, 9, 8, 9)
                    .Build()
            });

            animations.Add("WALK_RIGHT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(1, 0), 200)
                    .WithScale(3)
                    .WithBounds(8, 9, 8, 9)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(1, 1), 200)
                    .WithScale(3)
                    .WithBounds(8, 9, 8, 9)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(1, 2), 200)
                    .WithScale(3)
                    .WithBounds(8, 9, 8, 9)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(1, 3), 200)
                    .WithScale(3)
                    .WithBounds(8, 9, 8, 9)
                    .Build()
            });

            animations.Add("WALK_LEFT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(1, 0), 200)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(8, 9, 8, 9)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(1, 1), 200)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(8, 9, 8, 9)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(1, 2), 200)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(8, 9, 8, 9)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(1, 3), 200)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(8, 9, 8, 9)
                    .Build()
            });

            animations.Add("JUMP_RIGHT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(2, 0), 0)
                    .WithScale(3)
                    .WithBounds(8, 9, 8, 9)
                    .Build()
            });

            animations.Add("JUMP_LEFT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(2, 0), 0)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(8, 9, 8, 9)
                    .Build()
            });

            animations.Add("FALL_RIGHT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(3, 0), 0)
                    .WithScale(3)
                    .WithBounds(8, 9, 8, 9)
                    .Build()
            });

            animations.Add("FALL_LEFT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(3, 0), 0)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(8, 9, 8, 9)
                    .Build()
            });

            animations.Add("CROUCH_RIGHT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(4, 0), 0)
                    .WithScale(3)
                    .WithBounds(8, 12, 8, 6)
                    .Build()
            });

            animations.Add("CROUCH_LEFT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(4, 0), 0)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(8, 12, 8, 6)
                    .Build()
            });

            animations.Add("DEATH_RIGHT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(5, 0), 100)
                    .WithScale(3)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(5, 1), 100)
                    .WithScale(3)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(5, 2), -1)
                    .WithScale(3)
                    .Build()
            });

            animations.Add("DEATH_LEFT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(5, 0), 100)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(5, 1), 100)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(5, 2), -1)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .Build()
            });
            return animations;
        }
    };
}
