using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using TiledCS;
using System.Linq;
using System;
using System.Diagnostics;

namespace Explore_Your_Smth
{
    public class Game1 : Game
    {

        Map MainMap;
        Hero Bat;

        public string OnSpeedButtonText;
        public string LevelNumber;
        public GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public static bool ShowStoryText;

        SpriteFont Font;
        string Story_iSwear;
        Texture2D TextBack;

        #region Sound

        Song MainSong;
        SoundEffect WallCollapseEffect;
        public static bool WallIsCollapsed;
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
        public static bool IsMenuVisible;
        #endregion

        #region InGame Buttons

        List<Button> InGameButtons = new List<Button>();
        Button BackToMenuButton;
        Button ContinueButton;
        public static bool IsInGameMenuVisible;
        #endregion

        #region After Death Menu

        List<Button> DeadMenu = new List<Button>();
        Button RestartButton;
        #endregion

        public string ScenePath;
        public static int CurrentScene = 0;
        public static int StartLevel_menu;
        public static bool GameIsPaused;

        public static int WindowWidth;
        public static int WindowHeight;

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
            Map.MapWidth = 1280;
            Map.MapHeight = 720;
            _graphics.PreferredBackBufferWidth = Map.MapWidth;
            _graphics.PreferredBackBufferHeight = Map.MapHeight;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Font = Content.Load<SpriteFont>("Peace Sans");
            TextBack = Content.Load<Texture2D>("tile");

            MainMap = new Map()
            {
                LevelBackground = Content.Load<Texture2D>("dark red back fog2"),
                MenuImage = Content.Load<Texture2D>("MainMenuImage"),
                ForestImage = Content.Load<Texture2D>("DarkForest"),
                ForestImage_back = Content.Load<Texture2D>("DarkForest_back"),
            };

            Texture.Name = new Dictionary<string, Texture2D>()
            {
                    {"alien.tsx", Content.Load<Texture2D>("alien")},
                    {"dark green.tsx", Content.Load<Texture2D>("dark green")},
                    {"dark red.tsx", Content.Load<Texture2D>("dark red")},
                    {"gold.tsx", Content.Load<Texture2D>("gold")},
                    {"gray.tsx", Content.Load<Texture2D>("gray")},
                    {"ice.tsx", Content.Load<Texture2D>("ice")},
                    {"monster.tsx", Content.Load<Texture2D>("monster")},
                    {"orange.tsx", Content.Load<Texture2D>("orange")},
                    {"purple.tsx", Content.Load<Texture2D>("purple")},
                    {"red.tsx", Content.Load<Texture2D>("red")},
                    {"yellow.tsx", Content.Load<Texture2D>("yellow")}  
            };

            Bat = new Hero()
            {
                Sprite_Right = Content.Load<Texture2D>("whiteBatMovingRight_2x"),
                Sprite_Left = Content.Load<Texture2D>("whiteBatMovingLeft_2x"),

                HorizontalAttackSprite = Content.Load<Texture2D>("attack_left"),
                VerticalAttackSprite = Content.Load<Texture2D>("attack_up"),
                AttackSound = Content.Load<SoundEffect>("AttackSoundMP3"),

                Health_0 = Content.Load<Texture2D>("0_hearts"),
                Health_1 = Content.Load<Texture2D>("1_hearts"),
                Health_2 = Content.Load<Texture2D>("2_hearts"),
                Health_3 = Content.Load<Texture2D>("3_hearts1"),
                Health_4 = Content.Load<Texture2D>("4_hearts")
            };
            Bat.Sprite = Bat.Sprite_Right;

            ExtraHearts.Sprite = Content.Load<Texture2D>("extra_heart");
            Enemies.Sprite = Content.Load<Texture2D>("enemy flying");

            #region Background music

            MainSong = Content.Load<Song>("IEROD1903678-inner-peace-1903678-320");
            MediaPlayer.Play(MainSong);
            MediaPlayer.Volume = 0.01f;
            SoundEffect.MasterVolume = 0.05f;
            MediaPlayer.IsRepeating = true;
            #endregion

            WallCollapseEffect = Content.Load<SoundEffect>("WallCollapsed");

            #region Menu Buttons Creation

            #region Play & Quit

            PlayButton = new Button(
                Font,
                //"Играть",
                "Play",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, -100);

            QuitButton = new Button(
                Font,
                //"Выйти",
                "Quit",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, 275);


