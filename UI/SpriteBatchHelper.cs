using FirstGame.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FirstGame.UI;

public static class SpriteBatchHelper
{
    private static Texture2D _circleTex;
    private static Texture2D _lineTex;
    private static Texture2D _triangleTex;

    public static void DrawCircle(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y, int radius, Color color)
    {
        if (_circleTex == null || _circleTex.Width != radius * 2)
        {
            _circleTex = new Texture2D(device, radius * 2, radius * 2);
            Color[] data = new Color[radius * 2 * radius * 2];
            for (int i = 0; i < radius * 2; i++)
            for (int j = 0; j < radius * 2; j++)
            {
                int dx = i - radius;
                int dy = j - radius;
                if (dx * dx + dy * dy <= radius * radius)
                    data[j * radius * 2 + i] = color;
                else
                    data[j * radius * 2 + i] = Color.Transparent;
            }
            _circleTex.SetData(data);
        }
        spriteBatch.Draw(_circleTex, new Vector2(x - radius, y - radius), Color.White);
    }

    public static void DrawStickFigure(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y, int size, Color color, bool isFemale)
    {
        // Head
        DrawCircle(device, spriteBatch, x, y - size / 4, size / 4, color);
        // Body (line)
        DrawLine(device, spriteBatch, x, y, x, y + size / 2, color, 4);
        // Arms
        DrawLine(device, spriteBatch, x, y + size / 8, x - size / 4, y + size / 4, color, 3);
        DrawLine(device, spriteBatch, x, y + size / 8, x + size / 4, y + size / 4, color, 3);
        // Legs
        DrawLine(device, spriteBatch, x, y + size / 2, x - size / 6, y + size, color, 3);
        DrawLine(device, spriteBatch, x, y + size / 2, x + size / 6, y + size, color, 3);
        // Female: draw a triangle skirt
        if (isFemale)
        {
            DrawTriangle(device, spriteBatch, x, y + size / 2, size / 3, color * 0.7f);
        }
    }

