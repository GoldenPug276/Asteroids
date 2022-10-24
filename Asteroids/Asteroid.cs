using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class Asteroid : Sprite
    {
        public Game1.Size leSize;
        public Vector2 Velocity;

        public override Vector2 Origin
        {
            get
            {
                return new Vector2(Image.Width / 2, Image.Height / 2);
            }
        }

        public Asteroid(Vector2 position, Vector2 velocity, Texture2D image, float rot, float scale, Color color, Game1.Size size) : base(position, image, rot, scale, color)
        {
            Position = position;
            Velocity = velocity;
            Image = image;
            Rotation = rot;
            Scale = scale;
            Color = color;
            leSize = size;
        }

        private static Vector2 SpawnSpot(Rectangle playSpace, Texture2D Image)
        {
            Random rand = new Random();
            int width = playSpace.Width;
            int height = playSpace.Height;
            Vector2 spawn = new Vector2(rand.Next(-150, width + 150), rand.Next(-150, height + 150));
            Rectangle Thing = new Rectangle((int)spawn.X, (int)spawn.Y, Image.Width, Image.Height);

            while(playSpace.Intersects(Thing))
            {
                spawn = new Vector2(rand.Next(-150, width + 150), rand.Next(-150, height + 150));
                Thing = new Rectangle((int)spawn.X, (int)spawn.Y, Image.Width, Image.Height);
            }

            return spawn;
        }
        public static Asteroid NaturalSpawn(Rectangle playSpace, float velocity, Texture2D Image, Game1.Size Size)
        {
            Vector2 Velocity = new Vector2(velocity, velocity);
            Vector2 SpawnLocation = SpawnLocation = SpawnSpot(playSpace, Image);

            if (SpawnLocation.X>=0)
            {
                Velocity.X *= -1;
            }
            if (SpawnLocation.Y>=0)
            {
                Velocity.Y *= -1;
            }

            Asteroid New = new Asteroid(SpawnLocation, Velocity, Image, 0, 1 / 1f, Color.White, Size);
            return New;
        }

        public static Asteroid InitialSpawn(Rectangle playSpace, float lVelocity, Texture2D largeImage, Ship ship)
        {
            Asteroid New;
            int width = playSpace.Width;
            int height = playSpace.Height;
            Random rand = new Random();
            Vector2 SpawnLocation = new Vector2(0, 0);
            Vector2 Velocity = new Vector2(lVelocity, lVelocity);


            SpawnLocation = new Vector2(rand.Next(0, width + 1), rand.Next(0, height + 1));
            while (playSpace.Contains(SpawnLocation) == false) //if boolean explanation
            {
                SpawnLocation = new Vector2(rand.Next(0, width + 1), rand.Next(0, height + 1));
            }

            int direction = rand.Next(-1, 2);
            while (direction == 0)
            {
                direction = rand.Next(-1, 2);
            }
            Velocity.X *= direction;

            direction = rand.Next(-1, 2);
            while (direction == 0)
            {
                direction = rand.Next(-1, 2);
            }
            Velocity.Y *= direction;

            New = new Asteroid(SpawnLocation, Velocity, largeImage, 0, 1 / 1f, Color.White, Game1.Size.LeChonk);

            if(New.Hitbox.Intersects(ship.Hitbox))
            {
                SpawnLocation.X += 60;
            }

            return New;
        }

        public void IsInBounds(Rectangle playSpace)
        {
            int width = playSpace.Width;
            int height = playSpace.Height;

            if (!playSpace.Intersects(Hitbox))
            {
                if (Position.X < 0)
                {
                    Position = new Vector2(width, Position.Y);
                }
                else if (Position.Y < 0)
                {
                    Position = new Vector2(Position.X, height);
                }
                else if (Position.X + Image.Width >= width)
                {
                    Position = new Vector2(0, Position.Y);
                }
                else if (Position.Y + Image.Height >= height)
                {
                    Position = new Vector2(Position.X, 0);
                }
            }
        }
    }
}
