using GameEngineTest.Engine;
using GameEngineTest.Extensions;
using GameEngineTest.GameObjects;
using GameEngineTest.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

// This class represents a Map's "Camera", aka a piece of the map that is currently included in a level's update/draw logic based on what should be shown on screen.
// A majority of its job is just determining which map tiles, enemies, npcs, and enhanced map tiles are "active" each frame (active = included in update/draw cycle)
namespace GameEngineTest.Level
{
    public class Camera : RectangleGraphic
    {
        // the current map this camera is attached to
        private Map map;

        // width and height of each tile in the map (the map's tileset has this info)
        private int tileWidth, tileHeight;

        // if the screen is covered in full length tiles, often there will be some extra room that doesn't quite have enough space for another entire tile
        // this leftover space keeps track of that "extra" space, which is needed to calculate the camera's current "end" position on the screen (in map coordinates, not screen coordinates)
        private int leftoverSpaceX, leftoverSpaceY;

        // current map entities that are to be included in this frame's update/draw cycle
        private List<Enemy> activeEnemies = new List<Enemy>();
        private List<EnhancedMapTile> activeEnhancedMapTiles = new List<EnhancedMapTile>();
        private List<NPC> activeNPCs = new List<NPC>();

        // determines how many tiles off screen an entity can be before it will be deemed inactive and not included in the update/draw cycles until it comes back in range
        private const int UPDATE_OFF_SCREEN_RANGE = 4;

        public Camera(int startX, int startY, int tileWidth, int tileHeight, Map map)
            : base(startX, startY, ScreenManager.GetScreenWidth() / tileWidth, ScreenManager.GetScreenHeight() / tileHeight)
        {
            this.map = map;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.leftoverSpaceX = ScreenManager.GetScreenWidth() % tileWidth;
            this.leftoverSpaceY = ScreenManager.GetScreenHeight() % tileHeight;
        }

        // gets the tile index that the camera's x and y values are currently on (top left tile)
        // this is used to determine a starting place for the rectangle of area the camera currently contains on the map
        public Point GetTileIndexByCameraPosition()
        {
            int xIndex = X.Round() / tileWidth;
            int yIndex = Y.Round() / tileHeight;
            return new Point(xIndex, yIndex);
        }

        public void Update(Player player)
        {
            UpdateMapTiles();
            UpdateMapEntities(player);
        }

        // for each map tile that is determined to be "active" (within camera's current range)
        private void UpdateMapTiles()
        {
            Point tileIndex = GetTileIndexByCameraPosition();
            for (int i = (int)(tileIndex.Y - UPDATE_OFF_SCREEN_RANGE); i <= tileIndex.Y + Height + UPDATE_OFF_SCREEN_RANGE; i++)
            {
                for (int j = (int)(tileIndex.X - UPDATE_OFF_SCREEN_RANGE); j <= tileIndex.X + Width + UPDATE_OFF_SCREEN_RANGE; j++)
                {
                    MapTile tile = map.GetMapTile(j, i);
                    if (tile != null)
                    {
                        tile.Update();
                    }
                }
            }
        }

        // update map entities currently a part of the update/draw cycle
        // active entities are calculated each frame using the loadActiveEntity methods below
        public void UpdateMapEntities(Player player)
        {
            activeEnemies = LoadActiveEnemies();
            activeEnhancedMapTiles = LoadActiveEnhancedMapTiles();
            activeNPCs = LoadActiveNPCs();

            foreach (Enemy enemy in activeEnemies)
            {
                enemy.Update(player);
            }

            foreach (EnhancedMapTile enhancedMapTile in activeEnhancedMapTiles)
            {
                enhancedMapTile.Update(player);
            }

            foreach (NPC npc in activeNPCs)
            {
                npc.Update(player);
            }
        }

