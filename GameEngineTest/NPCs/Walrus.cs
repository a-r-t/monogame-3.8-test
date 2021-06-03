using GameEngineTest.Builders;
using GameEngineTest.Engine;
using GameEngineTest.Extensions;
using GameEngineTest.FontGraphics;
using GameEngineTest.GameObjects;
using GameEngineTest.Level;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

// This class is for the walrus NPC
namespace GameEngineTest.NPCs
{
    public class Walrus : NPC
    {
        public Walrus(Utils.Point location, Map map)
            : base(location.X, location.Y, new SpriteSheet(Screen.ContentManager.LoadTexture("Images/Walrus"), 24, 24), "TAIL_DOWN", 5000)
        {
        }


        protected override SpriteFontGraphic CreateMessage()
        {
            SpriteFont arial12 = Screen.ContentManager.LoadSpriteFont("SpriteFonts/Arial12");
            return new SpriteFontGraphic("Hello!", arial12, new Vector2(GetX(), GetY() - 10), Color.Black);
        }

        public override void Update(Player player)
        {
            // while npc is being talked to, it raises its tail up (in excitement?)
            if (talkedTo)
            {
                currentAnimationName = "TAIL_UP";
            }
            else
            {
                currentAnimationName = "TAIL_DOWN";
            }
            base.Update(player);
        }

        public override Dictionary<string, Frame[]> GetAnimations(SpriteSheet spriteSheet)
        {
            Dictionary<string, Frame[]> animations = new Dictionary<string, Frame[]>();
            animations.Add("TAIL_DOWN", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(0, 0), 0)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .Build()
            });
            animations.Add("TAIL_UP", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(1, 0), 0)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .Build()
            });
            return animations;
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            base.Draw(graphicsHandler);
        }

        public override void DrawMessage(GraphicsHandler graphicsHandler)
        {
            // draws a box with a border (think like a speech box)
            graphicsHandler.DrawFilledRectangleWithBorder(new Microsoft.Xna.Framework.Rectangle(GetCalibratedXLocation().Round() - 2, GetCalibratedYLocation().Round() - 24, 40, 25), Color.White, Color.Black, 2);

            // draws message "Hello" in the above speech box
            message.SetLocation(GetCalibratedXLocation() + 2, GetCalibratedYLocation() - 8);
            message.Draw(graphicsHandler);
        }
    }
}

