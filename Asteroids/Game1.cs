using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Test = 1,
            ShotSpeedUp1 = 2,
            ShotSpeedUp2 = 3,
            ShotSpeedUp3 = 4
        }
        public enum AbilityUpgradeType
        {
            None = 0,
            Machine = 1,
            Laser = 2,
            Warp = 4
        }
        List<Upgrade> PossibleUpgrades = new List<Upgrade>();
        List<Upgrade> UpgradesToDraw = new List<Upgrade>();
        List<Upgrade> ActiveUpgrades = new List<Upgrade>();
        List<Upgrade> ActiveAbilities = new List<Upgrade>();
        List<Upgrade> ActiveGuns = new List<Upgrade>();
        List<Upgrade> NoneHolder = new List<Upgrade>();

        bool UpgradeChosen = false;
        int UpgradeTime = 0;

        Upgrade TestUpgrade1;
        Upgrade TestUpgrade2;
        Upgrade None;
        Button UpgradeSkip;

        Upgrade ShotSpeedUp1;
        Upgrade ShotSpeedUp2;
        Upgrade ShotSpeedUp3;
        List<Upgrade> ShotSpeedProgHolder = new List<Upgrade>();

        Upgrade Warp;

        string GunInEffect = "Default";
        Upgrade MachineGun;
        TimeSpan MachineGunShotTimer = new TimeSpan(0, 0, 0, 0, 150);
        TimeSpan reserveMachineGunShotTimer = new TimeSpan(0, 0, 0, 0, 150);
        Upgrade Laser;
        TimeSpan LaserShotTimer = new TimeSpan(0, 0, 0, 0, 600);
        TimeSpan reserveLaserShotTimer = new TimeSpan(0, 0, 0, 0, 600);

        Powerup machine;
        Bullet machineShot;
        Powerup laser;
        Bullet laserShot;
        Texture2D[] powerDamages;
        List<Powerup> powerups = new List<Powerup>();



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
         * Make different gun powerups
         *      examples being: machine gun, laser, and bombs
         *                  I think that the wa spawning is done should work like so:
         *                  each different thing can spawn in a powerup
         *                  the harder it was to take out, the higher the chance of it spawning a powerup, done via a multiplier
         *                  certain powerups can only come from some things (only ufos can spawn lasers, etc.)
         *                          ok, so they now can spawn at any time, tho you'll have to manually ask them if you know what i mena
         *                          now i need to make sure that they can only spawn from specific things, but you do that when more powers made
         *      
         *      make bomb powerup
         *      
         *      
         *      make others
         *      
         *      PRIORITY DOWN HERE
         *      
         *      
         *      so by dumbass thought of maybe making this a rougelike; i hate myself but i want to
         *          have some unlockable abilities with meters and cooldowns and upgrades
         *          this would force me to make the game less of a pain so that you can, you know, progress
         *              som upgrade ideas: firing speed, shields, drones
         *              som ability ideas: timestop, time erase, time rewind, screen nuke
         *                  i'll need to make the powerups into weapon upgrades that can be swapped to
         *                  
         *      so first, i'll have to design an upgrade menu in-between levels where you can pick up or skip upgrades
         *          (use as reference: https://progameguides.com/wp-content/uploads/2022/06/roblox-hours-vitality-1024x576.jpg)
         *          made a slot asset
         *      
         *      
         *      make the gui work with its conditions; buttons are active by default rn for testing
         *          rn, make a gui that shows up and detects which choice is pressed
         *      ok, it does that
         *      
         *              ok, made a gui with working buttons
         *                  first, make the upgrades and their spots random, cuz right now it's the same upgrade in the same spot {DONE}
         *                  now, work on making the gui appear when a level is cleared {DONE}
         *                  
         *                  at home, make chosen upgrades get removed from bank and make the gui appearing between levels work {DONE}
         *                  
         *                  make a "none" upgrade with a "noneUpgrade" list {DONE}
         *                  
         *                  after that, make temporary upgrades that do shit {MOSTLY DONE}
         *                      try to do this with movement speed; the type will dictate the stat changes, for movement speed, try to do x/1000 for changes
         *                          actually, make a temporary upgrade that will make the defaultShotTimer lower and give it a tree of 3 total upgrades (might not keep these)
         *                          if i keep this, i'll make this shot speed increase all shots rather than just the default {NOT DONE}
         *                  
         *                  actually, first make upgrades sort into an "activeUpgrades" and "activeAbilities" list
         *                      we'll keep at least the "activeUpgrades" as a visual thing and maybe use activeAbilities
         *                  
         *                  after that, make the machine gun into an ability and test it {DONE}
         *                      have fancy little text on the side that says what weapon you have and allow switching
         *                      I can attempt to do this by making an ability class that is mostly a copy of powerup to test machine, then replace powerup with ability
         *                          have weapon swaping be done via scroll wheel in main class
         *                          
         *                  ok, so the weapons can switch via number bar, also there is no special ability class, just use upgrade
         *                      
         *                  convert powerups into gun upgrades once I have made machine gun work {DONE}
         *                  
         *                  since the temporary powerups became permanent upgrades, nerf them a bit {DOING}
         *                      since i can't be bothered to do this, reset them to their original states, but first do this:
         *                          give them an energy bar like in megaman, where using a weapon takes energy that regenerates after time
         *                          
         *                                  have energy bar as a rectangle for now
         *                                  it always has a length of 100, and each weapon has a specific amount of energy used per shot
         *                                  regenerate 4 energy a second smoothly
         *                                  it appears below the gun name
         *                                  the energy rectangles need to be private
         *                                  
         *                                  energy works, but needs changes as listed below
         *                                  
         *                                  rightt now, energy gain isn't smooth
         *                                  also, since they now use energy, remove the shot timers for the special guns
         *                                      rework the shot timer upgrades to increase energy gain rather than reduce shot timer for special guns
         *                                  
         *                                      will need to get better system for seeing active gun, rn i can have the draw detect a gun but i want to have a list for easier access
         *                                          will make an array that holds the collected guns, so cycling with mouse will work and i cna use class to cycle
         *                                              at the time of writing have an unused gun array, go from there
         *                                          
         *                                  for now, the guns check for energy before firing, change this after making it work
         *                                  
         *                                  after this, make a temp ability that uses the energy bar, and use the powerup code to make different meters stack
         *                      
         *                  clean up the old powerup code since it is no longer neccesary {AFTER}
         *                  
         *                  come up with a better way of checking if an upgrade has been collected that isn't "else ifs"
         *      
         *      
         *      CLEAN THIS SOON
         *      
         *      Most of the upgrades will be abilities with upgrade trees, and most normal upgrades will not just be stat boosts other than fire speed
         *      
         *      
         *      remember, sprite's positions are in the middle of the sprite
         *      
         *      **DONE**
         *      deal with upgrade class later (rn it's only a copy of powerup with 0 differences) {Do now}
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
         *      have a button class;        {FINISHED}
         *          inherits from sprite
         *          has these funstions:
         *              a function that detects when it's clicked on
         *              a function that detects if it's held
         *              a function that detects when it's released
         *              an update function that checks all of the above at once
         *              
         *      
         *      make a "you are an idiot" upgrade that, when chosen, replaces the asteroids, ufos, background, and all other upgrades into "you are a idiot"
         *          do this at home, and have them be in different sizes so that they can actually replace
         *          also have them alternate
         *              for now, before this is done, the test upgrades are "you are an idiot"
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
               baseNone.ProgressionList, baseNone.ProgressionLevel, baseNone.Cooldown, 0, baseNone.Image, baseNone.Rotation, baseNone.Scale, baseNone.Color, false);

            Upgrade None2 = new Upgrade(baseNone.Position, baseNone.StatType, baseNone.AbilityType, baseNone.UpgradeName, baseNone.UpgradeDescription1,
               baseNone.UpgradeDescription2, baseNone.UpgradeDescription3, baseNone.UpgradeDescription4,
               baseNone.ProgressionList, baseNone.ProgressionLevel, baseNone.Cooldown, 0, baseNone.Image, baseNone.Rotation, baseNone.Scale, baseNone.Color, false);

            Upgrade None3 = new Upgrade(baseNone.Position, baseNone.StatType, baseNone.AbilityType, baseNone.UpgradeName, baseNone.UpgradeDescription1,
               baseNone.UpgradeDescription2, baseNone.UpgradeDescription3, baseNone.UpgradeDescription4,
               baseNone.ProgressionList, baseNone.ProgressionLevel, baseNone.Cooldown, 0, baseNone.Image, baseNone.Rotation, baseNone.Scale, baseNone.Color, false);

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
            machineShot = new Bullet(new Vector2(-20, -20), shotVelocity * 1.1f, Content.Load<Texture2D>("ShipAndShots/MachineShot"), 0, 1 / 1f, Color.White);
            laserShot = new Bullet(new Vector2(-20, -20), shotVelocity * 1.5f, Content.Load<Texture2D>("ShipAndShots/LaserShot"), 0, 1 / 1f, Color.White);

            powerDamages = new Texture2D[] { Content.Load<Texture2D>("Hit 0"), Content.Load<Texture2D>("Hit 4"), Content.Load<Texture2D>("Hit 3"),
                Content.Load<Texture2D>("Hit 2"), Content.Load<Texture2D>("Hit 1"), Content.Load<Texture2D>("Hit 0") };
            machine = new Powerup(new Vector2(-50, -50), Powerup.Type.Machine, TimeSpan.FromMilliseconds(3500), TimeSpan.FromMilliseconds(4000), new TimeSpan(0, 0, 0, 0, 100),
                powerDamages, machineShot, 60, Content.Load<Texture2D>("MachineGunPower"), 0, 1 / 1f, Color.Aqua);
            laser = new Powerup(new Vector2(-60, -60), Powerup.Type.Laser, TimeSpan.FromMilliseconds(1500), TimeSpan.FromMilliseconds(2000), new TimeSpan(0, 0, 0, 0, 200),
                powerDamages, laserShot, 40, Content.Load<Texture2D>("LaserPower"), 0, 1 / 1f, Color.Yellow);

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


            //Upgrade Stuff (each description row can fit around 19-20 characters, tested with capital A's)

            TestUpgrade1 = new Upgrade(new Vector2(0, 0), StatUpgradeType.Test, AbilityUpgradeType.None, "Test1",
                "I dunno", "man V1", "(TESTING)", "", null, 0, BurnerTime, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(TestUpgrade1);

            TestUpgrade2 = new Upgrade(new Vector2(0, 0), StatUpgradeType.Test, AbilityUpgradeType.None, "Test2",
                "I dunno", "man V2", "(TESTING)", "", null, 0, BurnerTime, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(TestUpgrade2);

            Warp = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Warp, "Warp",
                "Right-click to warp", "to a random point", "on screen. Gain 0.4", "seconds of i-frames.", null, 0, TimeSpan.FromSeconds(3), 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(Warp);

            ShotSpeedUp1 = new Upgrade(new Vector2(0, 0), StatUpgradeType.ShotSpeedUp1, AbilityUpgradeType.None, "Shot Speed+",
                "Reduces the time", "between shots to", "one second.", "", ShotSpeedProgHolder, 1, BurnerTime, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(ShotSpeedUp1);
            ShotSpeedProgHolder.Add(ShotSpeedUp1);

            ShotSpeedUp2 = new Upgrade(new Vector2(0, 0), StatUpgradeType.ShotSpeedUp1, AbilityUpgradeType.None, "Shot Speed++",
                "Reduces the time", "between shots to", "three quarters of", "a second.", ShotSpeedProgHolder, 2, BurnerTime, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            ShotSpeedProgHolder.Add(ShotSpeedUp2);

            ShotSpeedUp3 = new Upgrade(new Vector2(0, 0), StatUpgradeType.ShotSpeedUp1, AbilityUpgradeType.None, "Shot Speed+++",
                "Reduces the time", "between shots to", "half a second.", "", ShotSpeedProgHolder, 3, BurnerTime, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            ShotSpeedProgHolder.Add(ShotSpeedUp3);

            MachineGun = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Machine, "Machine Gun",
                "Gives you a rapid-", "-firing machine gun.", "(Num-bar 2)", "", null, 0, BurnerTime, 7, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(MachineGun);

            Laser = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Laser, "Laser",
                "Gives you piercing", "laser gun.", "(Num-bar 3)", "", null, 0, BurnerTime, 33, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(Laser);

            None = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.None, "Cold Treasure",
                "We will not", "become stronger.", "", "", null, 0, BurnerTime, 0, Content.Load<Texture2D>("UpgradeImages/Cold Treasure"), 0, 1 / 1, Color.White, false);

            NoneRefresh(NoneHolder, None);

            UpgradeSkip = new Button(new Vector2(width - 47, height - 13), SkipUpgrade, 0, 1, Color.White);


            //Upgrade Stuff


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

            machine.Update(gameTime.ElapsedGameTime, shots);
            machine.IsInBounds(playSpace);
            laser.Update(gameTime.ElapsedGameTime, shots);
            laser.IsInBounds(playSpace);

            powerups.Clear();
            if (machine.isActive) { powerups.Add(machine); }
            if (laser.isActive) { powerups.Add(laser); }

            ship.Move(keyboardState);

            MachineGunShotTimer -= gameTime.ElapsedGameTime;
            LaserShotTimer -= gameTime.ElapsedGameTime;
            defaultShotTimer -= gameTime.ElapsedGameTime;

            Laser.AbilityUpdate(gameTime.ElapsedGameTime);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (MachineGun.isActive && MachineGun.inEffect && MachineGunShotTimer<=TimeSpan.Zero)
                {
                    shots.Add(Bullet.BulletTypeCopy(machineShot, ship.Position, ship.Rotation));
                    MachineGun.AbilityUse();
                }
                else if (Laser.isActive && Laser.inEffect && LaserShotTimer<=TimeSpan.Zero && Laser.energyRemaining.Width>=33)
                {
                    shots.Add(Bullet.BulletTypeCopy(laserShot, ship.Position, ship.Rotation));
                    Laser.AbilityUse();
                }
                else if (defaultShotTimer<=TimeSpan.Zero || lastMouseState.LeftButton==ButtonState.Released)
                {
                    if (!MachineGun.inEffect && !Laser.inEffect)
                    {
                        shots.Add(Bullet.BulletTypeCopy(defaultShot, ship.Position, ship.Rotation));
                    }
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
                if (UpgradeTime<1)
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

                        if (i!=0 && !none)
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
            if (UpgradeChosen==true)
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
                    UpgradesToDraw[i].WhenSelected(PossibleUpgrades, ActiveUpgrades, ActiveAbilities);
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
                            if (UpgradesToDraw[j]==None)
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
                    if (!machine.WhenShot(shots, i))
                    {
                        laser.WhenShot(shots, i);
                    }
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
                        machine.Spawned(asteroids[i].Position, new Vector2(rand.Next(1, 4), rand.Next(1, 4)), asteroids[i].leSize);

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

                        if (shots[j].Image!=laserShot.Image)
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
                        laser.Spawned(UFOs[i].Position, new Vector2(rand.Next(1, 3), rand.Next(1, 3)), UFOs[i].leSize);

                        if (UFOs[i].leSize == Size.Normal)
                        {
                            score += 100;
                        }
                        else if (UFOs[i].leSize == Size.Baby)
                        {
                            score += 200;
                        }

                        UFOs.RemoveAt(i);
                        if (shots[j].Image!=laserShot.Image)
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


            //Upgrade Effect Code

            //Ability Effect Code


            Warp.Cooldown -= gameTime.ElapsedGameTime;
            if (mouseState.RightButton==ButtonState.Pressed && lastMouseState.RightButton==ButtonState.Released && Warp.isActive && Warp.Cooldown<=TimeSpan.Zero)
            {
                ship.Position = new Vector2(rand.Next(0, width), rand.Next(0, height));
                iFrames = new TimeSpan(0, 0, 0, 0, 400);
                Warp.CooldownRefresh();
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

            machine.Draw(_spriteBatch);
            laser.Draw(_spriteBatch);

            for (int i = 0; i < powerups.Count; i++)
            {
                _spriteBatch.DrawString(font, $"{powerups[i].leType.ToString()}: {Math.Round(powerups[i].Duration.TotalMilliseconds)}",
                    new Vector2(10, 50 + (i * 20)), Color.DarkGray);
            }

            _spriteBatch.DrawString(font, $"{GunInEffect}", new Vector2(10, 50), Color.DarkGray);
            MachineGun.EnergyDraw(_spriteBatch);
            Laser.EnergyDraw(_spriteBatch);


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