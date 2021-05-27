
using GameEngineTest.Engine;
using GameEngineTest.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GameEngineTest.Level
{
    public class Map
    {
        // the tile map (map tiles that make up the entire map image)
        protected MapTile[] mapTiles;

        // width and height of the map in terms of the number of tiles width-wise and height-wise
        protected int width;
        protected int height;

        // the tileset this map uses for its map tiles
        protected Tileset tileset;

        // camera class that handles the viewable part of the map that is seen by the player of a game during a level
        protected Camera camera;

        // tile player should start on when this map is first loaded
        protected Point playerStartTile;

        // the location of the "mid point" of the screen
        // this is what tells the game that the player has reached the center of the screen, therefore the camera should move instead of the player
        // this goes into creating that "map scrolling" effect
        protected int xMidPoint, yMidPoint;

        // in pixels, this basically creates a rectangle defining how big the map is
        // startX and Y will always be 0, endX and Y is the number of tiles multiplied by the number of pixels each tile takes up
        protected int startBoundX, startBoundY, endBoundX, endBoundY;

        // the name of the map text file that has the tile map information
        protected String mapFileName;

        // lists to hold map entities that are a part of the map
        protected List<Enemy> enemies;
        protected List<EnhancedMapTile> enhancedMapTiles;
        protected List<NPC> npcs;

        // if set to false, camera will not move as player moves
        protected bool adjustCamera = true;

        public Map(String mapFileName, Tileset tileset, Point playerStartTile)
        {
            this.mapFileName = mapFileName;
            this.tileset = tileset;
            SetupMap();
            this.startBoundX = 0;
            this.startBoundY = 0;
            this.endBoundX = width * tileset.getScaledSpriteWidth();
            this.endBoundY = height * tileset.getScaledSpriteHeight();
            this.xMidPoint = GameLoop.ViewportWidth / 2;
            this.yMidPoint = GameLoop.ViewportHeight / 2;
            this.playerStartTile = playerStartTile;
        }

        // sets up map by reading in the map file to create the tile map
        // loads in enemies, enhanced map tiles, and npcs
        // and instantiates a Camera
        public void SetupMap()
        {
            LoadMapFile();

            this.enemies = LoadEnemies();
            foreach (Enemy enemy in this.enemies)
            {
                enemy.setMap(this);
            }

            this.enhancedMapTiles = LoadEnhancedMapTiles();
            foreach (EnhancedMapTile enhancedMapTile in this.enhancedMapTiles)
            {
                enhancedMapTile.SetMap(this);
            }

            this.npcs = LoadNPCs();
            foreach (NPC npc in this.npcs)
            {
                npc.SetMap(this);
            }

            this.camera = new Camera(0, 0, tileset.GetScaledSpriteWidth(), tileset.GetScaledSpriteHeight(), this);
        }

        // reads in a map file to create the map's tilemap
        private void LoadMapFile()
        {
            StreamReader fileInput;
            try
            {
                // open map file that is located in the MAP_FILES_PATH directory
                fileInput = new StreamReader(Config.MAP_FILES_PATH + this.mapFileName);
            }
            catch (FileNotFoundException ex)
            {
                // if map file does not exist, create a new one for this map (the map editor uses this)
                Debug.WriteLine("Map file " + Config.MAP_FILES_PATH + this.mapFileName + " not found! Creating empty map file...");

                try
                {
                    CreateEmptyMapFile();
                    fileInput = new StreamReader(Config.MAP_FILES_PATH + this.mapFileName);
                }
                catch (IOException ex2)
                {
                    Debug.WriteLine(ex2.Message);
                    Debug.WriteLine("Failed to create an empty map file!");
                    throw new IOException();
                }
            }

            // read in map width and height from the first line of map file
            string[] dimensions = fileInput.ReadLine().Split(" ");
            this.width = Convert.ToInt32(dimensions[0]);
            this.height = Convert.ToInt32(dimensions[1]);

            // define array size for map tiles, which is width * height (this is a standard array, NOT a 2D array)
            this.mapTiles = new MapTile[this.height * this.width];
            string[] tileIndexes = fileInput.ReadToEnd().Split(" ");

            // read in each tile index from the map file, use the defined tileset to get the associated MapTile to that tileset, and place it in the array
            int currentTileIndex = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int tileIndex = currentTileIndex;
                    int xLocation = j * tileset.GetScaledSpriteWidth();
                    int yLocation = i * tileset.GetScaledSpriteHeight();
                    MapTile tile = tileset.GetTile(tileIndex).build(xLocation, yLocation);
                    tile.SetMap(this);
                    SetMapTile(j, i, tile);
                    currentTileIndex++;
                }
            }

            fileInput.Close();
        }

        // creates an empty map file for this map if one does not exist
        // defaults the map dimensions to 0x0
        private void CreateEmptyMapFile() //throws IOException
        {
            StreamWriter fileWriter = null;
            fileWriter = new StreamWriter(Config.MAP_FILES_PATH + this.mapFileName);
            fileWriter.Write("0 0\n");
            fileWriter.Close();
        }

        // gets player start position based on player start tile (basically the start tile's position on the map)
        public Point GetPlayerStartPosition()
        {
            MapTile tile = GetMapTile((int)Math.Round(playerStartTile.X), (int)Math.Round(playerStartTile.Y));
            return new Point(tile.GetX(), tile.GetY());
        }

        // get position on the map based on a specfic tile index
        public Point GetPositionByTileIndex(int xIndex, int yIndex)
        {
            MapTile tile = GetMapTile(xIndex, yIndex);
            return new Point(tile.GetX(), tile.GetY());
        }

        public Tileset GetTileset()
        {
            return tileset;
        }

        public string GetMapFileName()
        {
            return mapFileName;
        }

        public int GetWidth()
        {
            return width;
        }

        public void SetWidth(int width)
        {
            this.width = width;
        }

        public int GetHeight()
        {
            return height;
        }

        public void SetHeight(int height)
        {
            this.height = height;
        }

        public int GetWidthPixels()
        {
            return width * tileset.getScaledSpriteWidth();
        }

        public int GetHeightPixels()
        {
            return height * tileset.getScaledSpriteHeight();
        }

        public MapTile[] getMapTiles()
        {
            return mapTiles;
        }

        public void SetMapTiles(MapTile[] mapTiles)
        {
            this.mapTiles = mapTiles;
        }

        // get specific map tile from tile map
        public MapTile GetMapTile(int x, int y)
        {
            if (IsInBounds(x, y))
            {
                return mapTiles[GetConvertedIndex(x, y)];
            }
            else
            {
                return null;
            }
        }

        // set specific map tile from tile map to a new map tile
        public void SetMapTile(int x, int y, MapTile tile)
        {
            mapTiles[GetConvertedIndex(x, y)] = tile;
        }

        // returns a tile based on a position in the map
        public MapTile GetTileByPosition(int xPosition, int yPosition)
        {
            Point tileIndex = GetTileIndexByPosition(xPosition, yPosition);
            if (IsInBounds((int)Math.Round(tileIndex.X), (int)Math.Round(tileIndex.Y)))
            {
                return GetMapTile((int)Math.Round(tileIndex.X), (int)Math.Round(tileIndex.Y));
            }
            else
            {
                return null;
            }
        }

        // returns the index of a tile (x index and y index) based on a position in the map
        public Point GetTileIndexByPosition(float xPosition, float yPosition)
        {
            int xIndex = Math.Round(xPosition) / tileset.getScaledSpriteWidth();
            int yIndex = Math.Round(yPosition) / tileset.getScaledSpriteHeight();
            return new Point(xIndex, yIndex);
        }

        // checks if map tile being requested is in bounds of the tile map array
        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        // since tile map array is a standard (1D) array and not a 2D,
        // instead of doing [y][x] to get a value, instead the same can be achieved with x + width * y
        private int GetConvertedIndex(int x, int y)
        {
            return x + width * y;
        }

        // list of enemies defined to be a part of the map, should be overridden in a subclass
        protected List<Enemy> LoadEnemies()
        {
            return new List<Enemy>();
        }

        // list of enhanced map tiles defined to be a part of the map, should be overridden in a subclass
        protected List<EnhancedMapTile> LoadEnhancedMapTiles()
        {
            return new List<EnhancedMapTile>();
        }

        // list of npcs defined to be a part of the map, should be overridden in a subclass
        protected List<NPC> LoadNPCs()
        {
            return new List<NPC>();
        }

        public Camera GetCamera()
        {
            return camera;
        }

        public List<Enemy> GetEnemies()
        {
            return enemies;
        }
        public List<EnhancedMapTile> GetEnhancedMapTiles()
        {
            return enhancedMapTiles;
        }
        public List<NPC> GetNPCs()
        {
            return npcs;
        }

        // returns all active enemies (enemies that are a part of the current update cycle) -- this changes every frame by the Camera class
        public List<Enemy> GetActiveEnemies()
        {
            return camera.GetActiveEnemies();
        }

        // returns all active enhanced map tiles (enhanced map tiles that are a part of the current update cycle) -- this changes every frame by the Camera class
        public List<EnhancedMapTile> GetActiveEnhancedMapTiles()
        {
            return camera.GetActiveEnhancedMapTiles();
        }

        // returns all active npcs (npcs that are a part of the current update cycle) -- this changes every frame by the Camera class
        public List<NPC> GetActiveNPCs()
        {
            return camera.GetActiveNPCs();
        }

        // add an enemy to the map's list of enemies
        public void AddEnemy(Enemy enemy)
        {
            enemy.SetMap(this);
            this.enemies.Add(enemy);
        }

        // add an enhanced map tile to the map's list of enhanced map tiles
        public void AddEnhancedMapTile(EnhancedMapTile enhancedMapTile)
        {
            enhancedMapTile.SetMap(this);
            this.enhancedMapTiles.Add(enhancedMapTile);
        }

        // add an npc to the map's list of npcs
        public void AddNPC(NPC npc)
        {
            npc.SetMap(this);
            this.npcs.Add(npc);
        }

        public void SetAdjustCamera(bool adjustCamera)
        {
            this.adjustCamera = adjustCamera;
        }

        public void Update(Player player)
        {
            if (adjustCamera)
            {
                AdjustMovementY(player);
                AdjustMovementX(player);
            }
            camera.Update(player);
        }

        // based on the player's current X position (which in a level can potentially be updated each frame),
        // adjust the player's and camera's positions accordingly in order to properly create the map "scrolling" effect
        private void AdjustMovementX(Player player)
        {
            // if player goes past center screen (on the right side) and there is more map to show on the right side, push player back to center and move camera forward
            if (player.GetCalibratedXLocation() > xMidPoint && camera.GetEndBoundX() < endBoundX)
            {
                float xMidPointDifference = xMidPoint - player.GetCalibratedXLocation();
                camera.MoveX(-xMidPointDifference);

                // if camera moved past the right edge of the map as a result from the move above, move camera back and push player forward
                if (camera.GetEndBoundX() > endBoundX)
                {
                    float cameraDifference = camera.GetEndBoundX() - endBoundX;
                    camera.MoveX(-cameraDifference);
                }
            }
            // if player goes past center screen (on the left side) and there is more map to show on the left side, push player back to center and move camera backwards
            else if (player.GetCalibratedXLocation() < xMidPoint && camera.GetX() > startBoundX)
            {
                float xMidPointDifference = xMidPoint - player.GetCalibratedXLocation();
                camera.MoveX(-xMidPointDifference);

                // if camera moved past the left edge of the map as a result from the move above, move camera back and push player backward
                if (camera.GetX() < startBoundX)
                {
                    float cameraDifference = startBoundX - camera.GetX();
                    camera.MoveX(cameraDifference);
                }
            }
        }

        // based on the player's current Y position (which in a level can potentially be updated each frame),
        // adjust the player's and camera's positions accordingly in order to properly create the map "scrolling" effect
        private void AdjustMovementY(Player player)
        {
            // if player goes past center screen (below) and there is more map to show below, push player back to center and move camera upward
            if (player.GetCalibratedYLocation() > yMidPoint && camera.GetEndBoundY() < endBoundY)
            {
                float yMidPointDifference = yMidPoint - player.GetCalibratedYLocation();
                camera.MoveY(-yMidPointDifference);

                // if camera moved past the bottom of the map as a result from the move above, move camera upwards and push player downwards
                if (camera.GetEndBoundY() > endBoundY)
                {
                    float cameraDifference = camera.GetEndBoundY() - endBoundY;
                    camera.MoveY(-cameraDifference);
                }
            }
            // if player goes past center screen (above) and there is more map to show above, push player back to center and move camera upwards
            else if (player.GetCalibratedYLocation() < yMidPoint && camera.GetY() > startBoundY)
            {
                float yMidPointDifference = yMidPoint - player.GetCalibratedYLocation();
                camera.MoveY(-yMidPointDifference);

                // if camera moved past the top of the map as a result from the move above, move camera downwards and push player upwards
                if (camera.GetY() < startBoundY)
                {
                    float cameraDifference = startBoundY - camera.GetY();
                    camera.MoveY(cameraDifference);
                }
            }
        }

        public void Reset()
        {
            SetupMap();
        }

        public void Draw(GraphicsHandler graphicsHandler)
        {
            camera.Draw(graphicsHandler);
        }
    }
}
