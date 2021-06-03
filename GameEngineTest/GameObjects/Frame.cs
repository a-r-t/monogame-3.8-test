using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.GameObjects
{
    public class Frame : Sprite
    {
        private int delay;

		public Frame(Texture2D image, float scale, SpriteEffects spriteEffect, RectangleGraphic bounds, int delay)
			: base(image, scale, spriteEffect)
		{
			if (bounds != null)
			{
				this.bounds = bounds;
				this.bounds.Scale = scale;
			}
			this.delay = delay;
		}

		public int GetDelay()
		{
			return delay;
		}

		public Frame Copy()
		{
			return new Frame(Image, Scale, SpriteEffect, bounds, delay);
		}
	}
}
