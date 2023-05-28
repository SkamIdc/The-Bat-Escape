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
    public class Map
    {
        public int StartLevel;
        public TiledMap map;
        public Dictionary<int, TiledTileset> tilesets;
        public Texture2D tilesetTexture;

        public Texture2D ForestImage { get; set; }
        public Texture2D ForestImage_back { get; set; }
        public Texture2D LevelBackground { get; set; }
        public Texture2D MenuImage { get; set; }

        public static int MapWidth { get; set; }
        public static int MapHeight { get; set; }

        public void LoadMap(string path)
        {
            map = new TiledMap(path);
            tilesets = map.GetTiledTilesets("Content\\tilesets/");
            if(Game1.CurrentScene != 0)
            {
                Enemies.PositionsLayer = map.Layers.First(l => l.name == "enemys");
                ExtraHearts.PositionsLayer = map.Layers.First(l => l.name == "hearts");
            }
        }

        public void Draw(Hero Bat, SpriteBatch _spriteBatch, GraphicsDeviceManager _graphics)
        {
            var tileLayers = map.Layers.Where(x => x.type == TiledLayerType.TileLayer);

            if (Game1.CurrentScene != 0)
            {
                var x_change = (_graphics.PreferredBackBufferWidth - MapWidth) / 2;
                var y_change = (_graphics.PreferredBackBufferHeight - MapHeight) / 2;

                if (Game1.CurrentScene != 9)
                    _spriteBatch.Draw(LevelBackground, new Rectangle(0, 0, Game1.WindowWidth, Game1.WindowHeight), Color.White);
                else
                {
                    _spriteBatch.Draw(ForestImage_back, new Rectangle(0, 0, Game1.WindowWidth, Game1.WindowHeight), Color.White);
                    _spriteBatch.Draw(ForestImage, new Rectangle(x_change, y_change, MapWidth, MapHeight), Color.White);
                }

                foreach (var layer in tileLayers)
                {
                    if (layer.name == "back1") continue;
                    if (Bat.KillsCount == Bat.KillsGoal && layer.name == "exit") continue;


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

                            tilesetTexture = Texture.Name[mapTileset.source];

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
                _spriteBatch.Draw(MenuImage, new Rectangle(0, 0, Game1.WindowWidth, Game1.WindowHeight), Color.White);

            }
        }
    }
}
