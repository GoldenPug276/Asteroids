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
        public Texture2D DisplayImage { get; set; }
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
            set
            {
                HitboxRefresh();
            }
        }

        public Rectangle HitboxRefresh()
        {
            return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)(Image.Width * Scale), (int)(Image.Height * Scale));
        }


        public virtual void Draw(SpriteBatch sb)
        {
            //sb.Draw(Image, Hitbox, Color);

            Texture2D pic = Image;
            Vector2 origin = Origin;
            if (DisplayImage!=null) 
            {
                pic = DisplayImage;
                origin = new Vector2(DisplayImage.Width / 2, DisplayImage.Height / 2);
            }

            sb.Draw(pic, Position, null, Color, Rotation, origin, Scale, SpriteEffects.None, 0);
        }

    }
}
