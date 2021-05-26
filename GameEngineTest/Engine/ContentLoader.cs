using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace GameEngineTest.Engine
{
    public class ContentLoader : ContentManager
    {
        public ContentLoader(IServiceProvider serviceProvider, string rootDirectory) 
            : base(serviceProvider, rootDirectory)
        {

        }

        public Texture2D LoadTexture(string texturePath)
        {
            return Load<Texture2D>(texturePath);
        }

        public SpriteFont LoadSpriteFont(string spriteFontPath)
        {
            return Load<SpriteFont>(spriteFontPath);
        }

        public BitmapFont LoadBitmapFont(string bitmapFontPath)
        {
            return Load<BitmapFont>(bitmapFontPath);
        }
    }
}
