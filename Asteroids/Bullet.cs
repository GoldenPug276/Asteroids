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
        public bool Burning;
        public float Penetration;

        public TimeSpan timeStopTravelTime = new TimeSpan(0, 0, 0, 0, 0);
        public Bullet(Vector2 position, float velocity, Texture2D image,float rot, float scale, Color color, float pen, bool burn) : base(position, image, rot, scale, color)
        {
            Position = position;
            Velocity = velocity;
            Image = image;
            Rotation = rot;
            Scale = scale;
            Color = color;
            Penetration = pen;
            Burning = burn;

            if (Game1.TimeHasStopped) { timeStopTravelTime = TimeSpan.FromMilliseconds(200); }
        }

        public void Move()
        {
            bool doMoving = true;

            if (Game1.TimeHasStopped)
            {
                timeStopTravelTime -= Game1.gameTime.ElapsedGameTime;
                if (timeStopTravelTime<=TimeSpan.Zero) { doMoving = false; }
            }

            if (doMoving==true)
            {
                float movement = Velocity;

                if (!Aligned)
                {
                    movement = 17f;
                    Aligned = true;
                }

                Position = new Vector2(Position.X + (float)Math.Sin(Rotation) * movement, Position.Y - (float)Math.Cos(Rotation) * movement);
            }
        }

        static public Bullet BulletTypeCopy(Bullet shot, Vector2 spot, float rotat)
        {
            Bullet newshot = new Bullet(spot, shot.Velocity, shot.Image, rotat, shot.Scale, shot.Color, shot.Penetration, shot.Burning);
            return newshot;
        }

        public void BulletPenInherit(Upgrade gun)
        {
            Penetration = gun.Penetration;
        }
    }
}
//u can use a drawline for like a laser sight