using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.Engine
{
    public abstract class Screen
    {
        public abstract void Initialize();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GraphicsHandler graphicsHandler);
    }
}
