using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Input.Spatial;

namespace Asteroid
{
    class Sprite
    {
        public Vector2 Position;
        public Texture2D Image { get; set; }
        public Texture2D DisplayImage { get; set; }
        public float Scale { get; set; }
        public virtual Vector2 Origin { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; }

        public Sprite(Vector2 position, Texture2D image, float rot, float scale, Color color)
        {
            Position = position;
            Image = image;
            Rotation = rot;
            Scale = scale;
            Color = color;

            Origin = new Vector2(Image.Width/2, Image.Height/2);
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

        public float Direction
        {
            get
            {
                int a = (int)Math.Floor(MathHelper.ToDegrees(Rotation)/360);

                double rot = MathHelper.ToDegrees(Rotation) - a * 360;

                if ((rot>0 && rot<=180)||(rot<-180))
                {
                    return 1;
                }
                else if ((rot<0 && rot>=-180)||(rot>180))
                {
                    return -1;
                }

                return -1;
            }
        }


        public virtual void Draw(SpriteBatch sb)
        {
            //sb.Draw(Image, Hitbox, Color);

            Texture2D pic = Image;
            Vector2 origin = Origin;
            if (DisplayImage!=null) 
            {
                pic = DisplayImage;
                origin = new Vector2(DisplayImage.Width/2, DisplayImage.Height/2);
            }

            sb.Draw(pic, Position, null, Color, Rotation, origin, Scale, SpriteEffects.None, 0);
        }
        public virtual void DrawSpecial(SpriteBatch sb, SpriteEffects effect)
        {
            //sb.Draw(Image, Hitbox, Color);

            Texture2D pic = Image;
            Vector2 origin = Origin;
            if (DisplayImage != null)
            {
                pic = DisplayImage;
                origin = new Vector2(DisplayImage.Width/2, DisplayImage.Height/2);
            }

            sb.Draw(pic, Position, null, Color, Rotation, origin, Scale, effect, 0);
        }

    }
}
