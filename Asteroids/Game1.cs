using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using Windows.UI.Notifications;
using Windows.UI.WebUI;
using static System.Net.Mime.MediaTypeNames;

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
        int counter = 0;

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
            Shield1 = 5,
            Shield2 = 6,
            Shield3 = 7,
            ShieldFinal = 8
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
        Upgrade None;
        Button UpgradeSkip;

        Upgrade ShotSpeedUp1;
        Upgrade ShotSpeedUp2;
        Upgrade ShotSpeedUp3;
        List<Upgrade> ShotSpeedProgHolder = new List<Upgrade>();
        Upgrade Drones1;
        Upgrade Drones2;
        Upgrade Drones3;
        List<Ship> DroneList = new List<Ship>();
        List<TimeSpan> DroneShotTimers = new List<TimeSpan>();
        TimeSpan reserveDroneShotTimer = new TimeSpan(0, 0, 0, 5, 0);
        List<Vector2> DroneTargets = new List<Vector2>();
        List<Upgrade> DroneProgHolder = new List<Upgrade>();

        Upgrade Warp;
        Upgrade Shield1;
        Upgrade Shield2;
        Upgrade Shield3;
        Upgrade ShieldFinal;
        Sprite ShieldSprite;
        List<Upgrade> ShieldProgHolder = new List<Upgrade>();

        List<Rectangle> ExtraHitboxes = new List<Rectangle>();

        string GunInEffectName = "Default";
        Color GunInEffectColor = Color.DarkGray;
        bool CanShoot = true;
        Upgrade MachineGun;
        TimeSpan MachineGunShotTimer = new TimeSpan(0, 0, 0, 0, 150);
        TimeSpan reserveMachineGunShotTimer = new TimeSpan(0, 0, 0, 0, 150);
        Upgrade Laser;
        TimeSpan LaserShotTimer = new TimeSpan(0, 0, 0, 0, 300);
        TimeSpan reserveLaserShotTimer = new TimeSpan(0, 0, 0, 0, 300);

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
         *      so first, i'll have to design an upgrade menu in-between levels where you can pick up or skip upgrades *DONE*
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
         *      have a button class;    *FINISHED*
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
         *      after that, make the machine gun into an ability and test it    *FINISHED*
         *          have fancy little text on the side that says what weapon you have and allow switching
         *          have weapon swaping be done via scroll wheel and num-bar  
         *              convert powerups into gun upgrades
         *      
         *      
         *      instead of nerfing the now permanent guns, do the following;   *FINISHED*
         *          give them an energy bar like in megaman, where using a weapon takes energy that regenerates after time
         *              have energy bar as a rectangle for now
         *              it always has a length of 100, and each weapon has a specific amount of energy used per shot
         *              regenerate 4 energy a second smoothly by default
         *              it appears below the gun name
         *          abilities use the same energy, but the position of the bar is different and the energy gain usually hasa multiplier
         *      
         *      make the energy bar smoother *DONE*
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
         *      replace the "test" upgarde amd ability with actual ones and remove the "test" parameter.
         *          Ability Done
         *          Upgrade not Done (ok back to this) (turn the "to dos" into "dones" when done with them)
         *      
         *          make the test upgrade into a drone that circles around the ship and shoots special bullets
         *          its progression is for fire rate and drone amount
         *              Progress and thoughts on this:
         *              Have made the types but haven't removed the Test
         *              Made the sprites
         *              Made the "Upgrade" upgrades and the shot timer (starts at 5 seconds)
         *                  The drones should be stuck to specific areas near the ship and they should turn to face what they shoot at. They shoot machine gun bullets
         *                  The drones' bullets are added to the ship's shots
         *              Made a drone appear for the first uprade and make it stay by the ship correctly (alter position draw to make position right)
         *                  The drones will be in a List in order to better track them. Each drone will be made as a Ship for simplicity
         *                  Drone1 will be made alone before the rest
         *              Made the drone turn to a target
         *                  After thinking about it, I have decided that the drones will just pick a target at random rather than aiming for the closest one
         *                      First, draw a line from the drone to its target in order to see if it works, then turn to the line *this worked and the line was removed*
         *                          (line drawn, delete later)
         *              Made the drone fire
         *              Make the drone upgrades
         *                  Created a List to hold each Drone's ShotTimer and a List to hold each Drone's Target
         *              a
         *                  Steal the shooting code from the UFO class and work it into the drone
         *              a
         *          
         *          make the test ability into a shield that, when held, surrounds the ship and protects from all damage whilst draining energy, but you cannot shoot out of it
         *          its progression is for time it can be up/energy regen, but the last upgrade lets you sacrifice that speed and turn it into a fire orb that melts enemies
         *              Progress and thoughts on this:
         *              Have made the types but haven't removed the Test
         *              Made the sprites
         *              Made the "Upgrade" upgrades
         *              Made the upgrade drain energy; it uses 1 energy per active frame, and each upgrade increases energy regen speed
         *              Made the shield texture into a Sprite
         *              Made the shield display when inEffect; also slightly changed the texture and capitalized it and the drone
         *              Made the effect; remember, can't shoot while active, protects from damage
         *                  Before continuing, make the hit detection for the asteroids and ufos better (likely a function that takes either shot or hitbox and reacts)
         *                      Pretty horrid and basic, but it works
         *                      Uses a Hitbox List that holds potential colliders, then removes them if they are unactive (like activating adds, deactivating removes)
         *              Removed the Test value
         *      
         *                                                          {{remove individual dones below when done with the test removal}}
         *      **DOING** (things done during the above task) 
         *      make Upgrade remove the previous levels of abilities from the activeAbilities when picking the next level *DONE*
         *      make quick energy regen possible by having high energyGainMultiplier's lead to more energy being added at once *DONE*
         *      make a function in Upgrade that lets you add an image to draw/thing to add to the screen when in effect (like abilitiy effect or upgrade thing) *SOMEWHAT DONE*
         *          actually, for this, just have manual draws since these will often need specific information from the Game1
         *          the draw might get cluttered up with this, so I will make sure to have a specific spot for it
         *      make an extra variable called CanShoot that will determine if you are able to fire, mainly used for a shield *DONE*
         *      add a parameter to Sprite that lets you visually change the Sprite if you want to (null by default, needs seperate assignment) *DONE*
         *          use this to make better hitboxes in the future
         *      
         *      clean up the code a bit; changes will mostly be using foreach instead of for(every one) and replacing variables in bool functions with returns *DONE*
         *      
         *      make it so if you run out of energy while holding an ability/gun, the bar's color changes and you need to regen 1/3 of it to use it again (overheat) *DONE*
         *          can likely do this with a variable in the Upgrade class that changes when WillAbilityGetUsed returns false
         *          is being done via an overheat that checks if the ability was used last frame and if it was + out of energy, activate overheat
         *      **DOING**
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
         * Have a better way of checking if an upgrade has been collected rather than just infinite "else ifs"
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

        bool EnemyCollisionDetection(Rectangle checkedHitbox, List<Bullet> bullets, List<Rectangle> hitboxes)
        {
            if (bullets != null)
            {
                foreach (Bullet bullet in bullets)
                {
                    if (checkedHitbox.Intersects(bullet.Hitbox))
                    {
                        if (bullet.Image != Laser.GunBullet.Image)
                        {
                            bullets.Remove(bullet);
                        }
                        return true;
                    }
                }

            }

            if (hitboxes != null)
            {
                foreach(Rectangle hitbox in hitboxes)
                {
                    if (checkedHitbox.Intersects(hitbox)) { return true; }
                }
            }

            return false;
        }
        bool IsBulletInBounds(List<Bullet> bullets, int i, Rectangle playSpace)
        {
            if (!playSpace.Contains(bullets[i].Position))
            {
                bullets.RemoveAt(i); return false;
            }
            return true;
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

            //Abilities

            Warp = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Warp, "Warp",
                "Right-click to warp", "to a random point", "on screen. Gain 0.4", "seconds of i-frames.", null, 0, 99, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.DarkGray, false);
            PossibleUpgrades.Add(Warp);

            Shield1 = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Shield1, "Shield",
                "Hold Z to activate", "a shield that", "protects you from", "all damage.", ShieldProgHolder, 1, 1, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(Shield1);
            ShieldProgHolder.Add(Shield1);
            ShieldSprite = new Sprite(new Vector2(0, 0), Content.Load<Texture2D>("Upgrades/ShieldHitbox"), 0, 1 / 1f, Color.White);
            ShieldSprite.DisplayImage = Content.Load<Texture2D>("Upgrades/Shield");

            Shield2 = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Shield2, "Shield+",
                "You can have the", "shield active for", "longer.", "", ShieldProgHolder, 2, 1, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            ShieldProgHolder.Add(Shield2);

            Shield3 = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Shield3, "Shield++",
                "You can have the", "shield active for", "almost forever.", "", ShieldProgHolder, 3, 1, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            ShieldProgHolder.Add(Shield3);

            ShieldFinal = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.ShieldFinal, "Shield Final",
                "Coat your shield in", "fire, reverting its", "length but melting", "all touched enemies.", ShieldProgHolder, 4, 1, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.OrangeRed, false);
            ShieldProgHolder.Add(ShieldFinal);

            //Abilities

            //Upgrades

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

            Drones1 = new Upgrade(new Vector2(0, 0), StatUpgradeType.Drones1, AbilityUpgradeType.None, "Drones",
                "Gain a drone that", "stays near you and", "shoots enemies", "every 5 seconds.", DroneProgHolder, 1, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            PossibleUpgrades.Add(Drones1);
            DroneProgHolder.Add(Drones1);

            Drones2 = new Upgrade(new Vector2(0, 0), StatUpgradeType.Drones2, AbilityUpgradeType.None, "Drones+",
                "Gain an additional", "drone. Your drones", "shoot every 3 sec.", "", DroneProgHolder, 2, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            DroneProgHolder.Add(Drones2);

            Drones3 = new Upgrade(new Vector2(0, 0), StatUpgradeType.Drones3, AbilityUpgradeType.None, "Drones++",
                "Gain one more drone.", "Your drones shoot", "every 1.5 seconds.", "", DroneProgHolder, 3, 0, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.White, false);
            DroneProgHolder.Add(Drones3);

            //Upgrades

            //Guns

            MachineGun = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Machine, "Machine Gun",
                "Gives you a rapid-", "-firing machine gun.", "", "", null, 0, 7, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.DarkSlateGray, false);
            PossibleUpgrades.Add(MachineGun);

            Laser = new Upgrade(new Vector2(0, 0), StatUpgradeType.None, AbilityUpgradeType.Laser, "Laser",
                "Gives you piercing", "laser gun.", "", "", null, 0, 33, Content.Load<Texture2D>("idiot/You Are An Idiot"), 0, 1 / 1, Color.Red, false);
            PossibleUpgrades.Add(Laser);

            MachineGun.GunBullet = new Bullet(new Vector2(-20, -20), shotVelocity * 1.1f, Content.Load<Texture2D>("ShipAndShots/MachineShot"), 0, 1 / 1f, Color.White);
            Laser.GunBullet = new Bullet(new Vector2(-20, -20), shotVelocity * 1.5f, Content.Load<Texture2D>("ShipAndShots/LaserShot"), 0, 1 / 1f, Color.White);

            //Guns

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

            if (mouseState.LeftButton == ButtonState.Pressed && CanShoot)
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
            if (!UpgradeSkip.wasClicked)
            {
                foreach (var upgrade in UpgradesToDraw)
                {
                    upgrade.UpgradeButton.Update(mouseState, lastMouseState);
                    upgrade.WhenSelected(PossibleUpgrades, ActiveUpgrades, ActiveAbilities, ActiveGuns);
                }
            }

            for (int i = 0; i < UpgradesToDraw.Count; i++)
            {
                bool broken = false;
                if (UpgradeSkip.wasClicked)
                {
                    tempPress = "skipped";
                    foreach (var upgrade in UpgradesToDraw)
                    {
                        upgrade.Skipped();
                        upgrade.UpgradeButton.wasClicked = false;
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

            foreach (var shot in shots)
            {
                shot.Move();

                if (!IsBulletInBounds(shots, counter, playSpace)) { break; }

                /*  Old Powerup Code, Archived
                if (!IsBulletInBounds(shots, i, playSpace))
                {
                    if (!machine.WhenShot(shots, i))
                    {
                        laser.WhenShot(shots, i);
                    }
                }
                */
                counter++;
            } counter = 0;
            foreach (var enemyShot in enemyShots)
            {
                enemyShot.Move();

                if (ship.Hitbox.Intersects(enemyShot.Hitbox))
                {
                    enemyShots.Remove(enemyShot);
                    if (iFrames <= TimeSpan.Zero)
                    {
                        ship.Position = playSpace.Center.ToVector2();
                        lives--;
                        iFrames = TimeSpan.FromMilliseconds(2000);
                        ship.Velocity = 0;
                    }
                    break;
                }

                if (!IsBulletInBounds(enemyShots, counter, playSpace)) { break; } counter++;
            } counter = 0;

            foreach (var asteroid in asteroids)
            {
                if (EnemyCollisionDetection(asteroid.Hitbox, shots, ExtraHitboxes))
                {
                    //Archived PowerUp Code: machine.Spawned(asteroids[i].Position, new Vector2(rand.Next(1, 4), rand.Next(1, 4)), asteroids[i].leSize);

                    if (asteroid.leSize == Size.LeChonk)
                    {
                        score += 10;
                        asteroid.leSize++;
                        asteroid.Image = SmallAsteroid;
                        asteroid.Velocity = new Vector2(asteroid.Velocity.X * (largeAsteroidVelocity / smallAsteroidVelocity),
                            asteroid.Velocity.Y * (largeAsteroidVelocity / smallAsteroidVelocity));
                        asteroids.Add(new Asteroid(new Vector2(asteroid.Position.X + 60, asteroid.Position.Y),
                            new Vector2(-asteroid.Velocity.X * 2, -asteroid.Velocity.Y * 2), SmallAsteroid, 0, 1 / 1f, Color.White, Size.Normal));
                    }
                    else if (asteroid.leSize == Size.Normal)
                    {
                        score += 30;
                        asteroid.leSize++;
                        asteroid.Image = TinyAsteroid;
                        asteroid.Velocity = new Vector2(asteroid.Velocity.X * (smallAsteroidVelocity / tinyAsteroidVelocity),
                            asteroid.Velocity.Y * (smallAsteroidVelocity / tinyAsteroidVelocity));
                        asteroids.Add(new Asteroid(new Vector2(asteroid.Position.X + 25, asteroid.Position.Y),
                            new Vector2(-asteroid.Velocity.X * 2, -asteroid.Velocity.Y * 2), TinyAsteroid, 0, 1 / 1f, Color.White, Size.Baby));
                    }
                    else if (asteroid.leSize == Size.Baby)
                    {
                        score += 50;

                        asteroids.Remove(asteroid);
                    }

                    break;
                }

                if (counter >= asteroids.Count)
                {
                    break;
                }

                if (asteroid.leSize == Size.LeChonk)
                {
                    asteroid.Rotation += 0.005f;
                }
                else
                {
                    asteroid.Rotation += 0.01f;
                }

                asteroid.Position = new Vector2(asteroid.Position.X + (float)Math.Sin(asteroid.Rotation) + asteroid.Velocity.X,
                    asteroid.Position.Y - (float)Math.Cos(asteroid.Rotation) + asteroid.Velocity.Y);

                asteroid.IsInBounds(playSpace);

                if (ship.Hitbox.Intersects(asteroid.Hitbox))
                {
                    if (iFrames <= TimeSpan.Zero)
                    {
                        ship.Position = playSpace.Center.ToVector2();
                        lives--;
                        iFrames = TimeSpan.FromMilliseconds(2000);
                        ship.Velocity = 0;
                    }
                    //make look good later
                } counter++;
            } counter = 0;

            UFOMovementSpace = new Rectangle((int)ship.Position.X - 60, 50, 120, 60);
            UFOShotArea = new Rectangle((int)ship.Position.X - level.UFORange, (int)ship.Position.Y, level.UFORange * 2, 1);

            foreach (var UFO in UFOs)
            {
                if (EnemyCollisionDetection(UFO.Hitbox, shots, ExtraHitboxes))
                {
                    //Archived PowerUp Code: laser.Spawned(UFOs[i].Position, new Vector2(rand.Next(1, 3), rand.Next(1, 3)), UFOs[i].leSize);

                    score += 100 * (((int)UFO.leSize) - 1);

                    UFOs.Remove(UFO);

                    break;
                }

                if (counter >= UFOs.Count)
                {
                    break;
                }

                if (UFO.Update(gameTime.ElapsedGameTime))
                {
                    enemyShots.Add(UFO.Shoot(UFOShotArea, UFOShot));
                    break;
                }

                if (UFO.Position.X <= UFOMovementSpace.X)
                {
                    UFO.Velocity.X = Math.Abs(UFO.Velocity.X);
                }
                else if (UFO.Position.X >= UFOMovementSpace.X + UFOMovementSpace.Width)
                {
                    UFO.Velocity.X = Math.Abs(UFO.Velocity.X);
                    UFO.Velocity.X *= -1;
                }


                if (UFO.Position.Y <= UFOMovementSpace.Y)
                {
                    UFO.Velocity.Y = Math.Abs(UFO.Velocity.Y);
                }
                else if (UFO.Position.Y >= UFOMovementSpace.Y + UFOMovementSpace.Height)
                {
                    UFO.Velocity.Y = Math.Abs(UFO.Velocity.Y);
                    UFO.Velocity.Y *= -1;
                }

                UFO.Position = new Vector2(UFO.Position.X + UFO.Velocity.X, UFO.Position.Y + UFO.Velocity.Y);

                UFO.IsInBounds(playSpace);

                if (ship.Hitbox.Intersects(UFO.Hitbox))
                {
                    if (iFrames <= TimeSpan.Zero)
                    {
                        ship.Position = playSpace.Center.ToVector2();
                        lives--;
                        iFrames = TimeSpan.FromMilliseconds(2000);
                        ship.Velocity = 0;
                    }
                } counter++;
            } counter = 0;

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
            Drones1.isActive = true;
            ActiveUpgrades.Add(Drones1);
            Drones2.isActive= true ;
            ActiveUpgrades.Add(Drones2);
            Drones3.isActive= true ;
            ActiveUpgrades.Add(Drones3);
            */


            for (int i = 0; i < DroneProgHolder.Count; i++)
            {
                if (DroneProgHolder[i].isActive && DroneProgHolder[i].inEffect==false) //cut down the drone number detection
                {
                    DroneList.Add(new Ship(new Vector2(0, 0), 0, Content.Load<Texture2D>("Upgrades/Drone"), 0, 1 / 1f, Color.White));

                    float a = 0; if (i==1) { a = 0.5f; }
                    reserveDroneShotTimer = TimeSpan.FromSeconds(5 - (1.5 * i) - a);
                    DroneShotTimers.Add(reserveDroneShotTimer);

                    DroneTargets.Add(new Vector2(0, 0));

                    DroneProgHolder[i].inEffect = true;
                }
                
                if (DroneProgHolder[i].inEffect)
                {
                    if (DroneTargets[i]==new Vector2(0,0))
                    {
                        bool asteroidOrUFO = true; //true = asteroid; false = UFO
                        if (asteroids.Count!=0 && UFOs.Count!=0) { asteroidOrUFO = Convert.ToBoolean(rand.Next(0, 2)); }
                        else if (asteroids.Count!=0 && UFOs.Count==0) { asteroidOrUFO = true; }
                        else if (asteroids.Count==0 && UFOs.Count!=0) { asteroidOrUFO = false; }
                        else if (asteroids.Count==0 && UFOs.Count==0) { break; }

                        int temp = 0;
                        if (asteroidOrUFO) { temp = asteroids.Count; }
                        else if (!asteroidOrUFO) { temp = UFOs.Count; }
                        int target = rand.Next(0, temp);

                        Vector2 droneTargetPosition = new Vector2(0, 0);
                        if (asteroidOrUFO) { droneTargetPosition = asteroids[target].Position; }
                        else if (!asteroidOrUFO) { droneTargetPosition = UFOs[target].Position; }
                        DroneTargets[i] = droneTargetPosition;
                    }

                    Vector2 start;
                    Vector2 destination;
                    Vector2 between;
                    //
                    start = DroneList[i].Hitbox.Center.ToVector2();
                    destination = DroneTargets[i];
                    between = start - destination;
                    //
                    float angle = (float)Math.Atan2((double)between.Y, (double)between.X) - MathHelper.ToRadians(90);

                    DroneList[i].Rotation = angle;

                    DroneShotTimers[i] -= gameTime.ElapsedGameTime;
                    if (DroneShotTimers[i]<=TimeSpan.Zero)
                    {
                        shots.Add(Bullet.BulletTypeCopy(MachineGun.GunBullet, DroneList[i].Position, angle));

                        DroneShotTimers[i] = reserveDroneShotTimer;
                        DroneTargets[i] = new Vector2(0, 0);
                    }
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

            for (int i = 0; i < ShieldProgHolder.Count; i++)
            {
                if (ShieldProgHolder[i].isActive)
                {
                    ShieldProgHolder[i].AbilityUpdate(gameTime.ElapsedGameTime);
                    if (keyboardState.IsKeyDown(Keys.Z) && ShieldProgHolder[i].WillAbilityGetUsed())
                    {
                        float energyGainMultiplier = 5;
                        if (i == 1) { energyGainMultiplier = 9; }
                        if (i == 2) { energyGainMultiplier = 18; }
                        if (i == 3) { energyGainMultiplier = 3.8f; }
                        ShieldProgHolder[i].EnergyGainMultiplier = energyGainMultiplier;
                        ShieldProgHolder[i].AbilityUse();
                        ShieldProgHolder[i].inEffect = true;
                        CanShoot = false;
                    }
                    else
                    {
                        ShieldProgHolder[i].inEffect = false;
                        CanShoot = true;
                        ExtraHitboxes.Remove(ShieldSprite.Hitbox);
                    }

                    if (ShieldProgHolder[i].inEffect)
                    {
                        if (iFrames <= TimeSpan.FromMilliseconds(20)) { iFrames = TimeSpan.FromMilliseconds(20); }

                        if ((i == ShieldFinal.ProgressionLevel - 1) && (!ExtraHitboxes.Contains(ShieldSprite.Hitbox)))
                        {
                            ExtraHitboxes.Add(ShieldSprite.Hitbox);
                        }
                    }

                    break;
                }
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

            foreach (var asteroid in asteroids)
            {
                asteroid.Draw(_spriteBatch);
            }
            foreach (var UFO in UFOs)
            {
                UFO.Draw(_spriteBatch);
            }


            ship.Draw(_spriteBatch);

            foreach (var shot in shots)
            {
                shot.Draw(_spriteBatch);
            }

            foreach (var enemyShot in enemyShots)
            {
                enemyShot.Draw(_spriteBatch);
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

            foreach (var ability in ActiveAbilities)
            {
                ability.AbilityEnergyStack(ActiveAbilities, counter);
                ability.EnergyDraw(_spriteBatch); counter++;
            } counter = 0;
            foreach (var gun in ActiveGuns)
            {
                gun.EnergyDraw(_spriteBatch);
            }


            //Ability and Upgrade Effects

            foreach (var upgrade in ActiveUpgrades)
            {
                if ((upgrade == Drones1 || upgrade == Drones2 || upgrade == Drones3) && upgrade.inEffect)
                {
                    for (int i = 0; i < DroneList.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                DroneList[i].Position = new Vector2(ship.Position.X + 22, ship.Position.Y - 12);
                                break;

                            case 1:
                                DroneList[i].Position = new Vector2(ship.Position.X - 22, ship.Position.Y - 12);
                                break;

                            case 2:
                                DroneList[i].Position = new Vector2(ship.Position.X, ship.Position.Y + 32);
                                break;
                        }

                        DroneList[i].Draw(_spriteBatch);

                        _spriteBatch.DrawLine(DroneList[i].Position, DroneTargets[i], Color.Red);
                    }


                    //DroneList[0].Position = new Vector2(ship.Position.X + 22, ship.Position.Y - 12);
                }
            }

            foreach (var ability in ActiveAbilities)
            {
                if ((ability == Shield1 || ability == Shield2 || ability == Shield3) && ability.inEffect)
                {
                    ShieldSprite.Position.X = ship.Position.X;
                    ShieldSprite.Position.Y = ship.Position.Y;
                    ShieldSprite.Rotation = ship.Rotation;
                    ShieldSprite.Draw(_spriteBatch);
                }
                else if (ability == ShieldFinal && ability.inEffect)
                {
                    ShieldSprite.Position.X = ship.Position.X;
                    ShieldSprite.Position.Y = ship.Position.Y;
                    ShieldSprite.Rotation = ship.Rotation;
                    ShieldSprite.Color = Color.OrangeRed;
                    ShieldSprite.Draw(_spriteBatch);
                }
            }


            //Ability and Upgrade Effects


            foreach (var upgrade in UpgradesToDraw)
            {
                upgrade.Draw(upgradeTitleFont, upgradeDescFont, _spriteBatch);
            }
            UpgradeSkip.Draw(_spriteBatch);


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}