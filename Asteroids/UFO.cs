using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class UFO : Asteroid
    {
        Random rand = new Random();
        public TimeSpan ShotTimer;
        private TimeSpan reserveShotTimer;
        public UFO(Vector2 position, Vector2 velocity, Texture2D image, float rot, float scale, Color color, Game1.Size size, TimeSpan shotTimer) : base(position, velocity, image, rot, scale, color, size)
        {
            Position = position;
            Velocity = velocity;
            Image = image;
            Rotation = rot;
            Scale = scale;
            Color = color;
            leSize = size;
            ShotTimer = shotTimer;
            reserveShotTimer = ShotTimer;
        }

        private static Vector2 SpawnSpot(Rectangle playSpace, Texture2D Image)
        {
            Random rand = new Random();
            int width = playSpace.Width;
            int height = playSpace.Height;
            Vector2 spawn = new Vector2(rand.Next(-80, width + 80), rand.Next(-80, height + 80));
            Rectangle Thing = new Rectangle((int)spawn.X, (int)spawn.Y, Image.Width, Image.Height);

            while (playSpace.Intersects(Thing))
            {
                spawn = new Vector2(rand.Next(-80, width + 80), rand.Next(-80, height + 80));
                Thing = new Rectangle((int)spawn.X, (int)spawn.Y, Image.Width, Image.Height);
            }

            return spawn;
        }
        public static UFO NaturalSpawn(Rectangle playSpace, float velocity, Texture2D Image, Game1.Size size, TimeSpan shotTimer)
        {
            Vector2 Velocity = new Vector2(velocity, velocity);
            Vector2 SpawnLocation = SpawnLocation = SpawnSpot(playSpace, Image);

            if (SpawnLocation.X >= 0)
            {
                Velocity.X *= -1;
            }
            if (SpawnLocation.Y >= 0)
            {
                Velocity.Y *= -1;
            }

            UFO New = new UFO(SpawnLocation, Velocity, Image, 0, 1 / 1f, Color.White, size, shotTimer);
            return New;
        }

        Vector2 start;
        Vector2 destination;
        Vector2 between;

        public Bullet Shoot(Rectangle shotArea, Bullet shotCopy)
        {
            start = new Vector2(Position.X, Position.Y + Image.Height/2);
            destination = new Vector2(rand.Next(shotArea.X, shotArea.X+shotArea.Width+1), shotArea.Y);
            between = start - destination;

            double angle = Math.Atan2((double)between.Y, (double)between.X) - MathHelper.ToRadians(90);

            Bullet shot = new Bullet(start, shotCopy.Velocity, shotCopy.Image, (float)angle, 1 / 1f, Color.White);
            ShotTimer = reserveShotTimer;
            return shot;
        }
        public bool Update(TimeSpan gameTime)
        {
            bool a = false;
            ShotTimer -= gameTime;

            if (ShotTimer<=TimeSpan.Zero)
            {
                a = true;
            }

            return a;
        }
    }
}
