using GameEngineTest.Engine;
using GameEngineTest.FontGraphics;
using GameEngineTest.GameObject;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

// This class is a base class for all npcs in the game -- all npcs should extend from it
namespace GameEngineTest.Level
{
    public class NPC : MapEntity
    {
        protected bool talkedTo = false;
        protected SpriteFontGraphic message;
        protected int talkedToTime;
        protected Stopwatch timer = new Stopwatch();

        public NPC(float x, float y, SpriteSheet spriteSheet, String startingAnimation, int talkedToTime)
            : base(x, y, spriteSheet, startingAnimation)
        {
            this.message = CreateMessage();
            this.talkedToTime = talkedToTime;
        }

        public NPC(float x, float y, Dictionary<string, Frame[]> animations, String startingAnimation, int talkedToTime)
            :base(x, y, animations, startingAnimation)
        {
            this.message = CreateMessage();
            this.talkedToTime = talkedToTime;
        }

        public NPC(Texture2D image, float x, float y, String startingAnimation, int talkedToTime)
            : base(image, x, y, startingAnimation)
        {
            this.message = CreateMessage();
            this.talkedToTime = talkedToTime;
        }

        public NPC(Texture2D image, float x, float y, int talkedToTime)
            : base(image, x, y)
        {
            this.message = CreateMessage();
            this.talkedToTime = talkedToTime;
        }

        public NPC(Texture2D image, float x, float y, int talkedToTime, float scale)
            : base(image, x, y, scale)
        {
            this.message = CreateMessage();
            this.talkedToTime = talkedToTime;
        }

        public NPC(Texture2D image, float x, float y, int talkedToTime, float scale, SpriteEffects spriteEffect)
            : base(image, x, y, scale, spriteEffect)
        {
            this.message = CreateMessage();
            this.talkedToTime = talkedToTime;
        }

        public NPC(Texture2D image, float x, float y, int talkedToTime, float scale, SpriteEffects spriteEffect, Rectangle bounds)
            : base(image, x, y, scale, spriteEffect, bounds)
        {
            this.message = CreateMessage();
            this.talkedToTime = talkedToTime;
        }

        protected virtual SpriteFontGraphic CreateMessage()
        {
            return null;
        }

        public virtual void Update(Player player)
        {
            base.Update();
            CheckTalkedTo(player);
        }

        public void CheckTalkedTo(Player player)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (Intersects(player) && keyboardState.IsKeyDown(Keys.Space))
            {
                talkedTo = true;
                timer.SetWaitTime(talkedToTime);
            };
            if (talkedTo && timer.IsTimeUp())
            {
                talkedTo = false;
            }
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            base.Draw(graphicsHandler);
            if (message != null && talkedTo)
            {
                DrawMessage(graphicsHandler);
            }
        }

        // A subclass can override this method to specify what message it displays upon being talked to
        public virtual void DrawMessage(GraphicsHandler graphicsHandler) { }
    }
}
