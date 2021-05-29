using GameEngineTest.Builders;
using GameEngineTest.Engine;
using GameEngineTest.Extensions;
using GameEngineTest.Level;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

/*
	The all important GameObject class is what every "entity" used in this game should be based off of
	It encapsulates all the other class logic in the GameObject package to be a "one stop shop" for all entity needs
	This includes:
	1. displaying an image (as a sprite) to represent the entity
	2. animation logic for the sprite
	3. collision detection with a map
	4. performing proper draw logic based on camera movement
 */
namespace GameEngineTest.GameObject
{
    public class GameObject : AnimatedSprite
    {
		// stores game object's start position
		// important to keep track of this as it's what allows the special draw logic to work
		protected float startPositionX, startPositionY;

		// how much game object's position has changed from start position over time
		// also important to keep track of for the special draw logic
		protected float amountMovedX, amountMovedY;

		// previous location the game object was in from the last frame
		protected float previousX, previousY;

		// the map instance this game object "belongs" to.
		protected Map map;

		public GameObject(SpriteSheet spriteSheet, float x, float y, string startingAnimation)
			: base(spriteSheet, x, y, startingAnimation)
		{
			this.startPositionX = x;
			this.startPositionY = y;
			this.previousX = x;
			this.previousY = y;
		}

		public GameObject(float x, float y, Dictionary<string, Frame[]> animations, string startingAnimation) 
			: base(x, y, animations, startingAnimation)
		{
			this.startPositionX = x;
			this.startPositionY = y;
			this.previousX = x;
			this.previousY = y;
		}

		public GameObject(Texture2D image, float x, float y, string startingAnimation)
			: base(image, x, y, startingAnimation)
		{
			this.startPositionX = x;
			this.startPositionY = y;
			this.previousX = x;
			this.previousY = y;
		}

		public GameObject(Texture2D image, float x, float y) 
			: base(x, y)
		{
			this.animations = new Dictionary<string, Frame[]>();
			this.animations.Add("DEFAULT", new Frame[] {
				new FrameBuilder(image, 0).Build()
			});
			
			this.currentAnimationName = "DEFAULT";
			UpdateCurrentFrame();
			this.startPositionX = x;
			this.startPositionY = y;
			this.previousX = x;
			this.previousY = y;
		}

		public GameObject(Texture2D image, float x, float y, float scale)
			: base(x, y)
		{
			this.animations = new Dictionary<string, Frame[]>();
			this.animations.Add("DEFAULT", new Frame[] {
				new FrameBuilder(image, 0)
					.WithScale(scale)
					.Build()
			});
			this.currentAnimationName = "DEFAULT";
			UpdateCurrentFrame();
			this.startPositionX = x;
			this.startPositionY = y;
			this.previousX = x;
			this.previousY = y;
		}

		public GameObject(Texture2D image, float x, float y, float scale, SpriteEffects spriteEffect) 
			: base(x, y)
		{
			this.animations = new Dictionary<string, Frame[]>();
			this.animations.Add("DEFAULT", new Frame[] {
				new FrameBuilder(image, 0)
					.WithScale(scale)
					.WithSpriteEffect(spriteEffect)
					.Build()
			});
			this.currentAnimationName = "DEFAULT";
			UpdateCurrentFrame();
			this.startPositionX = x;
			this.startPositionY = y;
			this.previousX = x;
			this.previousY = y;
		}

		public GameObject(Texture2D image, float x, float y, float scale, SpriteEffects spriteEffect, Rectangle bounds)
			: base(x, y)
		{
			this.animations = new Dictionary<string, Frame[]>();
			this.animations.Add("DEFAULT", new Frame[]{
				new FrameBuilder(image, 0)
					.WithScale(scale)
					.WithSpriteEffect(spriteEffect)
					.WithBounds(bounds)
					.Build()
			});
			this.currentAnimationName = "DEFAULT";
			UpdateCurrentFrame();
			this.startPositionX = x;
			this.startPositionY = y;
			this.previousX = x;
			this.previousY = y;
		}

		public override void Update()
		{
			// call to animation logic
			base.Update();

			// update previous position to be the current position
			previousX = x;
			previousY = y;
		}

		// move game object along the x axis
		// will stop object from moving based on map collision logic (such as if it hits a solid tile)
		public void MoveXHandleCollision(float dx)
		{
			if (map != null)
			{
				HandleCollisionX(dx);
			}
			else
			{
				base.MoveX(dx);
			}
		}

		// move game object along the y axis
		// will stop object from moving based on map collision logic (such as if it hits a solid tile)
		public void MoveYHandleCollision(float dy)
		{
			if (map != null)
			{
				HandleCollisionY(dy);
			}
			else
			{
				base.MoveY(dy);
			}
		}

