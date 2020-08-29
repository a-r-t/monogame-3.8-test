using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GameEngineTest
{
    // TODO: Look at this answer: https://stackoverflow.com/a/13905075
    public class GraphicsHandler
    {
        public GraphicsDevice GraphicsDevice { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }


        public GraphicsHandler(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = spriteBatch;
        }

        public void DrawRectangle(Rectangle rectangle, Color color, int borderThickness = 1)
        {
            Texture2D rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new[] { Color.White });

            SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X, rectangle.Y, borderThickness, rectangle.Height), color);
            SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + borderThickness, borderThickness), color);
            SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, borderThickness, rectangle.Height + borderThickness), color);
            SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + borderThickness, borderThickness), color);
        }

        public void DrawFilledRectangle(Rectangle rectangle, Color color)
        {
            Texture2D rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new[] { Color.White });

            SpriteBatch.Draw(rectangleTexture, rectangle, color);
        }
         
        public void DrawFilledRectangleWithBorder(Rectangle rectangle, Color color, Color? borderColor = null, int borderThickness = 1)
        {
            if (borderColor == null)
            {
                DrawFilledRectangle(rectangle, color);
            }
            else
            {
                Texture2D rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
                rectangleTexture.SetData(new[] { Color.White });

                SpriteBatch.Draw(rectangleTexture, rectangle, color);
                SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X, rectangle.Y, borderThickness, rectangle.Height), (Color)borderColor);
                SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + borderThickness, borderThickness), (Color)borderColor);
                SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, borderThickness, rectangle.Height + borderThickness), (Color)borderColor);
                SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + borderThickness, borderThickness), (Color)borderColor);
            }
        }

        public void DrawImage(Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0.0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0.0f)
        {
            if (sourceRectangle == null)
            {
                sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            }
            if (color == null)
            {
                color = Color.White;
            }
            if (origin == null)
            {
                origin = Vector2.Zero;
            }
            if (scale == null)
            {
                scale = Vector2.One;
            }

            SpriteBatch.Draw(texture, position, sourceRectangle, (Color)color, rotation, (Vector2)origin, (Vector2)scale, spriteEffects, layerDepth);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color? color = null, float rotation = 0.0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0.0f)
        {
            if (color == null)
            {
                color = Color.White;
            }
            if (origin == null)
            {
                origin = Vector2.Zero;
            }
            if (scale == null)
            {
                scale = Vector2.One;
            }

            SpriteBatch.DrawString(spriteFont, text, position, (Color)color, rotation, (Vector2)origin, (Vector2)scale, spriteEffects, layerDepth);
        }



    }
}
