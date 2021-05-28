using GameEngineTest.Builders;
using GameEngineTest.Engine;
using GameEngineTest.GameObject;
using GameEngineTest.Level;
using GameEngineTest.Utils;
using System;
using System.Collections.Generic;
using System.Text;

// This class is for the end level gold box tile
// when the player touches it, it will tell the player that the level has been completed
namespace GameEngineTest.EnchancedMapTiles
{
    public class EndLevelBox : EnhancedMapTile
    {
        public EndLevelBox(Point location)
            : base(location.X, location.Y, new SpriteSheet(Screen.ContentManager.LoadTexture("GoldBox.png"), 16, 16), "DEFAULT", TileType.PASSABLE)
        {
        }

        public override void Update(Player player)
        {
            base.Update(player);
            if (Intersects(player))
            {
                player.CompleteLevel();
            }
        }

        public override Dictionary<string, Frame[]> GetAnimations(SpriteSheet spriteSheet)
        {
            Dictionary<string, Frame[]>  animations = new Dictionary<string, Frame[]>();
            animations.Add("DEFAULT", new Frame[] {
                new FrameBuilder(spriteSheet.GetSprite(0, 0), 500)
                    .WithScale(3)
                    .WithBounds(1, 1, 14, 14)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(0, 1), 500)
                    .WithScale(3)
                    .WithBounds(1, 1, 14, 14)
                    .Build(),
                new FrameBuilder(spriteSheet.GetSprite(0, 2), 500)
                    .WithScale(3)
                    .WithBounds(1, 1, 14, 14)
                    .Build()
            });
            return animations;
        }
    };
}
