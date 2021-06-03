using GameEngineTest.Engine;
using GameEngineTest.GameObjects;
using GameEngineTest.Level;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

// This class is for a horizontal moving platform
// the platform will move back and forth between its start location and end location
// if the player is standing on top of it, the player will be moved the same amount as the platform is moving (so the platform will not slide out from under the player)
namespace GameEngineTest.EnchancedMapTiles
{
    public class HorizontalMovingPlatform : EnhancedMapTile
    {
        private Point startLocation;
        private Point endLocation;
        private float movementSpeed = 1f;
        private Direction startDirection;
        private Direction direction;


        public HorizontalMovingPlatform(Texture2D image, Point startLocation, Point endLocation, TileType tileType, float scale, RectangleGraphic bounds, Direction startDirection)
            : base(image, startLocation.X, startLocation.Y, tileType, scale, SpriteEffects.None, bounds)
        {
            this.startLocation = startLocation;
            this.endLocation = endLocation;
            this.startDirection = startDirection;
            this.Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            direction = startDirection;
        }

        public override void Update(Player player)
        {
            float startBound = startLocation.X;
            float endBound = endLocation.X;

            // move platform left or right based on its current direction
            float moveAmountX = 0;
            if (direction == Direction.RIGHT)
            {
                moveAmountX += movementSpeed;
            }
            else if (direction == Direction.LEFT)
            {
                moveAmountX -= movementSpeed;
            }

            MoveX(moveAmountX);

            // if platform reaches the start or end location, it turns around
            // platform may end up going a bit past the start or end location depending on movement speed
            // this calculates the difference and pushes the platform back a bit so it ends up right on the start or end location
            if (GetX1() + GetScaledWidth() >= endBound)
            {
                float difference = endBound - (GetX1() + GetScaledWidth());
                MoveX(-difference);
                moveAmountX -= difference;
                direction = Direction.LEFT;
            }
            else if (GetX1() <= startBound)
            {
                float difference = startBound - GetX1();
                MoveX(difference);
                moveAmountX += difference;
                direction = Direction.RIGHT;
            }

            // if tile type is NOT PASSABLE, if the platform is moving and hits into the player (x axis), it will push the player
            if (TileType == TileType.NOT_PASSABLE)
            {
                if (Intersects(player) && moveAmountX >= 0 && player.GetScaledBoundsX1() <= GetScaledBoundsX2())
                {
                    player.MoveXHandleCollision(GetScaledBoundsX2() - player.GetScaledBoundsX1());
                }
                else if (Intersects(player) && moveAmountX <= 0 && player.GetScaledBoundsX2() >= GetScaledBoundsX1())
                {
                    player.MoveXHandleCollision(GetScaledBoundsX1() - player.GetScaledBoundsX2());
                }
            }

            // if player is on standing on top of platform, move player by the amount the platform is moving
            // this will cause the player to "ride" with the moving platform
            // without this code, the platform would slide right out from under the player
            if (Overlaps(player) && player.GetScaledBoundsY2() == GetScaledBoundsY1() && player.AirGroundState == AirGroundState.GROUND)
            {
                player.MoveXHandleCollision(moveAmountX);
            }

            base.Update(player);
        }

        public override void Draw(GraphicsHandler graphicsHandler)
        {
            base.Draw(graphicsHandler);
            //base.DrawBounds(graphicsHandler, new Color(0, 0, 255, 100));
        }
    }
}
