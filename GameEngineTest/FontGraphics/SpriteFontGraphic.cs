using GameEngineTest.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameEngineTest.FontGraphics
{
    public class SpriteFontGraphic : FontGraphic
    {
        public SpriteFont SpriteFont { get; set; }

        public SpriteFontGraphic(string text, SpriteFont spriteFont, Vector2 position, Color color) : base(text, position, color)
        {
            SpriteFont = spriteFont;
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            graphicsHandler.DrawString(SpriteFont, Text, Position, Color);
        }
    }
}