    public static void DrawLine(GraphicsDevice device, SpriteBatch spriteBatch, int x1, int y1, int x2, int y2, Color color, int thickness)
    {
        if (_lineTex == null)
        {
            _lineTex = new Texture2D(device, 1, 1);
            _lineTex.SetData(new[] { Color.White });
        }
        float dx = x2 - x1;
        float dy = y2 - y1;
        float length = (float)Math.Sqrt(dx * dx + dy * dy);
        float angle = (float)Math.Atan2(dy, dx);
        spriteBatch.Draw(_lineTex, new Vector2(x1, y1), null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
    }

    public static void DrawTriangle(GraphicsDevice device, SpriteBatch spriteBatch, int x, int y, int size, Color color)
    {
        if (_triangleTex == null || _triangleTex.Width != size)
        {
            _triangleTex = new Texture2D(device, size, size);
            Color[] data = new Color[size * size];
            for (int j = 0; j < size; j++)
            for (int i = 0; i < size; i++)
            {
                if (i >= size / 2 - j / 2 && i <= size / 2 + j / 2)
                    data[j * size + i] = color;
                else
                    data[j * size + i] = Color.Transparent;
            }
            _triangleTex.SetData(data);
        }
        spriteBatch.Draw(_triangleTex, new Vector2(x - size / 2, y), Color.White);
    }

    public static void DrawUILine(GraphicsDevice device, ref Texture2D lineTex, SpriteBatch spriteBatch, Vector2 p1, Vector2 p2, Color color, int thickness)
    {
        if (lineTex == null)
        {
            lineTex = new Texture2D(device, 1, 1);
            lineTex.SetData(new[] { Color.White });
        }
        float dx = p2.X - p1.X;
        float dy = p2.Y - p1.Y;
        float length = (float)Math.Sqrt(dx * dx + dy * dy);
        float angle = (float)Math.Atan2(dy, dx);
        spriteBatch.Draw(lineTex, p1, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
    }

    public static void DrawHexagon(GraphicsDevice device, ref Texture2D hexTex, SpriteBatch spriteBatch, int cx, int cy, int radius, Color color)
    {
        if (hexTex == null || hexTex.Width != radius * 2)
        {
            hexTex = new Texture2D(device, radius * 2, radius * 2);
            Color[] data = new Color[radius * 2 * radius * 2];
            for (int y = 0; y < radius * 2; y++)
            for (int x = 0; x < radius * 2; x++)
            {
                double dx = x - radius;
                double dy = y - radius;
                double angle = Math.Atan2(dy, dx);
                double dist = Math.Sqrt(dx * dx + dy * dy);
                double a = Math.Abs((angle * 180 / Math.PI + 360) % 60 - 30);
                if (dist <= radius && a < 30)
                    data[y * radius * 2 + x] = color;
                else
                    data[y * radius * 2 + x] = Color.Transparent;
            }
            hexTex.SetData(data);
        }
        spriteBatch.Draw(hexTex, new Vector2(cx - radius, cy - radius), Color.White);
    }

    public static void DrawMenuPanel(SpriteBatch spriteBatch, SpriteFont font, Texture2D bg, int width, int height, int x, int y, string[] menuItems, int menuIndex)
    {
        spriteBatch.Draw(bg, new Rectangle(x, y, width, height), Color.White);
        string title = "FIRST PLATFORMER";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2((width - titleSize.X) / 2 + x, y + 30);
        spriteBatch.DrawString(font, title, titlePos + new Vector2(2,2), Color.Black);
        spriteBatch.DrawString(font, title, titlePos, Color.Cyan);
        for (int i = 0; i < menuItems.Length; i++)
        {
            string item = menuItems[i];
            Vector2 size = font.MeasureString(item);
            Vector2 pos = new Vector2((width - size.X) / 2 + x, y + 100 + i * 50);
            if (i == menuIndex)
            {
                spriteBatch.DrawString(font, item, pos + new Vector2(2,2), Color.Black, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, item, pos, Color.Yellow, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.DrawString(font, item, pos, Color.White);
            }
        }
    }

    public static void DrawCharacterCreationPanel(SpriteBatch spriteBatch, SpriteFont font, Texture2D bg, int width, int height, int x, int y, string[] genderLabels, Color[] genderColors, int genderIndex, string name, GraphicsDevice device)
    {
        spriteBatch.Draw(bg, new Rectangle(x, y, width, height), Color.White);
        string title = "Create Your Character";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2((width - titleSize.X) / 2 + x, y + 30);
        spriteBatch.DrawString(font, title, titlePos + new Vector2(2,2), Color.Black);
        spriteBatch.DrawString(font, title, titlePos, Color.Cyan);
        string genderLabel = "Choose Gender:";
        Vector2 genderLabelSize = font.MeasureString(genderLabel);
        Vector2 genderLabelPos = new Vector2((width - genderLabelSize.X) / 2 + x, y + 100);
        spriteBatch.DrawString(font, genderLabel, genderLabelPos, Color.White);
        int iconY = (int)genderLabelPos.Y + 50;
        int iconSpacing = 180;
        for (int i = 0; i < genderLabels.Length; i++)
        {
            int iconX = (width / 2) - iconSpacing / 2 + i * iconSpacing + x;
            Color color = genderColors[i];
            int size = (i == genderIndex) ? 96 : 72;
            DrawStickFigure(device, spriteBatch, iconX, iconY, size, color, i == 1);
            string label = genderLabels[i];
            Vector2 labelSize = font.MeasureString(label);
            spriteBatch.DrawString(font, label, new Vector2(iconX - labelSize.X / 2, iconY + size / 2 + 10), i == genderIndex ? Color.Yellow : Color.White);
        }
        string nameLabel = "Enter Name:";
        Vector2 nameLabelSize = font.MeasureString(nameLabel);
        Vector2 nameLabelPos = new Vector2((width - nameLabelSize.X) / 2 + x, iconY + 100);
        spriteBatch.DrawString(font, nameLabel, nameLabelPos, Color.White);
        string nameInput = name + (DateTime.Now.Millisecond % 1000 < 500 ? "_" : "");
        Vector2 nameInputSize = font.MeasureString(nameInput);
        Vector2 nameInputPos = new Vector2((width - nameInputSize.X) / 2 + x, nameLabelPos.Y + 40);
        spriteBatch.DrawString(font, nameInput, nameInputPos, Color.Yellow);
    }

    public static void DrawOptionsPanel(SpriteBatch spriteBatch, SpriteFont font)
    {
        spriteBatch.DrawString(font, "Options (Press ESC to return)", new Vector2(100, 100), Color.Yellow);
    }

    public static void DrawStatPanel(SpriteBatch spriteBatch, SpriteFont font, Texture2D bg, Player player, int panelW, int panelH, GraphicsDevice device, ref Texture2D lineTex, int[] pendingStatAlloc)
    {
        int panelX = 0, panelY = 0;
        int leftW = panelW / 2;
        int rightW = panelW - leftW;
        int rightX = leftW;
        // Draw left and right backgrounds (light/dark, not transparent)
        Color leftBg = new Color(220, 220, 230); // light gray-blue
        Color rightBg = new Color(40, 45, 60);   // dark blue-gray
        spriteBatch.Draw(bg, new Rectangle(panelX, panelY, leftW, panelH), leftBg);
        spriteBatch.Draw(bg, new Rectangle(panelX + leftW, panelY, rightW, panelH), rightBg);
        // --- Player Info Panel (left side) ---
        int infoPanelW = leftW - 32;
        int infoPanelH = 320;
        int infoPanelX = panelX + 16;
        int infoPanelY = panelH / 2 - infoPanelH / 2;
        Rectangle infoPanelRect = new Rectangle(infoPanelX, infoPanelY, infoPanelW, infoPanelH);
        // Draw info panel background with shadow
        spriteBatch.Draw(bg, new Rectangle(infoPanelRect.X + 8, infoPanelRect.Y + 8, infoPanelRect.Width, infoPanelRect.Height), new Color(0,0,0,40));
        spriteBatch.Draw(bg, infoPanelRect, new Color(255,255,255,245));
        // Player avatar (circle)
        int avatarRadius = 48;
        int avatarX = infoPanelX + 48;
        int avatarY = infoPanelY + 48;
        DrawCircle(device, spriteBatch, avatarX, avatarY, avatarRadius, Color.Orange);
        // Player name
        string playerName = player.Name ?? "Player";
        Vector2 nameSize = font.MeasureString(playerName);
        spriteBatch.DrawString(font, playerName, new Vector2(avatarX + avatarRadius + 18, avatarY - 18), Color.DodgerBlue);
        // Player level
        string levelStr = $"Lv {player.Level}";
        spriteBatch.DrawString(font, levelStr, new Vector2(avatarX + avatarRadius + 18, avatarY + 12), Color.DarkSlateGray);
        // HP bar
        int hpBarW = infoPanelW - 48;
        int hpBarH = 22;
        int hpBarX = infoPanelX + 24;
        int hpBarY = avatarY + avatarRadius + 24;
        int maxHp = Math.Max(1, player.TotalVIT * 10);
        int curHp = player.CurrentHP;
        spriteBatch.Draw(bg, new Rectangle(hpBarX, hpBarY, hpBarW, hpBarH), Color.LightGray);
        int hpBarFill = (int)(hpBarW * (curHp / (float)maxHp));
        spriteBatch.Draw(bg, new Rectangle(hpBarX, hpBarY, hpBarFill, hpBarH), Color.Red);
        string hpLabel = $"HP: {curHp} / {maxHp}";
        Vector2 hpLabelSize = font.MeasureString(hpLabel);
        spriteBatch.DrawString(font, hpLabel, new Vector2(hpBarX + (hpBarW - hpLabelSize.X)/2, hpBarY + (hpBarH - hpLabelSize.Y)/2), Color.White);
        // Stat points
        string spLabel = $"Stat Points: {player.StatPoints}";
        Vector2 spLabelSize = font.MeasureString(spLabel);
        spriteBatch.DrawString(font, spLabel, new Vector2(infoPanelX + (infoPanelW - spLabelSize.X)/2, hpBarY + hpBarH + 24), Color.DarkOrange);
        // --- Hex Star (Radar Chart) - upper right ---
        int starRadius = 110;
        int starCenterX = rightX + rightW / 2;
        int starCenterY = 90; // Move further up
        string[] statNames = { "STR", "VIT", "AGI", "INT", "DEX", "LUK" };
        int[] statValues = { player.STR, player.VIT, player.AGI, player.INT, player.DEX, player.LUK };
        Color[] statColors = { Color.Red, Color.Green, Color.Yellow, Color.Blue, Color.Cyan, Color.Purple };
        int statCount = statNames.Length;
        double angleStep = Math.PI * 2 / statCount;
        Vector2[] points = new Vector2[statCount];
        Vector2[] outerPoints = new Vector2[statCount];
        Vector2 center = new Vector2(starCenterX, starCenterY);
        for (int i = 0; i < statCount; i++)
        {
            double angle = i * angleStep - Math.PI / 2;
            float statRatio = MathHelper.Clamp(statValues[i] / 20f, 0.2f, 1f);
            points[i] = new Vector2(starCenterX + (float)Math.Cos(angle) * starRadius * statRatio, starCenterY + (float)Math.Sin(angle) * starRadius * statRatio);
            outerPoints[i] = new Vector2(starCenterX + (float)Math.Cos(angle) * starRadius, starCenterY + (float)Math.Sin(angle) * starRadius);
        }
        // Draw shadow for the stat chart area
        int shadowOffset = 12;
        Rectangle statArea = new Rectangle(starCenterX - starRadius - 20, starCenterY - starRadius - 40, starRadius * 2 + 40, starRadius * 2 + 80);
        spriteBatch.Draw(bg, new Rectangle(statArea.X + shadowOffset, statArea.Y + shadowOffset, statArea.Width, statArea.Height), new Color(0,0,0,60));
        spriteBatch.Draw(bg, statArea, new Color(255,255,255,240));
        // Draw concentric hexagons for grid (with more subtle color)
        int gridRings = 4;
        for (int ring = 1; ring <= gridRings; ring++)
        {
            float r = starRadius * ring / gridRings;
            Vector2[] gridPts = new Vector2[statCount];
            for (int i = 0; i < statCount; i++)
            {
                double angle = i * angleStep - Math.PI / 2;
                gridPts[i] = new Vector2(starCenterX + (float)Math.Cos(angle) * r, starCenterY + (float)Math.Sin(angle) * r);
            }
            for (int i = 0; i < statCount; i++)
            {
                int next = (i + 1) % statCount;
                DrawUILine(device, ref lineTex, spriteBatch, gridPts[i], gridPts[next], Color.LightBlue * 0.25f, 2);
            }
        }
        // Draw lines from center to each stat point (lighter)
        for (int i = 0; i < statCount; i++)
        {
            DrawUILine(device, ref lineTex, spriteBatch, center, points[i], Color.LightBlue * 0.4f, 2);
        }
        // Fill the star area with a semi-transparent blue
        Color fillColor = new Color(80, 180, 255, 80);
        for (int i = 0; i < statCount; i++)
        {
            int next = (i + 1) % statCount;
            DrawFilledTriangle(spriteBatch, device, center, points[i], points[next], fillColor);
        }
        // Draw the stat star outline
        for (int i = 0; i < statCount; i++)
        {
            int next = (i + 1) % statCount;
            DrawUILine(device, ref lineTex, spriteBatch, points[i], points[next], Color.DeepSkyBlue, 4);
        }
        // Draw stat circles and labels
        for (int i = 0; i < statCount; i++)
        {
            DrawCircle(device, spriteBatch, (int)points[i].X, (int)points[i].Y, 28, statColors[i]);
            // Find intersection with outer hexagon for label placement
            Vector2 dir = Vector2.Normalize(outerPoints[i] - center);
            float labelDist = starRadius + 18;
            Vector2 labelPos = center + dir * labelDist;
            string label = statNames[i];
            Vector2 labelSize = font.MeasureString(label);
            spriteBatch.DrawString(font, label, new Vector2(labelPos.X - labelSize.X / 2, labelPos.Y - labelSize.Y / 2), Color.DodgerBlue);
            // Stat value at the star point
            string value = statValues[i].ToString();
            Vector2 valueSize = font.MeasureString(value);
            spriteBatch.DrawString(font, value, new Vector2(points[i].X - valueSize.X / 2, points[i].Y - valueSize.Y / 2), Color.White);
        }
        // --- Stat Allocation UI (lower right, no overlap) ---
        int allocPanelW = 220;
        int allocPanelH = 320;
        int allocPanelX = rightX + rightW - allocPanelW - 32;
        int allocPanelY = panelH - allocPanelH - 40; // Move further down
        if (allocPanelY < starCenterY + starRadius + 32) allocPanelY = starCenterY + starRadius + 32; // Ensure no overlap
        Rectangle allocPanelRect = new Rectangle(allocPanelX, allocPanelY, allocPanelW, allocPanelH);
        // Draw allocation panel background with shadow and border
        spriteBatch.Draw(bg, new Rectangle(allocPanelRect.X + 8, allocPanelRect.Y + 8, allocPanelRect.Width, allocPanelRect.Height), new Color(0,0,0,40));
        spriteBatch.Draw(bg, allocPanelRect, new Color(245, 250, 255, 245));
        // Border
        DrawRectOutline(spriteBatch, allocPanelRect, Color.LightSkyBlue, 2);
        // Draw title
        string allocTitle = "Allocate Stats";
        Vector2 allocTitleSize = font.MeasureString(allocTitle);
        spriteBatch.DrawString(font, allocTitle, new Vector2(allocPanelX + (allocPanelW - allocTitleSize.X) / 2, allocPanelY + 18), Color.DodgerBlue);
        // Draw available points
        int availablePoints = player.StatPoints;
        string pointsLabel = $"Points: {availablePoints}";
        Vector2 pointsLabelSize = font.MeasureString(pointsLabel);
        spriteBatch.DrawString(font, pointsLabel, new Vector2(allocPanelX + (allocPanelW - pointsLabelSize.X) / 2, allocPanelY + 54), Color.DarkOrange);
        // Draw stat rows with icons and improved alignment
        int statRowH = 40;
        int statStartY = allocPanelY + 90;
        int btnSize = 28;
        int iconSize = 22;
        int iconX = allocPanelX + 14;
        int statNameX = iconX + iconSize + 8;
        int valueX = allocPanelX + 100;
        int minusBtnX = allocPanelX + 150;
        int plusBtnX = allocPanelX + 190;
        MouseState mouse = Mouse.GetState();
        bool mouseClicked = mouse.LeftButton == ButtonState.Pressed;
        for (int i = 0; i < statCount; i++)
        {
            int rowY = statStartY + i * statRowH;
            Rectangle rowRect = new Rectangle(allocPanelX + 8, rowY, allocPanelW - 16, statRowH - 4);
            // Highlight row on hover
            if (rowRect.Contains(mouse.Position))
                spriteBatch.Draw(bg, rowRect, new Color(200, 230, 255, 60));
            // Stat icon (SVG-like vector shape, pulse if pending)
            int iconY = rowY + (statRowH - iconSize) / 2 + iconSize / 2;
            float pulse = 1f;
            int pending = pendingStatAlloc != null && i < pendingStatAlloc.Length ? pendingStatAlloc[i] : 0;
            if (pending != 0)
            {
                double t = (DateTime.Now.TimeOfDay.TotalSeconds * 4 + i) % (2 * Math.PI);
                pulse = 1.0f + 0.13f * (float)Math.Sin(t);
            }
            int iconDrawX = iconX + iconSize / 2;
            int iconDrawY = iconY;
            int drawSize = (int)(iconSize * pulse);
            Color iconColor = statColors[i];
            DrawStatIcon(device, spriteBatch, i, iconDrawX, iconDrawY, drawSize, iconColor);
            // Stat name
            string stat = statNames[i];
            Vector2 statLabelSize = font.MeasureString(stat);
            int statNameY = rowY + (statRowH - (int)statLabelSize.Y) / 2;
            spriteBatch.DrawString(font, stat, new Vector2(statNameX, statNameY), statColors[i]);
            // Stat value (current + pending)
            int baseValue = statValues[i];
            string valueStr = baseValue.ToString();
            Color valueColor = Color.Black;
            if (pending > 0)
            {
                valueStr += $"  ";
                valueStr += $"(+{pending})";
                valueColor = Color.DarkGreen;
            }
            else if (pending < 0)
            {
                valueStr += $"  ";
                valueStr += $"({pending})";
                valueColor = Color.IndianRed;
            }
            Vector2 valueSize = font.MeasureString(valueStr);
            int valueY = rowY + (statRowH - (int)valueSize.Y) / 2;
            spriteBatch.DrawString(font, valueStr, new Vector2(valueX, valueY), valueColor);
            // Draw - button with animated feedback
            int minusBtnY = rowY + (statRowH - btnSize) / 2;
            Rectangle minusBtn = new Rectangle(minusBtnX, minusBtnY, btnSize, btnSize);
            bool minusHover = minusBtn.Contains(mouse.Position);
            float minusScale = minusHover ? 1.15f : 1f;
            Color minusColor = minusHover ? new Color(180,200,220) : new Color(220,220,220);
            if (minusHover && Mouse.GetState().LeftButton == ButtonState.Pressed)
                minusScale = 1.25f;
            DrawCircle(device, spriteBatch, minusBtn.X + btnSize/2, minusBtn.Y + btnSize/2, (int)(btnSize/2 * minusScale), minusColor);
            spriteBatch.DrawString(font, "-", new Vector2(minusBtn.X + btnSize/2 - font.MeasureString("-").X/2, minusBtn.Y + btnSize/2 - font.MeasureString("-").Y/2), Color.DimGray);
            if (minusHover)
                DrawTooltip(spriteBatch, font, "Remove point", mouse.Position.ToVector2() + new Vector2(16, 0));
            // Draw + button with animated feedback
            int plusBtnY = rowY + (statRowH - btnSize) / 2;
            Rectangle plusBtn = new Rectangle(plusBtnX, plusBtnY, btnSize, btnSize);
            bool plusHover = plusBtn.Contains(mouse.Position);
            float plusScale = plusHover ? 1.15f : 1f;
            Color plusColor = plusHover ? new Color(140,210,255) : new Color(200,230,255);
            if (plusHover && Mouse.GetState().LeftButton == ButtonState.Pressed)
                plusScale = 1.25f;
            DrawCircle(device, spriteBatch, plusBtn.X + btnSize/2, plusBtn.Y + btnSize/2, (int)(btnSize/2 * plusScale), plusColor);
            spriteBatch.DrawString(font, "+", new Vector2(plusBtn.X + btnSize/2 - font.MeasureString("+").X/2, plusBtn.Y + btnSize/2 - font.MeasureString("+").Y/2), Color.DodgerBlue);
            if (plusHover)
                DrawTooltip(spriteBatch, font, "Add point", mouse.Position.ToVector2() + new Vector2(16, 0));
        }
        // Draw confirm button with animated glow/scale
        int confirmBtnW = 140, confirmBtnH = 38;
        int confirmBtnX = allocPanelX + (allocPanelW - confirmBtnW) / 2;
        int confirmBtnY = allocPanelY + allocPanelH - confirmBtnH - 18;
        Rectangle confirmBtn = new Rectangle(confirmBtnX, confirmBtnY, confirmBtnW, confirmBtnH);
        bool confirmHover = confirmBtn.Contains(mouse.Position);
        float confirmPulse = confirmHover ? 1.08f + 0.04f * (float)Math.Sin(DateTime.Now.TimeOfDay.TotalSeconds * 8) : 1f;
        Color confirmGlow = confirmHover ? new Color(120,220,255,240) : new Color(80,180,255,220);
        int cW = (int)(confirmBtnW * confirmPulse);
        int cH = (int)(confirmBtnH * confirmPulse);
        int cX = confirmBtn.X - (cW - confirmBtnW)/2;
        int cY = confirmBtn.Y - (cH - confirmBtnH)/2;
        spriteBatch.Draw(bg, new Rectangle(cX + 3, cY + 3, cW, cH), new Color(0,0,0,30));
        spriteBatch.Draw(bg, new Rectangle(cX, cY, cW, cH), confirmGlow);
        DrawRectOutline(spriteBatch, new Rectangle(cX, cY, cW, cH), Color.DeepSkyBlue, 2);
        string confirmText = "Confirm";
        Vector2 confirmSize = font.MeasureString(confirmText);
        spriteBatch.DrawString(font, confirmText, new Vector2(cX + (cW - confirmSize.X)/2, cY + (cH - confirmSize.Y)/2), Color.White);
        if (confirmHover)
            DrawTooltip(spriteBatch, font, "Apply all pending stat points", mouse.Position.ToVector2() + new Vector2(16, 0));
    }

    public static void DrawPlayerHUD(SpriteBatch spriteBatch, SpriteFont font, Player player, Texture2D tileTex, GraphicsDevice device)
    {
        int iconSize = 64;
        int iconX = 30, iconY = 30 + iconSize / 2;
        DrawCircle(device, spriteBatch, iconX + iconSize / 2, iconY, iconSize / 2, Color.Orange);
        spriteBatch.DrawString(font, $"Lv {player.Level}", new Vector2(iconX + iconSize + 10, iconY - iconSize / 2), Color.White);
        int maxHp = Math.Max(1, player.TotalVIT * 10);
        int curHp = (int)(maxHp * 0.8f);
        int barW = 120, barH = 16;
        int barX = iconX + iconSize + 10, barY = iconY - iconSize / 2 + 30;
        spriteBatch.Draw(tileTex, new Rectangle(barX, barY, barW, barH), Color.DarkRed);
        int hpBarWidth = (int)(barW * (curHp / (float)maxHp));
        spriteBatch.Draw(tileTex, new Rectangle(barX, barY, hpBarWidth, barH), Color.Red);
    }

    // Draw a filled triangle using SpriteBatch (approximate by drawing a solid color texture)
    private static Texture2D _triangleFillTex;
    public static void DrawFilledTriangle(SpriteBatch spriteBatch, GraphicsDevice device, Vector2 p1, Vector2 p2, Vector2 p3, Color color)
    {
        if (_triangleFillTex == null)
        {
            _triangleFillTex = new Texture2D(device, 1, 1);
            _triangleFillTex.SetData(new[] { Color.White });
        }
        // Draw the triangle as three lines (approximation, since SpriteBatch can't natively fill triangles)
        // For a real fill, use a custom shader or a polygon batcher. Here, draw thin triangles by splitting into two right triangles.
        // We'll use a simple method: draw a filled polygon by drawing many thin triangles between p1 and the edge from p2 to p3.
        int steps = 32;
        for (int i = 0; i < steps; i++)
        {
            float t0 = i / (float)steps;
            float t1 = (i + 1) / (float)steps;
            Vector2 q0 = Vector2.Lerp(p2, p3, t0);
            Vector2 q1 = Vector2.Lerp(p2, p3, t1);
            Vector2[] tri = { p1, q0, q1 };
            DrawTriangleFan(spriteBatch, tri, color);
        }
    }

    private static void DrawTriangleFan(SpriteBatch spriteBatch, Vector2[] tri, Color color)
    {
        // Draw as three lines (not a real fill, but gives a visual effect)
        for (int i = 0; i < tri.Length; i++)
        {
            int next = (i + 1) % tri.Length;
            spriteBatch.Draw(_triangleFillTex, tri[i], null, color, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }

    // Draws a rectangle outline
    private static void DrawRectOutline(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness = 2)
    {
        if (_lineTex == null)
        {
            _lineTex = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _lineTex.SetData([Color.White]);
        }
        // Top
        spriteBatch.Draw(_lineTex, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom
        spriteBatch.Draw(_lineTex, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
        // Left
        spriteBatch.Draw(_lineTex, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right
        spriteBatch.Draw(_lineTex, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
    }

    // Draws a tooltip box with text
    private static void DrawTooltip(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 pos)
    {
        if (_lineTex == null)
        {
            _lineTex = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _lineTex.SetData(new[] { Color.White });
        }
        Vector2 size = font.MeasureString(text) + new Vector2(16, 8);
        Rectangle bgRect = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
        spriteBatch.Draw(_lineTex, bgRect, new Color(40, 45, 60, 240));
        DrawRectOutline(spriteBatch, bgRect, Color.LightSkyBlue, 2);
        spriteBatch.DrawString(font, text, pos + new Vector2(8, 4), Color.White);
    }

    // --- SVG-like stat icon drawing ---
    private static void DrawStatIcon(GraphicsDevice device, SpriteBatch spriteBatch, int statIndex, int x, int y, int size, Color color)
    {
        switch (statIndex)
        {
            case 0: // STR - Sword (triangle)
                DrawTriangle(device, spriteBatch, x, y, size, color);
                break;
            case 1: // VIT - Hexagon (shield)
                DrawHexagon(device, ref _lineTex, spriteBatch, x, y, size/2, color);
                break;
            case 2: // AGI - Wing (two triangles)
                DrawTriangle(device, spriteBatch, x - size/4, y, size/2, color);
                DrawTriangle(device, spriteBatch, x + size/4, y, size/2, color*0.7f);
                break;
            case 3: // INT - Book (rectangle with line)
                if (_lineTex == null)
                {
                    _lineTex = new Texture2D(device, 1, 1);
                    _lineTex.SetData(new[] { Color.White });
                }
                // Draw a colored rectangle (book)
                spriteBatch.Draw(_lineTex, new Rectangle(x - size/2, y - size/4, size, size/2), color);
                // Draw a white line (book spine)
                DrawLine(device, spriteBatch, x - size/2, y, x + size/2, y, Color.White, 2);
                break;
            case 4: // DEX - Arrow (triangle + line)
                DrawTriangle(device, spriteBatch, x, y - size/6, size/2, color);
                DrawLine(device, spriteBatch, x, y, x, y + size/2, color, 3);
                break;
            case 5: // LUK - Star (5-pointed)
                DrawStar(spriteBatch, device, x, y, size/2, size/4, 5, color);
                break;
            default:
                DrawCircle(device, spriteBatch, x, y, size/2, color);
                break;
        }
    }
    // Draws a star shape (for LUK)
    private static void DrawStar(SpriteBatch spriteBatch, GraphicsDevice device, int cx, int cy, int outerR, int innerR, int points, Color color)
    {
        Vector2[] pts = new Vector2[points * 2];
        double angleStep = Math.PI / points;
        for (int i = 0; i < points * 2; i++)
        {
            double angle = i * angleStep - Math.PI / 2;
            float r = (i % 2 == 0) ? outerR : innerR;
            pts[i] = new Vector2(cx + (float)Math.Cos(angle) * r, cy + (float)Math.Sin(angle) * r);
        }
        for (int i = 0; i < pts.Length; i++)
        {
            int next = (i + 1) % pts.Length;
            DrawUILine(device, ref _lineTex, spriteBatch, pts[i], pts[next], color, 2);
        }
    }
}