        // determine which enemies are active (within range of the camera)
        // if enemy is currently active and was also active last frame, nothing special happens and enemy is included in active list
        // if enemy is currently active but last frame was inactive, it will have its status set to active and enemy is included in active list
        // if enemy is currently inactive but last frame was active, it will have its status set to inactive, have its initialize method called if its respawnable
        //      (which will set it back up to its default state), and not include it in the active list
        //      next time a respawnable enemy is determined active, since it was reset back to default state upon going inactive, it will essentially be "respawned" in its starting state
        // if enemy is currently set to REMOVED, it is permanently removed from the map's list of enemies and will never be able to be active again
        private List<Enemy> LoadActiveEnemies()
        {
            List<Enemy> activeEnemies = new List<Enemy>();
            for (int i = map.GetEnemies().Count - 1; i >= 0; i--)
            {
                Enemy enemy = map.GetEnemies()[i];

                if (IsMapEntityActive(enemy))
                {
                    activeEnemies.Add(enemy);
                    if (enemy.MapEntityStatus == MapEntityStatus.INACTIVE)
                    {
                        enemy.MapEntityStatus = MapEntityStatus.ACTIVE;
                    }
                }
                else if (enemy.MapEntityStatus == MapEntityStatus.ACTIVE)
                {
                    enemy.MapEntityStatus = MapEntityStatus.INACTIVE;
                    if (enemy.IsRespawnable)
                    {
                        enemy.Initialize();
                    }
                }
                else if (enemy.MapEntityStatus == MapEntityStatus.REMOVED)
                {
                    map.GetEnemies().RemoveAt(i);
                }
            }
            return activeEnemies;
        }

        // determine which enhanced map tiles are active (within range of the camera)
        // if enhanced map tile is currently active and was also active last frame, nothing special happens and enhanced map tile is included in active list
        // if enhanced map tile is currently active but last frame was inactive, it will have its status set to active and enhanced map tile is included in active list
        // if enhanced map tile is currently inactive but last frame was active, it will have its status set to inactive, have its initialize method called if its respawnable
        //      (which will set it back up to its default state), and not include it in the active list
        //      next time a respawnable enemy is determined active, since it was reset back to default state upon going inactive, it will essentially be "respawned" in its starting state
        // if enhanced map tile is currently set to REMOVED, it is permanently removed from the map's list of enemies and will never be able to be active again
        private List<EnhancedMapTile> LoadActiveEnhancedMapTiles()
        {
            List<EnhancedMapTile> activeEnhancedMapTiles = new List<EnhancedMapTile>();
            for (int i = map.GetEnhancedMapTiles().Count - 1; i >= 0; i--)
            {
                EnhancedMapTile enhancedMapTile = map.GetEnhancedMapTiles()[i];

                if (IsMapEntityActive(enhancedMapTile))
                {
                    activeEnhancedMapTiles.Add(enhancedMapTile);
                    if (enhancedMapTile.MapEntityStatus == MapEntityStatus.INACTIVE)
                    {
                        enhancedMapTile.MapEntityStatus = MapEntityStatus.ACTIVE;
                    }
                }
                else if (enhancedMapTile.MapEntityStatus == MapEntityStatus.ACTIVE)
                {
                    enhancedMapTile.MapEntityStatus = MapEntityStatus.INACTIVE;
                    if (enhancedMapTile.IsRespawnable)
                    {
                        enhancedMapTile.Initialize();
                    }
                }
                else if (enhancedMapTile.MapEntityStatus == MapEntityStatus.REMOVED)
                {
                    map.GetEnhancedMapTiles().RemoveAt(i);
                }
            }
            return activeEnhancedMapTiles;
        }

        // determine which npcs are active (within range of the camera)
        // if npc is currently active and was also active last frame, nothing special happens and npc is included in active list
        // if npc is currently active but last frame was inactive, it will have its status set to active and npc is included in active list
        // if npc is currently inactive but last frame was active, it will have its status set to inactive, have its initialize method called if its respawnable
        //      (which will set it back up to its default state), and not include it in the active list
        //      next time a respawnable enemy is determined active, since it was reset back to default state upon going inactive, it will essentially be "respawned" in its starting state
        // if npc is currently set to REMOVED, it is permanently removed from the map's list of enemies and will never be able to be active again
        private List<NPC> LoadActiveNPCs()
        {
            List<NPC> activeNPCs = new List<NPC>();
            for (int i = map.GetNPCs().Count - 1; i >= 0; i--)
            {
                NPC npc = map.GetNPCs()[i];

                if (IsMapEntityActive(npc))
                {
                    activeNPCs.Add(npc);
                    if (npc.MapEntityStatus == MapEntityStatus.INACTIVE)
                    {
                        npc.MapEntityStatus = MapEntityStatus.ACTIVE;
                    }
                }
                else if (npc.MapEntityStatus == MapEntityStatus.ACTIVE)
                {
                    npc.MapEntityStatus = MapEntityStatus.INACTIVE;
                    if (npc.IsRespawnable)
                    {
                        npc.Initialize();
                    }
                }
                else if (npc.MapEntityStatus == MapEntityStatus.REMOVED)
                {
                    map.GetNPCs().RemoveAt(i);
                }
            }
            return activeNPCs;
        }

