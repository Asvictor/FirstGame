using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame.Entities;

public class Map
{
    public int[,] Tiles { get; private set; }
    public int Width => Tiles.GetLength(1);
    public int Height => Tiles.GetLength(0);
    public int TileSize { get; } = 48;
    public int GroundHeight { get; } = 64; // Thicker ground bar

    public Map(int[,] tiles)
    {
        Tiles = tiles;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D tileTex, Vector2 camera)
    {
        for (int y = 0; y < Height; y++)
        for (int x = 0; x < Width; x++)
        {
            if (Tiles[y, x] == 1)
            {
                Vector2 pos = new Vector2(x * TileSize, y * TileSize) - camera;
                int height = (y == Height - 1) ? GroundHeight : TileSize;
                spriteBatch.Draw(tileTex, new Rectangle((int)pos.X, (int)pos.Y + (TileSize - height), TileSize, height), Color.White);
            }
        }
    }
}
