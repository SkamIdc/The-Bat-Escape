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
    public class Hero
    {
        public Rectangle Position;

        public Texture2D Sprite { get; set; }
        public Texture2D Sprite_Right { get; set; }
        public Texture2D Sprite_Left { get; set; }


        public Texture2D HealthSprite;
        public Texture2D Health_0 { get; set; }
        public Texture2D Health_1 { get; set; }
        public Texture2D Health_2 { get; set; }
        public Texture2D Health_3 { get; set; }
        public Texture2D Health_4 { get; set; }


        public Texture2D AttackSprite;
        public Texture2D VerticalAttackSprite { get; set; }
        public Texture2D HorizontalAttackSprite { get; set; }

        public TiledLayer collisionLayer;
        public TiledLayer start;
        public TiledLayer finishBox;


        private Rectangle JustAhead;


        public int KillsGoal = 8;
        public Rectangle FinalExit;
        public int KillsCount = 0;

        public int Health;
        public float Velocity;
        public float Speed = 2;
        public Direction LastDirection = Direction.stay;


        public bool IsShooting = false;


        public SoundEffect AttackSound;
        public int AttackLimit = 15;
        public int AttackCount;
        public Rectangle AttackRect;
        public int AttackSpeed;
        public Direction AttackDirection;

        public int AttackFrames = 0;
        public float AttackElapsed;
        public float AttackAnimationDelay = 30f;

        public int Frame = 0;
        public float AnimationElapsed;
        public float AnimationDelay = 100f;

        public bool IsDead;
        public bool IsVisible;

        public bool Finished = false;


        public float Velocity_Save;
        public int AttackSpeed_Save;

        public bool Crashed;

        public Hero()
        {
            Sprite = Sprite_Right;
            AttackSprite = Sprite_Left;
        }

        public void LoadSceneCollision(TiledMap map, int CurrentScene, GraphicsDeviceManager _graphics)
        {
            IsShooting = false;
            LastDirection = Direction.stay;
            if (Game1.CurrentScene != 0)
            {
                IsVisible = true;
                start = map.Layers.First(l => l.name == "start");
                collisionLayer = map.Layers.First(l => l.name == "collision");
                finishBox = map.Layers.First(l => l.name == "finish");

                var noWayBack = start.objects[1];
                JustAhead = new Rectangle((int)noWayBack.x, (int)noWayBack.y, (int)noWayBack.width, (int)noWayBack.height);
                SetStartPosition(_graphics);

                if ((Scenes)CurrentScene == Scenes.final)
                {
                    var exit = start.objects[2];
                    FinalExit = new Rectangle((int)exit.x, (int)exit.y, (int)exit.width, (int)exit.height);
                }
            }
            else IsVisible = false;
        }

        private void SetStartPosition(GraphicsDeviceManager _graphics)
        {
            var startPoint = start.objects[0];

            Position = new Rectangle((int)startPoint.x + (Game1.WindowWidth - Map.MapWidth) / 2, (int)startPoint.y + (Game1.WindowHeight - Map.MapHeight) / 2, 20, 20);

            Velocity = 0;
            AttackCount = 0;
        }

        public void Reset(int CurrentScene, GraphicsDeviceManager _graphics, int StartLevel)
        {
            if (StartLevel != 0 || CurrentScene == 1 || CurrentScene == 5)
            {
                Health = 3;
                for (var i = 0; i < ExtraHearts.PositionsLayer.objects.Length; i++)
                    ExtraHearts.PositionsLayer.objects[i].name = "";
            }
            else Health -= 1;

            for (var i = 0; i < Enemies.PositionsLayer.objects.Length; i++)
                Enemies.PositionsLayer.objects[i].name = "";
            LastDirection = Direction.stay;
            Sprite = Sprite_Right;
            Crashed = false;
            IsDead = false;
            KillsCount = 0;
            Game1.WallIsCollapsed = false;
            SetStartPosition(_graphics);
        }

        private void Update_Shooting()
        {
            #region Shooting

            if (LastDirection != Direction.stay && !IsShooting && AttackCount < AttackLimit && Keyboard.GetState().IsKeyDown(Keys.Space) && !Crashed && !Game1.GameIsPaused)
            {
                AttackCount += 1;
                AttackSound.Play();
                IsShooting = true;
                AttackRect = new Rectangle(Position.X, Position.Y, 20, 20);
                AttackSpeed = (int)Velocity * 3;
                if (AttackSpeed == 0) AttackSpeed = (int)Speed * 3;
                AttackDirection = LastDirection;
            }

            if (IsShooting)
            {
                if (AttackDirection == Direction.left || AttackDirection == Direction.right) AttackRect.X += AttackSpeed;
                else AttackRect.Y += AttackSpeed;
            }

            #endregion
        }

        public void Update(GraphicsDeviceManager _graphics, GameTime gameTime, bool ShowText)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.R) && Crashed && !IsDead)
                Reset(Game1.CurrentScene, _graphics, Game1.StartLevel_menu);

            CorrectPosition(_graphics);
            Move(gameTime);
            Update_Shooting();

            CheckCollision(_graphics);

        }
        private void CorrectPosition(GraphicsDeviceManager _graphics)
        {
            Position.X += (_graphics.PreferredBackBufferWidth - Game1.WindowWidth) / 2;
            Position.Y += (_graphics.PreferredBackBufferHeight - Game1.WindowHeight) / 2;
            if (IsShooting)
            {
                AttackRect.X += (_graphics.PreferredBackBufferWidth - Game1.WindowWidth) / 2;
                AttackRect.Y += (_graphics.PreferredBackBufferHeight - Game1.WindowHeight) / 2;
            }
        }

        private void Move(GameTime gameTime)
        {
            var ks = Keyboard.GetState();
            if (LastDirection == Direction.left || LastDirection == Direction.right)
                Position.X += (int)Velocity;
            else
                Position.Y += (int)Velocity;

            if (!Crashed && !Game1.IsInGameMenuVisible && !Game1.ShowStoryText)
            {
                #region Animation

                var animationDelay = AnimationDelay / (Speed) + 50;
                AnimationElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (AnimationElapsed >= animationDelay)
                {
                    Frame = (Frame + 1) % 4;
                    AnimationElapsed = 0;
                }
                #endregion

                #region Movement

                if ((ks.IsKeyDown(Keys.D) || ks.IsKeyDown(Keys.Right)) && (LastDirection != Direction.left || LastDirection == Direction.stay))
                {
                    Sprite = Sprite_Right;
                    Velocity = Speed;
                    LastDirection = Direction.right;
                }
                if ((ks.IsKeyDown(Keys.A) || ks.IsKeyDown(Keys.Left)) && (LastDirection != Direction.right || LastDirection == Direction.stay))
                {
                    Sprite = Sprite_Left;
                    Velocity = -Speed;
                    LastDirection = Direction.left;
                }
                if ((ks.IsKeyDown(Keys.S) || ks.IsKeyDown(Keys.Down)) && (LastDirection != Direction.up || LastDirection == Direction.stay))
                {
                    Velocity = Speed;
                    LastDirection = Direction.down;
                }
                if ((ks.IsKeyDown(Keys.W) || ks.IsKeyDown(Keys.Up)) && (LastDirection != Direction.down || LastDirection == Direction.stay))
                {
                    Velocity = -Speed;
                    LastDirection = Direction.up;
                }
                #endregion

            }
        }

        private void CheckCollision(GraphicsDeviceManager _graphics)
        {
            var x_offset = (_graphics.PreferredBackBufferWidth - Map.MapWidth) / 2;
            var y_offset = (_graphics.PreferredBackBufferHeight - Map.MapHeight) / 2;

            #region finish line

            foreach (var a in finishBox.objects)
            {
                var x = (int)a.x + x_offset;
                var y = (int)a.y + y_offset;
                var vect = new Rectangle(x, y, (int)a.width, (int)a.height);
                if (vect.Intersects(Position))
                {
                    Finished = true;
                }
                if (vect.Intersects(AttackRect))
                {
                    IsShooting = false;
                }
            }
            #endregion

            #region enemys

            for (var i = 0; i < Enemies.PositionsLayer.objects.Length; i++)
            {
                if (Enemies.PositionsLayer.objects[i].name != "dead")
                {
                    var e = Enemies.PositionsLayer.objects[i];
                    var x = (int)e.x + x_offset;
                    var y = (int)e.y + y_offset;
                    var vect = new Rectangle(x, y + 1, 14, 14);
                    if (vect.Intersects(AttackRect) || AttackRect.Intersects(vect))
                    {
                        Enemies.PositionsLayer.objects[i].name = "dead";
                        IsShooting = false;
                        AttackRect = new Rectangle(0, 0, 0, 0);
                        if ((Scenes)Game1.CurrentScene == Scenes.final) KillsCount += 1;
                    }
                    if (vect.Intersects(Position) && Enemies.PositionsLayer.objects[i].name != "dead")
                    {
                        this.Crashed = true;
                        Velocity = 0;
                        if (Health - 1 == 0)
                            IsDead = true;
                    }
                }
            }
            #endregion

            #region extra hearts

            for (var i = 0; i < ExtraHearts.PositionsLayer.objects.Length; i++)
            {
                if (ExtraHearts.PositionsLayer.objects[i].name != "collected")
                {
                    var e = ExtraHearts.PositionsLayer.objects[i];
                    var x = (int)e.x + x_offset;
                    var y = (int)e.y + y_offset;
                    var vect = new Rectangle(x, y + 1, 14, 14);
                    if (vect.Intersects(Position))
                    {
                        ExtraHearts.PositionsLayer.objects[i].name = "collected";
                        Health = Math.Clamp(Health + 1, 0, 4);

                    }
                }
            }
            #endregion

            #region Hero & Attack Hitbox Correction

            var heroCollision = Position;
            heroCollision.Y += 3;
            heroCollision.Height -= 6;

            var attackCollision = AttackRect;
            attackCollision.Height -= 10;
            #endregion

            #region map collision

            foreach (var a in collisionLayer.objects)
            {
                var x = (int)a.x + x_offset;
                var y = (int)a.y + y_offset;
                var vect = new Rectangle(x, y, (int)a.width, (int)a.height);
                if (vect.Intersects(heroCollision))
                {
                    Crashed = true;
                    Velocity = 0;
                    if (Health - 1 == 0)
                        IsDead = true;
                }
                if (vect.Intersects(attackCollision))
                {
                    IsShooting = false;
                }
            }
            #endregion

            #region extra collision

            var noWayBack = JustAhead;
            noWayBack.X += x_offset;
            noWayBack.Y += y_offset;

            if (noWayBack.Intersects(Position))
            {
                if (LastDirection == Direction.left || LastDirection == Direction.right) Position.X += -(int)Velocity * 2;
                else Position.Y += -(int)Velocity * 2;
                Velocity = 0;
            }
            if (noWayBack.Intersects(AttackRect))
            {
                IsShooting = false;
            }
            if ((Scenes)Game1.CurrentScene == Scenes.final)
            {
                var exit = FinalExit;
                exit.X += x_offset;
                exit.Y += y_offset;
                if (exit.Intersects(Position) && KillsCount != KillsGoal)
                {
                    Crashed = true;
                    Velocity = 0;
                    if (Health - 1 == 0)
                        IsDead = true;
                }
            }
            #endregion
        }

        public void Draw(SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics, GameTime gameTime)
        {
            #region Hero + Health

            if (IsVisible)
            {
                switch (Frame)
                {
                    case 0:
                        _spriteBatch.Draw(Sprite, Position, new Rectangle(0, 0, 24, 20), Color.White);
                        break;
                    case 1:
                        _spriteBatch.Draw(Sprite, Position, new Rectangle(24, 0, 26, 20), Color.White);
                        break;
                    case 2:
                        _spriteBatch.Draw(Sprite, Position, new Rectangle(50, 0, 20, 20), Color.White);
                        break;
                    case 3:
                        _spriteBatch.Draw(Sprite, Position, new Rectangle(70, 0, 26, 20), Color.White);
                        break;
                }
                var health = Health;
                if (Crashed) health -= 1;
                var x = (_graphics.PreferredBackBufferWidth - Map.MapWidth + 88) / 2;
                var y = (_graphics.PreferredBackBufferHeight - Map.MapHeight + 22) / 2;
                switch (health)
                {
                    case 0:
                        HealthSprite = Health_0;
                        break;
                    case 1:
                        HealthSprite = Health_1;
                        break;
                    case 2:
                        HealthSprite = Health_2;
                        break;
                    case 3:
                        HealthSprite = Health_3;
                        break;
                    case 4:
                        x -= 16;
                        HealthSprite = Health_4;
                        break;

                }

                _spriteBatch.Draw(
                    HealthSprite,
                    new Rectangle(
                        x, y,
                        HealthSprite.Width, HealthSprite.Height),
                    Color.White);
            }
            #endregion
            DrawAttack(gameTime, _spriteBatch);
        }

        private void DrawAttack(GameTime gameTime, SpriteBatch _spriteBatch)
        {
            #region Shooting

            if (IsShooting)
            {
                #region Animation

                AttackElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (AttackElapsed >= AttackAnimationDelay)
                {
                    AttackFrames = (AttackFrames + 1) / 9;
                    AttackElapsed = 0;
                }
                #endregion

                #region Sprite

                if (AttackDirection == Direction.left || AttackDirection == Direction.right)
                {
                    AttackSprite = HorizontalAttackSprite;
                    var sourceRect = new Rectangle(32 * AttackFrames, 0, 16, 32);
                    if (AttackSpeed < 0)
                        _spriteBatch.Draw(AttackSprite, AttackRect, sourceRect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
                    else
                        _spriteBatch.Draw(AttackSprite, AttackRect, sourceRect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                else
                {
                    AttackSprite = VerticalAttackSprite;
                    var sourceRect = new Rectangle(0, 32 * AttackFrames, 32, 16);
                    if (AttackSpeed > 0)
                        _spriteBatch.Draw(AttackSprite, AttackRect, sourceRect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.FlipVertically, 0);
                    else
                        _spriteBatch.Draw(AttackSprite, AttackRect, sourceRect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                #endregion
            }
            #endregion
        }
    }
}
