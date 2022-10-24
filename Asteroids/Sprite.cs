using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class Sprite
    {
        public Vector2 Position;
        public Texture2D Image { get; set; }
        public float Scale { get; set; }
        public virtual Vector2 Origin { get; }
        public float Rotation { get; set; }
        public Color Color { get; set; }

        public Sprite(Vector2 position, Texture2D image, float rot, float scale, Color color)
        {
            Position = position;
            Image = image;
            Rotation = rot;
            Scale = scale;
            Color = color;

            Origin = new Vector2(Image.Width / 2, Image.Height / 2);
        }

        public Rectangle Hitbox
        { 
            get
            {
                return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)(Image.Width * Scale), (int)(Image.Height * Scale));
            }
        }


        public virtual void Draw(SpriteBatch sb)
        {
            //sb.Draw(Image, Hitbox, Color);
            sb.Draw(Image, Position, null, Color, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }

    }
}