            PlayButton.Click += PlayButton_Action;
            QuitButton.Click += QuitButton_Action;

            MenuButtons.Add(PlayButton);
            MenuButtons.Add(QuitButton);
            #endregion

            #region Difficult
            var speed_yOffset = -25;
            PlusSpeedButton = new Button(
                Font,
                "+",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                125, speed_yOffset)
            { IsReact = false };

            MinusSpeedButton = new Button(
               Font,
                "-",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                -125, speed_yOffset)
            { IsReact = false };

            ShowSpeedButton = new Button(
                Font,
                //"Настройки",
                "Settings",
                Content.Load<Texture2D>("tile1"),
                200, 50,
                0, speed_yOffset)
            { IsReact = false };

            PlusSpeedButton.Click += PlusSpeedButton_Action;
            MinusSpeedButton.Click += MinusSpeedButton_Action;

            MenuButtons.Add(PlusSpeedButton);
            MenuButtons.Add(MinusSpeedButton);
            MenuButtons.Add(ShowSpeedButton);
            #endregion

            #region Song volume 

            var volume_yOffset = 125;

            PlusSongVolumeButton = new Button(
                Font,
                "+",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                125, volume_yOffset)
            { IsReact = false };

            MinusSongVolumeButton = new Button(
                Font,
                "-",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                -125, volume_yOffset)
            { IsReact = false };

            ShowSongVolumeButton = new Button(
                Font,
                //"Настройки",
                "SongVolume",
                Content.Load<Texture2D>("tile1"),
                200, 50,
                0, volume_yOffset)
            { IsReact = false };


            MinusSongVolumeButton.Click += MinusSongVolumeButton_Action;
            PlusSongVolumeButton.Click += PlusSongVolumeButton_Action;

            MenuButtons.Add(PlusSongVolumeButton);
            MenuButtons.Add(MinusSongVolumeButton);
            MenuButtons.Add(ShowSongVolumeButton);


            #endregion

            #region Sound Effects volume

            var soundEffects_yOffset = 200;

            PlusSoundEffButton = new Button(
                Font,
                "+",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                125, soundEffects_yOffset)
            { IsReact = false };

            MinusSoundEffButton = new Button(
                Font,
                "-",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                -125, soundEffects_yOffset)
            { IsReact = false };

            ShowSoundEffVolumeButton = new Button(
                Font,
                //"Настройки",
                "SongVolume",
                Content.Load<Texture2D>("tile1"),
                200, 50,
                0, soundEffects_yOffset)
            { IsReact = false };


            MinusSoundEffButton.Click += MinusSoundEffButton_Action;
            PlusSoundEffButton.Click += PlusSoundEffButton_Action;

            MenuButtons.Add(MinusSoundEffButton);
            MenuButtons.Add(PlusSoundEffButton);
            MenuButtons.Add(ShowSoundEffVolumeButton);


            #endregion

            #region Choose start level 

            var startLevelBar_offset = 50;

            PlusStartLevelButton = new Button(
                Font,
                ">",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                125, startLevelBar_offset)
            { IsReact = false };

            MinusStartLevelButton = new Button(
                Font,
                "<",
                Content.Load<Texture2D>("tile1"),
                50, 50,
                -125, startLevelBar_offset)
            { IsReact = false };

            ShowStartLevelButton = new Button(
                Font,
                //"Настройки",
                "StartLevel",
                Content.Load<Texture2D>("tile1"),
                200, 50,
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
                Font,
                //"Продолжить",
                "Continue",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, -25);

            BackToMenuButton = new Button(
                Font,
                //"Вернуться в меню",
                "Back To Menu",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, 50);

            ContinueButton.Click += ContinueButton_Action;
            BackToMenuButton.Click += BackToMenuButton_Action;
            InGameButtons.Add(ContinueButton);
            InGameButtons.Add(BackToMenuButton);

            InGameButtons.Add(MinusSoundEffButton);
            InGameButtons.Add(PlusSoundEffButton);
            InGameButtons.Add(ShowSoundEffVolumeButton);

            InGameButtons.Add(MinusSongVolumeButton);
            InGameButtons.Add(PlusSongVolumeButton);
            InGameButtons.Add(ShowSongVolumeButton);
            #endregion

            #region DeadMenu Buttons Creation

            RestartButton = new Button(
                Font,
                //"Начать с начала",
                "Restart Game",
                Content.Load<Texture2D>("tile1"),
                300, 50,
                0, -50);

