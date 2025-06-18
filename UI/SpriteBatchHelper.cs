using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
}
