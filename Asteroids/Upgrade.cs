using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.VoiceCommands;

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
        public Bullet GunBullet;
        public int EnergyUse;
        public RectangleF energyTotal;
        public RectangleF energyRemaining;
        private RectangleF movingEnergy;
        private int energyMoveIf = -1;
        private TimeSpan energyRegen = TimeSpan.FromMilliseconds(125.5);
        private TimeSpan reserveEnergyRegen = TimeSpan.FromMilliseconds(125.5);
        public float EnergyGainMultiplier = 1;

        public bool isGun = false;

        public Upgrade(Vector2 position, Game1.StatUpgradeType statype, Game1.AbilityUpgradeType ability,
            string name, string descrip1, string descrip2, string descrip3, string descrip4, List<Upgrade> progList, int progLevel, int energy,
            Texture2D image, float rot, float scale, Color color, bool active) : base(position, image, rot, scale, color)
        {
            Position = position;
            StatType = statype;
            AbilityType = ability;

            if (StatType==Game1.StatUpgradeType.None)
            {
                if (AbilityType!=Game1.AbilityUpgradeType.None && AbilityType<Game1.AbilityUpgradeType.Warp)
                {
                    energyTotal = new Rectangle(10, 70, 100, 20);
                    isGun = true;
                }
                else if (AbilityType>=Game1.AbilityUpgradeType.Warp)
                {
                    //normal ability cooldown code
                    //will be at the bottom left, code is temp for now
                    energyTotal = new Rectangle(10, 400, 100, 20);
                    isGun = false;
                }
            }
            energyRemaining = energyTotal;
            movingEnergy = energyRemaining;

            UpgradeName = name;
            UpgradeDescription1 = descrip1;
            UpgradeDescription2 = descrip2;
            UpgradeDescription3 = descrip3;
            UpgradeDescription4 = descrip4;
            ProgressionList = progList;
            ProgressionLevel = progLevel;
            EnergyUse = energy;
            UpgradeImage = image;
            Rotation = rot;
            Scale = scale;
            Color = color;
            isActive = active;
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

        public void WhenSelected(List<Upgrade> possibleUpgrades, List<Upgrade> activeUpgrades, List<Upgrade> activeAbilities)
        {
            if (UpgradeButton.wasClicked)
            {
                Position = new Vector2(-500, -500);
                UpgradeButton.isActive = false;
                isActive = true;


                if (StatType!=Game1.StatUpgradeType.None && AbilityType==Game1.AbilityUpgradeType.None)
                {
                    activeUpgrades.Add(new Upgrade(Position, StatType, AbilityType, UpgradeName, UpgradeDescription1, UpgradeDescription2, UpgradeDescription3, 
                        UpgradeDescription4, ProgressionList, ProgressionLevel, EnergyUse, UpgradeImage, Rotation, Scale, Color, true));
                }
                else if (AbilityType!=Game1.AbilityUpgradeType.None && StatType==Game1.StatUpgradeType.None)
                {
                    activeAbilities.Add(new Upgrade(Position, StatType, AbilityType, UpgradeName, UpgradeDescription1, UpgradeDescription2, UpgradeDescription3,
                        UpgradeDescription4, ProgressionList, ProgressionLevel, EnergyUse, UpgradeImage, Rotation, Scale, Color, true));
                }

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

        public void AbilityUse()
        {
            energyRemaining.Width -= EnergyUse;
            movingEnergy.Width -= EnergyUse;
        }
        public void AbilityUpdate(TimeSpan ElapsedGameTime)
        {
            energyRegen -= ElapsedGameTime;
            if (energyRegen<=TimeSpan.Zero && energyRemaining.Width<100)
            {
                if (energyMoveIf > 0)
                {
                    energyRemaining.Width++;
                    energyMoveIf *= -1;
                }
                else if (energyMoveIf < 0)
                {
                    movingEnergy.Width++;
                    energyMoveIf *= -1;
                }
                energyRegen = reserveEnergyRegen / EnergyGainMultiplier;
            }
        }

        public bool WillGunShoot(TimeSpan shotTimer)
        {
            bool fire = false;

            if (isActive && inEffect && shotTimer <= TimeSpan.Zero && energyRemaining.Width >= EnergyUse)
            {
                fire = true;
            }

            return fire;
        }

        public bool WillAbilityGetUsed()
        {
            bool use = false;

            if (isActive && energyRemaining.Width >= EnergyUse)
            {
                use = true;
            }

            return use;
        }

        public void Draw(SpriteFont title, SpriteFont desc, SpriteBatch sb)
        {
            UpgradeButton.Draw(sb);

            sb.Draw(UpgradeImage, new Vector2(Position.X - 62, Position.Y - 173), null, Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);

            sb.DrawString(title, UpgradeName, new Vector2(Position.X - 90, Position.Y - 224), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);

            sb.DrawString(desc, UpgradeDescription1, new Vector2(Position.X - 120, Position.Y - 4), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
            sb.DrawString(desc, UpgradeDescription2, new Vector2(Position.X - 120, Position.Y + 16), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
            sb.DrawString(desc, UpgradeDescription3, new Vector2(Position.X - 120, Position.Y + 36), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
            sb.DrawString(desc, UpgradeDescription4, new Vector2(Position.X - 120, Position.Y + 56), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
        }
        public void EnergyDraw(SpriteBatch sb)
        {
            if ((isGun && inEffect)||(!isGun && isActive))
            {
                sb.DrawRectangle(energyTotal, Color.DarkGray);
                sb.FillRectangle(energyRemaining, Color.DarkGray);
                sb.FillRectangle(movingEnergy, Color.Lerp(Color.DarkGray, Color.Transparent, 0.5f));
            }
        }
    }
}