            RestartButton.Click += RestartButton_Action;
            DeadMenu.Add(RestartButton);
            DeadMenu.Add(BackToMenuButton);
            #endregion

            LoadScene((Scenes)CurrentScene);
        }

        #region Menu Buttons Reaction

        #region Speed
        private void PlusSpeedButton_Action(object sender, System.EventArgs e)
        {
            Bat.Speed += 1;
            Bat.Speed = Math.Clamp(Bat.Speed, 1, 5);
        }

        private void MinusSpeedButton_Action(object sender, System.EventArgs e)
        {
            Bat.Speed -= 1;
            Bat.Speed = Math.Clamp(Bat.Speed, 1, 5);
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
            Bat.AttackSound.Play();
        }

        private void MinusSoundEffButton_Action(object sender, System.EventArgs e)
        {
            SoundEffect.MasterVolume = Math.Clamp(SoundEffect.MasterVolume - 0.01f, 0, 1);
            Bat.AttackSound.Play();
        }
        #endregion

        #region Play & Quit

        private void PlayButton_Action(object sender, System.EventArgs e)
        {
            if (StartLevel_menu == 0)
                CurrentScene = 1;
            else CurrentScene = StartLevel_menu;
            Bat.Health = 3;
            Bat.Crashed = false;
            Bat.IsDead = false;
            Bat.IsShooting = false;
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
            StartLevel_menu -= 1;
            StartLevel_menu = Math.Clamp(StartLevel_menu, 0, 7);
        }

        private void PlusStartLevelButton_Action(object sender, System.EventArgs e)
        {
            StartLevel_menu += 1;
            StartLevel_menu = Math.Clamp(StartLevel_menu, 0, 7);
        }
        #endregion

        #endregion

        #region InGame Buttuons Reaction

        private void ContinueButton_Action(object sender, System.EventArgs e)
        {
            Bat.AttackSpeed = Bat.AttackSpeed_Save;
            Bat.Velocity = Bat.Velocity_Save;
            IsInGameMenuVisible = false;
            GameIsPaused = false;
        }
        private void BackToMenuButton_Action(object sender, System.EventArgs e)
        {
            IsInGameMenuVisible = false;
            Bat.Crashed = false;
            Bat.IsDead = false;
            Bat.IsShooting = false;
            GameIsPaused = false;
            CurrentScene = 0;
            LoadScene((Scenes)CurrentScene);
        }
        #endregion

        #region DeadMenu Buttons Reaction

        private void RestartButton_Action(object sender, System.EventArgs e)
        {
            Bat.Reset(CurrentScene,_graphics,StartLevel_menu);
            Bat.Health = 3;
            LoadScene((Scenes)CurrentScene);
        }
        #endregion

        public void LoadScene(Scenes scene)
        {
            var sceneNumb = (int)scene;
            if (StartLevel_menu == 0) ShowStoryText = true;
            else ShowStoryText = false;
            if(scene != 0) Story_iSwear = Story.Text[sceneNumb];
            MainMap.LoadMap("Content\\" + Scene.Path[sceneNumb]);
            Bat.LoadSceneCollision(MainMap.map, sceneNumb, _graphics);
        }

