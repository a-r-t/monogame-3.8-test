using GameEngineTest.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Diagnostics;

namespace GameEngineTest.Engine
{
    // TODO: Look at this answer: https://stackoverflow.com/a/13905075
    public class GraphicsHandler
    {
        public GraphicsDevice GraphicsDevice { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        private Rectangle currentViewportScissorRectangle;

        public GraphicsHandler(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            GraphicsDevice = graphicsDevice;
            SpriteBatch = spriteBatch;
            currentViewportScissorRectangle = SpriteBatch.GraphicsDevice.ScissorRectangle;
        }

        public void DrawRectangle(int x, int y, int width, int height, Color color, int borderThickness = 1)
        {
            Texture2D rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new[] { Color.White });

            SpriteBatch.Draw(rectangleTexture, new Rectangle(x, y, borderThickness, height), color);
            SpriteBatch.Draw(rectangleTexture, new Rectangle(x, y, width + borderThickness, borderThickness), color);
            SpriteBatch.Draw(rectangleTexture, new Rectangle(x + width, y, borderThickness, height + borderThickness), color);
            SpriteBatch.Draw(rectangleTexture, new Rectangle(x, y + height, width + borderThickness, borderThickness), color);
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

        public void DrawFilledRectangle(int x, int y, int width, int height, Color color)
        {
            Texture2D rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new[] { Color.White });

            SpriteBatch.Draw(rectangleTexture, new Rectangle(x, y, width, height), color);
        }

        public void DrawFilledRectangleWithBorder(Rectangle rectangle, Color color, Color? borderColor = null, int borderThickness = 1)
        {
            if (borderColor == null)
            {
                borderColor = color;
            }

            Texture2D rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new[] { Color.White });

            SpriteBatch.Draw(rectangleTexture, rectangle, color);
            SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X, rectangle.Y, borderThickness, rectangle.Height), (Color)borderColor);
            SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + borderThickness, borderThickness), (Color)borderColor);
            SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, borderThickness, rectangle.Height + borderThickness), (Color)borderColor);
            SpriteBatch.Draw(rectangleTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + borderThickness, borderThickness), (Color)borderColor);
        }

        public void DrawLine(Vector2 begin, Vector2 end, Color color, int width = 1)
        {
            Texture2D rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new[] { Color.White });
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            SpriteBatch.Draw(rectangleTexture, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void DrawImage(Texture2D texture, Vector2 position, Rectangle? sourceRectangle = null, Color? color = null, float rotation = 0.0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0f)
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
            position = new Vector2(position.X.Round(), position.Y.Round());
            
            SpriteBatch.Draw(texture, position, sourceRectangle, (Color)color, rotation, (Vector2)origin, (Vector2)scale, spriteEffects, layerDepth);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color? color = null, float rotation = 0.0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0f)
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

        public void DrawString(BitmapFont bitmapFont, string text, Vector2 position, Color? color = null, float rotation = 0.0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0f)
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

            SpriteBatch.DrawString(bitmapFont, text, position, (Color)color, rotation, (Vector2)origin, (Vector2)scale, spriteEffects, layerDepth);
        }

        // TODO: This doesn't really work
        public void DrawStringWithOutline(SpriteFont spriteFont, string text, Vector2 position, Color? color = null, Color? outlineColor = null, int outlineThickness = 1, float rotation = 0.0f, Vector2? origin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float layerDepth = 0f)
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
            if (outlineColor == null)
            {
                outlineColor = color;
            }

            SpriteBatch.DrawString(spriteFont, text, position + new Vector2(outlineThickness * scale.Value.X, outlineThickness * scale.Value.Y), (Color)outlineColor, rotation, (Vector2)origin, (Vector2)scale, spriteEffects, layerDepth);
            SpriteBatch.DrawString(spriteFont, text, position + new Vector2(-outlineThickness * scale.Value.X, outlineThickness * scale.Value.Y), (Color)outlineColor, rotation, (Vector2)origin, (Vector2)scale, spriteEffects, layerDepth);
            SpriteBatch.DrawString(spriteFont, text, position + new Vector2(-outlineThickness * scale.Value.X, -outlineThickness * scale.Value.Y), (Color)outlineColor, rotation, (Vector2)origin, (Vector2)scale, spriteEffects, layerDepth);
            SpriteBatch.DrawString(spriteFont, text, position + new Vector2(outlineThickness * scale.Value.X, -outlineThickness * scale.Value.Y), (Color)outlineColor, rotation, (Vector2)origin, (Vector2)scale, spriteEffects, layerDepth);

            SpriteBatch.DrawString(spriteFont, text, position, (Color)color, rotation, (Vector2)origin, (Vector2)scale, spriteEffects, layerDepth);
        }

        public void SetScissorRectangle(Rectangle scissorRectangle)
        {
            SpriteBatch.End();
            SpriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            RasterizerState rasterizerState = new RasterizerState() { ScissorTestEnable = true, CullMode = CullMode.CullCounterClockwiseFace };
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, rasterizerState);
        }

        public void RemoveScissorRectangle()
        {
            SpriteBatch.End();
            SpriteBatch.GraphicsDevice.ScissorRectangle = currentViewportScissorRectangle;
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise);
        }
    }


}

