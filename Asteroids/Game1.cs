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

        public enum StatUpgradeType
        {
            None = 0,
            Test = 1
        }
        public enum AbilityUpgradeType
        {
            None = 0,

        }
        List<Upgrade> PossibleUpgrades = new List<Upgrade>();
        List<Upgrade> UpgradesToDraw = new List<Upgrade>();
        List<Upgrade> ActiveUpgrades = new List<Upgrade>();
        List<Upgrade> ActiveAbilities = new List<Upgrade>();

        bool UpgradeChosen = false;
        int UpgradeTime = 0;

        Upgrade TestUpgrade1;
        Upgrade TestUpgrade2;
        Upgrade TestUpgrade3;
        Upgrade TestUpgrade4;
        Upgrade TestUpgrade5;
        Button UpgradeSkip;

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
         *              som upgrade ideas: speed, firing speed, shields, drones
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
         *                  now, work on making the gui appear when a level is cleared {AFTER}
         *                  
         *                  at home, make chosen upgrades get removed from bank and make the gui appearing between levels work
         *                  
         *                  after that, make temporary upgrades that do shit {AFTER}
         *      
         *          
         *      remember, sprite's positions are in the middle of the sprite
         *      
         *      **DONE**
         *      deal with upgrade class later (rn it's only a copy of powerup with 0 differences) {Do now}
         *      actually, make upgrade a class with a button and upgrade. Basically, the entire upgrade is in this class.
         *      **DONE**
         *      
         *          rn, the gui and its buttons will be in the main class just so I can first make them work
         *          
         *          **DONE**
         *          have it pick 3 random upgrades and place them with a certain amount of pixels in between
         *          have the location assigned when the upgrade is gotten
         *          spawn the text and image using the main button position
         *          call the button inside of the upgrade to check for presses
         *          set position and upgradebutton position from game1
         *          **DONE**
         *          
         *          have abilities have upgrade trees in forms of list, and have the current prog as an int
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
         *      convert powerups into gun upgrades
         *      
         * Make particles
         * Make actual death animations
         */

        /*REMINDERS:
         * In order to push to Git correctly, you must first create a commit and then push
         * 
         * Don't need to put ==true and can do !bool for false, apply to code
         * 
         * When on new PC, in order to make it work right, you must do the following:
         *      Map a network drive
         *      Install Monogame templates
         *      While it's installing, run the following command in cmd: "dotnet tool install -g dotnet-mgcb"
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

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ship = new Ship(playSpace.Center.ToVector2(), 0, Content.Load<Texture2D>("Ship"), 0, 1 / 1f, Color.White);

            defaultShot = new Bullet(new Vector2(-20, -20), shotVelocity, Content.Load<Texture2D>("Shot"), 0, 1 / 1f, Color.White);
            UFOShot = new Bullet(new Vector2(-20, -20), 5, Content.Load<Texture2D>("LaserShot"), 0, 1 / 1f, Color.White);
            machineShot = new Bullet(new Vector2(-20, -20), shotVelocity * 1.1f, Content.Load<Texture2D>("MachineShot"), 0, 1 / 1f, Color.White);
            laserShot = new Bullet(new Vector2(-20, -20), shotVelocity * 1.5f, Content.Load<Texture2D>("LaserShot"), 0, 1 / 1f, Color.White);

            powerDamages = new Texture2D[] { Content.Load<Texture2D>("Hit 0"), Content.Load<Texture2D>("Hit 4"), Content.Load<Texture2D>("Hit 3"),
                Content.Load<Texture2D>("Hit 2"), Content.Load<Texture2D>("Hit 1"), Content.Load<Texture2D>("Hit 0") };
            machine = new Powerup(new Vector2(-50, -50), Powerup.Type.Machine, TimeSpan.FromMilliseconds(3500), TimeSpan.FromMilliseconds(4000), new TimeSpan(0, 0, 0, 0, 100),
                powerDamages, machineShot, 60, Content.Load<Texture2D>("MachineGunPower"), 0, 1 / 1f, Color.Aqua);
            laser = new Powerup(new Vector2(-60, -60), Powerup.Type.Laser, TimeSpan.FromMilliseconds(1500), TimeSpan.FromMilliseconds(2000), new TimeSpan(0, 0, 0, 0, 200),
                powerDamages, laserShot, 40, Content.Load<Texture2D>("LaserPower"), 0, 1 / 1f, Color.Yellow);

            level = new Level(5, 1, 0, 0, TimeSpan.FromMilliseconds(20000), 100, 5, 1);

            asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, Content.Load<Texture2D>("BigAsteroid"), ship));
            asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, Content.Load<Texture2D>("BigAsteroid"), ship));
            asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, Content.Load<Texture2D>("BigAsteroid"), ship));

            UpgradeSlot = Content.Load<Texture2D>("UpgradeSlot");
            SkipUpgrade = Content.Load<Texture2D>("UpgradeSkip");
            UpgradeChoice1 = new Button(new Vector2(135, 240), UpgradeSlot, 0, 1, Color.White);
            UpgradeChoice2 = new Button(new Vector2(405, 240), UpgradeSlot, 0, 1, Color.White);
            UpgradeChoice3 = new Button(new Vector2(675, 240), UpgradeSlot, 0, 1, Color.White);
            UpgradeChoiceList = new Button[] { UpgradeChoice1, UpgradeChoice2, UpgradeChoice3 };

            TestUpgrade1 = new Upgrade(new Vector2(0, 0), StatUpgradeType.Test, AbilityUpgradeType.None, "Test1",
                "I dunno", "man V1", "(TESTING)", "", Content.Load<Texture2D>("SmallAsteroid"), 0, 1 / 1, Color.White);
            PossibleUpgrades.Add(TestUpgrade1);

            TestUpgrade2 = new Upgrade(new Vector2(0, 0), StatUpgradeType.Test, AbilityUpgradeType.None, "Test2",
                "I dunno", "man V2", "(TESTING)", "", Content.Load<Texture2D>("SmallAsteroid"), 0, 1 / 1, Color.White);
            PossibleUpgrades.Add(TestUpgrade2);

            TestUpgrade3 = new Upgrade(new Vector2(0, 0), StatUpgradeType.Test, AbilityUpgradeType.None, "Test3",
                "I dunno", "man V3", "(TESTING)", "", Content.Load<Texture2D>("SmallAsteroid"), 0, 1 / 1, Color.White);
            PossibleUpgrades.Add(TestUpgrade3);

            TestUpgrade4 = new Upgrade(new Vector2(0, 0), StatUpgradeType.Test, AbilityUpgradeType.None, "Test4",
                "I dunno", "man V4", "(TESTING)", "", Content.Load<Texture2D>("SmallAsteroid"), 0, 1 / 1, Color.White);
            PossibleUpgrades.Add(TestUpgrade4);

            TestUpgrade5 = new Upgrade(new Vector2(0, 0), StatUpgradeType.Test, AbilityUpgradeType.None, "Test5",
                "I dunno", "man V5", "(TESTING)", "", Content.Load<Texture2D>("SmallAsteroid"), 0, 1 / 1, Color.White);
            PossibleUpgrades.Add(TestUpgrade5);

            UpgradeSkip = new Button(new Vector2(width - 47, height - 13), SkipUpgrade, 0, 1, Color.White);




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

            machine.ShotTimer -= gameTime.ElapsedGameTime;
            laser.ShotTimer -= gameTime.ElapsedGameTime;
            defaultShotTimer -= gameTime.ElapsedGameTime;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (machine.isActive && machine.ShotTimer <= TimeSpan.Zero)
                {
                    shots.Add(Bullet.BulletTypeCopy(machine.ProjectileShot, ship.Position, ship.Rotation));
                }
                else if (laser.isActive && laser.ShotTimer <= TimeSpan.Zero)
                {
                    shots.Add(Bullet.BulletTypeCopy(laser.ProjectileShot, ship.Position, ship.Rotation));
                }
                else if (defaultShotTimer <= TimeSpan.Zero || lastMouseState.LeftButton == ButtonState.Released)
                {
                    shots.Add(Bullet.BulletTypeCopy(defaultShot, ship.Position, ship.Rotation));
                }
            }
            if (mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released)
            {
                ship.Position = new Vector2(rand.Next(0, width), rand.Next(0, height));
            }

            if (machine.ShotTimer <= TimeSpan.Zero)
            {
                machine.ShotTimer = machine.reserveShotTimer;
            }
            if (laser.ShotTimer <= TimeSpan.Zero)
            {
                laser.ShotTimer = laser.reserveShotTimer;
            }
            if (defaultShotTimer <= TimeSpan.Zero)
            {
                defaultShotTimer = reserveDefaultShotTimer;
            }

            level.VariablePass(tinyAsteroidVelocity, smallAsteroidVelocity, largeAsteroidVelocity, Content.Load<Texture2D>("SmallAsteroid"), Content.Load<Texture2D>("BigAsteroid"),
                Content.Load<Texture2D>("Small Saucer"), Content.Load<Texture2D>("Big Saucer"));

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
                        UpgradesToDraw.Add(PossibleUpgrades[rand.Next(0, PossibleUpgrades.Count)]);

                        if (i != 0)
                        {
                            for (int j = 0; j < UpgradesToDraw.Count - 1; j++)
                            {
                                if (UpgradesToDraw[j] == UpgradesToDraw[i])
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
                asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, Content.Load<Texture2D>("BigAsteroid"), ship));
                asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, Content.Load<Texture2D>("BigAsteroid"), ship));
                asteroids.Add(Asteroid.InitialSpawn(playSpace, largeAsteroidVelocity, Content.Load<Texture2D>("BigAsteroid"), ship));
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
                    UpgradesToDraw[i].WhenSelected(PossibleUpgrades, ActiveUpgrades, null, 0);
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
                        }
                    }
                    broken = true;
                }
                if (broken)
                {
                    UpgradeChosen = true;
                    UpgradeSkip.isActive = false;
                    UpgradesToDraw.Clear();
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
                            asteroids[i].Image = Content.Load<Texture2D>("SmallAsteroid");
                            asteroids[i].Velocity = new Vector2(asteroids[i].Velocity.X * (largeAsteroidVelocity / smallAsteroidVelocity),
                                asteroids[i].Velocity.Y * (largeAsteroidVelocity / smallAsteroidVelocity));
                            asteroids.Add(new Asteroid(new Vector2(asteroids[i].Position.X + 60, asteroids[i].Position.Y),
                                new Vector2(-asteroids[i].Velocity.X * 2, -asteroids[i].Velocity.Y * 2), Content.Load<Texture2D>("SmallAsteroid"), 0,
                                1 / 1f, Color.White, Size.Normal));
                        }
                        else if (asteroids[i].leSize == Size.Normal)
                        {
                            score += 30;
                            asteroids[i].leSize++;
                            asteroids[i].Image = Content.Load<Texture2D>("TinyAsteroid");
                            asteroids[i].Velocity = new Vector2(asteroids[i].Velocity.X * (smallAsteroidVelocity / tinyAsteroidVelocity),
                                asteroids[i].Velocity.Y * (smallAsteroidVelocity / tinyAsteroidVelocity));
                            asteroids.Add(new Asteroid(new Vector2(asteroids[i].Position.X + 25, asteroids[i].Position.Y),
                                new Vector2(-asteroids[i].Velocity.X * 2, -asteroids[i].Velocity.Y * 2), Content.Load<Texture2D>("TinyAsteroid"), 0,
                                1 / 1f, Color.White, Size.Baby));
                        }
                        else if (asteroids[i].leSize == Size.Baby)
                        {
                            score += 50;

                            asteroids.RemoveAt(i);
                        }

                        shots.RemoveAt(j);
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

                asteroids[i].Position = new Vector2(asteroids[i].Position.X + (float)Math.Sin(asteroids[i].Rotation) + asteroids[i].Velocity.X, asteroids[i].Position.Y - (float)Math.Cos(asteroids[i].Rotation) + asteroids[i].Velocity.Y);

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
                        shots.RemoveAt(j);
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
                _spriteBatch.Draw(Content.Load<Texture2D>("Life"), new Vector2(10 + (i * 15), 28), Color.DarkGray);
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