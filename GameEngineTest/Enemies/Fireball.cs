using GameEngineTest.Builders;
using GameEngineTest.Engine;
using GameEngineTest.GameObjects;
using GameEngineTest.Level;
using GameEngineTest.Utils;
using System;
using System.Collections.Generic;
using System.Text;

// This class is for the fireball enemy that the DinosaurEnemy class shoots out
// it will travel in a straight line (x axis) for a set time before disappearing
// it will disappear early if it collides with a solid map tile
namespace GameEngineTest.Enemies
{
    public class Fireball : Enemy
    {
        private float movementSpeed;
        private Stopwatch existenceTimer = new Stopwatch();

        public Fireball(Point location, float movementSpeed, int existenceTime)
            : base(location.X, location.Y, new SpriteSheet(Screen.ContentManager.LoadTexture("Images/Fireball"), 7, 7), "DEFAULT")
        {
            this.movementSpeed = movementSpeed;

            // how long the fireball will exist for before disappearing
            existenceTimer.SetWaitTime(existenceTime);

            // this enemy will not respawn after it has been removed
            IsRespawnable = false;

            Initialize();
        }

        public override void Update(Player player)
        {
            // if timer is up, set map entity status to REMOVED
            // the camera class will see this next frame and remove it permanently from the map
            if (existenceTimer.IsTimeUp())
            {
                this.MapEntityStatus = MapEntityStatus.REMOVED;
            }
            else
            {
                // move fireball forward
                MoveXHandleCollision(movementSpeed);
                base.Update(player);
            }
        }

        public override void OnEndCollisionCheckX(bool hasCollided, Direction direction)
        {
            // if fireball collides with anything solid on the x axis, it is removed
            if (hasCollided)
            {
                this.MapEntityStatus = MapEntityStatus.REMOVED;
            }
        }

        public override void TouchedPlayer(Player player)
        {
            // if fireball touches player, it disappears
            base.TouchedPlayer(player);
            this.MapEntityStatus = MapEntityStatus.REMOVED;
        }

        public override Dictionary<string, Frame[]> GetAnimations(SpriteSheet spriteSheet)
        {
            Dictionary<string, Frame[]> animations = new Dictionary<string, Frame[]>();
            animations.Add("DEFAULT", new Frame[]{
                new FrameBuilder(spriteSheet.GetSprite(0, 0), 0)
                    .WithScale(3)
                    .WithBounds(1, 1, 5, 5)
                    .Build()
            });
            return animations;
        }
    }
}