        /*
            determines if map entity (enemy, enhanced map tile, or npc) is active by the camera's standards
            1. if entity's status is REMOVED, it is not active, no questions asked
            2. if entity's status is not REMOVED, then there's additional checks that take place:
                1. if entity's isUpdateOffScreen attribute is true, it is active
                2. OR if the camera determines that it is in its boundary range, it is active
         */
        private bool IsMapEntityActive(MapEntity mapEntity)
        {
            return mapEntity.MapEntityStatus != MapEntityStatus.REMOVED && (mapEntity.IsUpdateOffScreen || ContainsUpdate(mapEntity));
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            DrawMapTiles(graphicsHandler);
            DrawMapEntities(graphicsHandler);
        }

        // draws visible map tiles to the screen
        // this is different than "active" map tiles as determined in the update method -- there is no reason to actually draw to screen anything that can't be seen
        // so this does not include the extra range granted by the UPDATE_OFF_SCREEN_RANGE value
        public void DrawMapTiles(GraphicsHandler graphicsHandler)
        {
            Point tileIndex = GetTileIndexByCameraPosition();
            for (int i = tileIndex.Y.Round() - 1; i <= tileIndex.Y + Height + 1; i++)
            {
                for (int j = tileIndex.X.Round() - 1; j <= tileIndex.X + Width + 1; j++)
                {
                    MapTile tile = map.GetMapTile(j, i);
                    if (tile != null)
                    {
                        tile.Draw(graphicsHandler);
                    }
                }
            }
        }

        // draws active map entities to the screen
        public void DrawMapEntities(GraphicsHandler graphicsHandler)
        {
            foreach (Enemy enemy in activeEnemies)
            {
                if (ContainsDraw(enemy))
                {
                    enemy.Draw(graphicsHandler);
                }
            }
            foreach (EnhancedMapTile enhancedMapTile in activeEnhancedMapTiles)
            {
                if (ContainsDraw(enhancedMapTile))
                {
                    enhancedMapTile.Draw(graphicsHandler);
                }
            }
            foreach (NPC npc in activeNPCs)
            {
                if (ContainsDraw(npc))
                {
                    npc.Draw(graphicsHandler);
                }
            }
        }

        // checks if a game object's position falls within the camera's current radius
        public bool ContainsUpdate(GameObject gameObject)
        {
            return GetX1() - (tileWidth * UPDATE_OFF_SCREEN_RANGE) < gameObject.GetX() + gameObject.GetScaledWidth() &&
                    GetEndBoundX() + (tileWidth * UPDATE_OFF_SCREEN_RANGE) > gameObject.GetX() &&
                    GetY1() - (tileHeight * UPDATE_OFF_SCREEN_RANGE) < gameObject.GetY() + gameObject.GetScaledHeight()
                    && GetEndBoundY() + (tileHeight * UPDATE_OFF_SCREEN_RANGE) > gameObject.GetY();
        }

        // checks if a game object's position falls within the camera's current radius
        // this does not include the extra range granted by the UDPATE_OFF_SCREEN_RANGE value, because there is no point to drawing graphics that can't be seen
        public bool ContainsDraw(GameObject gameObject)
        {
            return GetX1() - tileWidth < gameObject.GetX() + gameObject.GetScaledWidth() && GetEndBoundX() + tileWidth > gameObject.GetX() &&
                    GetY1() - tileHeight < gameObject.GetY() + gameObject.GetScaledHeight() && GetEndBoundY() + tileHeight > gameObject.GetY();
        }

        public List<Enemy> GetActiveEnemies()
        {
            return activeEnemies;
        }

        public List<EnhancedMapTile> GetActiveEnhancedMapTiles()
        {
            return activeEnhancedMapTiles;
        }

        public List<NPC> GetActiveNPCs()
        {
            return activeNPCs;
        }

        // gets end bound X position of the camera (start position is always 0)
        public float GetEndBoundX()
        {
            return X + (Width * tileWidth) + leftoverSpaceX;
        }

        // gets end bound Y position of the camera (start position is always 0)
        public float GetEndBoundY()
        {
            return Y + (Height * tileHeight) + leftoverSpaceY;
        }
    }
}