        protected override void Update(GameTime gameTime)
        {
            var ks = Keyboard.GetState();

            if (Bat.KillsCount == Bat.KillsGoal && !WallIsCollapsed)
            {
                WallIsCollapsed = true; 
                WallCollapseEffect.Play();
            }

            ShowSongVolumeButton.Text = "Main Theme Volume: " + Math.Round(MediaPlayer.Volume * 100).ToString();
            ShowSoundEffVolumeButton.Text = "Attack Effect Volume: " + Math.Round(SoundEffect.MasterVolume * 100).ToString();

            if (ShowStoryText && ks.IsKeyDown(Keys.E)) 
                ShowStoryText = false;

            #region Menu Visability

            if (CurrentScene == 0)
            {
                #region Start Level button text

                switch ((Scenes)StartLevel_menu)
                {
                    case Scenes.menu:
                        LevelNumber = "Story mode";
                        break;
                    case Scenes.lvl_1:
                        LevelNumber = "1";
                        break;
                    case Scenes.lvl_2:
                        LevelNumber = "2";
                        break;
                    case Scenes.lvl_3:
                        LevelNumber = "3";
                        break;
                    case Scenes.lvl_4:
                        LevelNumber = "4";
                        break;
                    case Scenes.lvl_5:
                        LevelNumber = "5";
                        break;
                    case Scenes.lvl_6:
                        LevelNumber = "6";
                        break;
                    case Scenes.lvl_7:
                        LevelNumber = "7";
                        break;
                }
                ShowStartLevelButton.Text = LevelNumber == "Story mode" ? LevelNumber : "Level: " + LevelNumber;

                #endregion

                #region Speed Select Button Text

                switch (Bat.Speed)
                {
                    case 1:
                        OnSpeedButtonText = " (Are you a kid?)";
                        break;
                    case 2:
                        OnSpeedButtonText = " (Normal)";
                        break;
                    case 3:
                        OnSpeedButtonText = " (Hard)";
                        break;
                    case 4:
                        OnSpeedButtonText = " (Very Hard)";
                        break;
                    case 5:
                        OnSpeedButtonText  =" (Impossible)";
                        break;
                }
                ShowSpeedButton.Text = "Speed: " + Bat.Speed.ToString() + OnSpeedButtonText;
                #endregion

                IsMenuVisible = true;
            }
            else
            {
                IsMenuVisible = false;
                Bat.Update(_graphics, gameTime, ShowStoryText);

                if (Bat.Finished && StartLevel_menu == 0)
                {
                    CurrentScene = (CurrentScene + 1) % 10;
                    LoadScene((Scenes)CurrentScene);
                    Bat.Finished = false;
                }
                else if (Bat.Finished)
                {
                    CurrentScene = 0;
                    LoadScene((Scenes)CurrentScene);
                    Bat.Finished = false;
                }
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
                    Bat.Velocity_Save = Bat.Velocity;
                    Bat.Velocity = 0;
                    Bat.AttackSpeed_Save = Bat.AttackSpeed;
                    Bat.AttackSpeed = 0;
                }
            }
            #endregion
 
            #region Buttons visibility

            if (IsMenuVisible)
                foreach (var button in MenuButtons)
                    button.Update(gameTime, _graphics);

            if (IsInGameMenuVisible)
                foreach (var button in InGameButtons)
                    button.Update(gameTime, _graphics);

            if (Bat.IsDead)
            {
                if (StartLevel_menu == 0)
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
                    CurrentScene = StartLevel_menu;
                    RestartButton.Text = "Restart Game (Level: " + StartLevel_menu.ToString() + ")";
                }
                foreach (var button in DeadMenu)
                    button.Update(gameTime, _graphics);
            }


            #endregion

            Enemies.Update(gameTime);

            WindowHeight = _graphics.PreferredBackBufferHeight;
            WindowWidth = _graphics.PreferredBackBufferWidth;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.BlueViolet);
            _spriteBatch.Begin();

            MainMap.Draw(Bat, _spriteBatch, _graphics);

            #region Enemys and Extra Health

            if (CurrentScene != 0)
            {
                Enemies.Draw(_spriteBatch, _graphics, Bat);
                ExtraHearts.Draw(_spriteBatch, _graphics);
            }
            #endregion

            Bat.Draw(_spriteBatch,_graphics, gameTime);

            #region Buttons visibility

            if (IsMenuVisible)
                foreach (var button in MenuButtons)
                    button.Draw(_spriteBatch);

            if (IsInGameMenuVisible)
                foreach (var button in InGameButtons)
                    button.Draw(_spriteBatch);

            if (Bat.IsDead)
                foreach (var button in DeadMenu)
                    button.Draw(_spriteBatch);

            #endregion

            if (ShowStoryText && CurrentScene != 0)
            {
                _spriteBatch.Draw(TextBack, new Rectangle(
                    (int)(WindowWidth - Font.MeasureString(Story_iSwear).X - 30) / 2,
                    (int)(WindowHeight - Font.MeasureString(Story_iSwear).Y - 20) / 2,
                    30 + (int)Font.MeasureString(Story_iSwear).X , 
                    20 + (int)Font.MeasureString(Story_iSwear).Y) , 
                    Color.BurlyWood);

                _spriteBatch.DrawString(Font, Story_iSwear, new Vector2(
                    (WindowWidth - Font.MeasureString(Story_iSwear).X) / 2,
                    (WindowHeight - Font.MeasureString(Story_iSwear).Y) / 2),
                    Color.Black); 
            }

            _spriteBatch.End();
            base.Draw(gameTime);

        }
    }
}