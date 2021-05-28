using GameEngineTest.GameObject;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngineTest.Level
{
    public abstract class Player : GameObject.GameObject
    {
        // values that affect player movement
        // these should be set in a subclass
        protected float walkSpeed = 0;
        protected float gravity = 0;
        protected float jumpHeight = 0;
        protected float jumpDegrade = 0;
        protected float terminalVelocityY = 0;
        protected float momentumYIncrease = 0;

        // values used to handle player movement
        protected float jumpForce = 0;
        protected float momentumY = 0;
        protected float moveAmountX, moveAmountY;

        // values used to keep track of player's current state
        public PlayerState PlayerState { get; set; }
        protected PlayerState previousPlayerState;
        public Direction FacingDirection { get; set; }
        public AirGroundState AirGroundState { get; set; }
        protected AirGroundState previousAirGroundState;
        public LevelState LevelState { get; set; }

        // classes that listen to player events can be added to this list
        protected List<PlayerListener> listeners = new List<PlayerListener>();

        // define keys
        protected KeyLocker keyLocker = new KeyLocker();
        protected Keys JUMP_KEY = Keys.Up;
        protected Keys MOVE_LEFT_KEY = Keys.Left;
        protected Keys MOVE_RIGHT_KEY = Keys.Right;
        protected Keys CROUCH_KEY = Keys.Down;

        // if true, player cannot be hurt by enemies (good for testing)
        protected bool isInvincible = false;

        public Player(SpriteSheet spriteSheet, float x, float y, String startingAnimationName)
            : base(spriteSheet, x, y, startingAnimationName)
        {
            FacingDirection = Direction.RIGHT;
            AirGroundState = AirGroundState.AIR;
            previousAirGroundState = AirGroundState;
            PlayerState = PlayerState.STANDING;
            previousPlayerState = PlayerState;
            LevelState = LevelState.RUNNING;
        }

        public override void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            moveAmountX = 0;
            moveAmountY = 0;

            // if player is currently playing through level (has not won or lost)
            if (LevelState == LevelState.RUNNING)
            {
                ApplyGravity();

                // update player's state and current actions, which includes things like determining how much it should move each frame and if its walking or jumping
                do
                {
                    previousPlayerState = PlayerState;
                    HandlePlayerState(keyboardState);
                } while (previousPlayerState != PlayerState);

                previousAirGroundState = AirGroundState;

                // update player's animation
                base.Update();

                // move player with respect to map collisions based on how much player needs to move this frame
                base.MoveYHandleCollision(moveAmountY);
                base.MoveXHandleCollision(moveAmountX);

                UpdateLockedKeys(keyboardState);
            }

            // if player has beaten level
            else if (LevelState == LevelState.LEVEL_COMPLETED)
            {
                UpdateLevelCompleted();
            }

            // if player has lost level
            else if (LevelState == LevelState.PLAYER_DEAD)
            {
                UpdatePlayerDead();
            }
        }

        // add gravity to player, which is a downward force
        protected void ApplyGravity()
        {
            moveAmountY += gravity + momentumY;
        }

        // based on player's current state, call appropriate player state handling method
        protected void HandlePlayerState(KeyboardState keyboardState)
        {
            switch (PlayerState)
            {
                case PlayerState.STANDING:
                    PlayerStanding(keyboardState);
                    break;
                case PlayerState.WALKING:
                    PlayerWalking(keyboardState);
                    break;
                case PlayerState.CROUCHING:
                    PlayerCrouching(keyboardState);
                    break;
                case PlayerState.JUMPING:
                    PlayerJumping(keyboardState);
                    break;
            }
        }

        // player STANDING state logic
        protected void PlayerStanding(KeyboardState keyboardState)
        {
            // sets animation to a STAND animation based on which way player is facing
            currentAnimationName = FacingDirection == Direction.RIGHT ? "STAND_RIGHT" : "STAND_LEFT";

            // if walk left or walk right key is pressed, player enters WALKING state
            if (keyboardState.IsKeyDown(MOVE_LEFT_KEY) || keyboardState.IsKeyDown(MOVE_RIGHT_KEY))
            {
                PlayerState = PlayerState.WALKING;
            }

            // if jump key is pressed, player enters JUMPING state
            else if (keyboardState.IsKeyDown(JUMP_KEY) && !keyLocker.IsKeyLocked(JUMP_KEY))
            {
                keyLocker.LockKey(JUMP_KEY);
                PlayerState = PlayerState.JUMPING;
            }

            // if crouch key is pressed, player enters CROUCHING state
            else if (keyboardState.IsKeyDown(CROUCH_KEY))
            {
                PlayerState = PlayerState.CROUCHING;
            }
        }

        // player WALKING state logic
        protected void PlayerWalking(KeyboardState keyboardState)
        {
            // sets animation to a WALK animation based on which way player is facing
            currentAnimationName = FacingDirection == Direction.RIGHT ? "WALK_RIGHT" : "WALK_LEFT";

            // if walk left key is pressed, move player to the left
            if (keyboardState.IsKeyDown(MOVE_LEFT_KEY))
            {
                moveAmountX -= walkSpeed;
                FacingDirection = Direction.LEFT;
            }

            // if walk right key is pressed, move player to the right
            else if (keyboardState.IsKeyDown(MOVE_RIGHT_KEY))
            {
                moveAmountX += walkSpeed;
                FacingDirection = Direction.RIGHT;
            }
            else if (keyboardState.IsKeyUp(MOVE_LEFT_KEY) && keyboardState.IsKeyUp(MOVE_RIGHT_KEY))
            {
                PlayerState = PlayerState.STANDING;
            }

            // if jump key is pressed, player enters JUMPING state
            if (keyboardState.IsKeyDown(JUMP_KEY) && !keyLocker.IsKeyLocked(JUMP_KEY))
            {
                keyLocker.LockKey(JUMP_KEY);
                PlayerState = PlayerState.JUMPING;
            }

            // if crouch key is pressed,
            else if (keyboardState.IsKeyDown(CROUCH_KEY))
            {
                PlayerState = PlayerState.CROUCHING;
            }
        }

        // player CROUCHING state logic
        protected void PlayerCrouching(KeyboardState keyboardState)
        {
            // sets animation to a CROUCH animation based on which way player is facing
            currentAnimationName = FacingDirection == Direction.RIGHT ? "CROUCH_RIGHT" : "CROUCH_LEFT";

            // if crouch key is released, player enters STANDING state
            if (keyboardState.IsKeyUp(CROUCH_KEY))
            {
                PlayerState = PlayerState.STANDING;
            }

            // if jump key is pressed, player enters JUMPING state
            if (keyboardState.IsKeyDown(JUMP_KEY) && !keyLocker.IsKeyLocked(JUMP_KEY))
            {
                keyLocker.LockKey(JUMP_KEY);
                PlayerState = PlayerState.JUMPING;
            }
        }

        // player JUMPING state logic
        protected void PlayerJumping(KeyboardState keyboardState)
        {
            // if last frame player was on ground and this frame player is still on ground, the jump needs to be setup
            if (previousAirGroundState == AirGroundState.GROUND && AirGroundState == AirGroundState.GROUND)
            {

                // sets animation to a JUMP animation based on which way player is facing
                currentAnimationName = FacingDirection == Direction.RIGHT ? "JUMP_RIGHT" : "JUMP_LEFT";

                // player is set to be in air and then player is sent into the air
                AirGroundState = AirGroundState.AIR;
                jumpForce = jumpHeight;
                if (jumpForce > 0)
                {
                    moveAmountY -= jumpForce;
                    jumpForce -= jumpDegrade;
                    if (jumpForce < 0)
                    {
                        jumpForce = 0;
                    }
                }
            }

            // if player is in air (currently in a jump) and has more jumpForce, continue sending player upwards
            else if (AirGroundState == AirGroundState.AIR)
            {
                if (jumpForce > 0)
                {
                    moveAmountY -= jumpForce;
                    jumpForce -= jumpDegrade;
                    if (jumpForce < 0)
                    {
                        jumpForce = 0;
                    }
                }

                // if player is moving upwards, set player's animation to jump. if player moving downwards, set player's animation to fall
                if (previousY > Math.Round(y))
                {
                    currentAnimationName = FacingDirection == Direction.RIGHT ? "JUMP_RIGHT" : "JUMP_LEFT";
                }
                else
                {
                    currentAnimationName = FacingDirection == Direction.RIGHT ? "FALL_RIGHT" : "FALL_LEFT";
                }

                // allows you to move left and right while in the air
                if (keyboardState.IsKeyDown(MOVE_LEFT_KEY))
                {
                    moveAmountX -= walkSpeed;
                }
                else if (keyboardState.IsKeyDown(MOVE_RIGHT_KEY))
                {
                    moveAmountX += walkSpeed;
                }

                // if player is falling, increases momentum as player falls so it falls faster over time
                if (moveAmountY > 0)
                {
                    IncreaseMomentum();
                }
            }

            // if player last frame was in air and this frame is now on ground, player enters STANDING state
            else if (previousAirGroundState == AirGroundState.AIR && AirGroundState == AirGroundState.GROUND)
            {
                PlayerState = PlayerState.STANDING;
            }
        }

        // while player is in air, this is called, and will increase momentumY by a set amount until player reaches terminal velocity
        protected void IncreaseMomentum()
        {
            momentumY += momentumYIncrease;
            if (momentumY > terminalVelocityY)
            {
                momentumY = terminalVelocityY;
            }
        }

        protected void UpdateLockedKeys(KeyboardState keyboardState)
        {
            if (keyboardState.IsKeyUp(JUMP_KEY))
            {
                keyLocker.UnlockKey(JUMP_KEY);
            }
        }

        public override void OnEndCollisionCheckX(bool hasCollided, Direction direction)
        {

        }

        public override void OnEndCollisionCheckY(bool hasCollided, Direction direction)
        {
            // if player collides with a map tile below it, it is now on the ground
            // if player does not collide with a map tile below, it is in air
            if (direction == Direction.DOWN)
            {
                if (hasCollided)
                {
                    momentumY = 0;
                    AirGroundState = AirGroundState.GROUND;
                }
                else
                {
                    PlayerState = PlayerState.JUMPING;
                    AirGroundState = AirGroundState.AIR;
                }
            }

            // if player collides with map tile upwards, it means it was jumping and then hit into a ceiling -- immediately stop upwards jump velocity
            else if (direction == Direction.UP)
            {
                if (hasCollided)
                {
                    jumpForce = 0;
                }
            }
        }

        // other entities can call this method to hurt the player
        public void HurtPlayer(MapEntity mapEntity)
        {
            if (!isInvincible)
            {
                // if map entity is an enemy, kill player on touch
                if (mapEntity is Enemy) {
                    LevelState = LevelState.PLAYER_DEAD;
                }
            }
        }

        // other entities can call this to tell the player they beat a level
        public void CompleteLevel()
        {
            LevelState = LevelState.LEVEL_COMPLETED;
        }

        // if player has beaten level, this will be the update cycle
        public void UpdateLevelCompleted()
        {
            // if player is not on ground, player should fall until it touches the ground
            if (AirGroundState != AirGroundState.GROUND && map.GetCamera().ContainsDraw(this))
            {
                currentAnimationName = "FALL_RIGHT";
                ApplyGravity();
                IncreaseMomentum();
                base.Update();
                MoveYHandleCollision(moveAmountY);
            }
            // move player to the right until it walks off screen
            else if (map.GetCamera().ContainsDraw(this))
            {
                currentAnimationName = "WALK_RIGHT";
                base.Update();
                MoveXHandleCollision(walkSpeed);
            }
            else
            {
                // tell all player listeners that the player has finished the level
                foreach (PlayerListener listener in listeners)
                {
                    listener.OnLevelCompleted();
                }
            }
        }

        // if player has died, this will be the update cycle
        public void UpdatePlayerDead()
        {
            // change player animation to DEATH
            if (!currentAnimationName.StartsWith("DEATH"))
            {
                if (FacingDirection == Direction.RIGHT)
                {
                    currentAnimationName = "DEATH_RIGHT";
                }
                else
                {
                    currentAnimationName = "DEATH_LEFT";
                }
                base.Update();
            }
            // if death animation not on last frame yet, continue to play out death animation
            else if (currentFrameIndex != GetCurrentAnimation().Length - 1)
            {
                base.Update();
            }
            // if death animation on last frame (it is set up not to loop back to start), player should continually fall until it goes off screen
            else if (currentFrameIndex == GetCurrentAnimation().Length - 1)
            {
                if (map.GetCamera().ContainsDraw(this))
                {
                    MoveY(3);
                }
                else
                {
                    // tell all player listeners that the player has died in the level
                    foreach (PlayerListener listener in listeners)
                    {
                        listener.OnDeath();
                    }
                }
            }
        }

        public void AddListener(PlayerListener listener)
        {
            listeners.Add(listener);
        }

    }
}
