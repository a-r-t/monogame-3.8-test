using GameEngineTest.Engine;
using GameEngineTest.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

/*
	Represents an animated sprite
	Animations can either be passed in directly or loaded automatically in a subclass by overriding the getAnimations method
	This class contains logic for transitioning animations as well as playing out the frames in an animation in a loop
	Subclasses need to call down to this class's update method in order for animation logic to be performed
	While this calls does not extend from Sprite, it is set up in a way where it is still treated by other classes as if it is a singular sprite (based on value of currentFrame)
*/

namespace GameEngineTest.GameObjects
{
    public class AnimatedSprite : IntersectableRectangle
    {
		// location of entity
		protected float x, y;

		// maps animation name to an array of Frames representing one animation
		protected Dictionary<string, Frame[]> animations;

		// keeps track of current animation the sprite is using
		protected string currentAnimationName = "";
		protected string previousAnimationName = "";

		// keeps track of current frame number in an animation the sprite is using
		protected int currentFrameIndex;

		// if an animation has looped, this is set to true
		protected bool hasAnimationLooped;

		// current Frame object the animation is using based on currentAnimationName and currentFrameIndex
		// this is essential for the class, as it uses this to be treated as "one sprite"
		protected Frame currentFrame;

		// times frame delay before transitioning into the next frame of an animation
		private Stopwatch frameTimer = new Stopwatch();

		public AnimatedSprite(SpriteSheet spriteSheet, float x, float y, string startingAnimationName)
		{
			this.x = x;
			this.y = y;
			this.animations = GetAnimations(spriteSheet);
			this.currentAnimationName = startingAnimationName;
			UpdateCurrentFrame();
		}

		public AnimatedSprite(float x, float y, Dictionary<string, Frame[]> animations, string startingAnimationName)
		{
			this.x = x;
			this.y = y;
			this.animations = animations;
			this.currentAnimationName = startingAnimationName;
			UpdateCurrentFrame();
		}

		public AnimatedSprite(Texture2D image, float x, float y, string startingAnimationName)
		{
			this.x = x;
			this.y = y;
			SpriteSheet spriteSheet = new SpriteSheet(image, image.Width, image.Height);
			this.animations = GetAnimations(spriteSheet);
			this.currentAnimationName = startingAnimationName;
			UpdateCurrentFrame();
		}

		public AnimatedSprite(float x, float y)
		{
			this.x = x;
			this.y = y;
			this.animations = new Dictionary<string, Frame[]>();
			this.currentAnimationName = "";
		}

		public virtual void Initialize() { }

		public virtual void Update()
		{
			// if animation name has been changed (previous no longer equals current), setup for the new animation and start using it
			if (!previousAnimationName.Equals(currentAnimationName))
			{
				currentFrameIndex = 0;
				UpdateCurrentFrame();
				frameTimer.SetWaitTime(GetCurrentFrame().GetDelay());
				hasAnimationLooped = false;
			}
			else
			{
				// if animation has more than one frame, check if it's time to transition to a new frame based on that frame's delay
				if (GetCurrentAnimation().Length > 1 && currentFrame.GetDelay() > 0)
				{

					// if enough time has passed based on current frame's delay and it's time to transition to a new frame,
					// update frame index to the next frame
					// It will also wrap around back to the first frame index if it was already on the last frame index (the animation will loop)
					if (frameTimer.IsTimeUp())
					{
						currentFrameIndex++;
						if (currentFrameIndex >= animations[currentAnimationName].Length)
						{
							currentFrameIndex = 0;
							hasAnimationLooped = true;
						}
						frameTimer.SetWaitTime(GetCurrentFrame().GetDelay());
						UpdateCurrentFrame();
					}
				}
			}
			previousAnimationName = currentAnimationName;
		}

		// Subclasses can override this method in order to add their own animations, which will be loaded in at initialization time
		public virtual Dictionary<string, Frame[]> GetAnimations(SpriteSheet spriteSheet)
		{
			return null;
		}

		// currentFrame is essentially a sprite, so each game loop cycle
		// the sprite needs to have its current state updated based on animation logic,
		// and location updated to match any changes to the animated sprite class
		protected void UpdateCurrentFrame()
		{
			currentFrame = GetCurrentFrame();
			currentFrame.X = x;
			currentFrame.Y = y;
		}

		// gets the frame from current animation that the animated sprite class is currently using
		protected Frame GetCurrentFrame()
		{
			return animations[currentAnimationName][currentFrameIndex];
		}

