using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Text;

namespace Asteroid
{
    class Button : Sprite
    {
        public bool isActive = false;
        public bool isPressed = false;
        public bool isHeld = false;
        public bool wasClicked = false;

        public Button(Vector2 position, Texture2D image, float rot, float scale, Color color) : base(position, image, rot, scale, color)
        {
            Position = position;
            Image = image;
            Rotation = rot;
            Scale = scale;
            Color = color;
        }

        public void Update(MouseState currentState, MouseState lastState)
        {
            Pressed(currentState);
            Held(lastState);
            Released(currentState);
        }

        public void Pressed(MouseState currentState)
        {
            if (isActive)
            {
                if (Hitbox.Contains(currentState.Position.ToVector2()) && currentState.LeftButton==ButtonState.Pressed)
                {
                    isPressed = true;
                }
            }
            else
            {
                wasClicked = false;
            }
        }

        public void Held(MouseState lastState)
        {
            if (isPressed)
            {
                if (lastState.LeftButton == ButtonState.Pressed)
                {
                    isHeld = true;
                }
            }
        }

        public void Released(MouseState currentState)
        {
            if (isPressed||isHeld)
            {
                if (currentState.LeftButton == ButtonState.Released)
                {
                    isHeld = false;
                    isPressed = false;
                    if (Hitbox.Contains(currentState.Position.ToVector2()))
                    {
                        wasClicked = true;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            if (isActive)
            {
                base.Draw(sb);
            }
        }
    }
}
