﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.Engine
{
    public abstract class Screen
    {
        public static ContentLoader ContentManager { get; private set; }

        public Screen()
        {
            ContentManager = new ContentLoader(GameLoop.GameServiceContainer, GameLoop.ContentManager.RootDirectory);
            ContentManager.RootDirectory = GameLoop.ContentManager.RootDirectory;
        }

        public virtual void Initialize() { }
        public virtual void LoadContent() { }
        public virtual void UnloadContent()
        {
            ContentManager.Unload();
        }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GraphicsHandler graphicsHandler) { }
    }
}
