﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace Asteroid
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //All Sprites are placeholders; change later
        Ship ship;
        Bullet defaultShot;
        TimeSpan defaultShotTimer = new TimeSpan(0, 0, 0, 1, 250);
        TimeSpan reserveDefaultShotTimer = new TimeSpan(0, 0, 0, 1, 250);
        Bullet UFOShot;
        List<Bullet> shots = new List<Bullet>();
        List<Bullet> enemyShots = new List<Bullet>();
        Level level;
        SpriteFont font;
        SpriteFont upgradeTitleFont;
        SpriteFont upgradeDescFont;
        TimeSpan BurnerTime = new TimeSpan(0, 0, 0, 0, 1);

        public enum StatUpgradeType
        {
            None = 0,
            Test = 0,
            ShotSpeedUp1 = 1,
            ShotSpeedUp2 = 2,
            ShotSpeedUp3 = 3,
            Drones1 = 4,
            Drones2 = 5,
            Drones3 = 6
        }
        public enum AbilityUpgradeType
        {
            None = 0,
            Machine = 1,
            Laser = 2,
            Warp = 4,
            Test = 0,
            Shield1 = 5,
            Shield2 = 6,
            Shield3 = 7,
            ShieldUltimate = 8
        }
        List<Upgrade> PossibleUpgrades = new List<Upgrade>();
        List<Upgrade> UpgradesToDraw = new List<Upgrade>();
        List<Upgrade> ActiveUpgrades = new List<Upgrade>();
        List<Upgrade> ActiveAbilities = new List<Upgrade>();
        List<Upgrade> ActiveGuns = new List<Upgrade>();
        int CurrentActiveGunIndex = 0;
        List<Upgrade> NoneHolder = new List<Upgrade>();

        bool UpgradeChosen = false;
        int UpgradeTime = 0;

        Upgrade TestUpgrade;
        Upgrade TestAbility;
        Upgrade None;
        Button UpgradeSkip;

        Upgrade ShotSpeedUp1;
        Upgrade ShotSpeedUp2;
        Upgrade ShotSpeedUp3;
        List<Upgrade> ShotSpeedProgHolder = new List<Upgrade>();

        Upgrade Warp;

        string GunInEffectName = "Default";
        Color GunInEffectColor = Color.DarkGray;
        Upgrade MachineGun;
        TimeSpan MachineGunShotTimer = new TimeSpan(0, 0, 0, 0, 150);
        TimeSpan reserveMachineGunShotTimer = new TimeSpan(0, 0, 0, 0, 150);
        Upgrade Laser;
        TimeSpan LaserShotTimer = new TimeSpan(0, 0, 0, 0, 600);
        TimeSpan reserveLaserShotTimer = new TimeSpan(0, 0, 0, 0, 600);

        /*  Old Powerup Code, Archived
        Powerup machine;
        Powerup laser;
        Texture2D[] powerDamages;
        List<Powerup> powerups = new List<Powerup>();
        */


        Button UpgradeChoice1;
        Button UpgradeChoice2;
        Button UpgradeChoice3;
        Button[] UpgradeChoiceList;

        Texture2D UpgradeSlot;
        Texture2D SkipUpgrade;
        string tempPress = "";

        public Rectangle playSpace;

        public Random rand = new Random();

        List<Asteroid> asteroids = new List<Asteroid>();
        List<UFO> UFOs = new List<UFO>();
        Texture2D BigAsteroid;
        Texture2D SmallAsteroid;
        Texture2D TinyAsteroid;
        Texture2D BigSaucer;
        Texture2D SmallSaucer;

        public enum Size
        {
            Baby = 3,
            Normal = 2,
            LeChonk = 1
        }

        string score0s = "0000";
        int score = 0;
        int lives = 4;
        TimeSpan iFrames = new TimeSpan(0, 0, 0, 4, 0);

        float largeAsteroidVelocity = 0.5f;
        float smallAsteroidVelocity = 1;
        float tinyAsteroidVelocity = 2;
        float shotVelocity = 9.5f;
        Rectangle UFOMovementSpace;
        Rectangle UFOShotArea;

        /*TO-DO:
         ******************************************************OLD ARCHIVED POWERUP TO-DO********************************************************************
         * Make different gun powerups                                                                                                                      *
         *      examples being: machine gun, laser, and bombs                                                                                               *
         *                  I think that the wa spawning is done should work like so:                                                                       *
         *                  each different thing can spawn in a powerup                                                                                     *
         *                  the harder it was to take out, the higher the chance of it spawning a powerup, done via a multiplier                            *
         *                  certain powerups can only come from some things (only ufos can spawn lasers, etc.)                                              *
         *                          ok, so they now can spawn at any time, tho you'll have to manually ask them if you know what i mean                     *
         *                          now i need to make sure that they can only spawn from specific things, but you do that when more powers made            *
         ******************************************************OLD ARCHIVED POWERUP TO-DO********************************************************************
         *      
         *      so by dumbass thought of maybe making this a rougelike; i hate myself but i want to
         *          have some unlockable abilities with meters and cooldowns and upgrades
         *          this would force me to make the game less of a pain so that you can, you know, progress
         *              som upgrade ideas: firing speed, shields, drones
         *              som ability ideas: timestop, time erase, time rewind, screen nuke
         *                  i'll need to make the powerups into weapon upgrades that can be swapped to
         *                  
         *      so first, i'll have to design an upgrade menu in-between levels where you can pick up or skip upgrades {DONE}
         *          (use as reference: https://progameguides.com/wp-content/uploads/2022/06/roblox-hours-vitality-1024x576.jpg)
         *      
         *      
         *      **DONE**
         *      deal with upgrade class later (rn it's only a copy of powerup with 0 differences)
         *      actually, make upgrade a class with a button and upgrade. Basically, the entire upgrade is in this class.
         *      **DONE**
         *          
         *          **DONE**
         *          have it pick 3 random upgrades and place them with a certain amount of pixels in between
         *          have the location assigned when the upgrade is gotten
         *          spawn the text and image using the main button position
         *          call the button inside of the upgrade to check for presses
         *          set position and upgradebutton position from game1
         *          have abilities have upgrade trees in forms of list, and have the current prog as an int
         *          **DONE**
         *          
         *          
         *      have a button class;    {FINISHED}
         *          inherits from sprite
         *          has these funstions:
         *              a function that detects when it's clicked on
         *              a function that detects if it's held
         *              a function that detects when it's released
         *              an update function that checks all of the above at once
         *      
         *      
         *      **DONE**
         *      first, make the upgrades and their spots random, cuz right now it's the same upgrade in the same spot
         *      now, work on making the gui appear when a level is cleared
         *      at home, make chosen upgrades get removed from bank and make the gui appearing between levels work
         *      make a "none" upgrade with a "noneUpgrade" list
         *      after that, make temporary upgrades that do shit
         *      actually, first make upgrades sort into an "activeUpgrades" and "activeAbilities" list
         *      **DONE**
         *      
         *      
         *      after that, make the machine gun into an ability and test it    {FINISHED}
         *          have fancy little text on the side that says what weapon you have and allow switching
         *          have weapon swaping be done via scroll wheel and num-bar  
         *              convert powerups into gun upgrades
         *      
         *      
         *      instead of nerfing the now permanent guns, do the following;   {FINISHED}
         *          give them an energy bar like in megaman, where using a weapon takes energy that regenerates after time
         *              have energy bar as a rectangle for now
         *              it always has a length of 100, and each weapon has a specific amount of energy used per shot
         *              regenerate 4 energy a second smoothly by default
         *              it appears below the gun name
         *          abilities use the same energy, but the position of the bar is different and the energy gain usually hasa multiplier
         *      
         *      make the energy bar smoother {DONE}
         *      
         *      clean up the old powerup code since it is no longer neccesary {has been archived}
         *      
         *      
         *      **DONE**
         *      make a temp ability to test energy
         *      give warp energy rather than a cooldown
         *      make energy bars stack using old powerup code (starts at the bottom and new abilities push old ones up)
         *      color code powerup enregy bars by making the upgrade title, energy bar color, and gun name color the "Color" variable of the upgrade
         *      **DONE**
         *      
         *      
         *      replace the "test" upgarde amd ability with actual ones and remove the "test" parameter
         *      
         *          make the test upgrade into a drone that circles around the ship and shoots special bullets
         *          its progression is for fire rate and drone amount
         *              Progress and thoughts on this:
         *              Have made the types but haven't removed the Test
         *                  the drones should be stuck to specific areas near the ship and they should turn to face what they shoot at
         *              a
         *          
         *          make the test ability into a shield that, when held, surrounds the ship and protects from all damage whilst draining energy, but you cannot shoot out of it
         *          its progression is for time it can be up, but the last upgrade lets you sacrifice that speed and turn it into a fire orb that melts enemies
         *              Progress and thoughts on this:
         *              Have made the types but haven't removed the Test
         *      
         *      
         *      (Next Thing)
         *      
         *      
         *      
         *      THIS WILL BE DONE AT EITHER A LATER DATE OR WHEN I HAVE TIME AT HOME
         *      
         *      make a "you are an idiot" upgrade that, when chosen, replaces the asteroids, ufos, background, and all other upgrades into "you are a idiot"
         *          do this at home, and have them be in different sizes so that they can actually replace
         *          also have them alternate
         *              for now, before this is done, the test upgrades are "you are an idiot"
         *
         *
         * Have a better way of checking if an upgrade has been collected rthaer than just infinite "else ifs"
         * A lot of of the upgrades will be abilities with upgrade trees, and most normal upgrades will not just be stat boosts other than fire speed
         * Remember to decrease the difficulty and the insta-death part of level 10
         * 
         * Make particles
         * Make actual death animations
         */

        /*REMINDERS:
         * In order to push to Git correctly, you must first create a commit and then push
         * 
         * Don't need to put ==true and can do !bool for false, apply to code
         * 
         * When on new PC, in order to make it work right, you must do the following (may not actually need to do this):
         *      Map a network drive
         *      Install Monogame templates
         *      While it's installing, run the following command in cmd: "dotnet tool install -g dotnet-mgcb"
         * 
         * Upgrade images have to have a resolution of 110 x 88 to fit correctly in the spot
         * 
         * I have made seperate folders in the "Content" folder to hold specific image groups
         * 
         * Sprite Positions are in the middle of the Sprite rather than the upper right corner
         */

        bool IsBulletInBounds(List<Bullet> bullets, int i, Rectangle playSpace)
        {
            bool removed = false;
            if (!playSpace.Contains(bullets[i].Position))
            {
                bullets.RemoveAt(i);
                removed = true;
            }
            return removed;
        }
        void NoneRefresh(List<Upgrade> nones, Upgrade baseNone)
        {
            nones.Clear();

            Upgrade None1 = new Upgrade(baseNone.Position, baseNone.StatType, baseNone.AbilityType, baseNone.UpgradeName, baseNone.UpgradeDescription1,
               baseNone.UpgradeDescription2, baseNone.UpgradeDescription3, baseNone.UpgradeDescription4,
               baseNone.ProgressionList, baseNone.ProgressionLevel, 0, baseNone.Image, baseNone.Rotation, baseNone.Scale, baseNone.Color, false);

            Upgrade None2 = new Upgrade(baseNone.Position, baseNone.StatType, baseNone.AbilityType, baseNone.UpgradeName, baseNone.UpgradeDescription1,
               baseNone.UpgradeDescription2, baseNone.UpgradeDescription3, baseNone.UpgradeDescription4,
               baseNone.ProgressionList, baseNone.ProgressionLevel, 0, baseNone.Image, baseNone.Rotation, baseNone.Scale, baseNone.Color, false);

            Upgrade None3 = new Upgrade(baseNone.Position, baseNone.StatType, baseNone.AbilityType, baseNone.UpgradeName, baseNone.UpgradeDescription1,
               baseNone.UpgradeDescription2, baseNone.UpgradeDescription3, baseNone.UpgradeDescription4,
               baseNone.ProgressionList, baseNone.ProgressionLevel, 0, baseNone.Image, baseNone.Rotation, baseNone.Scale, baseNone.Color, false);

            nones.Add(None1);
            nones.Add(None2);
            nones.Add(None3);
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            playSpace = new Rectangle(0, 0, width, height);
            font = Content.Load<SpriteFont>("Font");
            upgradeTitleFont = Content.Load<SpriteFont>("UpgradeTitleFont");
            upgradeDescFont = Content.Load<SpriteFont>("UpgradeFont");

            BigAsteroid = Content.Load<Texture2D>("Enemies/BigAsteroid");
            SmallAsteroid = Content.Load<Texture2D>("Enemies/SmallAsteroid");
            TinyAsteroid = Content.Load<Texture2D>("Enemies/TinyAsteroid");
            BigSaucer = Content.Load<Texture2D>("Enemies/Big Saucer");
            SmallSaucer = Content.Load<Texture2D>("Enemies/Small Saucer");

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ship = new Ship(playSpace.Center.ToVector2(), 0, Content.Load<Texture2D>("ShipAndShots/Ship"), 0, 1 / 1f, Color.White);

            defaultShot = new Bullet(new Vector2(-20, -20), shotVelocity, Content.Load<Texture2D>("ShipAndShots/Shot"), 0, 1 / 1f, Color.White);
            UFOShot = new Bullet(new Vector2(-20, -20), 5, Content.Load<Texture2D>("ShipAndShots/LaserShot"), 0, 1 / 1f, Color.White);


            level = new Level(5, 1, 0, 0, TimeSpan.FromMilliseconds(20000), 100, 5, 1);

            asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship));
            asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship));
            asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship));

            UpgradeSlot = Content.Load<Texture2D>("UpgradeImages/UpgradeSlot");
            SkipUpgrade = Content.Load<Texture2D>("UpgradeImages/UpgradeSkip");
            UpgradeChoice1 = new Button(new Vector2(135, 240), UpgradeSlot, 0, 1, Color.White);
            UpgradeChoice2 = new Button(new Vector2(405, 240), UpgradeSlot, 0, 1, Color.White);
            UpgradeChoice3 = new Button(new Vector2(675, 240), UpgradeSlot, 0, 1, Color.White);
            UpgradeChoiceList = new Button[] { UpgradeChoice1, UpgradeChoice2, UpgradeChoice3 };


            //Upgrade Stuff (each description row can fit around 20-21 characters, tested with capital A's)

            TestUpgrade = new Upgrade(new Vector2(0, 0), StatUpgradeType.Test, AbilityUpgradeType.None, "Test Upgrade",
                "I dunno", "man V1", "(TESTING)", "", null, 0, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(TestUpgrade);

            TestAbility = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Test, "Test Ability",
                "I dunno", "man V2", "(TESTING)", "(Z Key, 50 energy)", null, 0, 50, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(TestAbility);

            Warp = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Warp, "Warp",
                "Right-click to warp", "to a random point", "on screen. Gain 0.4", "seconds of i-frames.", null, 0, 99, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.DarkGray, false);
            PossibleUpgrades.Add(Warp);

            ShotSpeedUp1 = new Upgrade(new Vector2(0, 0), StatUpgradeType.ShotSpeedUp1, AbilityUpgradeType.None, "Shot Speed+",
                "Reduces the time bet-", "-ween default shots", "and increases special", "gun energy gain.", ShotSpeedProgHolder, 1, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(ShotSpeedUp1);
            ShotSpeedProgHolder.Add(ShotSpeedUp1);

            ShotSpeedUp2 = new Upgrade(new Vector2(0, 0), StatUpgradeType.ShotSpeedUp1, AbilityUpgradeType.None, "Shot Speed++",
                "Reduces the time bet-", "-ween default shots", "and increases special", "gun energy gain more.", ShotSpeedProgHolder, 2, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            ShotSpeedProgHolder.Add(ShotSpeedUp2);

            ShotSpeedUp3 = new Upgrade(new Vector2(0, 0), StatUpgradeType.ShotSpeedUp1, AbilityUpgradeType.None, "Shot Speed+++",
                "You can now shoot", "really fast and", "really often.", "", ShotSpeedProgHolder, 3, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            ShotSpeedProgHolder.Add(ShotSpeedUp3);

            MachineGun = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Machine, "Machine Gun",
                "Gives you a rapid-", "-firing machine gun.", "", "", null, 0, 7, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.DarkSlateGray, false);
            PossibleUpgrades.Add(MachineGun);

            Laser = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Laser, "Laser",
                "Gives you piercing", "laser gun.", "", "", null, 0, 33, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.Red, false);
            PossibleUpgrades.Add(Laser);

            MachineGun.GunBullet = new Bullet(new Vector2(-20, -20), shotVelocity * 1.1f, Content.Load<Texture2D>("ShipAndShots/MachineShot"), 0, 1 / 1f, Color.White);
            Laser.GunBullet = new Bullet(new Vector2(-20, -20), shotVelocity * 1.5f, Content.Load<Texture2D>("ShipAndShots/LaserShot"), 0, 1 / 1f, Color.White);

            None = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.None, "Cold Treasure",
                "We will not", "become stronger.", "", "", null, 0, 0, Content.Load<Texture2D>("UpgradeImages/Cold Treasure"), 0, 1 / 1, Color.White, false);

            NoneRefresh(NoneHolder, None);

            UpgradeSkip = new Button(new Vector2(width - 47, height - 13), SkipUpgrade, 0, 1, Color.White);

            ActiveGuns.Add(None);


            //Upgrade Stuff



            /*  Old Powerup Code, Archived
            powerDamages = new Texture2D[] { Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 0"), Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 4"), 
                Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 3"), Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 2"), 
                Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 1"), Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 0") };
            machine = new Powerup(new Vector2(-50, -50), Powerup.Type.Machine, TimeSpan.FromMilliseconds(3500), TimeSpan.FromMilliseconds(4000), new TimeSpan(0, 0, 0, 0, 100),
                powerDamages, MachineGun.GunBullet, 60, Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/MachineGunPower"), 0, 1 / 1f, Color.Aqua);
            laser = new Powerup(new Vector2(-60, -60), Powerup.Type.Laser, TimeSpan.FromMilliseconds(1500), TimeSpan.FromMilliseconds(2000), new TimeSpan(0, 0, 0, 0, 200),
                powerDamages, Laser.GunBullet, 40, Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/LaserPower"), 0, 1 / 1f, Color.Yellow);
            */

            // TODO: use this.Content to load your game content here
        }
        KeyboardState lastKeyboardState;
        MouseState lastMouseState;
        protected override void Update(GameTime gameTime)
        {
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;


            Window.Title = $"asteroidsCount: {asteroids.Count}      UFOCount: {UFOs.Count}      pressed: {tempPress}        width: {width}      height: {height}";

            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();





            score0s = "";

            if (keyboardState.CapsLock)
            {
                iFrames = TimeSpan.FromMilliseconds(300);
            }
            if (keyboardState.IsKeyDown(Keys.OemOpenBrackets) && lastKeyboardState.IsKeyUp(Keys.OemOpenBrackets))
            {
                asteroids.Clear();
                UFOs.Clear();
            }

            /*  Old Powerup Code, Archived
            machine.Update(gameTime.ElapsedGameTime, shots);
            machine.IsInBounds(playSpace);
            laser.Update(gameTime.ElapsedGameTime, shots);
            laser.IsInBounds(playSpace);

            powerups.Clear();
            if (machine.isActive) { powerups.Add(machine); }
            if (laser.isActive) { powerups.Add(laser); }
            */

            ship.Move(keyboardState);

            MachineGunShotTimer -= gameTime.ElapsedGameTime;
            LaserShotTimer -= gameTime.ElapsedGameTime;
            defaultShotTimer -= gameTime.ElapsedGameTime;

            MachineGun.AbilityUpdate(gameTime.ElapsedGameTime);
            Laser.AbilityUpdate(gameTime.ElapsedGameTime);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (MachineGun.WillGunShoot(MachineGunShotTimer) == true)
                {
                    shots.Add(Bullet.BulletTypeCopy(MachineGun.GunBullet, ship.Position, ship.Rotation));
                    MachineGun.AbilityUse();
                }
                else if (Laser.WillGunShoot(LaserShotTimer) == true)
                {
                    shots.Add(Bullet.BulletTypeCopy(Laser.GunBullet, ship.Position, ship.Rotation));
                    Laser.AbilityUse();
                }
                else if ((defaultShotTimer <= TimeSpan.Zero || lastMouseState.LeftButton == ButtonState.Released) && CurrentActiveGunIndex == 0)
                {
                    shots.Add(Bullet.BulletTypeCopy(defaultShot, ship.Position, ship.Rotation));
                }
            }

            if (MachineGunShotTimer <= TimeSpan.Zero)
            {
                MachineGunShotTimer = reserveMachineGunShotTimer;
            }
            if (LaserShotTimer <= TimeSpan.Zero)
            {
                LaserShotTimer = reserveLaserShotTimer;
            }
            if (defaultShotTimer <= TimeSpan.Zero)
            {
                defaultShotTimer = reserveDefaultShotTimer;
            }

            level.VariablePass(tinyAsteroidVelocity, smallAsteroidVelocity, largeAsteroidVelocity, SmallAsteroid, BigAsteroid, SmallSaucer, BigSaucer);

            level.Update(gameTime.ElapsedGameTime, playSpace, asteroids, UFOs);
            UFOShot.Velocity = level.UFOShotSpeed;

            if (asteroids.Count == 0 && UFOs.Count == 0 && level.Finished ||
                keyboardState.IsKeyDown(Keys.OemCloseBrackets) && lastKeyboardState.IsKeyUp(Keys.OemCloseBrackets))
            {
                if (UpgradeTime < 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        bool same = false;
                        bool none = false;

                        if (i + 1 > PossibleUpgrades.Count)
                        {
                            UpgradesToDraw.Add(NoneHolder[i]);
                            none = true;
                        }
                        else
                        {
                            UpgradesToDraw.Add(PossibleUpgrades[rand.Next(0, PossibleUpgrades.Count)]);
                        }

                        if (i != 0 && !none)
                        {
                            for (int j = 0; j < UpgradesToDraw.Count - 1; j++)
                            {
                                if (UpgradesToDraw[j] == UpgradesToDraw[i] || UpgradesToDraw[i].isActive)
                                {
                                    same = true;
                                }
                            }
                        }
                        if (same)
                        {
                            UpgradesToDraw.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            UpgradesToDraw[i].UpgradeButton = UpgradeChoiceList[i];
                            UpgradesToDraw[i].UpgradeButton.wasClicked = false;
                            UpgradesToDraw[i].UpgradeButton.isActive = true;
                            UpgradesToDraw[i].Position = UpgradesToDraw[i].UpgradeButton.Position;
                        }
                    }
                    UpgradeSkip.isActive = true;
                }

                UpgradeTime++;
            }
            if (UpgradeChosen == true)
            {
                level.GlobalSpawnTimer = level.reserveSpawnTimer;
                if (level.LevelNum == 10)
                {
                    level.GlobalSpawnTimer = new TimeSpan(0, 0, 0, 0, 500);
                }
                level = Level.NextLevel(level);
                ship.Position = playSpace.Center.ToVector2();
                iFrames = TimeSpan.FromMilliseconds(2000);
                lives++;
                ship.Velocity = 0;
                tinyAsteroidVelocity *= 1.05f;
                smallAsteroidVelocity *= 1.1f;
                largeAsteroidVelocity *= 1.2f;
                asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship));
                asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship));
                asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship));
                UpgradeChosen = false;
                UpgradeTime = 0;
            }

            //Upgrade Code

            UpgradeSkip.Update(mouseState, lastMouseState);
            for (int i = 0; i < UpgradesToDraw.Count; i++)
            {
                if (!UpgradeSkip.wasClicked)
                {
                    UpgradesToDraw[i].UpgradeButton.Update(mouseState, lastMouseState);
                    UpgradesToDraw[i].WhenSelected(PossibleUpgrades, ActiveUpgrades, ActiveAbilities, ActiveGuns);
                }
            }

            for (int i = 0; i < UpgradesToDraw.Count; i++)
            {
                bool broken = false;
                if (UpgradeSkip.wasClicked)
                {
                    tempPress = "skipped";
                    for (int j = 0; j < UpgradesToDraw.Count; j++)
                    {
                        UpgradesToDraw[j].Skipped();
                        UpgradesToDraw[j].UpgradeButton.wasClicked = false;
                    }
                    broken = true;
                }
                if (UpgradesToDraw[i].isActive && !UpgradeSkip.wasClicked && !broken)
                {
                    tempPress = UpgradesToDraw[i].UpgradeName;

                    for (int j = 0; j < UpgradesToDraw.Count; j++)
                    {
                        if (j != i)
                        {
                            UpgradesToDraw[j].Skipped();
                            UpgradesToDraw[j].UpgradeButton.wasClicked = false;
                        }
                        else
                        {
                            if (UpgradesToDraw[j] == None)
                            {
                                UpgradesToDraw[j].isActive = false;
                            }
                            else
                            {
                                PossibleUpgrades.Remove(UpgradesToDraw[j]);
                            }
                        }
                    }
                    broken = true;
                }
                if (broken)
                {
                    UpgradeChosen = true;
                    UpgradeSkip.isActive = false;
                    UpgradesToDraw.Clear();
                    NoneRefresh(NoneHolder, None);
                    for (int k = 0; k < UpgradeChoiceList.Length; k++)
                    {
                        UpgradeChoiceList[k] = UpgradeChoiceList[k];
                    }
                    break;
                }
            }

            //Upgrade Code

            iFrames -= gameTime.ElapsedGameTime;
            if (iFrames >= TimeSpan.Zero)
            {
                if ((int)iFrames.TotalMilliseconds % 2 < 1)
                {
                    ship.Color = Color.Black;
                }
                else
                {
                    ship.Color = Color.White;
                }
            }
            else
            {
                ship.Color = Color.White;
            }

            for (int i = 0; i < shots.Count; i++)
            {
                shots[i].Move();

                if (!IsBulletInBounds(shots, i, playSpace))
                {
                    /*  Old Powerup Code, Archived
                    if (!machine.WhenShot(shots, i))
                    {
                        laser.WhenShot(shots, i);
                    }
                    */
                }
            }
            for (int i = 0; i < enemyShots.Count; i++)
            {
                enemyShots[i].Move();

                if (ship.Hitbox.Intersects(enemyShots[i].Hitbox))
                {
                    enemyShots.RemoveAt(i);
                    if (iFrames <= TimeSpan.Zero)
                    {
                        ship.Position = playSpace.Center.ToVector2();
                        lives--;
                        iFrames = TimeSpan.FromMilliseconds(2000);
                        ship.Velocity = 0;
                    }
                    break;
                }

                IsBulletInBounds(enemyShots, i, playSpace);
            }

            for (int i = 0; i < asteroids.Count; i++)
            {
                for (int j = 0; j < shots.Count; j++)
                {
                    if (asteroids[i].Hitbox.Intersects(shots[j].Hitbox))
                    {
                        //Archived PowerUp Code: machine.Spawned(asteroids[i].Position, new Vector2(rand.Next(1, 4), rand.Next(1, 4)), asteroids[i].leSize);

                        if (asteroids[i].leSize == Size.LeChonk)
                        {
                            score += 10;
                            asteroids[i].leSize++;
                            asteroids[i].Image = SmallAsteroid;
                            asteroids[i].Velocity = new Vector2(asteroids[i].Velocity.X * (largeAsteroidVelocity / smallAsteroidVelocity),
                                asteroids[i].Velocity.Y * (largeAsteroidVelocity / smallAsteroidVelocity));
                            asteroids.Add(new Asteroid(new Vector2(asteroids[i].Position.X + 60, asteroids[i].Position.Y),
                                new Vector2(-asteroids[i].Velocity.X * 2, -asteroids[i].Velocity.Y * 2), SmallAsteroid, 0, 1 / 1f, Color.White, Size.Normal));
                        }
                        else if (asteroids[i].leSize == Size.Normal)
                        {
                            score += 30;
                            asteroids[i].leSize++;
                            asteroids[i].Image = TinyAsteroid;
                            asteroids[i].Velocity = new Vector2(asteroids[i].Velocity.X * (smallAsteroidVelocity / tinyAsteroidVelocity),
                                asteroids[i].Velocity.Y * (smallAsteroidVelocity / tinyAsteroidVelocity));
                            asteroids.Add(new Asteroid(new Vector2(asteroids[i].Position.X + 25, asteroids[i].Position.Y),
                                new Vector2(-asteroids[i].Velocity.X * 2, -asteroids[i].Velocity.Y * 2), TinyAsteroid, 0, 1 / 1f, Color.White, Size.Baby));
                        }
                        else if (asteroids[i].leSize == Size.Baby)
                        {
                            score += 50;

                            asteroids.RemoveAt(i);
                        }

                        if (shots[j].Image != Laser.GunBullet.Image)
                        {
                            shots.RemoveAt(j);
                        }

                        break;
                    }
                }

                if (i >= asteroids.Count)
                {
                    break;
                }

                if (asteroids[i].leSize == Size.LeChonk)
                {
                    asteroids[i].Rotation += 0.005f;
                }
                else
                {
                    asteroids[i].Rotation += 0.01f;
                }

                asteroids[i].Position = new Vector2(asteroids[i].Position.X + (float)Math.Sin(asteroids[i].Rotation) + asteroids[i].Velocity.X,
                    asteroids[i].Position.Y - (float)Math.Cos(asteroids[i].Rotation) + asteroids[i].Velocity.Y);

                asteroids[i].IsInBounds(playSpace);

                if (ship.Hitbox.Intersects(asteroids[i].Hitbox))
                {
                    if (iFrames <= TimeSpan.Zero)
                    {
                        ship.Position = playSpace.Center.ToVector2();
                        lives--;
                        iFrames = TimeSpan.FromMilliseconds(2000);
                        ship.Velocity = 0;
                    }
                    //make look good later
                }
            }

            UFOMovementSpace = new Rectangle((int)ship.Position.X - 60, 50, 120, 60);
            UFOShotArea = new Rectangle((int)ship.Position.X - level.UFORange, (int)ship.Position.Y, level.UFORange * 2, 1);

            for (int i = 0; i < UFOs.Count; i++)
            {
                for (int j = 0; j < shots.Count; j++)
                {
                    if (UFOs[i].Hitbox.Intersects(shots[j].Hitbox))
                    {
                        //Archived PowerUp Code: laser.Spawned(UFOs[i].Position, new Vector2(rand.Next(1, 3), rand.Next(1, 3)), UFOs[i].leSize);

                        if (UFOs[i].leSize == Size.Normal)
                        {
                            score += 100;
                        }
                        else if (UFOs[i].leSize == Size.Baby)
                        {
                            score += 200;
                        }

                        UFOs.RemoveAt(i);
                        if (shots[j].Image != Laser.GunBullet.Image)
                        {
                            shots.RemoveAt(j);
                        }
                        break;
                    }
                }

                if (i >= UFOs.Count)
                {
                    break;
                }

                if (UFOs[i].Update(gameTime.ElapsedGameTime))
                {
                    enemyShots.Add(UFOs[i].Shoot(UFOShotArea, UFOShot));
                    break;
                }

                if (UFOs[i].Position.X <= UFOMovementSpace.X)
                {
                    UFOs[i].Velocity.X = Math.Abs(UFOs[i].Velocity.X);
                }
                else if (UFOs[i].Position.X >= UFOMovementSpace.X + UFOMovementSpace.Width)
                {
                    UFOs[i].Velocity.X = Math.Abs(UFOs[i].Velocity.X);
                    UFOs[i].Velocity.X *= -1;
                }


                if (UFOs[i].Position.Y <= UFOMovementSpace.Y)
                {
                    UFOs[i].Velocity.Y = Math.Abs(UFOs[i].Velocity.Y);
                }
                else if (UFOs[i].Position.Y >= UFOMovementSpace.Y + UFOMovementSpace.Height)
                {
                    UFOs[i].Velocity.Y = Math.Abs(UFOs[i].Velocity.Y);
                    UFOs[i].Velocity.Y *= -1;
                }

                UFOs[i].Position = new Vector2(UFOs[i].Position.X + UFOs[i].Velocity.X, UFOs[i].Position.Y + UFOs[i].Velocity.Y);

                UFOs[i].IsInBounds(playSpace);

                if (ship.Hitbox.Intersects(UFOs[i].Hitbox))
                {
                    if (iFrames <= TimeSpan.Zero)
                    {
                        ship.Position = playSpace.Center.ToVector2();
                        lives--;
                        iFrames = TimeSpan.FromMilliseconds(2000);
                        ship.Velocity = 0;
                    }
                }
            }

            ship.IsInBounds(playSpace);

            if (lives < 0)
            {
                Exit();
            }

            for (int i = 0; i < 5 - score.ToString().Length; i++)
            {
                score0s += '0';
            }
            if (score > 99999)
            {
                score = 0;
            }
            if (score + 1 % 10000 == 0)
            {
                lives++;
            }

            //Upgrade Effect Code (definitly replace the bloody 'if elses' in the future)


            for (int i = 0; i < ShotSpeedProgHolder.Count; i++)
            {
                if (ShotSpeedProgHolder[i].isActive && !ShotSpeedProgHolder[i].inEffect)
                {
                    reserveDefaultShotTimer -= new TimeSpan(0, 0, 0, 0, 250);
                    MachineGun.EnergyGainMultiplier += 0.5f;
                    Laser.EnergyGainMultiplier += 0.5f;
                    ShotSpeedProgHolder[i].inEffect = true;
                    break;
                }
            }

            /*
            if (ShotSpeedUp1.isActive==true && ShotSpeedUp1.inEffect==false)
            {
                defaultShotTimer = new TimeSpan(0, 0, 0, 1, 0);
                reserveDefaultShotTimer = defaultShotTimer;
                ShotSpeedUp1.inEffect = true;
            }
            else if (ShotSpeedUp2.isActive==true && ShotSpeedUp2.inEffect==false)
            {
                defaultShotTimer = new TimeSpan(0, 0, 0, 0, 750);
                reserveDefaultShotTimer = defaultShotTimer;
                ShotSpeedUp2.inEffect = true;
            }
            else if (ShotSpeedUp3.isActive==true && ShotSpeedUp3.inEffect==false)
            {
                defaultShotTimer = new TimeSpan(0, 0, 0, 0, 500);
                reserveDefaultShotTimer = defaultShotTimer;
                ShotSpeedUp3.inEffect = true;
            }
            */


            for (int i = 0; i < ActiveGuns.Count; i++)
            {
                bool swapped = false;

                if (keyboardState.IsKeyDown(Keys.D1 + i) && lastKeyboardState.IsKeyUp(Keys.D1 + i))
                {
                    swapped = true;
                }
                if (mouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue)
                {
                    i = CurrentActiveGunIndex;
                    if (i + 1 == ActiveGuns.Count)
                    {
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                    swapped = true;
                }
                if (mouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue)
                {
                    i = CurrentActiveGunIndex;
                    if (i == 0)
                    {
                        i = ActiveGuns.Count - 1;
                    }
                    else
                    {
                        i--;
                    }
                    swapped = true;
                }

                if (swapped)
                {
                    CurrentActiveGunIndex = i;

                    if (i != 0) { GunInEffectName = ActiveGuns[i].UpgradeName; GunInEffectColor = ActiveGuns[i].Color; ActiveGuns[i].inEffect = true; }
                    else { GunInEffectName = "Default"; GunInEffectColor = Color.DarkGray; }

                    for (int j = 1; j < ActiveGuns.Count; j++)
                    {
                        if (j != i)
                        {
                            ActiveGuns[j].inEffect = false;
                        }
                    }
                    break;
                }

            }
            /*
            if (keyboardState.IsKeyDown(Keys.D1) && lastKeyboardState.IsKeyUp(Keys.D1))
            {
                GunInEffect = "Default";
                MachineGun.inEffect = false;
                Laser.inEffect = false;
            }
            else if (keyboardState.IsKeyDown(Keys.D2) && lastKeyboardState.IsKeyUp(Keys.D2) && MachineGun.isActive)
            {
                GunInEffect = "Machine Gun";
                MachineGun.inEffect = true;
                Laser.inEffect = false;
            }
            else if (keyboardState.IsKeyDown(Keys.D3) && lastKeyboardState.IsKeyUp(Keys.D3) && Laser.isActive)
            {
                GunInEffect = "Laser";
                MachineGun.inEffect = false;
                Laser.inEffect = true;
            }
            */

            //Upgrade Effect Code

            //Ability Effect Code


            Warp.AbilityUpdate(gameTime.ElapsedGameTime);
            if (mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released && Warp.WillAbilityGetUsed())
            {
                ship.Position = new Vector2(rand.Next(0, width), rand.Next(0, height));
                iFrames = new TimeSpan(0, 0, 0, 0, 400);
                Warp.EnergyGainMultiplier = 8.25f;
                Warp.AbilityUse();
            }

            TestAbility.AbilityUpdate(gameTime.ElapsedGameTime);
            if (keyboardState.IsKeyDown(Keys.Z) && lastKeyboardState.IsKeyUp(Keys.Z) && TestAbility.WillAbilityGetUsed())
            {
                TestAbility.EnergyGainMultiplier = 10;
                TestAbility.AbilityUse();
            }

            //Ability Effect Code


            // TODO: Add your update logic here

            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            for (int i = 0; i < asteroids.Count; i++)
            {
                asteroids[i].Draw(_spriteBatch);
            }
            for (int i = 0; i < UFOs.Count; i++)
            {
                UFOs[i].Draw(_spriteBatch);
            }


            ship.Draw(_spriteBatch);

            for (int i = 0; i < shots.Count; i++)
            {
                shots[i].Draw(_spriteBatch);
            }

            for (int i = 0; i < enemyShots.Count; i++)
            {
                enemyShots[i].Draw(_spriteBatch);
            }


            for (int i = 0; i < lives; i++)
            {
                _spriteBatch.Draw(Content.Load<Texture2D>("ShipAndShots/Life"), new Vector2(10 + (i * 15), 28), Color.DarkGray);
            }

            _spriteBatch.DrawString(font, $"{score0s}{score}", new Vector2(10, 10), Color.DarkGray);

            _spriteBatch.DrawString(font, $"Level: {level.LevelNum}", new Vector2(width - 150, 10), Color.DarkGray);

            /* Old Powerup Code, Archived
            machine.Draw(_spriteBatch);
            laser.Draw(_spriteBatch);

            for (int i = 0; i < powerups.Count; i++)
            {
                _spriteBatch.DrawString(font, $"{powerups[i].leType.ToString()}: {Math.Round(powerups[i].Duration.TotalMilliseconds)}",
                    new Vector2(10, 50 + (i * 20)), Color.DarkGray);
            }
            */

            _spriteBatch.DrawString(font, $"{GunInEffectName}", new Vector2(10, 50), GunInEffectColor);

            for (int i = 0; i < ActiveAbilities.Count; i++)
            {
                ActiveAbilities[i].AbilityEnergyStack(ActiveAbilities, i);
                ActiveAbilities[i].EnergyDraw(_spriteBatch);
            }
            for (int i = 0; i < ActiveGuns.Count; i++)
            {
                ActiveGuns[i].EnergyDraw(_spriteBatch);
            }


            for (int i = 0; i < UpgradesToDraw.Count; i++)
            {
                UpgradesToDraw[i].Draw(upgradeTitleFont, upgradeDescFont, _spriteBatch);
            }
            UpgradeSkip.Draw(_spriteBatch);


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}