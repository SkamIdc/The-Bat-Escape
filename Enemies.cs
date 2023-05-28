using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using TiledCS;
using System.Linq;
using System;

namespace Explore_Your_Smth
{
    public static class Enemies
    {
        public static TiledLayer PositionsLayer;

        public static Texture2D Sprite;

        private static int Frame = 0;
        private static float AnimationElapsed;
        private static float AnimationDelay = 150f;

        public static void Update(GameTime gameTime)
        {
            AnimationElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (AnimationElapsed >= AnimationDelay)
            {
                Frame = (Frame + 1) % 4;
                AnimationElapsed = 0;
            }
        }

        public static void Draw(SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics, Hero Bat)
        {

            var x_offset = (_graphics.PreferredBackBufferWidth - Map.MapWidth) / 2;
            var y_offset = (_graphics.PreferredBackBufferHeight - Map.MapHeight) / 2;
            for (var i = 0; i < PositionsLayer.objects.Length; i++)
            {
                if (PositionsLayer.objects[i].name != "dead")
                {
                    var e = PositionsLayer.objects[i];
                    var x = (int)e.x + x_offset;
                    var y = (int)e.y + y_offset;

                    var effect = SpriteEffects.None;
                    if (Bat.Position.X < x)
                        effect = SpriteEffects.FlipHorizontally;

                    switch (Frame)
                    {
                        case 0:
                            _spriteBatch.Draw(Sprite, new Rectangle(x - 10, y - 4, 20, 20), new Rectangle(0, 0, 24, 20), Color.White, 0f, new Vector2(0, 0), effect, 0);
                            break;
                        case 1:
                            _spriteBatch.Draw(Sprite, new Rectangle(x - 10, y - 4, 20, 20), new Rectangle(24, 0, 26, 20), Color.White, 0f, new Vector2(0, 0), effect, 0);
                            break;
                        case 2:
                            _spriteBatch.Draw(Sprite, new Rectangle(x - 10, y - 4, 20, 20), new Rectangle(50, 0, 20, 20), Color.White, 0f, new Vector2(0, 0), effect, 0);
                            break;
                        case 3:
                            _spriteBatch.Draw(Sprite, new Rectangle(x - 10, y - 4, 20, 20), new Rectangle(70, 0, 26, 20), Color.White, 0f, new Vector2(0, 0), effect, 0);
                            break;
                    }
                }
            }
        }
    }
}

