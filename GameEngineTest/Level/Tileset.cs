using GameEngineTest.Builders;
using GameEngineTest.Engine;
using GameEngineTest.GameObject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

// This class represents a tileset, which defines a set of tiles based on a sprite sheet image
namespace GameEngineTest.Level
{
    public abstract class Tileset : SpriteSheet
    {
        // global scale of all tiles in the tileset
        protected float tileScale = 1f;

        // stores tiles mapped to an index
        protected Dictionary<int, MapTileBuilder> tiles;

        // default tile defined for situations where no tile information for an index can be found (failsafe basically)
        protected MapTileBuilder defaultTile;

        public Tileset(Texture2D image, int tileWidth, int tileHeight)
            : base(image, tileWidth, tileHeight)
        {
            this.tiles = MapDefinedTilesToIndex();
            this.defaultTile = GetDefaultTile();
        }

        public Tileset(Texture2D image, int tileWidth, int tileHeight, int tileScale)
            : base(image, tileWidth, tileHeight)
        {
            this.tileScale = tileScale;
            this.tiles = MapDefinedTilesToIndex();
            this.defaultTile = GetDefaultTile();
        }

        // a subclass of this class must implement this method to define tiles in the tileset
        public abstract List<MapTileBuilder> DefineTiles();

        // get specific tile from tileset by index, if not found the default tile is returned
        public MapTileBuilder GetTile(int tileNumber)
        {
            return tiles.GetValueOrDefault(tileNumber, defaultTile);
        }

        public float GetTileScale()
        {
            return tileScale;
        }

        public int GetScaledSpriteWidth()
        {
            return (int)Math.Round(SpriteWidth * tileScale);
        }

        public int GetScaledSpriteHeight()
        {
            return (int)Math.Round(SpriteHeight * tileScale);
        }

        // maps all tiles to a tile index, which is how it is identified by the map file
        public Dictionary<int, MapTileBuilder> MapDefinedTilesToIndex()
        {
            List<MapTileBuilder> mapTileBuilders = DefineTiles();
            Dictionary<int, MapTileBuilder> tilesToIndex = new Dictionary<int, MapTileBuilder>();
            for (int i = 0; i < mapTileBuilders.Count; i++)
            {
                tilesToIndex.Add(i, mapTileBuilders[i].WithTileIndex(i));
            }
            return tilesToIndex;
        }

        public MapTileBuilder GetDefaultTile()
        {
            return new MapTileBuilder(new FrameBuilder(CreateRectangleTexture(SpriteWidth, SpriteHeight, Color.Black), 0).WithScale(tileScale).Build());
        }

        private Texture2D CreateRectangleTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(GameLoop.GraphicsDeviceManager.GraphicsDevice, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Length; pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = color;
            }

            //set the color
            texture.SetData(data);

            return texture;
        }
    }
}
