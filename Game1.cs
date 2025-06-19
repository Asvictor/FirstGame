using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FirstGame.UI;
using FirstGame.Entities;
using FirstGame.Core;

namespace FirstGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private SpriteFont _font;
    private string _playerName = "Hero"; // Placeholder for name input
    private Gender _playerGender = Gender.Male; // Placeholder for gender input
    private List<Skill> _skills;
    private Skill _lastUsedSkill;
    private int _lastSkillDamage;
    private List<Equipment> _sampleEquipment;
    private Texture2D _menuBg;
    private Map _map;
    private PlayerEntity _playerEntity;
    private Texture2D _tileTex;
    private Texture2D _playerTex;
    private Vector2 _camera;

    // Add these fields for line and hexagon texture caching
    private Texture2D _uiLineTex;

    private enum GameState { Menu, CharacterCreation, Playing, Options }
    private GameState _gameState = GameState.Menu;
    private int _menuIndex = 0;
    private readonly string[] _menuItems = { "Start New Game", "Load Game", "Options" };
    private KeyboardState _prevKeyboard;

    private int _charCreateGenderIndex = 0;
    private string _charCreateName = "";
    private readonly string[] _genderLabels = { "Male", "Female" };
    private readonly Color[] _genderColors = { Color.CornflowerBlue, Color.Pink };

    private bool _showStatPanel = false;
    private MouseState _prevMouse;
    private int[] _pendingStatAlloc = new int[6]; // STR, VIT, AGI, INT, DEX, LUK
    private readonly string[] _statNames = { "STR", "VIT", "AGI", "INT", "DEX", "LUK" };

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1024;
        _graphics.PreferredBackBufferHeight = 768;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("DefaultFont");
        // Create a simple semi-transparent background for the menu
        _menuBg = new Texture2D(GraphicsDevice, 1, 1);
        _menuBg.SetData(new[] { new Color(0, 0, 0, 180) });
        _player = new Player(_playerName, _playerGender);
        _skills = GenerateRandomSkills(20);
        // Create sample equipment
        _sampleEquipment =
        [
            new Equipment("Sword of Power", EquipmentType.Weapon, str: 10),
            new Equipment("Iron Helmet", EquipmentType.Head, vit: 5),
            new Equipment("Steel Armor", EquipmentType.Body, vit: 10),
            new Equipment("Gauntlets", EquipmentType.Arm, str: 3, agi: 2),
            new Equipment("Boots of Speed", EquipmentType.Feet, agi: 7),
            new Equipment("Lucky Ring", EquipmentType.AccessoryLeft, luk: 8),
            new Equipment("Magic Amulet", EquipmentType.AccessoryRight, @int: 6)
        ];
        // Create a simple map (1 = ground, 0 = empty)
        int[,] tiles = new int[12, 32];
        for (int x = 0; x < 32; x++)
        {
            tiles[11, x] = 1; // ground
            if (x % 5 == 0 && x > 0 && x < 30) tiles[8, x] = 1; // platforms
        }
        _map = new Map(tiles);
        _playerEntity = new PlayerEntity { Position = new Vector2(100, 100), Gender = _playerGender };
        // Simple textures
        _tileTex = new Texture2D(GraphicsDevice, 1, 1);
        _tileTex.SetData(new[] { Color.Gray });
        _playerTex = new Texture2D(GraphicsDevice, 1, 1);
        _playerTex.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        // Only handle S for stat panel in Playing state
        if (_gameState == GameState.Playing)
        {
            if ((keyboard.IsKeyDown(Keys.S) && !_prevKeyboard.IsKeyDown(Keys.S)) ||
                (Mouse.GetState().LeftButton == ButtonState.Pressed && !_prevMouse.LeftButton.HasFlag(ButtonState.Pressed)))
            {
                // Check if mouse is over icon
                var mouse = Mouse.GetState();
                int iconSize = 64;
                Rectangle iconRect = new Rectangle(30, 30, iconSize, iconSize);
                if (iconRect.Contains(mouse.Position) || (keyboard.IsKeyDown(Keys.S) && !_prevKeyboard.IsKeyDown(Keys.S)))
                {
                    _showStatPanel = !_showStatPanel;
                    if (_showStatPanel)
                        Array.Clear(_pendingStatAlloc, 0, _pendingStatAlloc.Length); // Reset pending on open
                }
            }
        }
        // Menu navigation and Enter key only in Menu/CharacterCreation
        if (_gameState == GameState.Menu)
        {
            if (keyboard.IsKeyDown(Keys.Down) && !_prevKeyboard.IsKeyDown(Keys.Down))
                _menuIndex = (_menuIndex + 1) % _menuItems.Length;
            if (keyboard.IsKeyDown(Keys.Up) && !_prevKeyboard.IsKeyDown(Keys.Up))
                _menuIndex = (_menuIndex - 1 + _menuItems.Length) % _menuItems.Length;
            if (keyboard.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter))
            {
                switch (_menuIndex)
                {
                    case 0: // Start New Game
                        _gameState = GameState.CharacterCreation;
                        _charCreateGenderIndex = 0;
                        _charCreateName = "";
                        break;
                    case 1: // Load Game
                        // TODO: Implement load logic
                        break;
                    case 2: // Options
                        _gameState = GameState.Options;
                        break;
                }
            }
            _prevKeyboard = keyboard;
            return;
        }
        if (_gameState == GameState.CharacterCreation)
        {
            // Gender selection
            if (keyboard.IsKeyDown(Keys.Left) && !_prevKeyboard.IsKeyDown(Keys.Left))
                _charCreateGenderIndex = (_charCreateGenderIndex + _genderLabels.Length - 1) % _genderLabels.Length;
            if (keyboard.IsKeyDown(Keys.Right) && !_prevKeyboard.IsKeyDown(Keys.Right))
                _charCreateGenderIndex = (_charCreateGenderIndex + 1) % _genderLabels.Length;
            // Name input
            foreach (var key in keyboard.GetPressedKeys())
            {
                if (!_prevKeyboard.IsKeyDown(key))
                {
                    if (key == Keys.Back && _charCreateName.Length > 0)
                        _charCreateName = _charCreateName.Substring(0, _charCreateName.Length - 1);
                    else if (key == Keys.Space)
                        _charCreateName += " ";
                    else if (key == Keys.Enter && _charCreateName.Length > 0)
                    {
                        // Confirm and start game
                        _player = new Player(_charCreateName, _charCreateGenderIndex == 0 ? Gender.Male : Gender.Female);
                        _skills = GenerateRandomSkills(20);
                        _playerEntity = new PlayerEntity { Position = new Vector2(100, 100), Gender = _charCreateGenderIndex == 0 ? Gender.Male : Gender.Female };
                        _gameState = GameState.Playing;
                    }
                    else if (_charCreateName.Length < 12 && key >= Keys.A && key <= Keys.Z)
                    {
                        bool shift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
                        char c = (char)(key - Keys.A + (shift ? 'A' : 'a'));
                        _charCreateName += c;
                    }
                }
            }
            if (keyboard.IsKeyDown(Keys.Escape) && !_prevKeyboard.IsKeyDown(Keys.Escape))
                _gameState = GameState.Menu;
            _prevKeyboard = keyboard;
            return;
        }
        if (_gameState == GameState.Options)
        {
            if (keyboard.IsKeyDown(Keys.Escape) && !_prevKeyboard.IsKeyDown(Keys.Escape))
                _gameState = GameState.Menu;
            _prevKeyboard = keyboard;
            return;
        }

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        // Example: Add XP and allocate stat for testing
        if (Keyboard.GetState().IsKeyDown(Keys.F1))
        {
            _player.AddExperience(120); // Test XP gain
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D1))
        {
            _player.AllocateStat("STR");
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D2))
        {
            _player.AllocateStat("VIT");
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D3))
        {
            _player.AllocateStat("AGI");
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D4))
        {
            _player.AllocateStat("INT");
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D5))
        {
            _player.AllocateStat("LUK");
        }

        if (_gameState == GameState.Playing)
        {
            _playerEntity.Update(_map, delta);
            // Camera follows player
            _camera = _playerEntity.Position - new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            _camera.X = MathHelper.Clamp(_camera.X, 0, _map.Width * _map.TileSize - GraphicsDevice.Viewport.Width);
            _camera.Y = MathHelper.Clamp(_camera.Y, 0, _map.Height * _map.TileSize - GraphicsDevice.Viewport.Height);
        }
        // Update cooldown timers (always, not just in Playing)
        foreach (var skill in _skills)
        {
            if (skill.CooldownTimer > 0)
                skill.CooldownTimer -= delta;
            if (skill.CooldownTimer < 0)
                skill.CooldownTimer = 0;
        }
        // Skill usage: F2-F11 for Skill_1 to Skill_10
        var unlocked = _skills.FindAll(s => _player.Level >= s.UnlockLevel);
        var keyboardState = Keyboard.GetState();
        for (int i = 0; i < Math.Min(10, unlocked.Count); i++)
        {
            Keys key = Keys.F2 + i; // F2=Skill_1, F3=Skill_2, ...
            var skill = unlocked[i];
            if (keyboardState.IsKeyDown(key) && skill.CooldownTimer == 0)
            {
                _lastUsedSkill = skill;
                _lastSkillDamage = skill.CalculateDamage(_player.STR, _player.INT);
                skill.CooldownTimer = skill.Cooldown;
            }
        }
        // Equip/unequip sample equipment with keys 6-9, 0, O, P
        if (Keyboard.GetState().IsKeyDown(Keys.D6)) _player.Equip(_sampleEquipment[0]); // Weapon
        if (Keyboard.GetState().IsKeyDown(Keys.D7)) _player.Equip(_sampleEquipment[1]); // Head
        if (Keyboard.GetState().IsKeyDown(Keys.D8)) _player.Equip(_sampleEquipment[2]); // Body
        if (Keyboard.GetState().IsKeyDown(Keys.D9)) _player.Equip(_sampleEquipment[3]); // Arm
        if (Keyboard.GetState().IsKeyDown(Keys.D0)) _player.Equip(_sampleEquipment[4]); // Feet
        if (Keyboard.GetState().IsKeyDown(Keys.O)) _player.Equip(_sampleEquipment[5]); // AccessoryLeft
        if (Keyboard.GetState().IsKeyDown(Keys.P)) _player.Equip(_sampleEquipment[6]); // AccessoryRight
        if (Keyboard.GetState().IsKeyDown(Keys.U)) _player.Unequip(EquipmentType.Weapon);
        if (Keyboard.GetState().IsKeyDown(Keys.I)) _player.Unequip(EquipmentType.Head);
        if (Keyboard.GetState().IsKeyDown(Keys.J)) _player.Unequip(EquipmentType.Body);
        if (Keyboard.GetState().IsKeyDown(Keys.K)) _player.Unequip(EquipmentType.Arm);
        if (Keyboard.GetState().IsKeyDown(Keys.L)) _player.Unequip(EquipmentType.Feet);
        if (Keyboard.GetState().IsKeyDown(Keys.M)) _player.Unequip(EquipmentType.AccessoryLeft);
        if (Keyboard.GetState().IsKeyDown(Keys.N)) _player.Unequip(EquipmentType.AccessoryRight);

        // Stat allocation panel input (keyboard only for now)
        if (_showStatPanel && _gameState == GameState.Playing)
        {
            // 1-6 to select stat, +/- to add/remove, Enter to confirm
            for (int i = 0; i < 6; i++)
            {
                Keys plusKey = Keys.NumPad1 + i; // NumPad1-6
                Keys minusKey = Keys.D1 + i;     // D1-D6
                // Add point (NumPad1-6)
                if (keyboard.IsKeyDown(plusKey) && !_prevKeyboard.IsKeyDown(plusKey))
                {
                    if (_player.StatPoints > 0)
                    {
                        _pendingStatAlloc[i]++;
                    }
                }
                // Remove point (D1-D6)
                if (keyboard.IsKeyDown(minusKey) && !_prevKeyboard.IsKeyDown(minusKey))
                {
                    if (_pendingStatAlloc[i] > 0)
                    {
                        _pendingStatAlloc[i]--;
                    }
                }
            }
            // Confirm allocation (Enter)
            if (keyboard.IsKeyDown(Keys.Enter) && !_prevKeyboard.IsKeyDown(Keys.Enter))
            {
                for (int i = 0; i < 6; i++)
                {
                    if (_pendingStatAlloc[i] > 0)
                    {
                        _player.AllocateStat(_statNames[i], _pendingStatAlloc[i]);
                    }
                }
                Array.Clear(_pendingStatAlloc, 0, _pendingStatAlloc.Length);
            }
        }

        _prevKeyboard = keyboard;
        base.Update(gameTime);
    }

    private List<Skill> GenerateRandomSkills(int count)
    {
        var skills = new List<Skill>();
        var random = new Random();
        int minLevelGap = 8;
        int currentLevel = 1 + random.Next(1, 10); // Start between 2-10
        for (int i = 0; i < count; i++)
        {
            string name = $"Skill_{i + 1}";
            int unlockLevel = currentLevel;
            int basePower = random.Next(10, 101); // 10-100
            float skillMultiplier = (float)Math.Round(random.NextDouble() * 4 + 1, 2); // 1.00-5.00
            float intMultiplier = (float)Math.Round(random.NextDouble() * 2, 2); // 0.00-2.00
            float cooldown = 5f; // Fixed cooldown for all skills
            skills.Add(new Skill(name, unlockLevel, basePower, skillMultiplier, intMultiplier, cooldown));
            currentLevel += minLevelGap + random.Next(0, 5); // Next unlock at least 8+ levels later
        }
        return skills;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();
        if (_gameState == GameState.Menu)
        {
            int panelWidth = 500;
            int panelHeight = 300;
            int panelX = (GraphicsDevice.Viewport.Width - panelWidth) / 2;
            int panelY = (GraphicsDevice.Viewport.Height - panelHeight) / 2;
            SpriteBatchHelper.DrawMenuPanel(_spriteBatch, _font, _menuBg, panelWidth, panelHeight, panelX, panelY, _menuItems, _menuIndex);
            _spriteBatch.End();
            return;
        }
        if (_gameState == GameState.CharacterCreation)
        {
            int panelWidth = 600;
            int panelHeight = 350;
            int panelX = (GraphicsDevice.Viewport.Width - panelWidth) / 2;
            int panelY = (GraphicsDevice.Viewport.Height - panelHeight) / 2;
            SpriteBatchHelper.DrawCharacterCreationPanel(_spriteBatch, _font, _menuBg, panelWidth, panelHeight, panelX, panelY, _genderLabels, _genderColors, _charCreateGenderIndex, _charCreateName, GraphicsDevice);
            _spriteBatch.End();
            return;
        }
        if (_gameState == GameState.Options)
        {
            SpriteBatchHelper.DrawOptionsPanel(_spriteBatch, _font);
            _spriteBatch.End();
            return;
        }
        if (_player != null && _font != null)
        {
            // Removed stats, skills, and name display for a cleaner UI
        }
        if (_lastUsedSkill != null)
        {
            string used = $"Last Skill: {_lastUsedSkill.Name} | Damage: {_lastSkillDamage}";
            _spriteBatch.DrawString(_font, used, new Vector2(20, 500), Color.Orange);
        }
        if (_gameState == GameState.Playing)
        {
            if (_showStatPanel)
            {
                // Only show the stat panel, not the game scene, when stat panel is open
                SpriteBatchHelper.DrawStatPanel(_spriteBatch, _font, _menuBg, _player, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, GraphicsDevice, ref _uiLineTex, _pendingStatAlloc);
            }
            else
            {
                SpriteBatchHelper.DrawPlayerHUD(_spriteBatch, _font, _player, _tileTex, GraphicsDevice);
                _map.Draw(_spriteBatch, _tileTex, _camera);
                _playerEntity.Draw(_spriteBatch, _playerTex, _camera);
            }
        }
        _spriteBatch.End();
    }
}
