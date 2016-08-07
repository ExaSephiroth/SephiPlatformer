using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SephiPlatformer
{
    public class Sprite
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { set; get; }
        public SpriteBatch SpriteBatch { get; set; }

        public float angle;

        public Sprite(Texture2D texture, Vector2 position, SpriteBatch batch)
        {
            Texture = texture;
            Position = position;
            SpriteBatch = batch;
        }

        public virtual void Draw()
        {
            SpriteBatch.Draw(Texture, Position, Color.White);
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y,
                          Texture.Width, Texture.Height);
            }
        }
    }
}
