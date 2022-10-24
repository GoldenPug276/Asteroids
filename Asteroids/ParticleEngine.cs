using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class ParticleEngine
    {
        //list of particle, random
        private List<Particle> Particles { get; set; }
        public List<Texture2D> ParticleImage { get; set; }
        private Random rand { get; set; }
        private int particleCount { get; set; }
        public Vector2 EmitterLocation { get; set; }
        public Vector2? Velocity { get; set; }
        public float? AngularVelocity { get; set; }
        public float? Angle { get; set; }
        public float? Scale { get; set; }
        public Color? Color { get; set; }
        public float? Lifetime { get; set; }
        public float? LifetimeLeft { get; set; }
        public float? ShrinkThreshold { get; set; }

        public ParticleEngine(Random rand, int particleCount, List<Texture2D> particleImage, Vector2? Velocity, float? AngularVelocity, float? Angle, float? Scale, Color? Color, float? Lifetime, float? shrink)
        {
            Particles = new List<Particle>();
            this.rand = rand;
            this.particleCount = particleCount;
            this.ParticleImage = particleImage;
            this.Velocity = Velocity;
            this.AngularVelocity = AngularVelocity;
            this.Angle = Angle;
            this.Scale = Scale;
            this.Color = Color;
            this.Lifetime = Lifetime;
            this.LifetimeLeft = this.Lifetime;
            this.ShrinkThreshold = shrink;
        }

        public void Update(GameTime gameTime)
        {
            //if Particles.Count < particleCount, call GenerateParticle()
            if (Particles.Count<particleCount)
            {
                Particles.Add(GenerateNewParticle());
            }
            //then, loop through the list of particles and call update on each one
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Update(gameTime);
                //if the particle's lifetime is 0 (or less), remove it from the list
                if (Particles[i].LifetimeLeft<=0)
                {
                    Particles.RemoveAt(i);
                    i--;
                }
            }
        }        
        //update
        public void Draw(SpriteBatch sb)
        {
            for(int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Draw(sb);
            }
        }
        //draw

        //generate particle function: randomly make a random particle; randomize particle generation rather than using hard-coded numbers
        private Particle GenerateNewParticle()
        {
            Texture2D texture = ParticleImage[rand.Next(ParticleImage.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = Velocity ?? new Vector2(
                1f * (float)(rand.NextDouble() * 2 - 1), 
                1f * (float)(rand.NextDouble() * 2 - 1));
            float angularVelocity = AngularVelocity ?? 0.1f * (float)(rand.NextDouble() * 2 - 1);
            float angle = Angle ?? MathHelper.ToRadians(rand.Next(0, 361));
            Color color = Color ?? new Color(
                (float)rand.NextDouble(),
                (float)rand.NextDouble(),
                (float)rand.NextDouble());
            float size = Scale ?? (float)rand.NextDouble();
            float lifetime = Lifetime ?? 200 + rand.Next(400);
            float shrinkThreshold = ShrinkThreshold ?? 0.5f;

            return new Particle(texture, size, color, position, velocity, angle, angularVelocity, lifetime, shrinkThreshold);
        }
    }
}
