using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Ecalpon.Combat
{
    public class CombatScreen
    {
        // =====================================================
        // DEPENDENCIES
        // =====================================================

        private CombatManager _manager;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;
        private Texture2D _pixel;

        // =====================================================
        // INPUT TRACKING
        // =====================================================

        private KeyboardState _currentKeys;
        private KeyboardState _previousKeys;

        // =====================================================
        // GRID CONSTANTS
        // =====================================================

        private const int GRID_COLS = 16;
        private const int GRID_ROWS = 16;
        private const int TILE_SIZE = 32;
        private const int GRID_ORIGIN_X = 16;
        private const int GRID_ORIGIN_Y = 16;
        private const int PANEL_X = GRID_ORIGIN_X + (GRID_COLS * TILE_SIZE) + 16;
        private const int PANEL_Y = 16;

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        public CombatScreen(SpriteBatch spriteBatch, SpriteFont font,
                            GraphicsDevice graphicsDevice)
        {
            _spriteBatch = spriteBatch;
            _font = font;
            _manager = new CombatManager();

            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White });
        }

        // =====================================================
        // PUBLIC ENTRY POINT
        // =====================================================

        public void BeginCombat(List<Combatant> playerParty,
                                List<Combatant> enemies)
        {
            _manager.StartCombat(playerParty, enemies);
        }

        // =====================================================
        // UPDATE
        // =====================================================

        public void Update(GameTime gameTime)
        {
            _previousKeys = _currentKeys;
            _currentKeys = Keyboard.GetState();

            // The game only responds to input on the player's turn
            // On the enemy turn we resolve immediately and wait
            // for the player again — no timing, no delays

            if (_manager.CurrentState == CombatState.PlayerTurn)
                HandlePlayerInput();

            if (_manager.CurrentState == CombatState.EnemyTurn)
                ResolveEnemyTurn();

            if (_manager.CurrentState == CombatState.SelectingTarget)
                HandlePlayerTargeting();
        }

        private void HandlePlayerTargeting()
        {
            if (WasKeyJustPressed(Keys.Up) || WasKeyJustPressed(Keys.Right))
            {
               //cycle DOWN in list of enemies
                return;
            }

            if (WasKeyJustPressed(Keys.Down) || WasKeyJustPressed(Keys.Left))
            {
                //cycle UP in list of enemies
                return;
            }

            if(WasKeyJustPressed(Keys.Enter))
            {
                //target acquired, determine is valid
                return;
            }
        }

        private void HandlePlayerInput()
        {
            if (WasKeyJustPressed(Keys.A))
            {
                _manager.TransitionTo(CombatState.SelectingTarget);
                return;
            }

            if (WasKeyJustPressed(Keys.M))
            {
                _manager.TransitionTo(CombatState.SelectingMove);
                return;
            }

            if (WasKeyJustPressed(Keys.P))
            {
                _manager.TransitionTo(CombatState.UsingPower);
                return;
            }

            if (WasKeyJustPressed(Keys.Space))
                _manager.EndCurrentTurn();
        }

        // =====================================================
        // ENEMY TURN
        // =====================================================

        private void ResolveEnemyTurn()
        {
            // Enemy does nothing yet — just passes their turn
            // AI behavior gets built here later, one piece at a time
            _manager.EndCurrentTurn();
        }

        // =====================================================
        // INPUT HELPER
        // =====================================================

        private bool WasKeyJustPressed(Keys key)
        {
            return _currentKeys.IsKeyDown(key)
                && _previousKeys.IsKeyUp(key);
        }

        // =====================================================
        // DRAW
        // =====================================================

        public void Draw(GameTime gameTime)
        {
            DrawGrid();
            DrawCombatants();
            DrawMessagePanel();
            DrawActionMenu();
        }

        // =====================================================
        // DRAW THE GRID
        // =====================================================

        private void DrawGrid()
        {
            for (int row = 0; row < GRID_ROWS; row++)
            {
                for (int col = 0; col < GRID_COLS; col++)
                {
                    Rectangle tileRect = new Rectangle(
                        GRID_ORIGIN_X + (col * TILE_SIZE),
                        GRID_ORIGIN_Y + (row * TILE_SIZE),
                        TILE_SIZE - 1,
                        TILE_SIZE - 1
                    );

                    Color tileColor;
                    if ((row + col) % 2 == 0)
                        tileColor = new Color(60, 50, 40);
                    else
                        tileColor = new Color(70, 60, 48);

                    DrawFilledRect(tileRect, tileColor);
                }
            }
        }

        // =====================================================
        // DRAW COMBATANTS
        // =====================================================

        private void DrawCombatants()
        {
            Combatant current = _manager.CurrentCombatant();

            foreach (Combatant combatant in _manager.GetAliveCombatants())
            {
                Rectangle combatantRect = new Rectangle(
                    GRID_ORIGIN_X + (combatant.GridCol * TILE_SIZE) + 4,
                    GRID_ORIGIN_Y + (combatant.GridRow * TILE_SIZE) + 4,
                    TILE_SIZE - 8,
                    TILE_SIZE - 8
                );

                Color combatantColor;
                if (combatant.Type == CombatantType.PlayerCharacter)
                    combatantColor = Color.CornflowerBlue;
                else if (combatant.Type == CombatantType.Companion)
                    combatantColor = Color.MediumSeaGreen;
                else
                    combatantColor = Color.Crimson;

                // Draw highlight behind active combatant
                if (current != null && combatant == current)
                {
                    Rectangle highlight = new Rectangle(
                        combatantRect.X - 3,
                        combatantRect.Y - 3,
                        combatantRect.Width + 6,
                        combatantRect.Height + 6
                    );
                    DrawFilledRect(highlight, Color.Yellow);
                }

                DrawFilledRect(combatantRect, combatantColor);

                // Draw first initial
                _spriteBatch.DrawString(
                    _font,
                    combatant.Name.Substring(0, 1),
                    new Vector2(combatantRect.X + 6, combatantRect.Y + 4),
                    Color.White
                );
            }
        }

        // =====================================================
        // DRAW MESSAGE PANEL
        // =====================================================

        private void DrawMessagePanel()
        {
            Rectangle panelRect = new Rectangle(PANEL_X, PANEL_Y, 280, 300);
            DrawFilledRect(panelRect, new Color(20, 20, 30));

            _spriteBatch.DrawString(
                _font,
                "-- Combat Log --",
                new Vector2(PANEL_X + 8, PANEL_Y + 8),
                Color.Gold
            );

            int lineY = PANEL_Y + 30;
            foreach (string message in _manager.RecentMessages)
            {
                _spriteBatch.DrawString(
                    _font,
                    message,
                    new Vector2(PANEL_X + 8, lineY),
                    Color.LightGray
                );
                lineY += 20;
            }
        }

        // =====================================================
        // DRAW ACTION MENU
        // =====================================================

        private void DrawActionMenu()
        {
            int menuX = PANEL_X;
            int menuY = PANEL_Y + 320;

            Rectangle menuRect = new Rectangle(menuX, menuY, 280, 160);
            DrawFilledRect(menuRect, new Color(20, 20, 30));

            _spriteBatch.DrawString(
                _font,
                "-- Actions --",
                new Vector2(menuX + 8, menuY + 8),
                Color.Gold
            );

            if (_manager.CurrentState == CombatState.PlayerTurn)
            {
                _spriteBatch.DrawString(_font, "[A] Attack",
                    new Vector2(menuX + 8, menuY + 30), Color.White);

                _spriteBatch.DrawString(_font, "[M] Move",
                    new Vector2(menuX + 8, menuY + 52), Color.White);

                _spriteBatch.DrawString(_font, "[P] Use Power",
                    new Vector2(menuX + 8, menuY + 74), Color.White);

                _spriteBatch.DrawString(_font, "[Space] End Turn",
                    new Vector2(menuX + 8, menuY + 96), Color.White);
            }
            else
            {
                _spriteBatch.DrawString(
                    _font,
                    _manager.CurrentState.ToString(),
                    new Vector2(menuX + 8, menuY + 30),
                    Color.Yellow
                );
            }
        }

        // =====================================================
        // DRAW HELPER
        // =====================================================

        private void DrawFilledRect(Rectangle rect, Color color)
        {
            _spriteBatch.Draw(_pixel, rect, color);
        }
    }
}