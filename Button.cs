using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Explore_Your_Smth
{
    public class Button
    {
        #region Button Parameters
        private int X_Offset;
        private int Y_Offset;
        private int Width;
        private int Height;
        #endregion

        private Texture2D Sprite;

        #region Mouse Info
        private MouseState MouseInfo;
        private MouseState PreviousMouseInfo;
        private bool _isHovering;
        public bool IsReact { get; set; }

        //public bool Clicked { get; private set; }
        public event EventHandler Click;
        #endregion

        public Rectangle Rectangle { get; set; }
        private Vector2 Position;

        #region OnButton Text
        public Color PenColour { get; set; }
        private SpriteFont Font;
        public string Text { get; set; }
        #endregion


        public Button(SpriteFont font, string text, Texture2D texture, int width, int height, int xOffset, int yOffset)
        {
            Font = font;
            Text = text;
            Sprite = texture;
            Width = width;
            Height = height;
            X_Offset = xOffset;
            Y_Offset = yOffset;
            IsReact = true;
            PenColour = Color.White;
        }

        //public void Draw(GameTime gameTime, SpriteBatch spriteBatch
        public void Draw(SpriteBatch spriteBatch)
        {
            var color = Color.DarkMagenta;
            if (_isHovering && IsReact)
                color = Color.Black;
            spriteBatch.Draw(Sprite, Rectangle, color);
            var penColor = Color.White;
            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2 )) - (Font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (Font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(Font, Text, new Vector2(x, y), penColor);//, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 1f);
            }
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager _graphics)
        {
            Position = new Vector2( 
                (_graphics.PreferredBackBufferWidth - Width) / 2 + X_Offset, 
                (_graphics.PreferredBackBufferHeight - Height) / 2 + Y_Offset);

            Rectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            PreviousMouseInfo = MouseInfo;
            MouseInfo = Mouse.GetState();

            var mouseRectangle = new Rectangle(MouseInfo.X, MouseInfo.Y, 1, 1);

            _isHovering = false;
            if (Rectangle.Intersects(mouseRectangle))
            {
                _isHovering = true;
                if (MouseInfo.LeftButton == ButtonState.Released && PreviousMouseInfo.LeftButton == ButtonState.Pressed)
                    Click?.Invoke(this, new EventArgs());
            }
        }

    }
}
