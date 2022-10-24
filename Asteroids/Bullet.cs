using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class Bullet : Sprite
    {
        public float Velocity;
        public bool Aligned = false;
        public Bullet(Vector2 position, float velocity, Texture2D image, float rot, float scale, Color color) : base(position, image, rot, scale, color)
        {
            Position = position;
            Velocity = velocity;
            Image = image;
            Rotation = rot;
            Scale = scale;
            Color = color;
        }

        public void Move()
        {
            if (!Aligned)
            {
                Position = new Vector2(Position.X + (float)Math.Sin(Rotation) * 17f, Position.Y - (float)Math.Cos(Rotation) * 17f);
                Aligned = true;
            }
            else
            {
                Position = new Vector2(Position.X + (float)Math.Sin(Rotation) * Velocity, Position.Y - (float)Math.Cos(Rotation) * Velocity);
            }


        }

        static public Bullet BulletTypeCopy(Bullet shot, Vector2 spot, float rotat)
        {
            Bullet newshot = new Bullet(spot, shot.Velocity, shot.Image, rotat, shot.Scale, shot.Color);
            return newshot;
        }
    }
}
//u can use a drawline for like a laser sight