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
        public bool inEffect = false;
        public Button UpgradeButton;
        public Texture2D UpgradeImage;
        public string UpgradeName;
        public string UpgradeDescription1;
        public string UpgradeDescription2;
        public string UpgradeDescription3;
        public string UpgradeDescription4;
        public List<Upgrade> ProgressionList;
        public int ProgressionLevel;

        public Upgrade(Vector2 position, Game1.StatUpgradeType statype, Game1.AbilityUpgradeType ability,
            string name, string descrip1, string descrip2, string descrip3, string descrip4, List<Upgrade> progList, int progLevel,
            Texture2D image, float rot, float scale, Color color) : base(position, image, rot, scale, color)
        {
            Position = position;
            StatType = statype;
            AbilityType = ability;
            UpgradeName = name;
            UpgradeDescription1 = descrip1;
            UpgradeDescription2 = descrip2;
            UpgradeDescription3 = descrip3;
            UpgradeDescription4 = descrip4;
            ProgressionList = progList;
            ProgressionLevel = progLevel;
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

        public void WhenSelected(List<Upgrade> possibleUpgrades, List<Upgrade> activeUpgrades)
        {
            if (UpgradeButton.wasClicked)
            {
                Position = new Vector2(-500, -500);
                UpgradeButton.isActive = false;
                isActive = true;

                if (ProgressionList != null && ProgressionLevel < ProgressionList.Count)
                {
                    possibleUpgrades.Add(ProgressionList[ProgressionLevel]);
                }
            }
        }
        
        public void Skipped()
        {
            Position = new Vector2(-500, -500);
            isActive = false;
        }

        public void Draw(SpriteFont title, SpriteFont desc, SpriteBatch sb)
        {
            UpgradeButton.Draw(sb);

            sb.Draw(UpgradeImage, new Vector2(Position.X - 62, Position.Y - 173), null, Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);

            sb.DrawString(title, UpgradeName, new Vector2(Position.X - 90, Position.Y - 224), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);

            sb.DrawString(desc, UpgradeDescription1, new Vector2(Position.X - 115, Position.Y - 4), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
            sb.DrawString(desc, UpgradeDescription2, new Vector2(Position.X - 115, Position.Y + 16), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
            sb.DrawString(desc, UpgradeDescription3, new Vector2(Position.X - 115, Position.Y + 36), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
            sb.DrawString(desc, UpgradeDescription4, new Vector2(Position.X - 115, Position.Y + 56), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
        }
    }
}