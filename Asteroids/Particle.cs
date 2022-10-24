using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class Particle
    {
        //texture, scale, color, position, velocity, rotation, angularVelocity, lifetime
        public Texture2D Image { get; set; }
        public float Scale { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Rotation { get; set; }
        public float AngularVelocity { get; set; }
        public float Lifetime { get; set; }
        public float LifetimeLeft { get; set; }
        public float ShrinkThreshold { get; set; }
        Color targetColor = Color.Transparent;

        public Particle(Texture2D image, float scale, Color color, Vector2 position, Vector2 velocity, float rotation, float angularVelocity, float lifetime, float shrink)
        {
            Image = image;
            Scale = scale;
            Color = color;
            Position = position;
            Velocity = velocity;
            Rotation = rotation;
            AngularVelocity = angularVelocity;
            Lifetime = lifetime;
            ShrinkThreshold = shrink;
            LifetimeLeft = Lifetime / ShrinkThreshold;
        }

        //update the position and rotation from velocity and angular velocity respectively
        //subtract the elapsed gametime from lifteime
        public void Update(GameTime gameTime)
        {

            Position += Velocity;
            Rotation += AngularVelocity;
            LifetimeLeft -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        //draw the particle at the specified position, scale, rotation, and color
        //also, make color fade out
        //also, make smaller as time progresses
        public void Draw(SpriteBatch sb)
        {
            float HalfLife = LifetimeLeft;
            if (LifetimeLeft<=Lifetime)
            {
                HalfLife = Lifetime;
            }
            float Percent = ((LifetimeLeft) / HalfLife);
            if (Percent>1)
            {
                Percent = 1;
            }
            float newScale = Vector2.Lerp(new Vector2(Scale, 0), Vector2.Zero, 1.0f-Percent).X;
            Vector4 result = Vector4.Lerp(Color.ToVector4(), targetColor.ToVector4(), 1-(LifetimeLeft/Lifetime));
            Color current = new Color(result.X, result.Y, result.Z, result.W);

            sb.Draw(Image, Position + new Vector2((float)Image.Width / 2.0f, (float)Image.Height / 2.0f), null, current, Rotation, new Vector2((float)Image.Width / 2.0f, (float)Image.Height / 2.0f), newScale, SpriteEffects.None, 0);
        }
    }
}
