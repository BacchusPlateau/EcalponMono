using Ecalpon.Combat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecalpon
{
    public class TheGame : Game
    {
        private bool _inCombat = true;  // true for now to test combat screen
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D rogueSword;
        private Texture2D tileset;
        private int TilesPerRow = 8;
        private float scale = 2.0f;
        private Vector2 spritePosition;
        private int playerGridX = 7;  // Starting position on the map
        private int playerGridY = 5;
        private float tileSize = 32.0f;  // Raw world tile size (matches sprite)
        private KeyboardState previousKeyboardState;
		private CombatScreen _combatScreen;
		private SpriteFont _combatFont;

		private int[,] map = new int[,]
        {   
        
        //15x10
        //                       1
        //   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},  //0
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},  //1
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},  //2
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},  //3
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},  //4
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},  //5
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},  //6
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},  //7
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},  //8
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},  //9
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},  //10
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1}   //11
        };


        public TheGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            //have grok explain the two below settings
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 800;

            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Original assets
            rogueSword = Content.Load<Texture2D>("sprites/rogueSword");
            tileset = Content.Load<Texture2D>("sheets/t1");

            // Combat screen setup
            _combatFont = Content.Load<SpriteFont>("fonts/CombatFont");
            _combatScreen = new CombatScreen(spriteBatch, _combatFont, GraphicsDevice);

            // Test combat encounter
            List<Combatant> party = new List<Combatant>
            {
                new Combatant
                {
                    Name = "Scientist",
                    Type = CombatantType.PlayerCharacter,
                    HitPoints = 20,
                    MaxHitPoints = 20,
                    ArmorClass = 6,
                    ThacO = 20,
                    DamageMin = 1,
                    DamageMax = 6,
                    DamageBonus = 1,
                    Level = 1,
                    MaxMoves = 3,
                    GridRow = 12,
                    GridCol = 7
                }
            };

            List<Combatant> enemies = new List<Combatant>
            {
                new Combatant
                {
                    Name = "Legionary",
                    Type = CombatantType.Enemy,
                    HitPoints = 12,
                    MaxHitPoints = 12,
                    ArmorClass = 4,
                    ThacO = 20,
                    DamageMin = 1,
                    DamageMax = 8,
                    DamageBonus = 0,
                    Level = 1,
                    MaxMoves = 2,
                    GridRow = 4,
                    GridCol = 7
                },
                new Combatant
                {
                    Name = "Jackal",
                    Type = CombatantType.Enemy,
                    HitPoints = 6,
                    MaxHitPoints = 6,
                    ArmorClass = 7,
                    ThacO = 19,
                    DamageMin = 1,
                    DamageMax = 4,
                    DamageBonus = 0,
                    Level = 1,
                    MaxMoves = 4,
                    GridRow = 3,
                    GridCol = 9
                }
            };

            _combatScreen.BeginCombat(party, enemies);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_inCombat)
            {
                _combatScreen.Update(gameTime);
            }
            else
            {
                KeyboardState currentKeyboardState = Keyboard.GetState();

                if (currentKeyboardState.IsKeyDown(Keys.W) && previousKeyboardState.IsKeyUp(Keys.W))
                    playerGridY--;

                if (currentKeyboardState.IsKeyDown(Keys.S) && previousKeyboardState.IsKeyUp(Keys.S))
                    playerGridY++;

                if (currentKeyboardState.IsKeyDown(Keys.A) && previousKeyboardState.IsKeyUp(Keys.A))
                    playerGridX--;

                if (currentKeyboardState.IsKeyDown(Keys.D) && previousKeyboardState.IsKeyUp(Keys.D))
                    playerGridX++;

                float logicalWidth = GraphicsDevice.Viewport.Width / scale;
                float logicalHeight = GraphicsDevice.Viewport.Height / scale;

                int maxGridX = (int)((logicalWidth - tileSize) / tileSize);
                int maxGridY = (int)((logicalHeight - tileSize) / tileSize);

                playerGridX = MathHelper.Clamp(playerGridX, 0, maxGridX);
                playerGridY = MathHelper.Clamp(playerGridY, 0, maxGridY);

                UpdateSpritePosition();

                previousKeyboardState = currentKeyboardState;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (_inCombat)
            {
                spriteBatch.Begin();
                _combatScreen.Draw(gameTime);
                spriteBatch.End();
            }
            else
            {
                Matrix scaleMatrix = Matrix.CreateScale(scale);

                spriteBatch.Begin(
                    transformMatrix: scaleMatrix,
                    samplerState: SamplerState.PointClamp
                );

                float logicalWidth = GraphicsDevice.Viewport.Width / scale;
                float logicalHeight = GraphicsDevice.Viewport.Height / scale;

                int tilesX = (int)(logicalWidth / tileSize) + 2;
                int tilesY = (int)(logicalHeight / tileSize) + 2;

                int startX = Math.Max(0, playerGridX - tilesX / 2);
                int startY = Math.Max(0, playerGridY - tilesY / 2);
                int endX = Math.Min(map.GetLength(1), startX + tilesX);
                int endY = Math.Min(map.GetLength(0), startY + tilesY);

                Vector2 cameraOffset = new Vector2(
                    (GraphicsDevice.Viewport.Width / scale / 2) - tileSize / 2 - (playerGridX * tileSize),
                    (GraphicsDevice.Viewport.Height / scale / 2) - tileSize / 2 - (playerGridY * tileSize)
                );

                for (int y = startY; y < endY; y++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        int tileId = map[y, x];

                        Rectangle sourceRect = new Rectangle(
                            (tileId % TilesPerRow) * (int)tileSize,
                            (tileId / TilesPerRow) * (int)tileSize,
                            (int)tileSize,
                            (int)tileSize
                        );

                        Vector2 pos = new Vector2(x * tileSize, y * tileSize) + cameraOffset;
                        spriteBatch.Draw(tileset, pos, sourceRect, Color.White);
                    }
                }

                spriteBatch.Draw(
                    rogueSword,
                    spritePosition + cameraOffset,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0f
                );

                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void UpdateSpritePosition()
        {
            spritePosition = new Vector2(playerGridX * tileSize, playerGridY * tileSize);
        }
    }
}
