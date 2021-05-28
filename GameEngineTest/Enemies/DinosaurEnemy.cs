using GameEngineTest.Builders;
using GameEngineTest.Engine;
using GameEngineTest.Extensions;
using GameEngineTest.GameObject;
using GameEngineTest.Level;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

// This class is for the green dinosaur enemy that shoots fireballs
// It walks back and forth between two set points (startLocation and endLocation)
// Every so often (based on shootTimer) it will shoot a Fireball enemy
namespace GameEngineTest.Enemies
{
    public class DinosaurEnemy : Enemy
    {
        // start and end location defines the two points that it walks between
        // is only made to walk along the x axis and has no air ground state logic, so make sure both points have the same Y value
        protected Point startLocation;
        protected Point endLocation;

        protected float movementSpeed = 1f;
        private Direction startFacingDirection;
        protected Direction facingDirection;
        protected AirGroundState airGroundState;

        // timer is used to determine when a fireball is to be shot out
        protected Stopwatch shootTimer = new Stopwatch();

        // can be either WALK or SHOOT based on what the enemy is currently set to do
        protected DinosaurState dinosaurState;
        protected DinosaurState previousDinosaurState;

        public DinosaurEnemy(Point startLocation, Point endLocation, Direction facingDirection)
            : base(startLocation.X, startLocation.Y, new SpriteSheet(Screen.ContentManager.LoadTexture("Images/DinosaurEnemy"), 14, 17), "WALK_RIGHT")
        {
            this.startLocation = startLocation;
            this.endLocation = endLocation;
            this.startFacingDirection = facingDirection;
            this.Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            dinosaurState = DinosaurState.WALK;
            previousDinosaurState = dinosaurState;
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

            // every 2 seconds, the fireball will be shot out
            shootTimer.SetWaitTime(2000);
        }

        public override void Update(Player player)
        {
            float startBound = startLocation.X;
            float endBound = endLocation.X;

            // if shoot timer is up and dinosaur is not currently shooting, set its state to SHOOT
            if (shootTimer.IsTimeUp() && dinosaurState != DinosaurState.SHOOT)
            {
                dinosaurState = DinosaurState.SHOOT;
            }

            base.Update(player);

            // if dinosaur is walking, determine which direction to walk in based on facing direction
            if (dinosaurState == DinosaurState.WALK)
            {
                if (facingDirection == Direction.RIGHT)
                {
                    currentAnimationName = "WALK_RIGHT";
                    MoveXHandleCollision(movementSpeed);
                }
                else
                {
                    currentAnimationName = "WALK_LEFT";
                    MoveXHandleCollision(-movementSpeed);
                }

                // if dinosaur reaches the start or end location, it turns around
                // dinosaur may end up going a bit past the start or end location depending on movement speed
                // this calculates the difference and pushes the enemy back a bit so it ends up right on the start or end location
                if (GetX1() + GetScaledWidth() >= endBound)
                {
                    float difference = endBound - (GetScaledX2());
                    MoveXHandleCollision(-difference);
                    facingDirection = Direction.LEFT;
                }
                else if (GetX1() <= startBound)
                {
                    float difference = startBound - GetX1();
                    MoveXHandleCollision(difference);
                    facingDirection = Direction.RIGHT;
                }

                // if dinosaur is shooting, it first turns read for 1 second
                // then the fireball is actually shot out
            }
            else if (dinosaurState == DinosaurState.SHOOT)
            {
                if (previousDinosaurState == DinosaurState.WALK)
                {
                    shootTimer.SetWaitTime(1000);
                    currentAnimationName = facingDirection == Direction.RIGHT ? "SHOOT_RIGHT" : "SHOOT_LEFT";
                }
                else if (shootTimer.IsTimeUp())
                {

                    // define where fireball will spawn on map (x location) relative to dinosaur enemy's location
                    // and define its movement speed
                    int fireballX;
                    float movementSpeed;
                    if (facingDirection == Direction.RIGHT)
                    {
                        fireballX = GetX().Round() + GetScaledWidth();
                        movementSpeed = 1.5f;
                    }
                    else
                    {
                        fireballX = GetX().Round();
                        movementSpeed = -1.5f;
                    }

                    // define where fireball will spawn on the map (y location) relative to dinosaur enemy's location
                    int fireballY = GetY().Round() + 4;

                    // create Fireball enemy
                    Fireball fireball = new Fireball(new Point(fireballX, fireballY), movementSpeed, 1000);

                    // add fireball enemy to the map for it to offically spawn in the level
                    map.AddEnemy(fireball);

                    // change dinosaur back to its WALK state after shooting, reset shootTimer to wait another 2 seconds before shooting again
                    dinosaurState = DinosaurState.WALK;
                    shootTimer.SetWaitTime(2000);
                }
            }
            previousDinosaurState = dinosaurState;
        }

        public override void OnEndCollisionCheckX(bool hasCollided, Direction direction)
        {
            // if dinosaur enemy collides with something on the x axis, it turns around and walks the other way
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

        public override Dictionary<string, Frame[]> GetAnimations(SpriteSheet spriteSheet)
        {
            Dictionary<string, Frame[]> animations = new Dictionary<string, Frame[]>();
            animations.Add("WALK_LEFT", new Frame[]{
                new FrameBuilder(spriteSheet.GetSprite(0, 0), 200)
                    .WithScale(3)
                    .WithBounds(4, 2, 5, 13)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(0, 1), 200)
                    .WithScale(3)
                    .WithBounds(4, 2, 5, 13)
                    .Build()
            });

            animations.Add("WALK_RIGHT", new Frame[]{
                new FrameBuilder(spriteSheet.GetSprite(0, 0), 200)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(4, 2, 5, 13)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(0, 1), 200)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(4, 2, 5, 13)
                    .Build()
            });

            animations.Add("SHOOT_LEFT", new Frame[]{
                new FrameBuilder(spriteSheet.GetSprite(1, 0), 0)
                    .WithScale(3)
                    .WithBounds(4, 2, 5, 13)
                    .Build(),
            });

            animations.Add("SHOOT_RIGHT", new Frame[]{
                new FrameBuilder(spriteSheet.GetSprite(1, 0), 0)
                    .WithScale(3)
                    .WithSpriteEffect(SpriteEffects.FlipHorizontally)
                    .WithBounds(4, 2, 5, 13)
                    .Build(),
            });
            return animations;
        }
    }
    
    public enum DinosaurState
    {
        WALK, SHOOT
    }
}