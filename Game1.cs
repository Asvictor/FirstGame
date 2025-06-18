using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FirstGame.UI;

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

    private enum GameState { Menu, CharacterCreation, Playing, Options }
    private GameState _gameState = GameState.Menu;
    private int _menuIndex = 0;
    private readonly string[] _menuItems = { "Start New Game", "Load Game", "Options" };
    private KeyboardState _prevKeyboard;

    private int _charCreateGenderIndex = 0;
    private string _charCreateName = "";
    private readonly string[] _genderLabels = { "Male", "Female" };
    private readonly Color[] _genderColors = { Color.CornflowerBlue, Color.Pink };

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
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
    }

    protected override void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        if (_gameState == GameState.Menu)
        {
            // Menu navigation
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

        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        // Update cooldown timers
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
            // Draw semi-transparent background panel
            int panelWidth = 500;
            int panelHeight = 300;
            int panelX = (GraphicsDevice.Viewport.Width - panelWidth) / 2;
            int panelY = (GraphicsDevice.Viewport.Height - panelHeight) / 2;
            _spriteBatch.Draw(_menuBg, new Rectangle(panelX, panelY, panelWidth, panelHeight), Color.White);
            // Draw title
            string title = "FIRST PLATFORMER";
            Vector2 titleSize = _font.MeasureString(title);
            Vector2 titlePos = new Vector2((GraphicsDevice.Viewport.Width - titleSize.X) / 2, panelY + 30);
            _spriteBatch.DrawString(_font, title, titlePos + new Vector2(2,2), Color.Black);
            _spriteBatch.DrawString(_font, title, titlePos, Color.Cyan);
            // Draw menu items centered
            for (int i = 0; i < _menuItems.Length; i++)
            {
                string item = _menuItems[i];
                Vector2 size = _font.MeasureString(item);
                Vector2 pos = new Vector2((GraphicsDevice.Viewport.Width - size.X) / 2, panelY + 100 + i * 50);
                if (i == _menuIndex)
                {
                    // Draw highlight (shadow + color + scale)
                    _spriteBatch.DrawString(_font, item, pos + new Vector2(2,2), Color.Black, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
                    _spriteBatch.DrawString(_font, item, pos, Color.Yellow, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
                }
                else
                {
                    _spriteBatch.DrawString(_font, item, pos, Color.White);
                }
            }
            _spriteBatch.End();
            return;
        }
        if (_gameState == GameState.CharacterCreation)
        {
            int panelWidth = 600;
            int panelHeight = 350;
            int panelX = (GraphicsDevice.Viewport.Width - panelWidth) / 2;
            int panelY = (GraphicsDevice.Viewport.Height - panelHeight) / 2;
            _spriteBatch.Draw(_menuBg, new Rectangle(panelX, panelY, panelWidth, panelHeight), Color.White);
            string title = "Create Your Character";
            Vector2 titleSize = _font.MeasureString(title);
            Vector2 titlePos = new Vector2((GraphicsDevice.Viewport.Width - titleSize.X) / 2, panelY + 30);
            _spriteBatch.DrawString(_font, title, titlePos + new Vector2(2,2), Color.Black);
            _spriteBatch.DrawString(_font, title, titlePos, Color.Cyan);
            // Gender selection
            string genderLabel = "Choose Gender:";
            Vector2 genderLabelSize = _font.MeasureString(genderLabel);
            Vector2 genderLabelPos = new Vector2((GraphicsDevice.Viewport.Width - genderLabelSize.X) / 2, panelY + 100);
            _spriteBatch.DrawString(_font, genderLabel, genderLabelPos, Color.White);
            int iconY = (int)genderLabelPos.Y + 50;
            int iconSpacing = 180;
            for (int i = 0; i < _genderLabels.Length; i++)
            {
                int iconX = (GraphicsDevice.Viewport.Width / 2) - iconSpacing / 2 + i * iconSpacing;
                Color color = _genderColors[i];
                int size = (i == _charCreateGenderIndex) ? 96 : 72;
                SpriteBatchHelper.DrawStickFigure(GraphicsDevice, _spriteBatch, iconX, iconY, size, color, i == 1);
                string label = _genderLabels[i];
                Vector2 labelSize = _font.MeasureString(label);
                _spriteBatch.DrawString(_font, label, new Vector2(iconX - labelSize.X / 2, iconY + size / 2 + 10), i == _charCreateGenderIndex ? Color.Yellow : Color.White);
            }
            // Name input
            string nameLabel = "Enter Name:";
            Vector2 nameLabelSize = _font.MeasureString(nameLabel);
            Vector2 nameLabelPos = new Vector2((GraphicsDevice.Viewport.Width - nameLabelSize.X) / 2, iconY + 100);
            _spriteBatch.DrawString(_font, nameLabel, nameLabelPos, Color.White);
            string nameInput = _charCreateName + (DateTime.Now.Millisecond % 1000 < 500 ? "_" : "");
            Vector2 nameInputSize = _font.MeasureString(nameInput);
            Vector2 nameInputPos = new Vector2((GraphicsDevice.Viewport.Width - nameInputSize.X) / 2, nameLabelPos.Y + 40);
            _spriteBatch.DrawString(_font, nameInput, nameInputPos, Color.Yellow);
            _spriteBatch.End();
            return;
        }
        if (_gameState == GameState.Options)
        {
            _spriteBatch.DrawString(_font, "Options (Press ESC to return)", new Vector2(100, 100), Color.Yellow);
            _spriteBatch.End();
            return;
        }
        if (_player != null && _font != null)
        {
            string stats = $"Name: {_player.Name}\nGender: {_player.Gender}\nLevel: {_player.Level}\nXP: {_player.Experience}/{_player.ExperienceToNextLevel}\nStat Points: {_player.StatPoints}\nSTR: {_player.STR}  VIT: {_player.VIT}  AGI: {_player.AGI}  INT: {_player.INT}  LUK: {_player.LUK}";
            _spriteBatch.DrawString(_font, stats, new Vector2(20, 20), Color.White);
            // Show unlocked skills
            var unlocked = _skills.FindAll(s => _player.Level >= s.UnlockLevel);
            string skillText = "Unlocked Skills:";
            foreach (var skill in unlocked)
            {
                string cd = skill.CooldownTimer > 0 ? $" (CD: {skill.CooldownTimer:F1}s)" : "";
                skillText += $"\n{skill.Name} (Lv{skill.UnlockLevel}) Pow:{skill.BasePower} Mult:{skill.SkillMultiplier} INTx:{skill.IntMultiplier} CD:{skill.Cooldown}s{cd}";
            }
            _spriteBatch.DrawString(_font, skillText, new Vector2(20, 180), Color.Yellow);
            // Show equipped items and total stats
            string equipText = $"\nEquipped:\nWeapon: {_player.Weapon?.Name ?? "-"}\nHead: {_player.Head?.Name ?? "-"}\nBody: {_player.Body?.Name ?? "-"}\nArm: {_player.Arm?.Name ?? "-"}\nFeet: {_player.Feet?.Name ?? "-"}\nAccL: {_player.AccessoryLeft?.Name ?? "-"}\nAccR: {_player.AccessoryRight?.Name ?? "-"}";
            equipText += $"\nTotal STR: {_player.TotalSTR}  VIT: {_player.TotalVIT}  AGI: {_player.TotalAGI}  INT: {_player.TotalINT}  LUK: {_player.TotalLUK}";
            _spriteBatch.DrawString(_font, equipText, new Vector2(400, 20), Color.LightGreen);
        }
        if (_lastUsedSkill != null)
        {
            string used = $"Last Skill: {_lastUsedSkill.Name} | Damage: {_lastSkillDamage}";
            _spriteBatch.DrawString(_font, used, new Vector2(20, 500), Color.Orange);
        }
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
