using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Timers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class Enemy : Sprite
    {
        public enum Type
        {
            Asteroid = 1,
            UFO = 2
        }
        public Type EType;

        //Original UFO and Asteroid Variables
        Random rand = new Random();
        public Game1.Size leSize;
        public Vector2 Velocity;
        public TimeSpan ShotTimer;
        public TimeSpan reserveShotTimer;
        //Original UFO and Asteroid Variables

        public bool HasCollided = false;
        public float ArmorValue;
        public Texture2D[] ArmorImages;

        //Time Stop Parameters
        public float stoppedDamage = 0;
        public bool broken = false;
        public float brokenTimes = 0;
        public int hits = 0;
        //Time Stop Parameters

        public Enemy(Vector2 position, Vector2 velocity, Texture2D image, float rot, float scale, Color color, Game1.Size size, TimeSpan shotTimer, Type type, float armorValue)
            : base(position, image, rot, scale, color)
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
            EType = type;
            ArmorValue = armorValue;
            armorUp();
            if (ArmorValue!=0) 
            {
                DisplayImage = ArmorImages[(int)Math.Ceiling(ArmorValue)];
            }
        }
        private void armorUp()
        {
            switch (EType)
            {
                case Type.Asteroid:
                    ArmorImages = Game1.AsteroidArmor;
                    if (leSize!=Game1.Size.LeChonk)  { ArmorValue = 0; } break;

                case Type.UFO:
                    if (leSize == Game1.Size.Normal) { ArmorImages = Game1.BigUFOArmor; }
                    if (leSize == Game1.Size.Baby)   { ArmorImages = Game1.SmallUFOArmor; } break;
            }
        }

        public bool ArmorDamage(float pen)
        {
            //true = armor still up
            //false = armor broken
            //upon the armor breaking, still return true until the next check so that an armor of 1 doesn't act as an armor of 0

            if (ArmorValue-pen<=0 && ArmorValue>0)
            {
                ArmorValue = 0;
                DisplayImage = null;
                return true;
            }
            
            ArmorValue -= pen;

            if (ArmorValue<=0)
            {
                ArmorValue = 0;
            }

            DisplayImage = ArmorImages[(int)Math.Ceiling(ArmorValue)];
            if (ArmorValue==0)
            {
                DisplayImage = null;
                return false;
            }

            return true;
        }

        public void TimeStopDamage()
        {
            if (!Game1.TimeHasStopped && stoppedDamage>0)
            {
                if (ArmorValue-stoppedDamage<0)
                {
                    brokenTimes = hits;
                    broken = true;
                }
                ArmorValue -= stoppedDamage;
                ArmorDamage(0);
                stoppedDamage = 0;
                hits = 0;
            }
        }

        public static void Sync(List<Enemy> enemyList, List<Enemy> asteroidList, List<Enemy> UFOList)
        {
            enemyList.Clear();
            foreach (var enemy in asteroidList)   { enemyList.Add(enemy); }
            foreach (var enemy in UFOList)        { enemyList.Add(enemy); }
        }

        public static void Split(List<Enemy> enemyList, List<Enemy> asteroidList, List<Enemy>UFOList)
        {
            asteroidList.Clear();
            UFOList.Clear();

            foreach (var enemy in enemyList)
            {
                switch (enemy.EType)
                {
                    case Type.Asteroid:
                        asteroidList.Add(enemy); break;

                    case Type.UFO:
                        UFOList.Add(enemy); break;
                }
            }
        }

        //Universal w/ Tweaks

        private static Vector2 SpawnSpot(Rectangle playSpace, Texture2D Image, Type type)
        {
            Random rand = new Random();
            int width = playSpace.Width;
            int height = playSpace.Height;

            //extra to make use with both
            int adjustment = 0;
            switch (type)
            {
                case Type.Asteroid:
                    adjustment = -150; break;

                case Type.UFO:
                    adjustment = -80; break;
            }
            //extra to make use with both

            Vector2 spawn = new Vector2(rand.Next(adjustment, width - adjustment), rand.Next(adjustment, height - adjustment));
            Rectangle Thing = new Rectangle((int)spawn.X, (int)spawn.Y, Image.Width, Image.Height);

            while (playSpace.Intersects(Thing))
            {
                spawn = new Vector2(rand.Next(-80, width + 80), rand.Next(-80, height + 80));
                Thing = new Rectangle((int)spawn.X, (int)spawn.Y, Image.Width, Image.Height);
            }

            return spawn;
        }
        public static Enemy NaturalSpawn(Type type, Rectangle playSpace, float velocity, Texture2D Image, Game1.Size size, TimeSpan shotTimer, float armor)
        {
            Vector2 Velocity = new Vector2(velocity, velocity);
            Vector2 SpawnLocation = SpawnLocation = SpawnSpot(playSpace, Image, type);

            if (SpawnLocation.X >= 0) { Velocity.X *= -1; }
            if (SpawnLocation.Y >= 0) { Velocity.Y *= -1; }

            Enemy New = new Enemy(SpawnLocation, Velocity, Image, 0, 1 / 1f, Color.White, size, shotTimer, type, armor);
            return New;
        }
        public void IsInBounds(Rectangle playSpace)
        {
            int width = playSpace.Width;
            int height = playSpace.Height;

            if (!playSpace.Intersects(Hitbox))
            {
                if      (Position.X < 0)                        { Position = new Vector2(width, Position.Y); }
                else if (Position.Y < 0)                        { Position = new Vector2(Position.X, height); }
                else if (Position.X + Image.Width >= width)     { Position = new Vector2(0, Position.Y); }
                else if (Position.Y + Image.Height >= height)   { Position = new Vector2(Position.X, 0); }
            }
        }

        //Universal w/ Tweaks


        //Original Asteroid Functions

        public static Enemy InitialSpawn(Rectangle playSpace, float lVelocity, Texture2D largeImage, Ship ship, float armor)
        {
            Enemy New;
            int width = playSpace.Width;
            int height = playSpace.Height;
            Random rand = new Random();
            Vector2 SpawnLocation = new Vector2(0, 0);
            Vector2 Velocity = new Vector2(lVelocity, lVelocity);


            SpawnLocation = new Vector2(rand.Next(0, width + 1), rand.Next(0, height + 1));
            while (playSpace.Contains(SpawnLocation) == false) //if boolean explanation (never got around to that, did we?)
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

            New = new Enemy(SpawnLocation, Velocity, largeImage, 0, 1 / 1f, Color.White, Game1.Size.LeChonk, TimeSpan.Zero, Type.Asteroid, armor);

            if (New.Hitbox.Intersects(ship.Hitbox))
            {
                SpawnLocation.X += 60;
            }

            return New;
        }

        //Original Asteroid Functions

        //Original UFO Functions

        public Bullet Shoot(Rectangle shotArea, Bullet shotCopy)
        {
            Vector2 start = new Vector2(Position.X, Position.Y + Image.Height / 2);
            Vector2 destination = new Vector2(rand.Next(shotArea.X, shotArea.X + shotArea.Width + 1), shotArea.Y);
            Vector2 between = start - destination;

            double angle = Math.Atan2((double)between.Y, (double)between.X) - MathHelper.ToRadians(90);

            Bullet shot = Bullet.BulletTypeCopy(shotCopy, start, (float)angle);

            return shot;
        }

        //Name changed from Update to UFOUpdate, still exists so as to not mess up old code
        //Scratch that, removed as new method of taking time will now be used
        //
        /*public bool UFOUpdate(TimeSpan gameTime)
        {
            ShotTimer -= gameTime;

            if (ShotTimer <= TimeSpan.Zero) { return true; }

            return false;
        }*/

        //Original UFO Functions

        //UFO Function Conversion

        public void UFOMovement(Rectangle space)
        {
            if (Position.X <= space.X)
            {
                Velocity.X = Math.Abs(Velocity.X);
            }
            else if (Position.X >= space.X + space.Width)
            {
                Velocity.X = Math.Abs(Velocity.X);
                Velocity.X *= -1;
            }

            if (Position.Y <= space.Y)
            {
                Velocity.Y = Math.Abs(Velocity.Y);
            }
            else if (Position.Y >= space.Y + space.Height)
            {
                Velocity.Y = Math.Abs(Velocity.Y);
                Velocity.Y *= -1;
            }

            Position = new Vector2(Position.X + Velocity.X, Position.Y + Velocity.Y);
        }

        //UFO Function Conversion
    }
}
