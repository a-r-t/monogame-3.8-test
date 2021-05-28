﻿using GameEngineTest.GameObject;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

// Builder class to instantiate a Frame class
namespace GameEngineTest.Builders
{
    public class FrameBuilder
    {
        private Texture2D image;
        private int delay;
        private Rectangle bounds;
        private float scale;
        private SpriteEffects spriteEffect;

        public FrameBuilder(Texture2D image, int delay)
        {
            this.image = image;
            this.delay = delay;
            if (this.delay < 0)
            {
                this.delay = 0;
            }
            this.scale = 1;
            this.spriteEffect = SpriteEffects.None;
        }

        public FrameBuilder WithBounds(Rectangle bounds)
        {
            this.bounds = bounds;
            return this;
        }

        public FrameBuilder WithBounds(float x, float y, int width, int height)
        {
            this.bounds = new Rectangle((int)Math.Round(x), (int)Math.Round(y), width, height);
            return this;
        }

        public FrameBuilder WithScale(float scale)
        {
            if (this.scale >= 0)
            {
                this.scale = scale;
            }
            else
            {
                this.scale = 0;
            }
            return this;
        }

        public FrameBuilder WithSpriteEffect(SpriteEffects spriteEffect)
        {
            this.spriteEffect = spriteEffect;
            return this;
        }

        public Frame Build()
        {
            return new Frame(image, scale, spriteEffect, bounds, delay);
        }
    }
}