using GameEngineTest.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameEngineTest.GameObjects
{
    public class Sprite : RectangleGraphic, IntersectableRectangle
    {
        public Texture2D Image { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        protected RectangleGraphic bounds;

        public Sprite(Texture2D image, float scale, SpriteEffects spriteEffect)
            : base(0, 0, image.Width, image.Height, scale)
        {
            Image = image;
            this.bounds = new RectangleGraphic(0, 0, image.Width, image.Height, scale);
            SpriteEffect = spriteEffect;
        }

        public Sprite(Texture2D image, float x, float y, float scale, SpriteEffects spriteEffect)
            : base(x, y, image.Width, image.Height, scale)
        {
            Image = image;
            this.bounds = new RectangleGraphic(x, y, image.Width, image.Height, scale);
            SpriteEffect = spriteEffect;
        }

        public void SetImage(String textureFilePath)
        {
            Image = Screen.ContentManager.LoadTexture(textureFilePath);
        }

        public RectangleGraphic GetHurtbox()
        {
            return new RectangleGraphic(GetBoundsX1(), GetBoundsY1(), bounds.Width, bounds.Height, Scale);
        }

        public RectangleGraphic GetBounds()
        {
            return new RectangleGraphic(GetBoundsX1(), GetBoundsY1(), bounds.Width, bounds.Height, Scale);
        }


        public float GetBoundsX1()
        {
            return X + bounds.GetX1();
        }

        public float GetBoundsX2()
        {
            return X + bounds.GetX2();
        }

        public float GetBoundsY1()
        {
            return Y + bounds.GetY1();
        }

        public float GetBoundsY2()
        {
            return Y + bounds.GetY2();
        }

        public float GetScaledBoundsX1()
        {
            return X + (bounds.GetX1() * Scale);
        }

        public float GetScaledBoundsX2()
        {
            return GetScaledBoundsX1() + bounds.GetScaledWidth();
        }

        public float GetScaledBoundsY1()
        {
            return Y + (bounds.GetY1() * Scale);
        }

        public float GetScaledBoundsY2()
        {
            return GetScaledBoundsY1() + bounds.GetScaledHeight();
        }

        public RectangleGraphic GetScaledBounds()
        {
            return new RectangleGraphic(GetScaledBoundsX1(), GetScaledBoundsY1(), bounds.GetScaledWidth(), bounds.GetScaledHeight());
        }

        public void SetBounds(RectangleGraphic hurtbox)
        {
            this.bounds = new RectangleGraphic(hurtbox.X, hurtbox.Y, hurtbox.Width, hurtbox.Height, Scale);
        }

        public void SetBounds(float x, float y, int width, int height)
        {
            this.bounds = new RectangleGraphic(x, y, width, height, Scale);
        }

        public override RectangleGraphic GetIntersectRectangle()
        {
            return GetScaledBounds();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            graphicsHandler.DrawImage(Image, new Vector2(X, Y), spriteEffects: SpriteEffect, scale: new Vector2(Scale, Scale));
        }

        public void DrawBounds(GraphicsHandler graphicsHandler, Color color)
        {
            RectangleGraphic scaledBounds = GetScaledBounds();
            scaledBounds.Color = color;
            scaledBounds.Draw(graphicsHandler);
        }

        public override string ToString()
        {
            return string.Format("Sprite: x=%s y=%s width=%s height=%s bounds=(%s, %s, %s, %s)", X, Y, GetScaledWidth(), GetScaledHeight(), GetScaledBoundsX1(), GetScaledBoundsY1(), GetScaledBounds().Width, GetScaledBounds().Height);
        }
    }
}
