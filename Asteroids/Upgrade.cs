﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;
using Windows.Media.Capture;

namespace Asteroid
{
    class Upgrade : Sprite
    {
        public Game1.StatUpgradeType UpgradeType;
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
        public int Rarity;
        public int LevelAvailability;
        public Bullet GunBullet;
        public float EnergyUse;
        public RectangleF energyTotal;
        public RectangleF energyRemaining;
        private RectangleF movingEnergy;
        private int energyMoveIf = -1;
        private TimeSpan energyRegen = TimeSpan.FromMilliseconds(125.5);
        private TimeSpan reserveEnergyRegen = TimeSpan.FromMilliseconds(125.5);
        public float EnergyGainMultiplier = 1;

        public bool isGun;
        public TimeSpan ShotTimer;
        private bool readyToFire = false;
        public TimeSpan reserveShotTimer;
        private bool usedThisFrame = false;
        private bool usedLastFrame = false;
        public bool Overheat = false;
        public float Penetration = 0;
        public Upgrade[] Dependencies;

        public Upgrade(Vector2 position, Game1.StatUpgradeType statype, Game1.AbilityUpgradeType ability,
            string name, string descrip1, string descrip2, string descrip3, string descrip4, int rarity, int levelSeen, List<Upgrade> progList, int progLevel, float energy, float pen,
            Texture2D image, float rot, float scale, Color color, Upgrade[] dependencies, bool active) : base(position, image, rot, scale, color)
        {
            Position = position;
            UpgradeType = statype;
            AbilityType = ability;

            if (UpgradeType == Game1.StatUpNone)
            {
                if (AbilityType != Game1.AbilityUpNone && AbilityType < Game1.AbilityUpgradeType.Warp)
                {
                    energyTotal = new Rectangle(10, 70, 100, 20);
                    isGun = true;
                }
                else if (AbilityType >= Game1.AbilityUpgradeType.Warp)
                {
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
            Rarity = rarity;
            LevelAvailability = levelSeen;
            ProgressionList = progList;
            ProgressionLevel = progLevel;
            EnergyUse = energy;
            UpgradeImage = image;
            Rotation = rot;
            Scale = scale;
            Color = color;
            isActive = active;
            Penetration = pen;
            Dependencies = dependencies;
        }

        public void RarityChange(int rarity)
        {
            Rarity = rarity;
        }

        public bool UpgradeDependency(List<Upgrade> all)
        {
            if (Dependencies==null)
            {
                return false;
            }

            List<bool> found = new List<bool>();

            foreach (Upgrade Upgrade in all)
            {
                foreach (Upgrade upgrade in Dependencies)
                {
                    if (Upgrade==upgrade)
                    {
                        found.Add(true);
                    }
                }
            }

            if (found.Count==Dependencies.Length) { return true; }
            else { return false; }
        }

        public static Upgrade Generation(List<Upgrade> PossibleUpgrades)
        {
            Random rand = new Random();

            List<Upgrade> potential = new List<Upgrade>();

            foreach (var upgrade in PossibleUpgrades) { potential.Add(upgrade); }

            while (potential.Count > 1)
            {
                for (int i = 0; i < potential.Count; i++)
                {
                    if (rand.Next(1, 101)>potential[i].Rarity)
                    {
                        potential.Remove(potential[i]);
                        i--;
                    }

                    if (potential.Count==1) { break; }
                }
            }

            return potential[0];
        }

        public void WhenSelected(List<Upgrade> possibleUpgrades, List<Upgrade> activeUpgrades, List<Upgrade> activeAbilities, List<Upgrade> activeGuns)
        {
            if (UpgradeButton.wasClicked)
            {
                Position = new Vector2(-500, -500);
                UpgradeButton.isActive = false;
                isActive = true;


                if (UpgradeType!=Game1.StatUpNone && AbilityType==Game1.AbilityUpNone)
                {
                    activeUpgrades.Add(this);
                }
                else if (AbilityType!=Game1.AbilityUpNone && UpgradeType==Game1.StatUpNone)
                {
                    if (isGun) { activeGuns.Add(this); }
                    else { activeAbilities.Add(this); }
                }

                if (ProgressionList!=null)
                {
                    if (ProgressionLevel<ProgressionList.Count)
                    {
                        possibleUpgrades.Add(ProgressionList[ProgressionLevel]);
                    }
                    if (ProgressionLevel>1)
                    {
                        if (ProgressionList[0].UpgradeType!=Game1.StatUpgradeType.Drones1)
                        {
                            ProgressionList[ProgressionLevel - 2].inEffect = false;
                            ProgressionList[ProgressionLevel - 2].isActive = false;
                            activeAbilities.Remove(ProgressionList[ProgressionLevel - 2]);
                        }
                    }
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
            if (!Game1.GameFrozen)
            {
                energyRemaining.Width -= EnergyUse;
                movingEnergy.Width -= EnergyUse;
            }
        }

        public void AbilityUpdate()
        {
            if (!Game1.GameFrozen)
            {
                usedLastFrame = usedThisFrame;
                usedThisFrame = false;

                float energyGain;
                if (EnergyGainMultiplier < 10) { energyGain = 1; }
                else { energyGain = EnergyGainMultiplier / 10; }

                energyRegen -= Game1.gameTime.ElapsedGameTime;

                if (energyRegen<=TimeSpan.Zero && energyRemaining.Width<100)
                {
                    if (energyMoveIf>0)
                    {
                        energyRemaining.Width += energyGain;
                        energyMoveIf *= -1;
                    }
                    else if (energyMoveIf<0)
                    {
                        movingEnergy.Width += energyGain;
                        energyMoveIf *= -1;
                    }

                    energyRegen = reserveEnergyRegen / EnergyGainMultiplier;
                }

                if (Overheat && energyRemaining.Width>=energyTotal.Width/3) { Overheat = false; }

                if (isGun) { ShotTimer -= Game1.gameTime.ElapsedGameTime; }
                if (ShotTimer<=TimeSpan.Zero) { readyToFire = true; } else { readyToFire = false; }
            }
        }

        public bool WillGunShoot()
        {
            if (isActive && inEffect && readyToFire && energyRemaining.Width>=EnergyUse && !Overheat)
            {
                usedThisFrame = true;
                ShotTimer = reserveShotTimer;
                return true;
            }

            if (usedLastFrame && energyRemaining.Width < EnergyUse)
            {
                Overheat = true;
                energyRemaining.Width = 0;
                movingEnergy.Width = 0;
            }
            return false;
        }
        public bool WillAbilityGetUsed()
        {
            if (isActive && energyRemaining.Width >= EnergyUse && !Overheat) { usedThisFrame = true; return true; }

            if (usedLastFrame && energyRemaining.Width < EnergyUse)
            {
                Overheat = true;
                energyRemaining.Width = 0;
                movingEnergy.Width = 0;
            }

            return false;
        }

        public void AbilityEnergyStack(List<Upgrade> upgrades, int i)
        {
            int count = upgrades.Count;

            int value = count - i;
            if (count == 0) { value = 0; }

            energyTotal.Y = 475 - (25 * value);
            energyRemaining.Y = 475 - (25 * value);
            movingEnergy.Y = 475 - (25 * value);

            //energyTotal.Y = 450 - (25 * i);
            //energyRemaining.Y = 450 - (25 * i);
            //movingEnergy.Y = 450 - (25 * i);

            /*
            for (int i = 0; i < upgrades.Count; i++)
            {
                if (upgrades[i]==this)
                {

                }
            }
            */
        }

        public void RefreshEnergy()
        {
            energyRemaining.Width = 100;
            if (AbilityType!=Game1.AbilityUpNone && !isGun) { inEffect = false; }
        }

        public void Draw(SpriteFont title, SpriteFont desc, SpriteBatch sb)
        {
            UpgradeButton.Draw(sb);

            sb.Draw(UpgradeImage, new Vector2(Position.X - 62, Position.Y - 173), null, Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);

            sb.DrawString(title, UpgradeName, new Vector2(Position.X - 90, Position.Y - 224), Color, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);

            sb.DrawString(desc, UpgradeDescription1, new Vector2(Position.X - 120, Position.Y - 4), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
            sb.DrawString(desc, UpgradeDescription2, new Vector2(Position.X - 120, Position.Y + 16), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
            sb.DrawString(desc, UpgradeDescription3, new Vector2(Position.X - 120, Position.Y + 36), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
            sb.DrawString(desc, UpgradeDescription4, new Vector2(Position.X - 120, Position.Y + 56), Color.White, Rotation, new Vector2(0, 0), Scale, SpriteEffects.None, 0);
        }
        public void EnergyDraw(SpriteBatch sb)
        {
            Color tColor = Color;
            if (Overheat) { tColor = Color.Red; }

            if ((isGun && inEffect) || (!isGun && isActive))
            {
                sb.FillRectangle(energyRemaining, tColor);
                sb.DrawRectangle(energyTotal, Color);
                sb.FillRectangle(movingEnergy, Color.Lerp(tColor, Color.Transparent, 0.5f));
            }
        }



        public static void Sync(List<Upgrade> allActive, List<Upgrade> activeUpgrades, List<Upgrade> activeAbilities, List<Upgrade> activeGuns)
        {
            allActive.Clear();
            foreach (var upgrade in activeUpgrades) { allActive.Add(upgrade); }
            foreach (var ability in activeAbilities) { allActive.Add(ability); }
            foreach (var gun in activeGuns) { allActive.Add(gun); }
        }

        public static void Split(List<Upgrade> allActive, List<Upgrade> activeUpgrades, List<Upgrade> activeAbilities, List<Upgrade> activeGuns)
        {
            activeUpgrades.Clear();
            activeAbilities.Clear();
            activeGuns.Clear();

            foreach (var upgrade in allActive)
            {
                if      (upgrade.isGun)                                         { activeGuns.Add(upgrade); }
                else if (upgrade.AbilityType!=Game1.AbilityUpgradeType.None)    { activeAbilities.Add(upgrade); }
                else if (upgrade.UpgradeType!=Game1.StatUpgradeType.None)       { activeUpgrades.Add(upgrade); }
            }
        }
    }
}