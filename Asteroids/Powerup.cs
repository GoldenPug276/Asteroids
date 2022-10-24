using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class Powerup : Sprite
    {
        Random rand = new Random();
        public Vector2 Velocity;
        public enum Type
        {
            Machine = 1, //nonstop firing
            Laser, //ray of death
            Foresight, //EPITAPH and like laser sight on all bullets
            Bomb //explosion shots
        }
        public Type leType;
        public Texture2D[] Damages;
        public TimeSpan DespawnTime;
        private TimeSpan reserveDespawnTime;
        public TimeSpan Duration;
        private TimeSpan reserveDuration;
        public TimeSpan ShotTimer;
        public TimeSpan reserveShotTimer;
        public int Rarity;
        public int Durability = 1;
        public bool isActive = false;
        public bool isSpawned = false;
        public Bullet ProjectileShot;

        public Powerup(Vector2 position, Type type, TimeSpan time, TimeSpan duration, TimeSpan shotsTime, Texture2D[] damage, Bullet shot, int rarity, Texture2D image, float rot, float scale, Color color) : base(position, image, rot, scale, color)
        {
            Position = position;
            leType = type;
            DespawnTime = time;
            reserveDespawnTime = DespawnTime;
            Duration = duration;
            reserveDuration = Duration;
            ShotTimer = shotsTime;
            reserveShotTimer = ShotTimer;
            Damages = damage;
            ProjectileShot = shot;
            Rarity = rarity;
            Image = image;
            Rotation = rot;
            Scale = scale;
            Color = color;
        }

        public void Update(TimeSpan gameTime, List<Bullet> shots)
        {
            Position = new Vector2(Position.X + Velocity.X, Position.Y + Velocity.Y);
            bool reset = false;
            if (!isActive)
            {
                DespawnTime -= gameTime;
                if (DespawnTime<=TimeSpan.Zero)
                {
                    reset = true;
                    DespawnTime = reserveDespawnTime;
                    isSpawned = false;
                }
            }
            else
            {
                Duration -= gameTime;
                if (Duration<=TimeSpan.Zero)
                {
                    reset = true;
                    Duration = reserveDuration;
                    isActive = false;
                    shots.Clear();
                }
            }
            if (reset)
            {
                Position = new Vector2(-20, -20);
                Velocity.X = -50;
                Velocity.Y = -50;
                Durability = 1;
            }
        }

        public void Spawned(Vector2 position, Vector2 velocity, Game1.Size size)
        {
            if (rand.Next(0, 1001)<=Rarity*(int)size && !isSpawned)
            {
                Position = position;
                Velocity = velocity;
                Durability = 5;
                isSpawned = true;
                DespawnTime = reserveDespawnTime;
            }
        }

        public bool WhenShot(List<Bullet> shots, int index)
        {
            bool gotShot = false;
            if (Hitbox.Intersects(shots[index].Hitbox))
            {
                gotShot = true;
                shots.RemoveAt(index);
                Durability--;
                DespawnTime += new TimeSpan(0, 0, 0, 0, 750);
                if (Durability<=0)
                {
                    isActive = true;
                    isSpawned = false;
                    Position = new Vector2(-20, -20);
                    Velocity.X = -20;
                    Velocity.Y = -20;
                    DespawnTime = reserveDespawnTime;
                    Duration += reserveDuration;
                }
            }
            return gotShot;
        }

        public void IsInBounds(Rectangle playSpace)
        {
            int width = playSpace.Width;
            int height = playSpace.Height;

            if (!playSpace.Intersects(Hitbox) && isSpawned==true)
            {
                if (Position.X<=0)
                {
                    Velocity.X *= -1;
                    //Position.X = 0;
                }
                else if (Position.Y<=0)
                {
                    Velocity.Y *= -1;
                    //Position.Y = 0;
                }
                else if (Position.X+Image.Width>=width)
                {
                    Velocity.X *= -1;
                    //Position.X = width - Image.Width;
                }
                else if (Position.Y+Image.Height>=height)
                {
                    Velocity.Y *= -1;
                    //Position.Y = height - Image.Height;
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.Draw(Damages[Durability], Position, null, Color.Black, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