		// gets the animation that the animated sprite class is currently using
		protected Frame[] GetCurrentAnimation() 
		{ 
			return animations[currentAnimationName]; 
		}

		public virtual void Draw(GraphicsHandler graphicsHandler)
		{
			currentFrame.Draw(graphicsHandler);
		}

		public virtual void DrawBounds(GraphicsHandler graphicsHandler, Color color)
		{
			currentFrame.DrawBounds(graphicsHandler, color);
		}

		public float GetX() { return currentFrame.X; }
		public float GetY() { return currentFrame.Y; }
		public float GetX1() { return currentFrame.GetX1(); }
		public float GetY1() { return currentFrame.GetY1(); }
		public float GetX2() { return currentFrame.GetX2(); }
		public float GetScaledX2() { return currentFrame.GetScaledX2(); }
		public float GetY2() { return currentFrame.GetY2(); }
		public float GetScaledY2() { return currentFrame.GetScaledY2(); }

		public void SetX(float x)
		{
			this.x = x;
			currentFrame.X = x;
		}

		public void SetY(float y)
		{
			this.y = y;
			currentFrame.Y = y;
		}

		public void SetLocation(float x, float y)
		{
			this.SetX(x);
			this.SetY(y);
		}

		public void MoveX(float dx)
		{
			this.x += dx;
			currentFrame.MoveX(dx);
		}

		public void MoveRight(float dx)
		{
			this.x += dx;
			currentFrame.MoveRight(dx);
		}

		public void MoveLeft(float dx)
		{
			this.x -= dx;
			currentFrame.MoveLeft(dx);
		}

		public void MoveY(float dy)
		{
			this.y += dy;
			currentFrame.MoveY(dy);
		}

		public void MoveDown(float dy)
		{
			this.y += dy;
			currentFrame.MoveDown(dy);
		}

		public void MoveUp(float dy)
		{
			this.y -= dy;
			currentFrame.MoveUp(dy);
		}

		public float GetScale()
		{
			return currentFrame.Scale;
		}

		public void SetScale(float scale)
		{
			currentFrame.Scale = scale;
		}

		public int GetWidth()
		{
			return currentFrame.Width;
		}
		public int GetHeight()
		{
			return currentFrame.Height;
		}
		public void SetWidth(int width)
		{
			currentFrame.Width = width;
		}
		public void SetHeight(int height)
		{
			currentFrame.Height = height;
		}
		public int GetScaledWidth()
		{
			return currentFrame.GetScaledWidth();
		}
		public int GetScaledHeight()
		{
			return currentFrame.GetScaledHeight();
		}

		public RectangleGraphic GetBounds()
		{
			return currentFrame.GetBounds();
		}

		public RectangleGraphic GetScaledBounds()
		{
			return currentFrame.GetScaledBounds();
		}

		public float GetBoundsX1()
		{
			return currentFrame.GetBoundsX1();
		}

		public float GetScaledBoundsX1()
		{
			return currentFrame.GetScaledBoundsX1();
		}

		public float GetBoundsX2()
		{
			return currentFrame.GetBoundsX2();
		}

		public float GetScaledBoundsX2()
		{
			return currentFrame.GetScaledBoundsX2();
		}

		public float GetBoundsY1()
		{
			return currentFrame.GetBoundsY1();
		}

		public float GetScaledBoundsY1()
		{
			return currentFrame.GetScaledBoundsY1();
		}

		public float GetBoundsY2()
		{
			return currentFrame.GetBoundsY2();
		}

		public float GetScaledBoundsY2()
		{
			return currentFrame.GetScaledBoundsY2();
		}

		public void SetBounds(RectangleGraphic bounds)
		{
			currentFrame.SetBounds(bounds);
		}

		public RectangleGraphic GetIntersectRectangle()
		{
			return currentFrame.GetIntersectRectangle();
		}

		public bool Intersects(IntersectableRectangle other)
		{
			return currentFrame.Intersects(other);
		}

		public bool Overlaps(IntersectableRectangle other) 
		{ 
			return currentFrame.Overlaps(other);
		}

		public override string ToString()
		{
			return string.Format("Current Sprite: x=%s y=%s width=%s height=%s bounds=(%s, %s, %s, %s)", x, y, GetScaledWidth(), GetScaledHeight(), GetScaledBoundsX1(), GetScaledBoundsY1(), GetScaledBounds().Width, GetScaledBounds().Height);
		}

	}
}