		// performs collision check logic for moving along the x axis against the map's tiles
		public float HandleCollisionX(float moveAmountX)
		{
			// determines amount to move (whole number)
			int amountToMove = (int)Math.Abs(moveAmountX);

			// gets decimal remainder from amount to move
			float moveAmountXRemainder = MathUtils.GetRemainder(moveAmountX);

			// determines direction that will be moved in based on if moveAmountX is positive or negative
			Direction direction = moveAmountX < 0 ? Direction.LEFT : Direction.RIGHT;

			// moves game object one pixel at a time until total move amount is reached
			// if at any point a map tile collision is determined to have occurred from the move,
			// move player back to right in front of the "solid" map tile's position, and stop attempting to move further
			float amountMoved = 0;
			bool hasCollided = false;
			for (int i = 0; i < amountToMove; i++)
			{
				MoveX((int)direction);
				float newLocation = MapTileCollisionHandler.GetAdjustedPositionAfterCollisionCheckX(this, map, direction);
				if (newLocation != 0)
				{
					hasCollided = true;
					SetX(newLocation);
					break;
				}
				amountMoved = (i + 1) * (int)direction;
			}

			// if no collision occurred in the above steps, this deals with the decimal remainder from the original move amount (stored in moveAmountXRemainder)
			// it starts by moving the game object by that decimal amount
			// it then does one more check for a collision in the case that this added decimal amount was enough to change the rounding and move the game object to the next pixel over
			// if a collision occurs from this move, the player is moved back to right in front of the "solid" map tile's position
			if (!hasCollided)
			{
				MoveX(moveAmountXRemainder * (int)direction);
				float newLocation = MapTileCollisionHandler.GetAdjustedPositionAfterCollisionCheckX(this, map, direction);
				if (newLocation != 0)
				{
					hasCollided = true;
					SetX(newLocation);
				}
			}

			// call this method which a game object subclass can override to listen for collision events and react accordingly
			OnEndCollisionCheckX(hasCollided, direction);

			// returns the amount actually moved -- this isn't really used by the game, but I have it here for debug purposes
			return amountMoved + (moveAmountXRemainder * (int)direction);
		}

		// performs collision check logic for moving along the y axis against the map's tiles
		public float HandleCollisionY(float moveAmountY)
		{
			// determines amount to move (whole number)
			int amountToMove = (int)Math.Abs(moveAmountY);

			// gets decimal remainder from amount to move
			float moveAmountYRemainder = MathUtils.GetRemainder(moveAmountY);

			// determines direction that will be moved in based on if moveAmountY is positive or negative
			Direction direction = moveAmountY < 0 ? Direction.UP : Direction.DOWN;

			// moves game object one pixel at a time until total move amount is reached
			// if at any point a map tile collision is determined to have occurred from the move,
			// move player back to right in front of the "solid" map tile's position, and stop attempting to move further
			float amountMoved = 0;
			bool hasCollided = false;
			for (int i = 0; i < amountToMove; i++)
			{
				MoveY((int)direction);
				float newLocation = MapTileCollisionHandler.GetAdjustedPositionAfterCollisionCheckY(this, map, direction);
				if (newLocation != 0)
				{
					hasCollided = true;
					SetY(newLocation);
					break;
				}
				amountMoved = (i + 1) * (int)direction;
			}

			// if no collision occurred in the above steps, this deals with the decimal remainder from the original move amount (stored in moveAmountYRemainder)
			// it starts by moving the game object by that decimal amount
			// it then does one more check for a collision in the case that this added decimal amount was enough to change the rounding and move the game object to the next pixel over
			// if a collision occurs from this move, the player is moved back to right in front of the "solid" map tile's position
			if (!hasCollided)
			{
				MoveY(moveAmountYRemainder * (int)direction);
				float newLocation = MapTileCollisionHandler.GetAdjustedPositionAfterCollisionCheckY(this, map, direction);
				if (newLocation != 0)
				{
					hasCollided = true;
					SetY(newLocation);
				}
			}

			// call this method which a game object subclass can override to listen for collision events and react accordingly
			OnEndCollisionCheckY(hasCollided, direction);

			// returns the amount actually moved -- this isn't really used by the game, but I have it here for debug purposes
			return amountMoved + (moveAmountYRemainder * (int)direction);
		}

		// game object subclass can override this method to listen for x axis collision events and react accordingly after calling "moveXHandleCollision"
		public virtual void OnEndCollisionCheckX(bool hasCollided, Direction direction) { }

		// game object subclass can override this method to listen for y axis collision events and react accordingly after calling "moveYHandleCollision"
		public virtual void OnEndCollisionCheckY(bool hasCollided, Direction direction) { }

		// gets x location taking into account map camera position
		public float GetCalibratedXLocation()
		{
			if (map != null)
			{
				return x.Round() - map.GetCamera().X.Round();
			}
			else
			{
				return GetX();
			}
		}

		// gets y location taking into account map camera position
		public float GetCalibratedYLocation()
		{
			if (map != null)
			{
				return y.Round() - map.GetCamera().Y.Round();
			}
			else
			{
				return GetY();
			}
		}

		// gets scaled bounds taking into account map camera position
		public Rectangle GetCalibratedScaledBounds()
		{
			if (map != null)
			{
				Rectangle scaledBounds = GetScaledBounds();
				return new Rectangle(
					scaledBounds.GetX1().Round() - map.GetCamera().X.Round(),
					scaledBounds.GetY1().Round() - map.GetCamera().Y.Round(),
					scaledBounds.GetScaledWidth(),
					scaledBounds.GetScaledHeight()
				);
			}
			else
			{
				return GetScaledBounds();
			}
		}

		// set this game object's map to make it a "part of" the map, allowing calibrated positions and collision handling logic to work
		public void SetMap(Map map)
		{
			this.map = map;
		}

		public override void Draw(GraphicsHandler graphicsHandler)
		{
			if (map != null)
			{
				graphicsHandler.DrawImage(
					currentFrame.Image,
					new Vector2(
						GetCalibratedXLocation().Round(),
						GetCalibratedYLocation().Round()
					),
					spriteEffects: currentFrame.SpriteEffect,
					scale: new Vector2(currentFrame.Scale, currentFrame.Scale)
				);
			}
			else
			{
				base.Draw(graphicsHandler);
			}
		}

		public override void DrawBounds(GraphicsHandler graphicsHandler, Color color)
		{
			if (map != null)
			{
				Rectangle scaledCalibratedBounds = GetCalibratedScaledBounds();
				scaledCalibratedBounds.Color = color;
				scaledCalibratedBounds.Draw(graphicsHandler);
			}
			else
			{
				base.DrawBounds(graphicsHandler, color);
			}
		}
	}
}
