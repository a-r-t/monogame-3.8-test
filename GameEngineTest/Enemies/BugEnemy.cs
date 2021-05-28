using GameEngineTest.Builders;
using GameEngineTest.Engine;
using GameEngineTest.GameObject;
using GameEngineTest.Level;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

// This class is for the black bug enemy
// enemy behaves like a Mario goomba -- walks forward until it hits a solid map tile, and then turns around
// if it ends up in the air from walking off a cliff, it will fall down until it hits the ground again, and then will continue walking
namespace GameEngineTest.Enemies
{
    public class BugEnemy : Enemy
    {
        private float gravity = .5f;
        private float movementSpeed = .5f;
        private Direction startFacingDirection;
        private Direction facingDirection;
        private AirGroundState airGroundState;

        public BugEnemy(Point location, Direction facingDirection)
            : base(location.X, location.Y, new SpriteSheet(Screen.ContentManager.LoadTexture("Images/BugEnemy"), 24, 15), "WALK_LEFT")
        {
            this.startFacingDirection = facingDirection;
            this.Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            facingDirection = startFacingDirection;
            if (facingDirection == Direction.RIGHT)
            {
                currentAnimationName = "WALK_RIGHT";
            }
            else if (facingDirection == Direction.LEFT)
            {
                currentAnimationName = "WALK_LEFT";
            }
            airGroundState = AirGroundState.GROUND;
        }

        public override void Update(Player player)
        {
            float moveAmountX = 0;
            float moveAmountY = 0;

            // add gravity (if in air, this will cause bug to fall)
            moveAmountY += gravity;

            // if on ground, walk forward based on facing direction
            if (airGroundState == AirGroundState.GROUND)
            {
                if (facingDirection == Direction.RIGHT)
                {
                    moveAmountX += movementSpeed;
                }
                else
                {
                    moveAmountX -= movementSpeed;
                }
            }

            // move bug
            MoveYHandleCollision(moveAmountY);
            MoveXHandleCollision(moveAmountX);

            base.Update(player);
        }

        public override void OnEndCollisionCheckX(bool hasCollided, Direction direction)
        {
            // if bug has collided into something while walking forward,
            // it turns around (changes facing direction)
            if (hasCollided)
            {
                if (direction == Direction.RIGHT)
                {
                    facingDirection = Direction.LEFT;
                    currentAnimationName = "WALK_LEFT";
                }
                else
                {
                    facingDirection = Direction.RIGHT;
                    currentAnimationName = "WALK_RIGHT";
                }
            }
        }

        public override void OnEndCollisionCheckY(bool hasCollided, Direction direction)
        {
            // if bug is colliding with the ground, change its air ground state to GROUND
            // if it is not colliding with the ground, it means that it's currently in the air, so its air ground state is changed to AIR
            if (direction == Direction.DOWN)
            {
                if (hasCollided)
                {
                    airGroundState = AirGroundState.GROUND;
                }
                else
                {
                    airGroundState = AirGroundState.AIR;
                }
            }
        }

        public override Dictionary<string, Frame[]> GetAnimations(SpriteSheet spriteSheet)
        {
            Dictionary<string, Frame[]> animations = new Dictionary<string, Frame[]>();
            animations.Add("WALK_LEFT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(0, 0), 100)
                    .WithScale(2)
                    .WithBounds(6, 6, 12, 7)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(0, 1), 100)
                    .WithScale(2)
                    .WithBounds(6, 6, 12, 7)
                    .Build()
            });

            animations.Add("WALK_RIGHT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(0, 0), 100)
                    .WithScale(2)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(6, 6, 12, 7)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(0, 1), 100)
                    .WithScale(2)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(6, 6, 12, 7)
                    .Build()
            });
            return animations;
        }
    }
}