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
    public static class ExtraHearts
    {
        public static Texture2D Sprite { get; set; }
        public static TiledLayer PositionsLayer;

        public static void Draw(SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {
            var x_offset = (_graphics.PreferredBackBufferWidth - Map.MapWidth) / 2;
            var y_offset = (_graphics.PreferredBackBufferHeight - Map.MapHeight) / 2;
            for (var i = 0; i < PositionsLayer.objects.Length; i++)
            {
                if (PositionsLayer.objects[i].name != "collected")
                {
                    var e = PositionsLayer.objects[i];
                    var x = (int)e.x + x_offset;
                    var y = (int)e.y + y_offset;
                    _spriteBatch.Draw(Sprite, new Rectangle(x, y + 1, Sprite.Width, Sprite.Height), Color.White);
                }
            }    
        }          
    }
}
