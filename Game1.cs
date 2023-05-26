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
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        #region Sound

        Song MainSong;
        SoundEffect AttackSound;
        #endregion

        #region Hero

        public Rectangle heroPos;

        private Texture2D heroSprite;
        private Texture2D hero_Right;
        private Texture2D hero_Left;

        private float Speed = 2;
        private float Velocity = 0;
        private float Velocity_Save;

        int HeroFrames = 0;
        float HeroElapsed;
        float HeroAnimationDelay = 100f;

        private bool IsHeroVisible;
        private bool IsHorizontal;
        private bool GameIsOver = false;

        private int HeroHealth = 3;

        private Texture2D HealthSprite;
        private Texture2D Health_0;
        private Texture2D Health_1;
        private Texture2D Health_2;
        private Texture2D Health_3;
        private Texture2D Health_4;


        #region Shooting

        private Rectangle HeroAttackRect;
        private int AttackSpeed;
        private int AttackSpeed_Save;
        private bool AttackIsHorizontal;
        private bool IsShooting;

        private int AttackCount;
        readonly int AttackLimit = 15;

        private Texture2D HeroAttack;
        Texture2D HorizontalAttack;
        Texture2D VerticalAttack;

        int AttackFrames = 0;
        float AttackElapsed;
        float AttackAnimationDelay = 30f;
        #endregion

        #endregion

        #region Menu Buttons

        List<Button> MenuButtons = new List<Button>();
        Button PlayButton;

        Button MinusStartLevelButton;
        Button PlusStartLevelButton;
        Button ShowStartLevelButton;

        Button MinusSpeedButton;
        Button ShowSpeedButton;
        Button PlusSpeedButton;

        Button MinusSongVolumeButton;
        Button PlusSongVolumeButton;
        Button ShowSongVolumeButton;

        Button MinusSoundEffButton;
        Button PlusSoundEffButton;
        Button ShowSoundEffVolumeButton;


        Button QuitButton;
        bool IsMenuVisible;
        #endregion

        #region InGame Buttons

        List<Button> InGameButtons = new List<Button>();
        Button BackToMenuButton;
        Button ContinueButton;
        bool IsInGameMenuVisible;
        #endregion

        #region After Death Menu

        List<Button> DeadMenu = new List<Button>();
        Button RestartButton;

        public bool HeroIsDead;
        #endregion

        #region images (backs')

        private Texture2D ForestImage;
        private Texture2D ForestImage_back;
        private Texture2D LevelBackground;
        private Texture2D MenuImage;
        #endregion

        #region params for final

        private int FinalGoal;
        private Rectangle FinalExit;
        private int KillsCount = 0;
        #endregion

        #region Map
        const int MapWidth = 1280;
        const int MapHeight = 720;

        public string ScenePath;
        public int CurrentScene = 0;

        private int StartLevel;
        private TiledMap map;
        private Dictionary<int, TiledTileset> tilesets;
        private Texture2D tilesetTexture;
        private TiledLayer collisionLayer;
        private TiledLayer start;
        private TiledLayer finishBox;
        private TiledLayer enemys;
        private TiledLayer hearts;

        private bool GameIsPaused;
        private Rectangle JustAhead;
        #endregion

        #region Enemy

        private Texture2D enemysSprite;
        int EnemysFrames = 0;
        float EnemyElapsed;
        float EnemyAnimationDelay = 150f;
        #endregion

        #region extra hearts

        private Texture2D heartSprtie;
        #endregion

        private int WindowWidth = 1920;
        private int WindowHeight = 1080;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.Title = "The Bat Escape";
        }


        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = MapWidth;
            _graphics.PreferredBackBufferHeight = MapHeight;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadScene((Scenes)CurrentScene);

            heartSprtie = Content.Load<Texture2D>("extra_heart");
            enemysSprite = Content.Load<Texture2D>("enemy flying");

            #region Background music

            MainSong = Content.Load<Song>("IEROD1903678-inner-peace-1903678-320");
            MediaPlayer.Play(MainSong);
            MediaPlayer.Volume = 0.01f;
            SoundEffect.MasterVolume = 0.05f;
            MediaPlayer.IsRepeating = true;
            #endregion

            #region SoundEffects

            AttackSound = Content.Load<SoundEffect>("AttackSoundMP3");
            #endregion

            #region BackGrounds

            LevelBackground = Content.Load<Texture2D>("dark red back fog2");
            MenuImage = Content.Load<Texture2D>("MainMenuImage");
            ForestImage = Content.Load<Texture2D>("DarkForest");
            ForestImage_back = Content.Load<Texture2D>("DarkForest_back");
            #endregion

            #region Hero Health 

            Health_0 = Content.Load<Texture2D>("0_hearts");
            Health_1 = Content.Load<Texture2D>("1_hearts");
            Health_2 = Content.Load<Texture2D>("2_hearts");
            Health_3 = Content.Load<Texture2D>("3_hearts1");
            Health_4 = Content.Load<Texture2D>("4_hearts");

            HealthSprite = Health_3;
            #endregion

            #region Hero Texture

            hero_Right = Content.Load<Texture2D>("whiteBatMovingRight_2x");
            hero_Left = Content.Load<Texture2D>("whiteBatMovingLeft_2x");
            HorizontalAttack = Content.Load<Texture2D>("attack_left");
            VerticalAttack = Content.Load<Texture2D>("attack_up");

            heroSprite = hero_Right;
            #endregion

            #region Menu Buttons Creation

            #region Play & Quit
            PlayButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                //"Играть",
                "Play",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, -100);

            QuitButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                //"Выйти",
                "Quit",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, 100);


            PlayButton.Click += PlayButton_Action;
            QuitButton.Click += QuitButton_Action;

            MenuButtons.Add(PlayButton);
            MenuButtons.Add(QuitButton);
            #endregion

            #region Difficult

            PlusSpeedButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                "+",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                175, 0);

            MinusSpeedButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                "-",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                -175, 0);

            ShowSpeedButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                //"Настройки",
                "Settings",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, 0)
            { IsReact = false };

            PlusSpeedButton.Click += PlusSpeedButton_Action;
            MinusSpeedButton.Click += MinusSpeedButton_Action;

            MenuButtons.Add(PlusSpeedButton);
            MenuButtons.Add(MinusSpeedButton);
            MenuButtons.Add(ShowSpeedButton);
            #endregion

            #region Song volume 

            var volume_yOffset = -200;

            PlusSongVolumeButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                "+",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                175, volume_yOffset);

            MinusSongVolumeButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                "-",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                -175, volume_yOffset);

            ShowSongVolumeButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                //"Настройки",
                "SongVolume",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, volume_yOffset)
            { IsReact = false };


            MinusSongVolumeButton.Click += MinusSongVolumeButton_Action;
            PlusSongVolumeButton.Click += PlusSongVolumeButton_Action;

            MenuButtons.Add(PlusSongVolumeButton);
            MenuButtons.Add(MinusSongVolumeButton);
            MenuButtons.Add(ShowSongVolumeButton);


            #endregion

            #region Sound Effects volume

            var soundEffects_yOffset = -300;

            PlusSoundEffButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                "+",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                175, soundEffects_yOffset);

            MinusSoundEffButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                "-",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                -175, soundEffects_yOffset);

            ShowSoundEffVolumeButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                //"Настройки",
                "SongVolume",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, soundEffects_yOffset)
            { IsReact = false };


            MinusSoundEffButton.Click += MinusSoundEffButton_Action;
            PlusSoundEffButton.Click += PlusSoundEffButton_Action;

            MenuButtons.Add(MinusSoundEffButton);
            MenuButtons.Add(PlusSoundEffButton);
            MenuButtons.Add(ShowSoundEffVolumeButton);


            #endregion


            #region Choose start level 

            var startLevelBar_offset = 300;

            PlusStartLevelButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                ">",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                175, startLevelBar_offset);

            MinusStartLevelButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                "<",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                -175, startLevelBar_offset);

            ShowStartLevelButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                //"Настройки",
                "StartLevel",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, startLevelBar_offset)
            { IsReact = false };


            PlusStartLevelButton.Click += PlusStartLevelButton_Action;
            MinusStartLevelButton.Click += MinusStartLevelButton_Action;

            MenuButtons.Add(PlusStartLevelButton);
            MenuButtons.Add(MinusStartLevelButton);
            MenuButtons.Add(ShowStartLevelButton);


            #endregion

            #endregion

            #region InGame Buttons Creation

            ContinueButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                //"Продолжить",
                "Continue",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, -50);

            BackToMenuButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                //"Вернуться в меню",
                "Back To Menu",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, 50);

            ContinueButton.Click += ContinueButton_Action;
            BackToMenuButton.Click += BackToMenuButton_Action;
            InGameButtons.Add(ContinueButton);
            InGameButtons.Add(BackToMenuButton);

            InGameButtons.Add(ShowSoundEffVolumeButton);
            InGameButtons.Add(MinusSoundEffButton);
            InGameButtons.Add(PlusSoundEffButton);

            InGameButtons.Add(ShowSongVolumeButton);
            InGameButtons.Add(MinusSongVolumeButton);
            InGameButtons.Add(PlusSongVolumeButton);
            #endregion

            #region DeadMenu Buttons Creation

            RestartButton = new Button(
                Content.Load<SpriteFont>("Peace Sans"),
                //"Начать с начала",
                "Restart Game",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, -50);

            RestartButton.Click += RestartButton_Action;
            DeadMenu.Add(RestartButton);
            DeadMenu.Add(BackToMenuButton);
            #endregion
        }

        #region Menu Buttons Reaction

        #region Speed
        private void PlusSpeedButton_Action(object sender, System.EventArgs e)
        {
            Speed += 1;
            Speed = Math.Clamp(Speed, 1, 5);
        }

        private void MinusSpeedButton_Action(object sender, System.EventArgs e)
        {
            Speed -= 1;
            Speed = Math.Clamp(Speed, 1, 5);
        }
        #endregion

        #region Song Volume
        private void PlusSongVolumeButton_Action(object sender, System.EventArgs e)
        {
            MediaPlayer.Volume += 0.01f;
        }

        private void MinusSongVolumeButton_Action(object sender, System.EventArgs e)
        {
            MediaPlayer.Volume -= 0.01f;
        }
        #endregion

        #region Sound Effects Volume
        private void PlusSoundEffButton_Action(object sender, System.EventArgs e)
        {
            SoundEffect.MasterVolume = Math.Clamp(SoundEffect.MasterVolume + 0.01f, 0, 1);
            AttackSound.Play();
        }

        private void MinusSoundEffButton_Action(object sender, System.EventArgs e)
        {
            SoundEffect.MasterVolume = Math.Clamp(SoundEffect.MasterVolume - 0.01f, 0, 1);
            AttackSound.Play();
        }
        #endregion

        #region Play & Quit
        private void PlayButton_Action(object sender, System.EventArgs e)
        {
            if (StartLevel == 0)
                CurrentScene = 1;
            else CurrentScene = StartLevel;
            HeroHealth = 3;
            GameIsOver = false;
            HeroIsDead = false;
            IsShooting = false;
            GameIsPaused = false;
            LoadScene((Scenes)CurrentScene);
        }

        private void QuitButton_Action(object sender, System.EventArgs e)
        {
            Exit();
        }
        #endregion

        #region Start Level
        private void MinusStartLevelButton_Action(object sender, System.EventArgs e)
        {
            StartLevel -= 1;
            StartLevel = Math.Clamp(StartLevel, 0, 7);
        }

        private void PlusStartLevelButton_Action(object sender, System.EventArgs e)
        {
            StartLevel += 1;
            StartLevel = Math.Clamp(StartLevel, 0, 7);
        }
        #endregion

        #endregion

        #region InGame Buttuons Reaction

        private void ContinueButton_Action(object sender, System.EventArgs e)
        {
            AttackSpeed = AttackSpeed_Save;
            Velocity = Velocity_Save;
            IsInGameMenuVisible = false;
            GameIsPaused = false;
        }
        private void BackToMenuButton_Action(object sender, System.EventArgs e)
        {
            IsInGameMenuVisible = false;
            GameIsOver = false;
            HeroIsDead = false;
            IsShooting = false;
            GameIsPaused = false;
            CurrentScene = 0;
            LoadScene((Scenes)CurrentScene);
        }
        #endregion

        #region DeadMenu Buttons Reaction

        private void RestartButton_Action(object sender, System.EventArgs e)
        {
            Reset();
            GameIsOver = false;
            HeroHealth = 3;
            HeroIsDead = false;
            LoadScene((Scenes)CurrentScene);
        }
        #endregion

        private void LoadScene(Scenes scene)
        {
            #region Find ScenePath

            switch (scene)
            {
                case Scenes.menu:
                    ScenePath = "menu.tmx";
                    break;
                case Scenes.lvl_1:
                    ScenePath = "bloody One.tmx";
                    break;
                case Scenes.lvl_2:
                    ScenePath = "lvl_2.tmx";
                    break;
                case Scenes.lvl_3:
                    ScenePath = "lvl_3.tmx";
                    break;
                case Scenes.lvl_4:
                    ScenePath = "lvl_4.tmx";
                    break;
                case Scenes.lvl_5:
                    ScenePath = "lvl_5.tmx";
                    if (HeroHealth != 4)
                        HeroHealth = 3;
                    break;
                case Scenes.lvl_6:
                    ScenePath = "lvl_6.tmx";
                    break;
                case Scenes.lvl_7:
                    ScenePath = "lvl_7.tmx";
                    break;
                case Scenes.final:
                    ScenePath = "final.tmx";
                    break;
                case Scenes.victory:
                    ScenePath = "victory.tmx";
                    break;
            }
            #endregion

            #region Load Level Settings

            if (scene == Scenes.menu)
                IsHeroVisible = false;
            else IsHeroVisible = true;

            map = new TiledMap("Content\\" + ScenePath);
            tilesets = map.GetTiledTilesets("Content\\tilesets/");
            IsShooting = false;
            if (CurrentScene != 0)
            {
                start = map.Layers.First(l => l.name == "start");
                collisionLayer = map.Layers.First(l => l.name == "collision");
                finishBox = map.Layers.First(l => l.name == "finish");

                enemys = map.Layers.First(l => l.name == "enemys");
                hearts = map.Layers.First(l => l.name == "hearts");
                var noWayBack = start.objects[1];
                JustAhead = new Rectangle((int)noWayBack.x, (int)noWayBack.y, (int)noWayBack.width, (int)noWayBack.height);
                SetStartPosition();
                if ((Scenes)CurrentScene == Scenes.final)
                {
                    var exit = start.objects[2];
                    FinalExit = new Rectangle((int)exit.x, (int)exit.y, (int)exit.width, (int)exit.height);
                    FinalGoal = enemys.objects.Length;
                }

            }
            #endregion
        }

        private void SetStartPosition()
        {
            var startPoint = start.objects[0];
            heroPos = new Rectangle((int)startPoint.x + (WindowWidth - MapWidth) / 2, (int)startPoint.y + (WindowHeight - MapHeight) / 2, 20, 20);

            Velocity = 0;
            AttackCount = 0;
        }

        private void Reset()
        {
            if (StartLevel != 0 || CurrentScene == 1 || CurrentScene == 5)
            {
                HeroHealth = 3;
                for (var i = 0; i < hearts.objects.Length; i++)
                    hearts.objects[i].name = "";
            }
            else HeroHealth -= 1;

            for (var i = 0; i < enemys.objects.Length; i++)
                enemys.objects[i].name = "";
            GameIsOver = false;
            IsHorizontal = true;
            heroSprite = hero_Right;

            SetStartPosition();
        }


        protected override void Update(GameTime gameTime)
        {
            var ks = Keyboard.GetState();

            ShowSongVolumeButton.Text = "Main Theme Volume: " + Math.Round(MediaPlayer.Volume * 100).ToString();
            ShowSoundEffVolumeButton.Text = "Attack Effect Volume: " + Math.Round(SoundEffect.MasterVolume * 100).ToString();

            #region Menu Visability

            if (CurrentScene == 0) IsMenuVisible = true;
            else IsMenuVisible = false;

            if (CurrentScene == 0)
            {
                #region Menu Buttons Text


                #region Start Level button text

                switch ((Scenes)StartLevel)
                {
                    case Scenes.menu:
                        ShowStartLevelButton.Text = "Story mode";
                        break;
                    case Scenes.lvl_1:
                        ShowStartLevelButton.Text = "Level: 1";
                        break;
                    case Scenes.lvl_2:
                        ShowStartLevelButton.Text = "Level: 2";
                        break;
                    case Scenes.lvl_3:
                        ShowStartLevelButton.Text = "Level: 3";
                        break;
                    case Scenes.lvl_4:
                        ShowStartLevelButton.Text = "Level: 4";
                        break;
                    case Scenes.lvl_5:
                        ShowStartLevelButton.Text = "Level: 5";
                        break;
                    case Scenes.lvl_6:
                        ShowStartLevelButton.Text = "Level: 6";
                        break;
                    case Scenes.lvl_7:
                        ShowStartLevelButton.Text = "Level: 7";
                        break;
                }
                #endregion

                #region Speed Button Text

                switch (Speed)
                {
                    case 1:
                        ShowSpeedButton.Text = "Speed: " + Speed.ToString() + " (Are you a kid?)";
                        break;
                    case 2:
                        ShowSpeedButton.Text = "Speed: " + Speed.ToString() + " (Normal)";
                        break;
                    case 3:
                        ShowSpeedButton.Text = "Speed: " + Speed.ToString() + " (Hard)";
                        break;
                    case 4:
                        ShowSpeedButton.Text = "Speed: " + Speed.ToString() + " (Very Hard)";
                        break;
                    case 5:
                        ShowSpeedButton.Text = "Speed: " + Speed.ToString() + " (Impossible)";
                        break;
                }

                #endregion

                #endregion
            }


            #endregion

            #region Shooting

            if (!IsShooting && AttackCount < AttackLimit && Keyboard.GetState().IsKeyDown(Keys.Space) && !GameIsOver && !GameIsPaused)
            {
                AttackCount += 1;
                AttackSound.Play();
                IsShooting = true;
                HeroAttackRect = new Rectangle(heroPos.X, heroPos.Y, 20, 20);
                AttackSpeed = (int)Velocity * 3;
                if (AttackSpeed == 0) AttackSpeed = (int)Speed * 3;
                AttackIsHorizontal = IsHorizontal;
            }

            if (IsShooting)
            {
                if (AttackIsHorizontal) HeroAttackRect.X += AttackSpeed;
                else HeroAttackRect.Y += AttackSpeed;
            }

            #endregion

            #region Enemy Animation

            EnemyElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (EnemyElapsed >= EnemyAnimationDelay)
            {
                EnemysFrames = (EnemysFrames + 1) % 4;
                EnemyElapsed = 0;
            }
            #endregion

            #region Functional buttons

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !GameIsPaused)
            {
                if (CurrentScene == 0)
                    Exit();
                else
                {
                    GameIsPaused = true;
                    IsInGameMenuVisible = true;
                    Velocity_Save = Velocity;
                    Velocity = 0;
                    AttackSpeed_Save = AttackSpeed;
                    AttackSpeed = 0;
                }
            }

            if (ks.IsKeyDown(Keys.R) && GameIsOver && !HeroIsDead)
            {
                Reset();
            }
            #endregion

            #region Collision and Offset check 

            #region Hero Position Correction (if windows size changed)

            if (WindowHeight != _graphics.PreferredBackBufferHeight
                || WindowWidth != _graphics.PreferredBackBufferWidth)
            {
                heroPos.X += (_graphics.PreferredBackBufferWidth - WindowWidth) / 2;
                heroPos.Y += (_graphics.PreferredBackBufferHeight - WindowHeight) / 2;
                if (IsShooting)
                {
                    HeroAttackRect.X += (_graphics.PreferredBackBufferWidth - WindowWidth) / 2;
                    HeroAttackRect.Y += (_graphics.PreferredBackBufferHeight - WindowHeight) / 2;
                }
            }
            #endregion

            WindowHeight = _graphics.PreferredBackBufferHeight;
            WindowWidth = _graphics.PreferredBackBufferWidth;


            if (CurrentScene != 0)
            {
                var x_offset = (_graphics.PreferredBackBufferWidth - MapWidth) / 2;
                var y_offset = (_graphics.PreferredBackBufferHeight - MapHeight) / 2;

                #region finish line

                foreach (var a in finishBox.objects)
                {
                    var x = (int)a.x + x_offset;
                    var y = (int)a.y + y_offset;
                    var vect = new Rectangle(x, y, (int)a.width, (int)a.height);
                    if (vect.Intersects(heroPos))
                    {
                        if (StartLevel == 0)
                            CurrentScene = (CurrentScene + 1) % 10;
                        else
                            CurrentScene = 0;
                        LoadScene((Scenes)CurrentScene);
                    }
                    if (vect.Intersects(HeroAttackRect))
                    {
                        IsShooting = false;
                    }
                }
                #endregion

                #region enemys
                for (var i = 0; i < enemys.objects.Length; i++)
                {
                    if (enemys.objects[i].name != "dead")
                    {
                        var e = enemys.objects[i];
                        var x = (int)e.x + x_offset;
                        var y = (int)e.y + y_offset;
                        var vect = new Rectangle(x, y + 1, 14, 14);
                        if (vect.Intersects(HeroAttackRect) || HeroAttackRect.Intersects(vect))
                        {
                            enemys.objects[i].name = "dead";
                            IsShooting = false;
                            HeroAttackRect = new Rectangle(0, 0, 0, 0);
                            if ((Scenes)CurrentScene == Scenes.final) KillsCount += 1;
                        }

                        if (vect.Intersects(heroPos) && enemys.objects[i].name != "dead")
                        {
                            GameIsOver = true;
                            Velocity = 0;
                            if (HeroHealth - 1 == 0)
                                HeroIsDead = true;
                        }
                    }
                }
                #endregion

                #region extra hearts
                for (var i = 0; i < hearts.objects.Length; i++)
                {
                    if (hearts.objects[i].name != "collected")
                    {
                        var e = hearts.objects[i];
                        var x = (int)e.x + x_offset;
                        var y = (int)e.y + y_offset;
                        var vect = new Rectangle(x, y + 1, 14, 14);
                        if (vect.Intersects(heroPos))
                        {
                            hearts.objects[i].name = "collected";
                            HeroHealth = Math.Clamp(HeroHealth + 1, 0, 4);

                        }
                    }
                }
                #endregion

                #region Hero & Attack Hitbox Correction

                var heroCollision = heroPos;
                heroCollision.Y += 3;
                heroCollision.Height -= 6;

                var attackCollision = HeroAttackRect;
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
                        GameIsOver = true;
                        Velocity = 0;
                        if (HeroHealth - 1 == 0)
                            HeroIsDead = true;
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

                if (noWayBack.Intersects(heroPos))
                {
                    if (IsHorizontal) heroPos.X += -(int)Velocity * 2;
                    else heroPos.Y += -(int)Velocity * 2;
                    Velocity = 0;
                }
                if(noWayBack.Intersects(HeroAttackRect))
                {
                    IsShooting = false;
                }
                if ((Scenes)CurrentScene == Scenes.final)
                {
                    var exit = FinalExit;
                    exit.X += x_offset;
                    exit.Y += y_offset;
                    if (exit.Intersects(heroPos) && KillsCount != FinalGoal)
                    {
                        GameIsOver = true;
                        Velocity = 0;
                        if (HeroHealth - 1 == 0)
                            HeroIsDead = true;
                    }
                }
                #endregion

            }
            #endregion

            #region Hero

            if (IsHorizontal) heroPos.X += (int)Velocity;
            else heroPos.Y += (int)Velocity;



            if (!GameIsOver && !IsInGameMenuVisible)
            {
                #region Animation

                var animationDelay = HeroAnimationDelay / (Speed) + 50;
                HeroElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (HeroElapsed >= animationDelay)
                {
                    HeroFrames = (HeroFrames + 1) % 4;
                    HeroElapsed = 0;
                }
                #endregion

                #region Movement

                if ((ks.IsKeyDown(Keys.D) || ks.IsKeyDown(Keys.Right)) && !IsHorizontal)
                {
                    heroSprite = hero_Right;
                    Velocity = Speed;
                    IsHorizontal = true;
                }
                if ((ks.IsKeyDown(Keys.A) || ks.IsKeyDown(Keys.Left)) && !IsHorizontal)
                {
                    heroSprite = hero_Left;
                    Velocity = -Speed;
                    IsHorizontal = true;
                }
                if ((ks.IsKeyDown(Keys.S) || ks.IsKeyDown(Keys.Down)) && IsHorizontal)
                {
                    Velocity = Speed;
                    IsHorizontal = false;
                }
                if ((ks.IsKeyDown(Keys.W) || ks.IsKeyDown(Keys.Up)) && IsHorizontal)
                {
                    Velocity = -Speed;
                    IsHorizontal = false;
                }
                #endregion

            }
            #endregion

            #region Buttons visibility

            if (IsMenuVisible)
                foreach (var button in MenuButtons)
                    button.Update(gameTime, _graphics);

            if (IsInGameMenuVisible)
                foreach (var button in InGameButtons)
                    button.Update(gameTime, _graphics);

            if (HeroIsDead)
            {
                if (StartLevel == 0)
                {
                    if (CurrentScene >= 5)
                    {
                        CurrentScene = 5;
                        RestartButton.Text = "Restart Game (Level: 5)";
                    }
                    else
                    {
                        CurrentScene = 1;
                        RestartButton.Text = "Restart Game (Level: 1)";
                    }
                }
                else
                {
                    CurrentScene = StartLevel;
                    RestartButton.Text = "Restart Game (Level: " + StartLevel.ToString() + ")";
                }
                foreach (var button in DeadMenu)
                    button.Update(gameTime, _graphics);
            }


            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            #region Draw Level

            var tileLayers = map.Layers.Where(x => x.type == TiledLayerType.TileLayer);

            if (CurrentScene != 0)
            {
                var x_change = (_graphics.PreferredBackBufferWidth - MapWidth) / 2;
                var y_change = (_graphics.PreferredBackBufferHeight - MapHeight) / 2;

                if (CurrentScene != 9)
                    _spriteBatch.Draw(LevelBackground, new Rectangle(0, 0, WindowWidth, WindowHeight), Color.White);
                else
                {
                    _spriteBatch.Draw(ForestImage_back, new Rectangle(0, 0, WindowWidth, WindowHeight), Color.White);
                    _spriteBatch.Draw(ForestImage, new Rectangle(x_change, y_change, MapWidth, MapHeight), Color.White);
                }

                foreach (var layer in tileLayers)
                {
                    if (layer.name == "back1") continue;
                    if (KillsCount == FinalGoal && layer.name == "exit") continue;


                    for (var y = 0; y < layer.height; y++)
                        for (var x = 0; x < layer.width; x++)
                        {
                            var index = (y * layer.width) + x;
                            var gid = layer.data[index];
                            var tileX = x * map.TileWidth;
                            var tileY = y * map.TileHeight;

                            if (gid == 0) continue;

                            var mapTileset = map.GetTiledMapTileset(gid);

                            var tileset = tilesets[mapTileset.firstgid];

                            var rect = map.GetSourceRect(mapTileset, tileset, gid);

                            var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);

                            var destination = new Rectangle(tileX + x_change, tileY + y_change, map.TileWidth, map.TileHeight);

                            var textureName = mapTileset.source;
                            var a = textureName.Split('.');
                            var b = a[0];
                            tilesetTexture = Content.Load<Texture2D>(b);
                            SpriteEffects effects = SpriteEffects.None;
                            double rotation = 0f;

                            var color = Color.White;
                            if (layer.tintcolor != null)
                                color = new Color(Color.Black, 80);

                            _spriteBatch.Draw(tilesetTexture, destination, source, color, (float)rotation, Vector2.Zero, effects, 0);
                        }
                }

            }
            else
            {
                _spriteBatch.Draw(MenuImage, new Rectangle(0, 0, WindowWidth, WindowHeight), Color.White);
            }
            #endregion

            #region Enemys and Extra Health

            if (CurrentScene != 0)
            {
                var x_offset = (_graphics.PreferredBackBufferWidth - MapWidth) / 2;
                var y_offset = (_graphics.PreferredBackBufferHeight - MapHeight) / 2;

                for (var i = 0; i < enemys.objects.Length; i++)
                {

                    if (enemys.objects[i].name != "dead")
                    {
                        var e = enemys.objects[i];
                        var x = (int)e.x + x_offset;
                        var y = (int)e.y + y_offset;

                        var effect = SpriteEffects.None;
                        if (heroPos.X < x)
                            effect = SpriteEffects.FlipHorizontally;
                        else
                            effect = SpriteEffects.None;

                        switch (EnemysFrames)
                        {
                            case 0:
                                _spriteBatch.Draw(enemysSprite, new Rectangle(x - 10, y - 4, 20, 20), new Rectangle(0, 0, 24, 20), Color.White, 0f, new Vector2(0, 0), effect, 0);
                                break;
                            case 1:
                                _spriteBatch.Draw(enemysSprite, new Rectangle(x - 10, y - 4, 20, 20), new Rectangle(24, 0, 26, 20), Color.White, 0f, new Vector2(0, 0), effect, 0);
                                break;
                            case 2:
                                _spriteBatch.Draw(enemysSprite, new Rectangle(x - 10, y - 4, 20, 20), new Rectangle(50, 0, 20, 20), Color.White, 0f, new Vector2(0, 0), effect, 0);
                                break;
                            case 3:
                                _spriteBatch.Draw(enemysSprite, new Rectangle(x - 10, y - 4, 20, 20), new Rectangle(70, 0, 26, 20), Color.White, 0f, new Vector2(0, 0), effect, 0);
                                break;
                        }
                    }
                }

                for (var i = 0; i < hearts.objects.Length; i++)
                {
                    if (hearts.objects[i].name != "collected")
                    {
                        var e = hearts.objects[i];
                        var x = (int)e.x + x_offset;
                        var y = (int)e.y + y_offset;
                        _spriteBatch.Draw(heartSprtie, new Rectangle(x, y + 1, heartSprtie.Width, heartSprtie.Height), Color.White);
                    }
                }

            }
            #endregion

            #region Hero + Health

            if (IsHeroVisible)
            {
                switch (HeroFrames)
                {
                    case 0:
                        _spriteBatch.Draw(heroSprite, heroPos, new Rectangle(0, 0, 24, 20), Color.White);
                        break;
                    case 1:
                        _spriteBatch.Draw(heroSprite, heroPos, new Rectangle(24, 0, 26, 20), Color.White);
                        break;
                    case 2:
                        _spriteBatch.Draw(heroSprite, heroPos, new Rectangle(50, 0, 20, 20), Color.White);
                        break;
                    case 3:
                        _spriteBatch.Draw(heroSprite, heroPos, new Rectangle(70, 0, 26, 20), Color.White);
                        break;
                }
                var health = HeroHealth;
                if (GameIsOver) health -= 1;
                var x = (_graphics.PreferredBackBufferWidth - MapWidth + 88) / 2;
                var y = (_graphics.PreferredBackBufferHeight - MapHeight + 22) / 2;
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

            #region Buttons visibility

            if (IsMenuVisible)
                foreach (var button in MenuButtons)
                    button.Draw(_spriteBatch);

            if (IsInGameMenuVisible)
                foreach (var button in InGameButtons)
                    button.Draw(_spriteBatch);

            if (HeroIsDead)
                foreach (var button in DeadMenu)
                    button.Draw(_spriteBatch);

            #endregion

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

                if (AttackIsHorizontal)
                {
                    HeroAttack = HorizontalAttack;
                    var sourceRect = new Rectangle(32 * AttackFrames, 0, 16, 32);
                    if (AttackSpeed < 0)
                        _spriteBatch.Draw(HeroAttack, HeroAttackRect, sourceRect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 0);
                    else
                        _spriteBatch.Draw(HeroAttack, HeroAttackRect, sourceRect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                else
                {
                    HeroAttack = VerticalAttack;
                    var sourceRect = new Rectangle(0, 32 * AttackFrames, 32, 16);
                    if (AttackSpeed > 0)
                        _spriteBatch.Draw(HeroAttack, HeroAttackRect, sourceRect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.FlipVertically, 0);
                    else
                        _spriteBatch.Draw(HeroAttack, HeroAttackRect, sourceRect, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0);
                }
                #endregion
            }
            #endregion


            _spriteBatch.End();
            base.Draw(gameTime);

        }
    }
}