using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

/*
 * Default Screen that does nothing
 * Its existence is really just to prevent null pointers from occurring if no Screen is set somewhere
 * Think of it as the equivalent as setting a String to "" instead of just leaving it as null
 */
namespace GameEngineTest.Engine
{
    public class DefaultScreen : Screen
    {
        public DefaultScreen() { }

        public override void Initialize() { }

        public override void Update(GameTime gameTime) { }

        public override void Draw(GraphicsHandler graphicsHandler) { }
    }
}
