using FirstGame.Core;
using FirstGame.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FirstGame.Entities;

public class PlayerEntity
{
    public Vector2 Position;
    public Vector2 Velocity;
    public bool OnGround;
    public int Width = 32;
    public int Height = 48;
    public Gender Gender { get; set; } = Gender.Male;

    public void Update(Map map, float delta)
    {
        // Input
        var k = Keyboard.GetState();
        float speed = 180f;
        if (k.IsKeyDown(Keys.Left) || k.IsKeyDown(Keys.A)) Velocity.X = -speed;
        else if (k.IsKeyDown(Keys.Right) || k.IsKeyDown(Keys.D)) Velocity.X = speed;
        else Velocity.X = 0;
        if ((k.IsKeyDown(Keys.Space) || k.IsKeyDown(Keys.Up) || k.IsKeyDown(Keys.W)) && OnGround)
        {
            Velocity.Y = -520f; // Increased jump strength
            OnGround = false;
        }
        // Gravity
        Velocity.Y += 900f * delta;
        // Move and collide with map
        Vector2 next = Position + Velocity * delta;
        Rectangle nextRect = new((int)next.X, (int)next.Y, Width, Height);
        OnGround = false;
        for (int y = 0; y < map.Height; y++)
        for (int x = 0; x < map.Width; x++)
        {
            if (map.Tiles[y, x] == 1)
            {
                int tileHeight = (y == map.Height - 1) ? map.GroundHeight : map.TileSize;
                int tileTop = y * map.TileSize + (map.TileSize - tileHeight);
                Rectangle tileRect = new(x * map.TileSize, tileTop, map.TileSize, tileHeight);
                if (nextRect.Intersects(tileRect))
                {
                    // Simple ground collision
                    if (Position.Y + Height <= tileRect.Top && Velocity.Y > 0)
                    {
                        next.Y = tileRect.Top - Height;
                        Velocity.Y = 0;
                        OnGround = true;
                    }
                }
            }
        }
        // Clamp to bottom of map (use ground height)
        float maxY = map.Height * map.TileSize - map.GroundHeight - Height;
        if (next.Y > maxY)
        {
            next.Y = maxY;
            Velocity.Y = 0;
            OnGround = true;
        }
        Position = next;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D tex, Vector2 camera)
    {
        // Draw stick figure based on gender
        int drawX = (int)(Position.X - camera.X + Width / 2);
        int drawY = (int)(Position.Y - camera.Y + Height / 2);
        SpriteBatchHelper.DrawStickFigure(spriteBatch.GraphicsDevice, spriteBatch, drawX, drawY, Height, Color.Orange, Gender == Gender.Female);
    }
}
