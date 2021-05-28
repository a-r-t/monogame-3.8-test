using GameEngineTest.Extensions;
using GameEngineTest.Utils;
using System;
using System.Collections.Generic;
using System.Text;

// This class has methods to check if a game object has collided with a map tile
// it is used by the game object class to determine if a collision occurred
namespace GameEngineTest.Level
{
    public class MapTileCollisionHandler
    {
        public static float GetAdjustedPositionAfterCollisionCheckX(GameObject.GameObject gameObject, Map map, Direction direction)
        {
            int numberOfTilesToCheck = Math.Max(gameObject.GetScaledBounds().Height / map.GetTileset().GetScaledSpriteHeight(), 1);
            float edgeBoundX = direction == Direction.LEFT ? gameObject.GetScaledBounds().GetX1() : gameObject.GetScaledBounds().GetX2();
            Point tileIndex = map.GetTileIndexByPosition(edgeBoundX, gameObject.GetScaledBounds().GetY1());
            for (int j = -1; j <= numberOfTilesToCheck + 1; j++)
            {
                MapTile mapTile = map.GetMapTile(tileIndex.X.Round(), tileIndex.Y.Round() + j);
                if (mapTile != null && HasCollidedWithMapTile(gameObject, mapTile, direction))
                {
                    if (direction == Direction.RIGHT)
                    {
                        float boundsDifference = gameObject.GetScaledX2() - gameObject.GetScaledBoundsX2();
                        return mapTile.GetScaledBoundsX1() - gameObject.GetScaledWidth() + boundsDifference;
                    }
                    else if (direction == Direction.LEFT)
                    {
                        float boundsDifference = gameObject.GetScaledBoundsX1() - gameObject.GetX();
                        return mapTile.GetScaledBoundsX2() - boundsDifference;
                    }
                }
            }
            foreach (EnhancedMapTile enhancedMapTile in map.GetActiveEnhancedMapTiles())
            {
                if (HasCollidedWithMapTile(gameObject, enhancedMapTile, direction))
                {
                    if (direction == Direction.RIGHT)
                    {
                        float boundsDifference = gameObject.GetScaledX2() - gameObject.GetScaledBoundsX2();
                        return enhancedMapTile.GetScaledBoundsX1() - gameObject.GetScaledWidth() + boundsDifference;
                    }
                    else if (direction == Direction.LEFT)
                    {
                        float boundsDifference = gameObject.GetScaledBoundsX1() - gameObject.GetX();
                        return enhancedMapTile.GetScaledBoundsX2() - boundsDifference;
                    }
                }
            }
            return 0;
        }

        public static float GetAdjustedPositionAfterCollisionCheckY(GameObject.GameObject gameObject, Map map, Direction direction)
        {
            int numberOfTilesToCheck = Math.Max(gameObject.GetScaledBounds().Width / map.GetTileset().GetScaledSpriteWidth(), 1);
            float edgeBoundY = direction == Direction.UP ? gameObject.GetScaledBounds().GetY1() : gameObject.GetScaledBounds().GetY2();
            Point tileIndex = map.GetTileIndexByPosition(gameObject.GetScaledBounds().GetX1(), edgeBoundY);
            for (int j = -1; j <= numberOfTilesToCheck + 1; j++)
            {
                MapTile mapTile = map.GetMapTile(tileIndex.X.Round() + j, tileIndex.Y.Round());
                if (mapTile != null && HasCollidedWithMapTile(gameObject, mapTile, direction))
                {
                    if (direction == Direction.DOWN)
                    {
                        float boundsDifference = gameObject.GetScaledY2() - gameObject.GetScaledBoundsY2();
                        return mapTile.GetScaledBoundsY1() - gameObject.GetScaledHeight() + boundsDifference;
                    }
                    else if (direction == Direction.UP)
                    {
                        float boundsDifference = gameObject.GetScaledBoundsY1() - gameObject.GetY();
                        return mapTile.GetScaledBoundsY2() - boundsDifference;
                    }
                }
            }
            foreach (EnhancedMapTile enhancedMapTile in map.GetActiveEnhancedMapTiles())
            {
                if (HasCollidedWithMapTile(gameObject, enhancedMapTile, direction))
                {
                    if (direction == Direction.DOWN)
                    {
                        float boundsDifference = gameObject.GetScaledY2() - gameObject.GetScaledBoundsY2();
                        return enhancedMapTile.GetScaledBoundsY1() - gameObject.GetScaledHeight() + boundsDifference;
                    }
                    else if (direction == Direction.UP)
                    {
                        float boundsDifference = gameObject.GetScaledBoundsY1() - gameObject.GetY();
                        return enhancedMapTile.GetScaledBoundsY2() - boundsDifference;
                    }
                }
            }
            return 0;
        }

        // based on tile type, perform logic to determine if a collision did occur with an intersecting tile or not
        private static bool HasCollidedWithMapTile(GameObject.GameObject gameObject, MapTile mapTile, Direction direction)
        {
            switch (mapTile.TileType)
            {
                case TileType.PASSABLE:
                    return false;
                case TileType.NOT_PASSABLE:
                    return gameObject.Intersects(mapTile);
                case TileType.JUMP_THROUGH_PLATFORM:
                    return direction == Direction.DOWN && gameObject.Intersects(mapTile) &&
                            gameObject.GetScaledBoundsY2().Round() - 1 == mapTile.GetScaledBoundsY1().Round();
                default:
                    return false;
            }
        }
    }
}
