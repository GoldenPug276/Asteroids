using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class Upgrade : Sprite
    {
        public Game1.StatUpgradeType StatType;
        public Game1.AbilityUpgradeType AbilityType;
        Random rand = new Random();
        public bool isActive = false;
        public Button UpgradeButton;
        public Texture2D UpgradeImage;
        public string UpgradeName;
        public string UpgradeDescription;

        public Upgrade(Vector2 position, Game1.StatUpgradeType statype, Game1.AbilityUpgradeType ability, string name, string descrip, Button button, 
            Texture2D image, float rot, float scale, Color color) : base(position, image, rot, scale, color)
        {
            Position = position;
            StatType = statype;
            AbilityType = ability;
            UpgradeName = name;
            UpgradeDescription = descrip;
            UpgradeImage = image;
            Rotation = rot;
            Scale = scale;
            Color = color;
        }

        /*
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
        */

        public void WhenSelected(List<Upgrade> possibleUpgrades, List<Upgrade> activeUpgrades, List<Upgrade> abilityProg, int currentProg)
        {
            if (UpgradeButton.isPressed)
            {
                Position = new Vector2(-500, -500);
                UpgradeButton.Position = new Vector2(-500, -500);
                isActive = true;

                if (abilityProg!=null && currentProg<abilityProg.Count)
                {
                    currentProg++;
                    possibleUpgrades.Add(abilityProg[currentProg]);
                }
            }
        }

        public void Draw(SpriteFont font, SpriteBatch sb)//current position changs are placeholders, change when sure of position
        {
            sb.Draw(UpgradeButton.Image, Position, null, Color.Black, Rotation, Origin, Scale, SpriteEffects.None, 0);

            sb.Draw(UpgradeImage, new Vector2(Position.X, Position.Y - 20), null, Color.Black, Rotation, Origin, Scale, SpriteEffects.None, 0);

            sb.DrawString(font, UpgradeDescription, new Vector2(Position.X, Position.Y + 10), Color.Black, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
