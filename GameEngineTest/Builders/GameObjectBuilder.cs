using GameEngineTest.GameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Builder class to instantiate a GameObject class
namespace GameEngineTest.Builders
{
    public class GameObjectBuilder
    {
        protected Dictionary<string, Frame[]> animations = new Dictionary<string, Frame[]>();
        protected string startingAnimationName = "DEFAULT";

        public GameObjectBuilder() { }

        public GameObjectBuilder(Frame frame)
        {
            AddDefaultAnimation(frame);
        }

        public GameObjectBuilder(Frame[] frames)
        {
            AddDefaultAnimation(frames);
        }

        public GameObjectBuilder AddAnimation(string animationName, Frame[] frames)
        {
            animations.Add(animationName, frames);
            return this;
        }

        public GameObjectBuilder AddDefaultAnimation(Frame[] frames)
        {
            animations.Add("DEFAULT", frames);
            return this;
        }

        public GameObjectBuilder AddAnimation(string animationName, Frame frame)
        {
            animations.Add(animationName, new Frame[] { frame });
            return this;
        }

        public GameObjectBuilder AddDefaultAnimation(Frame frame)
        {
            animations.Add("DEFAULT", new Frame[] { frame });
            return this;
        }

        public GameObjectBuilder WithStartingAnimation(string startingAnimationName)
        {
            this.startingAnimationName = startingAnimationName;
            return this;
        }

        public Dictionary<string, Frame[]> CloneAnimations()
        {
            Dictionary<string, Frame[]> animationsCopy = new Dictionary<string, Frame[]>();
            foreach (string key in animations.Keys)
            {
                Frame[] frames = animations[key];
                animationsCopy.Add(key, frames.Select(frame => frame.Copy()).ToArray());
            }
            return animationsCopy;
        }

        public virtual GameObject.GameObject Build(float x, float y)
        {
            return new GameObject.GameObject(x, y, CloneAnimations(), startingAnimationName);
        }
    }
}
