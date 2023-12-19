using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class Ship : Sprite
    {
        public float Velocity { get; set; }
        private float acceleration = 0;
        private float decceleration = 0;
        private bool isAccelerating;
        public Ship(Vector2 position, float velocity, Texture2D image, float rot, float scale, Color color) : base(position, image, rot, scale, color)
        {
            Position = position;
            Velocity = velocity;
            Image = image;
            Rotation = rot;
            Scale = scale;
            Color = color;
        }

        public void Move(KeyboardState keyboardState)
        {
            float a = 1;
            float d = 1;
            float t = 1;
            if (Game1.EGOManifested)
            {
                a += 2;
                d += 1.5f;
                t += 0.5f;
            }

            isAccelerating = false;
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                isAccelerating = true;
                acceleration += 0.001f * a;
                decceleration -= 0.002f * d;
            }
            else
            {
                isAccelerating = false;
                decceleration += 0.001f * d;
                acceleration -= 0.002f * a;
            }
            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                isAccelerating = false;
                decceleration += 0.002f * d;
                acceleration -= 0.002f * a;
            }
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                Rotation -= 0.065f * t;
            }
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                Rotation += 0.065f * t;
            }

            if (isAccelerating)
            {
                Velocity += acceleration;
            }
            else
            {
                Velocity -= decceleration;
            }

            if (Velocity<=0)
            {
                Velocity = 0;
                decceleration = 0;
            }
            if (decceleration<=0)
            {
                decceleration = 0;
            }
            if (acceleration<=0)
            {
                acceleration = 0;
            }

            Position = new Vector2((float)Math.Sin(Rotation), -(float)Math.Cos(Rotation)) * Velocity + Position;
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
