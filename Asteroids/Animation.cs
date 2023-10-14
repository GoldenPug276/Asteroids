using Asteroid;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Timers;

namespace Asteroids
{
    class Animation : Sprite
    {
        public TimeSpan FrameTimes;
        private TimeSpan reserveFrameTimes;
        public int FrameCount = 10;
        private int reserveFrameCount;
        public int FrameSize;
        public bool AnimationRunning = false;

        public Animation(Vector2 position, Texture2D spritesheet, TimeSpan frameTimes, int frameCount, int frameSize, float rot, float scale, Color color)
            : base(position, spritesheet, rot, scale, color)
        {
            Position = position;
            Image = spritesheet;
            Rotation = rot;
            Scale = scale;
            Color = color;

            FrameTimes = frameTimes;
            reserveFrameTimes = FrameTimes;
            FrameCount = frameCount;
            reserveFrameCount = FrameCount;
            FrameSize = frameSize;
        }

        public void AnimateWhileFrozen(SpriteBatch sb)
        {
            animateFrame(reserveFrameCount - FrameCount, sb);
            FrameTimes -= Game1.gameTime.ElapsedGameTime;
            if (FrameTimes<=TimeSpan.Zero)
            {
                FrameCount--;
                FrameTimes = reserveFrameTimes;
                if (FrameCount<0)
                {
                    AnimationRunning = false;
                    FrameCount = reserveFrameCount;
                    Game1.GameFrozen = false;
                }
            }
        }
        public void animateFrame(int frameNumber, SpriteBatch sb)
        {
            int frameSizeY = Image.Height;
            sb.Draw(Image, Position, new Rectangle((frameNumber) * FrameSize, 0, FrameSize, frameSizeY), Color.White);
        }
    }
}
