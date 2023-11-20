using Asteroids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
//using MonoGame.Extended.Sprites;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using Windows.Media.Playback;
using Windows.UI.Notifications;
using Windows.UI.WebUI;
using static System.Net.Mime.MediaTypeNames;

namespace Asteroid
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static GameTime gameTime;

        //All Sprites are placeholders; change later
        Ship ship;
        Bullet defaultShot; Upgrade defaultGun;
        Bullet devBullets; Upgrade devGun; TimeSpan devShotTimer = new TimeSpan(0, 0, 0, 0, 25);
        TimeSpan defaultTimer = new TimeSpan(0, 0, 0, 1, 575);
        TimeSpan reserveDefaultTimer = new TimeSpan(0, 0, 0, 1, 575);
        Bullet UFOShot;
        List<Bullet> shots = new List<Bullet>();
        List<Bullet> enemyShots = new List<Bullet>();
        Level level;
        SpriteFont font;
        SpriteFont upgradeTitleFont;
        SpriteFont upgradeDescFont;
        Vector2 BurnVec = new Vector2(0, 0);
        bool animBurnBool = true;
        bool BoolForRandomTests = false;
        int counter = 0;

        public static bool GameFrozen = false;

        public enum StatUpgradeType
        {
            None = 0,
            ShotSpeedUp1 = 1,
            ShotSpeedUp2 = 2,
            ShotSpeedUp3 = 3,
            Drones1 = 4,
            Drones2 = 5,
            Drones3 = 6,
            DronesFinal = 7,
            ArmorPen1 = 8,
            ArmorPen2 = 9,
            ArmorPen3 = 10,

        }
        public static StatUpgradeType StatUpNone = StatUpgradeType.None;
        public enum AbilityUpgradeType
        {
            None = 0,
            Machine = 1,
            Laser = 2,
            Warp = 4,
            Shield1 = 5,
            Shield2 = 6,
            Shield3 = 7,
            ShieldFinal = 8,
            TimeStop1 = 9,
            TimeStop2 = 10,
            TimeStop3 = 11,
            TimeStopFinal = 12,
            Mimicry = 13,
            EGO = 14
        }
        public static AbilityUpgradeType AbilityUpNone = AbilityUpgradeType.None;
        List<Upgrade> AllUpgrades = new List<Upgrade>();
        List<Upgrade> PossibleUpgrades = new List<Upgrade>();
        List<Upgrade> UpgradesToDraw = new List<Upgrade>();
        List<Upgrade> ActiveUpgrades = new List<Upgrade>();
        List<Upgrade> ActiveAbilities = new List<Upgrade>();
        List<Upgrade> ActiveGuns = new List<Upgrade>();
        int CurrentActiveGunIndex = 0;
        List<Upgrade> NoneHolder = new List<Upgrade>();

        bool UpgradeChosen = false;
        int UpgradeTime = 0;

        Upgrade None;
        Button UpgradeSkip;

        Upgrade ShotSpeedUp1;
        Upgrade ShotSpeedUp2;
        Upgrade ShotSpeedUp3;
        List<Upgrade> ShotSpeedProgHolder = new List<Upgrade>();
        Upgrade Drones1;
        Upgrade Drones2;
        Upgrade Drones3;
        Upgrade DronesFinal;
        List<Ship> DroneList = new List<Ship>();
        List<TimeSpan> DroneShotTimers = new List<TimeSpan>();
        TimeSpan reserveDroneShotTimer = new TimeSpan(0, 0, 0, 3, 500);
        List<TimeSpan> DroneLockOnTimers = new List<TimeSpan>();
        List<int> DroneTargetValues = new List<int>();
        List<Upgrade> DroneProgHolder = new List<Upgrade>();
        Upgrade ArmorPen1;
        Upgrade ArmorPen2;
        Upgrade ArmorPen3;
        List<Upgrade> ArmorPenProgHolder = new List<Upgrade>();

        Upgrade Warp;
        Upgrade Shield1;
        Upgrade Shield2;
        Upgrade Shield3;
        Upgrade ShieldFinal;
        Sprite ShieldSprite;
        List<Upgrade> ShieldProgHolder = new List<Upgrade>();
        Upgrade TimeStop1;
        Upgrade TimeStop2;
        Upgrade TimeStop3;
        Upgrade TimeStopFinal;
        public static bool TimeHasStopped = false;
        List<Upgrade> TimeStopProgHolder = new List<Upgrade>();
        Upgrade Mimicry;
        Sprite MimicrySprite;
        Upgrade EGO;
        Sprite EGOSprite;
        Animation GreaterSplitVertical;
        Animation GreaterSplitHorizontal;
        Animation hSplitSwing;
        Sprite hSplitSwingEffect;
        Animation hSplitBack;
        Animation vSplitSwing;
        Sprite vSplitSwingEffect;

        TimeSpan hSplitSwingToEffect = TimeSpan.Zero;
        TimeSpan hSplitPostSwing = TimeSpan.Zero;
        TimeSpan vSplitPostSwing = TimeSpan.Zero;
        TimeSpan splitBack = TimeSpan.Zero;
        Color splitBackColor = Color.Black;


        List<Rectangle> ExtraHitboxes = new List<Rectangle>();
        List<float> ExtraPens = new List<float>();

        string GunInEffectName = "Default";
        Color GunInEffectColor = Color.DarkGray;
        bool CanShoot = true;
        Upgrade MachineGun;
        TimeSpan MachineGunShotTimer = new TimeSpan(0, 0, 0, 0, 150);
        TimeSpan reserveMachineGunShotTimer = new TimeSpan(0, 0, 0, 0, 150);
        Upgrade Laser;
        TimeSpan LaserShotTimer = new TimeSpan(0, 0, 0, 0, 300);
        TimeSpan reserveLaserShotTimer = new TimeSpan(0, 0, 0, 0, 300);

        float CollisionPenetration = 0;
        float extraArmorPenetration = 0;

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

        public Rectangle playSpace;

        public Random rand = new Random();

        List<Enemy> AllEnemies = new List<Enemy>();
        List<Enemy> lastEnemies = new List<Enemy>();
        List<Enemy> Asteroids = new List<Enemy>();
        List<Enemy> UFOs = new List<Enemy>();
        Texture2D BigAsteroid;
        Texture2D SmallAsteroid;
        Texture2D TinyAsteroid;
        Texture2D BigSaucer;
        Texture2D SmallSaucer;
        public static Texture2D[] AsteroidArmor;
        public static Texture2D[] BigUFOArmor;
        public static Texture2D[] SmallUFOArmor;

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
         *      -----------------------(small barrier for this chunk)
         *      replace the "test" upgarde amd ability with actual ones and remove the "test" parameter.   *FINISHED*
         *      
         *          make the test upgrade into a drone that circles around the ship and shoots special bullets  **DONE**
         *          its progression is for fire rate and drone amount
         *              Progress and thoughts on this:
         *              Have made the types but haven't removed the Test
         *              Made the sprites
         *              Made the "Upgrade" upgrades and the shot timer (starts at 5 seconds)
         *                  The drones should be stuck to specific areas near the ship and they should turn to face what they shoot at
         *                  The drones' bullets are added to the ship's shots
         *              Made a drone appear for the first uprade and make it stay by the ship correctly (alter position draw to make position right)
         *                  The drones will be in a List in order to better track them. Each drone will be made as a Ship for simplicity
         *                  Drone1 will be made alone before the rest
         *              Made the drone turn to a target
         *                  After thinking about it, I have decided that the drones will just pick a target at random rather than aiming for the closest one
         *                      First, draw a line from the drone to its target in order to see if it works, then turn to the line *this worked and the line was removed*
         *              Made the drone fire
         *              Made the drone upgrades
         *                  Created a List to hold each Drone's ShotTimer and a List to hold each Drone's Target
         *                      Combined the UFOs and Asteroids into one Enemy Class (progress below)
         *              Made a 1.5 second wait time between shooting and choosing a new target so that the Drones don't shoot and instantly snap to the next target
         *                  Actually, the Drones will now work like this:
         *                  It takes each Drone 1.5 seconds between locking on and firing
         *                  After a shot, have a "cooldown" time where it sits and idles
         *                  The cooldown timer is what gets decreased with each upgrade
         *                  The ShotTimers will remain as what they were, however one and a half seconds of them will be replaced with a LockOnTimer
         *              Implemented a failsafe for when the Drone's target is destroyed
         *                  If its target is destroyed, the Drone's LockOnTimer and target will reset
         *              Make a final upgrade for the Drones
         *                  It will double the LockOnTimer but will change the projectile to a unique one that acts as a burning and piercing bullet
         *              Removed the Test Value
         *          
         *          make the test ability into a shield that, when held, surrounds the ship and protects from all damage whilst draining energy, but blocks your gun    **DONE**
         *          its progression is for time it can be up/energy regen, but the last upgrade lets you sacrifice that speed and turn it into a fire orb that melts enemies
         *              Progress and thoughts on this:
         *              Have made the types but haven't removed the Test
         *              Made the sprites
         *              Made the "Upgrade" upgrades
         *              Made the upgrade drain energy; it uses 1 energy per active frame, and each upgrade increases energy regen speed
         *              Made the shield texture into a Sprite
         *              Made the shield display when inEffect; also slightly changed the texture and capitalized it and the drone
         *              Made the effect; remember, can't shoot while active, protects from damage
         *                  Before continuing, make the hit detection for the Asteroids and UFOs better (likely a function that takes either shot or hitbox and reacts)
         *                      Pretty horrid and basic, but it works
         *                      Uses a Hitbox List that holds potential colliders, then removes them if they are unactive (like activating adds, deactivating removes)
         *              Removed the Test value
         *              
         *      **DONE** (things done during the above task) 
         *      make Upgrade remove the previous levels of abilities from the activeAbilities when picking the next level
         *      make quick energy regen possible by having high energyGainMultiplier's lead to more energy being added at once
         *      make a function in Upgrade that lets you add an image to draw/thing to add to the screen when in effect (like abilitiy effect or upgrade thing) *SOMEWHAT DONE*
         *          actually, for this, just have manual draws since these will often need specific information from the Game1
         *          the draw might get cluttered up with this, so I will make sure to have a specific spot for it
         *      make an extra variable called CanShoot that will determine if you are able to fire, mainly used for a shield
         *      add a parameter to Sprite that lets you visually change the Sprite if you want to (null by default, needs seperate assignment)
         *          use this to make better hitboxes in the future
         *      
         *      clean up the code a bit; changes will mostly be using foreach instead of for(every one) and replacing variables in bool functions with returns
         *      
         *      make it so if you run out of energy while holding an ability/gun, the bar's color changes and you need to regen 1/3 of it to use it again (overheat)
         *          can likely do this with a variable in the Upgrade class that changes when WillAbilityGetUsed returns false
         *          is being done via an overheat that checks if the ability was used last frame and if it was + out of energy, activate overheat
         *          
         *      combine the UFOs and Asteroids into a single enemy class
         *          use types to split the enemies into ufos and Asteroids
         *              this should help keep track of them
         *              i added comments to show what parts of it were from the original classes
         *          enemy class made, now convert
         *              do it by commenting out asteroid and ufo and fixing the errors. also capitalize the asteroids list while you're at it
         *          have a function in enemy that splits the enemies into ufo and asteroid lists for ease of use
         *              follow this up with a function that syncs the split lists with the main list
         *          completed
         *      create a list that holds the last enemy states
         *          this will be for things that check for changes between enemies after each frame
         *      
         *      make a debug shot that replaces the screen clear dev shortcut. it is basically laser but takes no energy and fires really fast
         *      
         *      replace all the new Vector2(0,0)'s with a variable (BurnVec)
         *      **DONE**
         *      
         *      
         *      -----------------------(armor chunk)
         *      add a new enemy varient: armored enemies   *FINISHED*
         *          There will be different armor tiers, and armor needs a certain amount of hits before destruction. Incremental upgrades
         *          It will work sort of like bloons in the Bloons Tower Defense series, where each armer level is a "layer" that must be popped
         *              Every 3 levels, add 1 armor point to the max possible armor value. Start to decrease the lowest armor possible after armor level 3
         *          Each bullet will have a Penetration value which will dictate how many layers of armor one shot can pierce
         *          Only Large Asteroids and UFOs have armor
         *              Large Asteroids armor will go up to 6. All UFOs go up to 3 and scale to half of the asteroids. Penetration can be upgraded to 3
         *              Depending on how you balance the game, these values can change. The armor should scale with SpawnTime so as to not take forever
         *          The armor value of an enemy will go down when shot, and the images of each armor will cycle in an array like what was done for the powerups
         *          Once the value hits 0 or less, there is no armor.
         *          The armor value can be a float, but the array value will be rounded up. This is for shit that takes multiple hits to break off one layer
         *              For example, if the armor value is 1.5, it is registered as an armor level of 2
         *          Bullets that count as "burning"/piercing still go through armor, but every frame the armor value is reduced. If this isn't good, change.
         *                  Maybe in the future make enemies with burning shots that can "break" or pierce your armor/shields at a weaker rate
         *              Work Order and Progress:
         *              Added the armor value to enemies and their functions
         *              Made it have a min and max value for some randomness
         *              Gave all the weapons Penetration Values
         *                  Default: 1
         *                  Machine: 0.2
         *                  Laser: 1 (burning)
         *                  DronesDefault: 0.5
         *                  DronesBurning: 0.5 (burning)
         *                      (u can probably also use these in upgrades for ones that do damage)
         *                  ShieldFinal: 1
         *              Made an array to hold the armors
         *                  They are all static in Game1
         *                      Remember to 
         *              Did a few quick tasks that I thought of:
         *                  Added a "Burning" parameter to bullets so as to not check the image
         *                  Made the current gameTime variable have a slightly different name and gave the old name to a static version
         *                      When this was done originally, I also made functions to do the TimeCheck thing for me, one with a reserve and one without
         *                      However, they basically never worked right, so they have since been removed
         *                  Made a ShipGotHit function to deal with the ship taking damage
         *                      (might end up changing it when i make armor for the ship)
         *                  Made specific static variables to hold StatUpgradeType.None and AbilityUpgradeType.None
         *              Made the placeholder textures for the first 3 levels of Asteroid armor
         *              Did something to account for non-Big Asteroids
         *                  What I did was I just set the small Asteroids to use the same armor textures but always have an ArmorValue of 0
         *                  This may be a bit redundant, as I will likely always manually set their armor to 0, but hey, a failsafe never hurt anyone
         *              Gave the Upgrade Class an optional ShotTimer and reserveShotTimer parameter so as to make shooting with guns simpler
         *              Converted the defaultShot into a gun so as to make Penetration calculation easier
         *                  And whilst I was at it, I made the gun swap code look less horrid
         *              Made and implemented a ContentLoad2D function to load Texture2D's without having to type out Content.Load<Texture2D>
         *              Made and implemented strings in LoadContent in order to load Texture2D's for long subfolder paths (like "ShipAndShots/" and "UpgradeImages/")
         *                  (ufoArmors have the paths "Armor/LargeUFOArmor" and "Armor/SmallUFOArmor" [with the number at the end added manually])
         *              Figured out how to make it work for the Asteroids
         *                  First, made the armor display just in general
         *                      Remember that at ArmorValue 0, armor is not displayed
         *                          I added a variable called noArmorChain that makes is so that the 3rd 0 armorValue roll will be rerolled with a minimum of 1
         *                      First displayed using the main function, then updated using the below function
         *                  Then, made an ArmorDamage function in the Enemy class with the following parameters:
         *                      Takes in the Penetration of whatever caused damage
         *                          I am currently using a variable in Game1 called CollisionPenetration updated in the similar function to set this, change later if this is bad
         *                          I also gave each bullet a Penetration value as well for simplicity
         *                          Also gave the EnemyCollisionDetection function with an extra penetration list that will be parallel to ExtraHitboxes
         *                                  (temp UFOShot has 0 pen)
         *                      Gets called upon collision,
         *                      Does armor calculation,
         *                      And returns if the armor is still stable
         *                      If the armor is still stable, then it sets the DisplayImage to armor[armorValue]
         *                  Then, tested it, and it worked perfectly with no issues, trust me
         *              Copied the stuff over to UFOs
         *                  Made the placeholder textures for all 3 large and small UFO armor levels
         *              Lastly, made the placeholder textures for the last 3 levels of Asteroid armor
         *      
         *      Armor works perfectly, I think
         *      -----------------------(armor chunk)
         *      
         *      
         *      quickly fix the small UFOs...uhm...listening to Free Bird (accidently set the small UFOs the the Asteroid Type) **DONE**
         *          i also made the UFO movement area increase with levels inversly proportional to shotArea and added a UFOMoveExtra variable to Level
         *              (half as much since -X and +width on the area rectangle)
         *          i also changed large UFO from largevelocity to smallvelocity
         *      
         *      add a system for upgrade control. like rarity and making certain upgrades only appear at certain points **DONE**
         *          gave a new Rarity variable to Upgrade
         *              this variable makes some Upgrades more likely to appear than others, but Nones will still only appear when there are no more options
         *                      (can probably also use this to make the upgrades i'm testing show up instantly)
         *          added a static Generation function in Upgrade to apply Rarity
         *          the functon works as such:
         *              it rolls every possible Upgrade at once
         *              the function will generate a random number from 1-100
         *                  if the number is less than the Rarity, the Upgrade is held on to
         *                  otherwise, it i removed from the list
         *              it continues to do this until only one Upgrade remains
         *              if the last possible Upgrade left were to be removed, break and keep that final possibility
         *                  this allows higher rarities to actually mean something rather than relying on chance to even be selected
         *          added a RarityChange function in Upgrade to change the odds from outside influence
         *              the function itself only takes in a number to change the rarity to
         *              the numbers are changed outside the function
         *                      (alter this functionality later if neccesary)
         *          add a function in Level that will influence Upgrade Rarity
         *              upon entering a new level, each Upgrade in PossibleUpgrades will have its Rarity increase by 5% up to 75%
         *              this function will also add Upgrades to the PossibleUpgrades pool, doing this every level
         *                  maybe also give all Upgrades a variable that dictates at which level it is added to the pool
         *                      added an AllUpgrades List to make this easier
         *      
         *      had an idea: rework the spawning system/level transition entirely    **DONE**
         *          right now, the SpawnTimer and spawn counts decrease and increase respectively per level, leading to VERY long levels
         *          the following system will DEFINITLY need testing and balancing, but it should be better
         *          replace this system with the following:
         *              Remove the increase in spawn size and decrease in SpawnTimer
         *              Replace the spawn size with a spawn chance for each Enemy
         *                      (the old stuff in the NewLevel function still remains until all imported)
         *                      (the old things in Update were moved to the bottom)
         *              Each Level has a 20-30 second timer
         *                      (probably eventually make the timer change with Level; 30 seconds for now)
         *              Spawning occurs during that time
         *              The SpawnTimer is always somewhere around 1 secnd
         *              When the SpawnTimer finishes, roll the dice to see if a thing spawns at that SpawnOpportunity
         *              Each individual Enemy's spawn chance increases at a dfferent rate for each Level
         *          this should be a much less tedious system
         *          archive the old functions just because why not
         *      
         *      
         *      alright, basic ahh upgrades and abilities done. now the hard ones. they will be made using the same process as above
         *      
         *      upgrade plans:
         *          Armor Penetrating Rounds    **ADDED**
         *              3 total levels
         *              This upgrade relates to the armor values of enemies
         *              It will simply increase the Penetration value of all guns
         *              Different levels give different Penetration
         *                  While doing this, I made sure that a Gun's Bullets inherit the Gun's Penetration when fired
         *                  I decided to add a function in the Bullet Class to do this cuz why the hell not
         *                  
         *          Nano-Armor
         *              3 total levels
         *              This upgrade will act as a barrier that will be able to tank a hit and take time to recharge
         *              Higher levels give more hits and less recharge
         *              
         *          ECM/Radio Jammer
         *              3 total levels
         *              For each level, that many UFO spawns get ignored
         *              Like at level 1, UFOs will only spawn every 2 successful SpawnOpportunities
         *              At level 3, UFOs will only spawn every 4 successful SpawnOpportunities
         *              
         *          "some cool name"
         *              3 total levels
         *              Same exact thing as the above except for Asteroids
         *              
         *          Ricoshot Shots |ultrakill coins, figure out how to make keybind work right|
         *          
         *          Teleport |an upgrade to warp. increase the cooldown and decrease i-frames, but allow control over where you teleport to|
         *          
         *          Conversion |some asteroids you hurt will, rather than being split/destroyed, be converted and act as kamikaze-type effects}
         *          
         *          
         *      ability plans:
         *          Time Stop       **ADDED**
         *          4 total levels with a final level
         *              Upon pressing a keybind (currently will probably be X), time instantly stops an a slight grey rectangle filter is applied beneath the UI
         *              Stopping time makes all Enemies stop movement, and all spawning will be halted
         *              The main Level timer will also stop
         *              You gain i-frames while time is stopped
         *              Any fired bullets will move a little bit and then stop as well
         *              All damage dealt to enemies gets applied when time resumes
         *              The different levels increase the length of the timestop but also increase the cooldown, going from 2 seconds, to 5 seconds, to 10 seconds
         *              The final upgrade will allow you to end the timestop early, thus preserving energy and gaining a shorter cooldown
         *              All non-gun weapons/abilities will have a penetration value of 1+ArmorPen during stopped time for simplicity
         *              Progress:
         *                  added a public static TimeHasStopped bool to make it easier to determine when time has stopped throught the entire game
         *                  made it so that when the bool is true, enemy movement and shooting is halted
         *                  made it so that you cannot take damage while time is stopped
         *                  made it so that spawning is also paused when frozen
         *                  made enemy bullets stop immediatly upon time stopping
         *                  made player shots travel for 0.2 seconds before stopping
         *                      added this via the Bullet Class
         *                  made bullets stop right on the edge of enemies they were about to collide with in time stop
         *                  made a way for damage to truly only apply after a time stop
         *                      gave each enemy a public float stoppedDamage which will add up all armor pen dealt during stopped time
         *                      then when time is resumed, that damage is applied
         *                      armor can both be broken and the enemy beneath can take normal damage during stopped time
         *                      for enemies without armor, give the bool broken and the float brokenTimes to each enemy
         *                      these variables will determine how many layers the enemy loses when time resumes while not affecting offspring
         *                      also added an int hits in order for penetration to not affect enemies without armor
         *                      made penetration changes for when time is stopped using a float extraArmorPenetration variable in Game1
         *                          variable made
         *                          effect added
         *                          effect tested
         *                              while doing this i had a bug with the shield [fixed it]
         *                              fixed via purging the ExtraPens and ExtraHitboxes before adding to them rather than removing individual things
         *                  added a little filter to the screen when time is stopped
         *                  
         *          Mimicry and E.G.O. Manifestion |Mimicry gives Greater Split: Vertical and E.G.O. gives speed and i-frames + turns Mimicry into Horizontal|
         *              Progress:
         *                  Added the Upgrades
         *                  Added something based on Time Stop that stops everything for use in cutscenes and stuff
         *                  Added the Mimicry Sprite
         *                  Added an E.G.O. Sprite
         *                      Create Particles to work as the hair
         *                  Determine how to add animated things to the game because you know damn well I need it to look cool
         *                      https://www.codeandweb.com/texturepacker/tutorials/how-to-create-sprite-sheets-and-animations-with-monogame
         *                      This can be done by using a Sprite Sheet
         *                      Using sourceRectangle in the Draw function lets you pick which part of the Sprite Sheet is used
         *                          Ripped the animations for Greater Split directly from Library of Ruina
         *                          https://drive.google.com/drive/folders/12ifYsKtsT7SdkjCiJOGaH40aJ-0uJ4X9
         *                              Each frame on the SpriteSheet for Vertical is 576 x 480
         *                              Each frame on the SpriteSheet for Horizontal is 800 x 480
         *                                  Vertical has all the frames
         *                                  Horizontal will use frames 5-10, though this isn't 100% accurate (don't forget the background will move a bit during it)
         *                                      wait this whole project is scuffed, who cares
         *                  Made a class called Animation that will hole all Animation parameters
         *                  Made a function called AnimateFrame that inputs all needed data to display part of a SpriteSheet
         *                      The class holds variables for tme between frames, total frames, frame size, and whether or not the animation is running
         *                  Made a function called AnimateWhileFrozen that will run AnimateFrame for a defined amount of frames after a defined interval
         *                      This specific function needs the game to be paused. Make one that doesn't need it later
         *                  Quickly tested animating by making the animations play when pressing O and P
         *                      First though, tested to see if you can display all the frames by linking each frame to a keybind (Z - / on the keyboard)
         *                  Added a function MovementAnimate in Animation to animate via movement rather than a spritesheet
         *                      For now, still needs to be frozen
         *                  Made an animation for the horizontal swing
         *                      It will need to have the background moving and Mimicry swinging
         *                      Remember that Mimicry is at the top right, swinging from what I guess is the guard
         *                  Combined Greater Split Horizontal the swing with the effect
         *                      Added an animateAfter parameter to the Animation functions to trigger other animations after that use bools
         *                      Bool animBurnBool will be used for animations that don't trigger anything
         *                  Changed the names of the animation variables and the Split assets
         *                      preGreaterSplitHorizontalSwing to hSplitSwing
         *                      preGreaterSplitHorizontalBackground to hSplitBack
         *                      assets changed with a similar naming scheme
         *                  Made a function called TimeFromMilli to conserve a little space
         *                  Made a function called Angle whih runs Angle() to conserve a little space
         *                  Made an animation for the vertical swing
         *                      This one has the following actions:
         *                          Mimicry starts overhead
         *                          Moves into position, being a little angled up
         *                          Moves down slowly for 3 frames, ending horizontal from the grip
         *                          On frame 4, cuts to the effect
         *                          After the attack, Mimicry should be angled down in after-swing
         *                      Remember that this animation always faces the same way and is a bit to the left of the ship
         *                          Result isn't perfect, however it is close enough.
         *                          
         *                                  (just to remember for now,
         *                                  O is for vertical effect
         *                                  P is for horizontal effect
         *                                  Z is for horizontal swing and horizontal effect
         *                                  X is for vertical swing and vertical effect)
         *                  Downloaded the effect/trail of Mimicry
         *                  Add the trails for both Greater Splits
         *                      Remember that the effects need to instantly appear then fade out
         *                          Almost forgot, also add Mimicry's backswing to Split Horizontal
         *                          Place it based on the effect
         *                      I think you need to use opacity on the color to do it
         *                      
         *                                  (vertical effect placed, displayed, no fade)
         *                                  (horizontal effect placed, displayed, no fade)
         *                  
         *                  
         *                  
         *                  
         *                  (note for when making split vertical attack: the damage should be dealt after the attack, not after the game is unfrozen)
         *                  (for both splits: there should probably be like 1-2 seconds of i-frames after to not die to new asteroids)
         *                  
         *              
         *          Time Erase |just the i-frames|
         *          
         *          Epitaph |shouldn't be too hard based on how i wrote the code|
         *              Time Erase can be upgraded to also use Epitaph if it has been obtained
         *              
         *          Screen Nuke
         *          
         *          
         *      gun plans:
         *          Acid Gun |enemies shot by it get covered in acid, which gradually eats through armor and prevents splitting, but only through death via acid|
         *          
         *          Mines |shoots explosive mines|
         *      
         *      a
         *      
         *      after the upgrades, balance the progression and wrap shit up
         *      
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

        float Angle(float angle)
        {
            return MathHelper.ToRadians(angle);
        }
        TimeSpan TimeFromMilli(float time)
        {
            return TimeSpan.FromMilliseconds(time);
        }
        Texture2D ContentLoad2D(string path)
        {
            return Content.Load<Texture2D>(path);
        }
        bool EnemyCollisionDetection(Rectangle checkedHitbox, List<Bullet> bullets, List<Rectangle> hitboxes, List<float> hitboxPens)
        {
            if (bullets != null && bullets.Count > 0)
            {
                foreach (Bullet bullet in bullets)
                {
                    if (checkedHitbox.Intersects(bullet.Hitbox))
                    {
                        CollisionPenetration = bullet.Penetration;
                        if (!bullet.Burning) { bullets.Remove(bullet); }
                        return true;
                    }
                }

            }

            if (hitboxes != null && hitboxes.Count > 0)
            {
                for (int i = 0; i < hitboxes.Count; i++)
                {
                    if (checkedHitbox.Intersects(hitboxes[i]))
                    {
                        if (TimeHasStopped) { CollisionPenetration = 1 + extraArmorPenetration; }
                        else { CollisionPenetration = hitboxPens[i]; }
                        return true;
                    }
                }
            }

            return false;
        }
        void ShipGotHit()
        {
            if (iFrames <= TimeSpan.Zero)
            {
                ship.Position = playSpace.Center.ToVector2();
                lives--;
                iFrames = TimeFromMilli(2000);
                ship.Velocity = 0;
            }
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

            Upgrade None1 = new Upgrade(baseNone.Position, baseNone.UpgradeType, baseNone.AbilityType, baseNone.UpgradeName, baseNone.UpgradeDescription1,
               baseNone.UpgradeDescription2, baseNone.UpgradeDescription3, baseNone.UpgradeDescription4, baseNone.Rarity, baseNone.LevelAvailability,
               baseNone.ProgressionList, baseNone.ProgressionLevel, 0, 0, baseNone.Image, baseNone.Rotation, baseNone.Scale, baseNone.Color, false);

            Upgrade None2 = new Upgrade(baseNone.Position, baseNone.UpgradeType, baseNone.AbilityType, baseNone.UpgradeName, baseNone.UpgradeDescription1,
               baseNone.UpgradeDescription2, baseNone.UpgradeDescription3, baseNone.UpgradeDescription4, baseNone.Rarity, baseNone.LevelAvailability,
               baseNone.ProgressionList, baseNone.ProgressionLevel, 0, 0, baseNone.Image, baseNone.Rotation, baseNone.Scale, baseNone.Color, false);

            Upgrade None3 = new Upgrade(baseNone.Position, baseNone.UpgradeType, baseNone.AbilityType, baseNone.UpgradeName, baseNone.UpgradeDescription1,
               baseNone.UpgradeDescription2, baseNone.UpgradeDescription3, baseNone.UpgradeDescription4, baseNone.Rarity, baseNone.LevelAvailability,
               baseNone.ProgressionList, baseNone.ProgressionLevel, 0, 0, baseNone.Image, baseNone.Rotation, baseNone.Scale, baseNone.Color, false);

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

            string shipShots = "ShipAndShots/";
            string upgradeImages = "UpgradeImages/";
            string assArm = "Armor/AsteroidArmor";
            string UFOArm = "Armor/BigUFOArmor";
            string ufoArm = "Armor/SmallUFOArmor";
            string tempIdiot = "idiot/You Are An Idiot";

            BigAsteroid = ContentLoad2D("Enemies/BigAsteroid");
            SmallAsteroid = ContentLoad2D("Enemies/SmallAsteroid");
            TinyAsteroid = ContentLoad2D("Enemies/TinyAsteroid");
            BigSaucer = ContentLoad2D("Enemies/Big Saucer");
            SmallSaucer = ContentLoad2D("Enemies/Small Saucer");

            AsteroidArmor = new Texture2D[] { BigAsteroid, ContentLoad2D($"{assArm}1"), ContentLoad2D($"{assArm}2"), ContentLoad2D($"{assArm}3"),
                ContentLoad2D($"{assArm}4"), ContentLoad2D($"{assArm}5"), ContentLoad2D($"{assArm}6") };
            BigUFOArmor = new Texture2D[] { BigSaucer, ContentLoad2D($"{UFOArm}1"), ContentLoad2D($"{UFOArm}2"), ContentLoad2D($"{UFOArm}3") };
            SmallUFOArmor = new Texture2D[] { SmallSaucer, ContentLoad2D($"{ufoArm}1"), ContentLoad2D($"{ufoArm}2"), ContentLoad2D($"{ufoArm}3") };

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ship = new Ship(playSpace.Center.ToVector2(), 0, ContentLoad2D($"{shipShots}Ship"), 0, 1 / 1f, Color.White);

            defaultShot = new Bullet(new Vector2(-20, -20), shotVelocity, ContentLoad2D($"{shipShots}Shot"), 0, 1 / 1f, Color.White, 1, false);

            UFOShot = new Bullet(new Vector2(-20, -20), 5, ContentLoad2D($"{shipShots}LaserShot"), 0, 1 / 1f, Color.White, 0, true);


            level = new Level(100, 0, 5, 1);

            Asteroids.Add(Enemy.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship, 0));
            Asteroids.Add(Enemy.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship, 0));
            Asteroids.Add(Enemy.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship, 0));

            UpgradeSlot = ContentLoad2D($"{upgradeImages}UpgradeSlot");
            SkipUpgrade = ContentLoad2D($"{upgradeImages}UpgradeSkip");
            UpgradeChoice1 = new Button(new Vector2(135, 240), UpgradeSlot, 0, 1, Color.White);
            UpgradeChoice2 = new Button(new Vector2(405, 240), UpgradeSlot, 0, 1, Color.White);
            UpgradeChoice3 = new Button(new Vector2(675, 240), UpgradeSlot, 0, 1, Color.White);
            UpgradeChoiceList = new Button[] { UpgradeChoice1, UpgradeChoice2, UpgradeChoice3 };


            //Upgrade Stuff (each description row can fit around 20-21 characters, tested with capital A's)

            // Example = new Upgrade(position, StatUpgradeType.statype, AbilityUpgradeType.ability, name, string descrip1, string descrip2, string descrip3, string descrip4,
            // int rarity, int levelSeen, List<Upgrade> progList, int progLevel, float energy, float pen, Texture2D image, float rot, float scale, Color color, bool active)

            //levelSeen of 0 means that it will not enter the Possible Upgrade pool naturally

            //Abilities

            Warp = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.Warp, "Warp", "Press M2 to warp", "to a random point", "on screen. Gain 0.4", "seconds of i-frames.",
                65, 1, null, 0, 99, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.DarkGray, false);
            AllUpgrades.Add(Warp);

            Shield1 = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.Shield1, "Shield", "Hold Z to activate", "a shield that", "protects you from", "all damage.",
                50, 1, ShieldProgHolder, 1, 1, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            AllUpgrades.Add(Shield1);
            ShieldProgHolder.Add(Shield1);
            ShieldSprite = new Sprite(BurnVec, ContentLoad2D("Upgrades/ShieldHitbox"), 0, 1 / 1f, Color.White);
            ShieldSprite.DisplayImage = ContentLoad2D("Upgrades/Shield");

            Shield2 = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.Shield2, "Shield+", "You can have the", "shield active for", "longer.", "",
                40, 0, ShieldProgHolder, 2, 1, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            ShieldProgHolder.Add(Shield2);

            Shield3 = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.Shield3, "Shield++", "You can have the", "shield active for", "almost forever.", "",
                50, 0, ShieldProgHolder, 3, 1, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            ShieldProgHolder.Add(Shield3);

            ShieldFinal = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.ShieldFinal, "Flaming Guard", "Coat your shield in", "fire, reverting its", "length but melting",
                "all touched enemies.", 40, 0, ShieldProgHolder, 4, 1, 1, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.OrangeRed, false);
            ShieldProgHolder.Add(ShieldFinal);

            TimeStop1 = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.TimeStop1, "Time Stop", "Press X to stop time", "for 2 seconds. You", "cannot take damage",
                "and can shoot.", 50, 5, TimeStopProgHolder, 1, 1, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.LightYellow, false);
            AllUpgrades.Add(TimeStop1);
            TimeStopProgHolder.Add(TimeStop1);

            TimeStop2 = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.TimeStop2, "Time Stop+", "Your time stop will", "now last 5 seconds,", "but the cooldown",
                "is increased.", 40, 0, TimeStopProgHolder, 2, 1, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.LightYellow, false);
            TimeStopProgHolder.Add(TimeStop2);

            TimeStop3 = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.TimeStop3, "Time Stop++", "Your time stop will", "now last 10 seconds,", "but the cooldown",
                "is increased.", 40 * 2, 0, TimeStopProgHolder, 3, 1, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.LightYellow, false);
            TimeStopProgHolder.Add(TimeStop3);

            TimeStopFinal = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.TimeStopFinal, "Your World", "Your time stop can", "be ended early,", "preserving energy",
                "and thus cooldown.", 30 * 3, 0, TimeStopProgHolder, 4, 1, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.Yellow, false);
            TimeStopProgHolder.Add(TimeStopFinal);

            Mimicry = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.Mimicry, "Mimicry", "Press Q to", "perform Greater", "Split: Vertical",
                "in front of you.", 30, 4, null, 1, 1, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.Red, false);
            MimicrySprite = new Sprite(BurnVec, ContentLoad2D("Upgrades/Mimicry"), 0, 1 / 1f, Color.White);
            AllUpgrades.Add(Mimicry);

            EGO = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.EGO, "Manifestation", "Press E to manifest", "your E.G.O., giving", "a speed buff and",
                "i-frames.", 50, 8, null, 1, 1, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.DarkRed, false);
            EGOSprite = new Sprite(BurnVec, ContentLoad2D("Upgrades/EGO"), 0, 1 / 1f, Color.White);
            AllUpgrades.Add(EGO);


            hSplitSwing = new Animation(new Vector2(width + 100, height/2 - 55), ContentLoad2D("Upgrades/hSplitMimicry"), TimeFromMilli(400), 24, 0, 0, 1 / 1, Color.White);
            hSplitSwing.Origin = new Vector2(hSplitSwing.Image.Width - 50, hSplitSwing.Image.Height);
            hSplitSwingEffect = new Sprite(new Vector2(width/2, height/2), ContentLoad2D("Upgrades/hSplitEffect"), 0, 1 / 1, Color.White);

            hSplitBack = new Animation(new Vector2(0, 0), ContentLoad2D("Upgrades/hSplitBack"), TimeFromMilli(400), 24, 0, 0, 1 / 1, Color.White);
            hSplitBack.Position += new Vector2(hSplitBack.Image.Width/2, hSplitBack.Image.Height/2);

            GreaterSplitHorizontal = new Animation(new Vector2(0, 0), ContentLoad2D("Upgrades/hSplitSpriteSheet"), TimeFromMilli(240), 6 - 1, 800, 0, 1 / 1f, Color.White);


            vSplitSwing = new Animation(ship.Position, ContentLoad2D("Upgrades/Mimicry"), TimeFromMilli(3330), 4, 0, Angle(90), 1 / 1, Color.White);
            //since position changes, the position and origin is defined where it is used
            vSplitSwingEffect = new Sprite(new Vector2(0, 0), ContentLoad2D("Upgrades/vSplitEffect"), 0, 1 / 1, Color.White);

            GreaterSplitVertical = new Animation(new Vector2(100, 0), ContentLoad2D("Upgrades/vSplitSpriteSheet"), TimeFromMilli(450), 10 - 1, 576, 0, 1 / 1f, Color.White);

            //Abilities

            //Upgrades

            ShotSpeedUp1 = new Upgrade(BurnVec, StatUpgradeType.ShotSpeedUp1, AbilityUpNone, "Shot Speed+", "Reduces the time bet-", "-ween default shots",
                "and increases special", "gun energy gain.", 60, 1, ShotSpeedProgHolder, 1, 0, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            AllUpgrades.Add(ShotSpeedUp1);
            ShotSpeedProgHolder.Add(ShotSpeedUp1);

            ShotSpeedUp2 = new Upgrade(BurnVec, StatUpgradeType.ShotSpeedUp1, AbilityUpNone, "Shot Speed++", "Increases fire speed", "and special gun", "energy gain more.", "",
                50, 0, ShotSpeedProgHolder, 2, 0, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            ShotSpeedProgHolder.Add(ShotSpeedUp2);

            ShotSpeedUp3 = new Upgrade(BurnVec, StatUpgradeType.ShotSpeedUp1, AbilityUpNone, "Shot Speed+++", "You can now shoot", "really fast and", "really often.", "",
                40, 0, ShotSpeedProgHolder, 3, 0, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            ShotSpeedProgHolder.Add(ShotSpeedUp3);

            Drones1 = new Upgrade(BurnVec, StatUpgradeType.Drones1, AbilityUpNone, "Drones", "Gain a drone that", "stays near you and", "shoots enemies", "every 5 seconds.",
                40, 3, DroneProgHolder, 1, 0, 0.5f, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            AllUpgrades.Add(Drones1);
            DroneProgHolder.Add(Drones1);

            Drones2 = new Upgrade(BurnVec, StatUpgradeType.Drones2, AbilityUpNone, "Drones+", "Gain an additional", "drone. Your drones", "shoot every 3 sec.", "",
                45, 0, DroneProgHolder, 2, 0, 0.5f, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            DroneProgHolder.Add(Drones2);

            Drones3 = new Upgrade(BurnVec, StatUpgradeType.Drones3, AbilityUpNone, "Drones++", "Gain one more drone.", "Your drones shoot", "every 1.5 seconds.", "",
                50, 0, DroneProgHolder, 3, 0, 0.5f, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            DroneProgHolder.Add(Drones3);

            DronesFinal = new Upgrade(BurnVec, StatUpgradeType.DronesFinal, AbilityUpNone, "Burning Drones", "Your drones take", "twice as long to", "lock on, but they",
                "fire burning bullets.", 40, 0, DroneProgHolder, 4, 0, 0.5f, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.OrangeRed, false);
            DronesFinal.GunBullet = new Bullet(new Vector2(-20, -20), shotVelocity * 1.1f, ContentLoad2D($"{shipShots}BurningDroneShot"), 0, 1 / 1f, Color.White, 1, true);
            DroneProgHolder.Add(DronesFinal);

            ArmorPen1 = new Upgrade(BurnVec, StatUpgradeType.ArmorPen1, AbilityUpNone, "AP Rounds", "Allows all your guns", "to penetrate 1 extra", "layer of armor.",
                "(From the default)", 60, 4, ArmorPenProgHolder, 1, 0, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            AllUpgrades.Add(ArmorPen1);
            ArmorPenProgHolder.Add(ArmorPen1);

            ArmorPen2 = new Upgrade(BurnVec, StatUpgradeType.ArmorPen2, AbilityUpNone, "AP Rounds+", "Allows all your guns", "to penetrate 2 extra", "layers of armor.",
                "(From the default)", 35, 0, ArmorPenProgHolder, 2, 0, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            ArmorPenProgHolder.Add(ArmorPen2);

            ArmorPen3 = new Upgrade(BurnVec, StatUpgradeType.ArmorPen3, AbilityUpNone, "AP Rounds++", "Allows all your guns", "to penetrate 3 extra", "layers of armor.",
                "(From the default)", 20, 0, ArmorPenProgHolder, 3, 0, 0, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.White, false);
            ArmorPenProgHolder.Add(ArmorPen3);

            //Upgrades

            //Guns

            MachineGun = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.Machine, "Machine Gun", "Gives you a rapid-", "-firing machine gun.", "", "",
                45, 2, null, 0, 7, 0.25f, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.DarkSlateGray, false);
            MachineGun.ShotTimer = MachineGunShotTimer;
            MachineGun.reserveShotTimer = reserveMachineGunShotTimer;
            AllUpgrades.Add(MachineGun);

            Laser = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.Laser, "Laser", "Gives you a", "searing and piercing", "laser gun.", "",
                35, 3, null, 0, 33, 1, ContentLoad2D($"{tempIdiot}"), 0, 1 / 1, Color.Red, false);
            Laser.ShotTimer = LaserShotTimer;
            Laser.reserveShotTimer = reserveLaserShotTimer;
            AllUpgrades.Add(Laser);

            MachineGun.GunBullet = new Bullet(new Vector2(-20, -20), shotVelocity * 1.1f, ContentLoad2D($"{shipShots}MachineShot"), 0, 1 / 1f, Color.White,
                MachineGun.Penetration, false);
            Laser.GunBullet = new Bullet(new Vector2(-20, -20), shotVelocity * 1.5f, ContentLoad2D($"{shipShots}LaserShot"), 0, 1 / 1f, Color.White,
                Laser.Penetration, true);

            //Debug Gun (irrelevant)

            devBullets = new Bullet(new Vector2(-20, -20), 20, ContentLoad2D($"{shipShots}LaserShot"), 0, 1 / 1f, Color.White, 1, true);
            devGun = new Upgrade(BurnVec, StatUpNone, AbilityUpgradeType.Laser, "Dev Gun",
                "OP Debug Gun", "", "", "", 0, 0, null, 0, 0, 1, ContentLoad2D($"{shipShots}LaserShot"), 0, 1 / 1, Color.White, false);
            devGun.ShotTimer = devShotTimer;
            devGun.reserveShotTimer = TimeFromMilli(25);

            //Debug Gun (irrelevant)

            //Guns

            foreach (var upgrade in AllUpgrades)
            {
                if (upgrade.LevelAvailability == 1)
                {
                    PossibleUpgrades.Add(upgrade);
                }
            }

            None = new Upgrade(BurnVec, StatUpNone, AbilityUpNone, "Cold Treasure",
                "We will not", "become stronger.", "", "", 100, 0, null, 0, 0, 1, ContentLoad2D($"{upgradeImages}Cold Treasure"), 0, 1 / 1, Color.White, false);

            NoneRefresh(NoneHolder, None);

            UpgradeSkip = new Button(new Vector2(width - 47, height - 13), SkipUpgrade, 0, 1, Color.White);

            defaultGun = None;
            defaultGun.UpgradeName = "";
            defaultGun.Penetration = defaultShot.Penetration;
            defaultGun.GunBullet = defaultShot;
            defaultGun.ShotTimer = defaultTimer;
            defaultGun.reserveShotTimer = reserveDefaultTimer;
            ActiveGuns.Add(defaultGun);


            //Upgrade Stuff



            /*  Old Powerup Code, Archived
            powerDamages = new Texture2D[] { Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 0"), Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 4"), 
                Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 3"), Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 2"), 
                Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 1"), Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/Hit 0") };
            machine = new Powerup(new Vector2(-50, -50), Powerup.Type.Machine, TimeFromMilli(3500), TimeFromMilli(4000), new TimeSpan(0, 0, 0, 0, 100),
                powerDamages, MachineGun.GunBullet, 60, Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/MachineGunPower"), 0, 1 / 1f, Color.Aqua);
            laser = new Powerup(new Vector2(-60, -60), Powerup.Type.Laser, TimeFromMilli(1500), TimeFromMilli(2000), new TimeSpan(0, 0, 0, 0, 200),
                powerDamages, Laser.GunBullet, 40, Content.Load<Texture2D>("PowerUp Images (ARCHIVE)/LaserPower"), 0, 1 / 1f, Color.Yellow);
            */

            // TODO: use this.Content to load your game content here
        }
        KeyboardState lastKeyboardState;
        MouseState lastMouseState;
        protected override void Update(GameTime gameTimer)
        {
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;
            gameTime = gameTimer;


            Window.Title = $"AsteroidsCount: {Asteroids.Count}      UFOCount: {UFOs.Count}      width: {width}      height: {height}        armor: {level.GlobalArmorValue}     levelTimer: {level.LevelSpawnTimer}";

            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();


            Enemy.Sync(AllEnemies, Asteroids, UFOs);
            Enemy.Split(AllEnemies, Asteroids, UFOs);
            lastEnemies = AllEnemies;

            score0s = "";

            if (keyboardState.CapsLock)
            {
                iFrames = TimeFromMilli(300);
            }
            if (keyboardState.IsKeyDown(Keys.OemOpenBrackets) && lastKeyboardState.IsKeyUp(Keys.OemOpenBrackets))
            {
                devGun.isActive = true;
                ActiveGuns.Add(devGun);
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

            if (!GameFrozen) { ship.Move(keyboardState); }

            //Gun Code

            defaultTimer -= gameTime.ElapsedGameTime;
            MachineGun.AbilityUpdate();
            Laser.AbilityUpdate();
            devGun.AbilityUpdate();

            if (mouseState.LeftButton == ButtonState.Pressed && CanShoot)
            {
                if (MachineGun.WillGunShoot())
                {
                    shots.Add(Bullet.BulletTypeCopy(MachineGun.GunBullet, ship.Position, ship.Rotation));
                    MachineGun.AbilityUse();
                }
                else if (Laser.WillGunShoot())
                {
                    shots.Add(Bullet.BulletTypeCopy(Laser.GunBullet, ship.Position, ship.Rotation));
                    Laser.AbilityUse();
                }
                else if ((defaultTimer <= TimeSpan.Zero || lastMouseState.LeftButton == ButtonState.Released) && CurrentActiveGunIndex == 0)
                {
                    shots.Add(Bullet.BulletTypeCopy(defaultGun.GunBullet, ship.Position, ship.Rotation));
                    defaultTimer = reserveDefaultTimer;
                }
                else if (devGun.WillGunShoot())
                {
                    shots.Add(Bullet.BulletTypeCopy(devBullets, ship.Position, ship.Rotation));
                }
            }

            //Gun Code

            //Level Code

            level.VariablePass(tinyAsteroidVelocity, smallAsteroidVelocity, largeAsteroidVelocity, SmallAsteroid, BigAsteroid, SmallSaucer, BigSaucer);

            level.Update(playSpace, Asteroids, UFOs);
            UFOShot.Velocity = level.UFOShotSpeed;

            if (Asteroids.Count == 0 && UFOs.Count == 0 && level.Finished ||
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
                            UpgradesToDraw.Add(Upgrade.Generation(PossibleUpgrades));
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
                if (level.LevelNum == 10)
                {
                    level.LargeAsteroidSpawnChance = 90;
                    level.SmallAsteroidSpawnChance = 90;
                    level.LargeUFOSpawnChance = 90;
                    level.SmallUFOSpawnChance = 90;
                }
                level = Level.NextLevel(level);
                ship.Position = playSpace.Center.ToVector2();
                iFrames = TimeFromMilli(2000);
                lives++;
                ship.Velocity = 0;
                tinyAsteroidVelocity *= 1.05f;
                smallAsteroidVelocity *= 1.1f;
                largeAsteroidVelocity *= 1.2f;
                Asteroids.Add(Enemy.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship, level.GlobalArmorValue));
                Asteroids.Add(Enemy.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship, level.GlobalArmorValue));
                Asteroids.Add(Enemy.InitialSpawn(playSpace, largeAsteroidVelocity, BigAsteroid, ship, level.GlobalArmorValue));
                foreach (var upgrade in PossibleUpgrades)
                {
                    if (upgrade.Rarity < 75)
                    {
                        upgrade.RarityChange(upgrade.Rarity + 5);
                    }
                }
                foreach (var upgrade in AllUpgrades)
                {
                    if (upgrade.LevelAvailability == level.LevelNum)
                    {
                        PossibleUpgrades.Add(upgrade);
                    }
                }
                UpgradeChosen = false;
                UpgradeTime = 0;
            }

            //Level Code

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
                    foreach (var upgrade in UpgradesToDraw)
                    {
                        upgrade.Skipped();
                        upgrade.UpgradeButton.wasClicked = false;
                    }
                    broken = true;
                }
                if (UpgradesToDraw[i].isActive && !UpgradeSkip.wasClicked && !broken)
                {
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

            if ((iFrames -= gameTime.ElapsedGameTime) >= TimeSpan.Zero)
            {
                int color = 255 * ((int)iFrames.TotalMilliseconds % 2);
                ship.Color = new Color(color, color, color);
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
            }
            counter = 0;
            foreach (var enemyShot in enemyShots)
            {
                if (!TimeHasStopped && !GameFrozen) { enemyShot.Move(); }

                if (ship.Hitbox.Intersects(enemyShot.Hitbox) && (!TimeHasStopped && !GameFrozen))
                {
                    enemyShots.Remove(enemyShot);
                    ShipGotHit();
                    break;
                }

                if (!IsBulletInBounds(enemyShots, counter, playSpace) && (!TimeHasStopped && !GameFrozen))
                {
                    break;
                }
                counter++;
            }
            counter = 0;

            foreach (var asteroid in Asteroids)
            {
                asteroid.TimeStopDamage();
                if (EnemyCollisionDetection(asteroid.Hitbox, shots, ExtraHitboxes, ExtraPens) || asteroid.broken)
                {
                    //Archived PowerUp Code: machine.Spawned(Asteroids[i].Position, new Vector2(rand.Next(1, 4), rand.Next(1, 4)), Asteroids[i].leSize);

                    if (TimeHasStopped || GameFrozen)
                    {
                        if (asteroid.ArmorValue - asteroid.stoppedDamage < 0)
                        {
                            asteroid.hits++;
                        }
                        asteroid.stoppedDamage += CollisionPenetration;
                    }
                    else if (asteroid.leSize == Size.LeChonk && (!asteroid.ArmorDamage(CollisionPenetration) || asteroid.broken))
                    {
                        score += 10;
                        asteroid.leSize++;
                        asteroid.Image = SmallAsteroid;
                        asteroid.Velocity = new Vector2(asteroid.Velocity.X * (largeAsteroidVelocity / smallAsteroidVelocity),
                            asteroid.Velocity.Y * (largeAsteroidVelocity / smallAsteroidVelocity));
                        Asteroids.Add(new Enemy(new Vector2(asteroid.Position.X + 60, asteroid.Position.Y), new Vector2(-asteroid.Velocity.X * 2, -asteroid.Velocity.Y * 2),
                            SmallAsteroid, 0, 1 / 1f, Color.White, Size.Normal, TimeSpan.Zero, Enemy.Type.Asteroid, 0));
                        asteroid.HitboxRefresh();
                    }
                    else if (asteroid.leSize == Size.Normal)
                    {
                        score += 30;
                        asteroid.leSize++;
                        asteroid.Image = TinyAsteroid;
                        asteroid.Velocity = new Vector2(asteroid.Velocity.X * (smallAsteroidVelocity / tinyAsteroidVelocity),
                            asteroid.Velocity.Y * (smallAsteroidVelocity / tinyAsteroidVelocity));
                        Asteroids.Add(new Enemy(new Vector2(asteroid.Position.X + 25, asteroid.Position.Y), new Vector2(-asteroid.Velocity.X * 2, -asteroid.Velocity.Y * 2),
                            TinyAsteroid, 0, 1 / 1f, Color.White, Size.Baby, TimeSpan.Zero, Enemy.Type.Asteroid, 0));
                        asteroid.HitboxRefresh();
                    }
                    else if (asteroid.leSize == Size.Baby)
                    {
                        score += 50;
                        Asteroids.Remove(asteroid);
                    }

                    if (asteroid.broken)
                    {
                        asteroid.brokenTimes--;
                        if (asteroid.brokenTimes <= 0)
                        {
                            asteroid.broken = false;
                        }
                    }

                    break;
                }

                if (counter >= Asteroids.Count)
                {
                    break;
                }

                if (!TimeHasStopped && !GameFrozen)
                {
                    if (asteroid.leSize == Size.LeChonk) { asteroid.Rotation += 0.005f; }
                    else { asteroid.Rotation += 0.01f; }

                    asteroid.Position = new Vector2(asteroid.Position.X + (float)Math.Sin(asteroid.Rotation) + asteroid.Velocity.X,
                        asteroid.Position.Y - (float)Math.Cos(asteroid.Rotation) + asteroid.Velocity.Y);

                    asteroid.IsInBounds(playSpace);

                    if (ship.Hitbox.Intersects(asteroid.Hitbox)) { ShipGotHit(); }
                }
                counter++;
            }
            counter = 0;

            UFOMovementSpace = new Rectangle((int)ship.Position.X - 60 - level.UFOMoveExtra, 50, 120 + (2 * level.UFOMoveExtra), 60);
            UFOShotArea = new Rectangle((int)ship.Position.X - level.UFORange, (int)ship.Position.Y, level.UFORange * 2, 1);

            foreach (var UFO in UFOs)
            {
                UFO.TimeStopDamage();
                if (EnemyCollisionDetection(UFO.Hitbox, shots, ExtraHitboxes, ExtraPens) || UFO.broken)
                {
                    if (TimeHasStopped || GameFrozen)
                    {
                        if (UFO.ArmorValue - UFO.stoppedDamage < 0)
                        {
                            UFO.hits++;
                        }
                        UFO.stoppedDamage += CollisionPenetration;
                    }
                    else if (!UFO.ArmorDamage(CollisionPenetration) || UFO.broken)
                    {
                        //Archived PowerUp Code: laser.Spawned(UFOs[i].Position, new Vector2(rand.Next(1, 3), rand.Next(1, 3)), UFOs[i].leSize);

                        score += 100 * (((int)UFO.leSize) - 1);

                        UFOs.Remove(UFO);

                        break;
                    }

                    if (UFO.broken)
                    {
                        UFO.brokenTimes--;
                        if (UFO.brokenTimes <= 0)
                        {
                            UFO.broken = false;
                        }
                    }
                }

                if (counter >= UFOs.Count) { break; }

                if (!TimeHasStopped && !GameFrozen)
                {
                    UFO.ShotTimer -= gameTime.ElapsedGameTime;

                    UFO.UFOMovement(UFOMovementSpace);

                    UFO.IsInBounds(playSpace);

                    if (ship.Hitbox.Intersects(UFO.Hitbox)) { ShipGotHit(); }
                }

                if (UFO.ShotTimer <= TimeSpan.Zero)
                {
                    enemyShots.Add(UFO.Shoot(UFOShotArea, UFOShot));
                    UFO.ShotTimer = UFO.reserveShotTimer;
                    break;
                }
                counter++;
            }
            counter = 0;

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
                    reserveDefaultTimer -= new TimeSpan(0, 0, 0, 0, 250);
                    reserveDefaultTimer -= TimeFromMilli(125 * i);
                    MachineGun.EnergyGainMultiplier += 0.5f;
                    Laser.EnergyGainMultiplier += 0.5f;
                    ShotSpeedProgHolder[i].inEffect = true;
                    break;
                }
            }

            for (int i = 0; i < DroneProgHolder.Count; i++)
            {
                if (DroneProgHolder[3].isActive && !DroneProgHolder[3].inEffect)
                {
                    foreach (var drone in DroneList)
                    {
                        drone.Color = Color.OrangeRed;
                    }
                    for (int k = 0; k < DroneLockOnTimers.Count; k++)
                    {
                        DroneLockOnTimers[k] = TimeFromMilli(3000);
                    }
                    DroneProgHolder[0].GunBullet = DronesFinal.GunBullet;
                }
                if (i == 3 && DroneProgHolder[3].inEffect) { break; }

                if (DroneProgHolder[i].isActive && !DroneProgHolder[i].inEffect)
                {
                    DroneList.Add(new Ship(BurnVec, 0, Content.Load<Texture2D>("Upgrades/Drone"), 0, 1 / 1f, Color.White));
                    if (i != 3) { DroneProgHolder[0].GunBullet = MachineGun.GunBullet; }

                    float a = 0; if (i >= 1) { a = 500; }
                    reserveDroneShotTimer = TimeFromMilli(3500 - (1500 * i) - a);
                    DroneShotTimers.Add(reserveDroneShotTimer);
                    DroneLockOnTimers.Add(TimeFromMilli(1500));

                    DroneTargetValues.Add(-1);

                    DroneProgHolder[i].inEffect = true;
                    break;
                }

                if (DroneProgHolder[i].inEffect)
                {
                    /*
                    In case I ever need this code again

                    bool asteroidOrUFO = true; //true = asteroid; false = UFO
                    if (Asteroids.Count != 0 && UFOs.Count != 0) { asteroidOrUFO = Convert.ToBoolean(rand.Next(0, 2)); }
                    else if (Asteroids.Count != 0 && UFOs.Count == 0) { asteroidOrUFO = true; }
                    else if (Asteroids.Count == 0 && UFOs.Count != 0) { asteroidOrUFO = false; }
                    else if (Asteroids.Count == 0 && UFOs.Count == 0) { break; }

                    int temp = 0;
                    if (asteroidOrUFO) { temp = Asteroids.Count; }
                    else if (!asteroidOrUFO) { temp = UFOs.Count; }
                    int target = rand.Next(0, AllEnemies.Count);

                    Vector2 droneTargetPosition = AllEnemies[rand.Next(0, AllEnemies.Count)].Position;
                    if (asteroidOrUFO) { droneTargetPosition = Asteroids[target].Position; }
                    else if (!asteroidOrUFO) { droneTargetPosition = UFOs[target].Position; }
                    */

                    DroneShotTimers[i] -= gameTime.ElapsedGameTime;
                    if (DroneShotTimers[i] <= TimeSpan.Zero)
                    {
                        if (DroneTargetValues[i] == -1 || DroneTargetValues[i] >= AllEnemies.Count)
                        {
                            DroneTargetValues[i] = rand.Next(0, AllEnemies.Count);
                        }

                        if (lastEnemies.Count > 0 && AllEnemies.Count > 0 && i < lastEnemies.Count && i < AllEnemies.Count &&
                            lastEnemies[DroneTargetValues[i]] != AllEnemies[DroneTargetValues[i]])
                        {
                            for (int j = 0; j < AllEnemies.Count; j++)
                            {
                                if (AllEnemies[j] == lastEnemies[DroneTargetValues[i]])
                                {
                                    DroneTargetValues[i] = j;
                                    break;
                                }
                                if (j == AllEnemies.Count - 1)
                                {
                                    DroneLockOnTimers[i] = TimeFromMilli(1500);
                                    if (DroneProgHolder[3].isActive) { DroneLockOnTimers[i] = TimeFromMilli(3000); }
                                    DroneTargetValues[i] = rand.Next(0, AllEnemies.Count);
                                    break;
                                }
                            }
                        }


                        Vector2 start;
                        Vector2 destination;
                        Vector2 between;
                        //
                        start = DroneList[i].Hitbox.Center.ToVector2();
                        if (DroneTargetValues[i] == -1 || AllEnemies.Count == 0) { destination = BurnVec; }
                        else { destination = AllEnemies[DroneTargetValues[i]].Position; }
                        between = start - destination;
                        //
                        float angle = (float)Math.Atan2((double)between.Y, (double)between.X) - Angle(90);
                        DroneList[i].Rotation = angle;

                        DroneLockOnTimers[i] -= gameTime.ElapsedGameTime;
                        if (DroneLockOnTimers[i] <= TimeSpan.Zero)
                        {
                            shots.Add(Bullet.BulletTypeCopy(DroneProgHolder[0].GunBullet, DroneList[i].Position, angle));
                            DroneShotTimers[i] = reserveDroneShotTimer;
                            DroneLockOnTimers[i] = TimeFromMilli(1500);
                            if (DroneProgHolder[3].isActive) { DroneLockOnTimers[i] = TimeFromMilli(3000); }
                            DroneTargetValues[i] = -1;
                        }
                    }
                }
            }

            for (int i = 0; i < ArmorPenProgHolder.Count; i++)
            {
                if (ArmorPenProgHolder[i].isActive && !ArmorPenProgHolder[i].inEffect)
                {
                    defaultGun.Penetration += 1; defaultShot.BulletPenInherit(defaultGun);
                    float a = 0.25f + ((i + (float)Math.Floor((double)i / 2)) * 0.25f);
                    MachineGun.Penetration += a; MachineGun.GunBullet.BulletPenInherit(MachineGun); //lower from faster firerate
                    //Laser.Penetration += 1; Laser.GunBullet.BulletPenInherit(Laser);  - not needed because it's burning
                    extraArmorPenetration += 1;
                    ArmorPenProgHolder[i].inEffect = true;
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

                if (mouseState.ScrollWheelValue != lastMouseState.ScrollWheelValue)
                {
                    i = CurrentActiveGunIndex;
                    swapped = true;
                    if (mouseState.ScrollWheelValue > lastMouseState.ScrollWheelValue) { i++; }
                    if (mouseState.ScrollWheelValue < lastMouseState.ScrollWheelValue) { i--; }
                }

                if (i == ActiveGuns.Count) { i = 0; }
                if (i < 0) { i = ActiveGuns.Count - 1; }

                if (swapped)
                {
                    CurrentActiveGunIndex = i;

                    GunInEffectName = ActiveGuns[i].UpgradeName; GunInEffectColor = ActiveGuns[i].Color; ActiveGuns[i].inEffect = true;

                    foreach (var gun in ActiveGuns)
                    {
                        if (gun != ActiveGuns[i]) { gun.inEffect = false; }
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

            ExtraPens.Clear();
            ExtraHitboxes.Clear();

            Warp.AbilityUpdate();
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
                    ShieldProgHolder[i].AbilityUpdate();
                    if (keyboardState.IsKeyDown(Keys.Z) && ShieldProgHolder[i].WillAbilityGetUsed())
                    {
                        float energyGainMultiplier = 5;
                        if (i == 1) { energyGainMultiplier = 9; }
                        if (i == 2) { energyGainMultiplier = 18; }
                        if (i == 3) { energyGainMultiplier = 3.8f; }
                        ShieldProgHolder[i].EnergyGainMultiplier = energyGainMultiplier;
                        ShieldProgHolder[i].AbilityUse();
                        CanShoot = false;
                        /*
                        if (i == ShieldFinal.ProgressionLevel - 1 && !ShieldProgHolder[i].inEffect)
                        {
                            ExtraHitboxes.Add(ShieldSprite.Hitbox);
                            ExtraPens.Add(ShieldFinal.Penetration);
                        }*/
                        ShieldProgHolder[i].inEffect = true;
                    }
                    else
                    {
                        ShieldProgHolder[i].inEffect = false;
                        CanShoot = true;
                        //ExtraHitboxes.Remove(ShieldSprite.Hitbox);
                        //ExtraPens.Remove(ShieldFinal.Penetration);
                    }

                    if (ShieldProgHolder[i].inEffect)
                    {
                        if (iFrames <= TimeFromMilli(20)) { iFrames = TimeFromMilli(20); }

                        if ((i == ShieldFinal.ProgressionLevel - 1) /*&& (!ExtraHitboxes.Contains(ShieldSprite.Hitbox))*/)
                        {
                            ExtraHitboxes.Add(ShieldSprite.Hitbox);
                            ExtraPens.Add(ShieldFinal.Penetration);
                        }

                    }
                    /*
                    else
                    {
                        ExtraHitboxes.Remove(ShieldSprite.Hitbox);
                        ExtraPens.Remove(ShieldFinal.Penetration);
                    }
                    */

                    break;
                }
            }

            for (int i = 0; i < TimeStopProgHolder.Count; i++)
            {
                if (TimeStopProgHolder[i].isActive)
                {
                    TimeStopProgHolder[i].AbilityUpdate();
                    if (keyboardState.IsKeyDown(Keys.X) && lastKeyboardState.IsKeyUp(Keys.X)
                        && TimeStopProgHolder[i].WillAbilityGetUsed() && (!TimeHasStopped && !GameFrozen)
                        && (i == 3 || TimeStopProgHolder[i].energyRemaining.Width >= 98))
                    {
                        TimeStopProgHolder[i].EnergyGainMultiplier = 0.2f;
                        float energyUse = 0.9f;
                        if (i == 1) { energyUse = 0.4f; TimeStopProgHolder[i].EnergyGainMultiplier = 1.5f; }
                        if (i == 2) { energyUse = 0.2f; TimeStopProgHolder[i].EnergyGainMultiplier = 0.5f; }
                        if (i == 3) { energyUse = 0.2f; TimeStopProgHolder[i].EnergyGainMultiplier = 0.5f; }
                        TimeStopProgHolder[i].EnergyUse = energyUse;
                        TimeStopProgHolder[i].inEffect = true;
                        TimeHasStopped = true;
                    }
                    else if (TimeStopProgHolder[i].energyRemaining.Width <= 0.5f
                        || (i == 3 && keyboardState.IsKeyDown(Keys.X) && lastKeyboardState.IsKeyUp(Keys.X))
                        && TimeHasStopped)
                    {
                        float energyGainMultiplier = 7;
                        if (i == 1) { energyGainMultiplier = 5; }
                        if (i == 2) { energyGainMultiplier = 3; }
                        if (i == 3) { energyGainMultiplier = 3; }
                        TimeStopProgHolder[i].EnergyGainMultiplier = energyGainMultiplier;
                        TimeStopProgHolder[i].inEffect = false;
                        TimeHasStopped = false;
                    }

                    if (TimeStopProgHolder[i].inEffect) { TimeStopProgHolder[i].AbilityUse(); }

                    break;
                }
            }

            //Ability Effect Code


            // TODO: Add your update logic here

            lastKeyboardState = keyboardState;
            lastMouseState = mouseState;
            base.Update(gameTime);
            //test

            if (rand.Next(0,2)==1)
            {
                BoolForRandomTests = true;
            }
            else
            {
                BoolForRandomTests = false;
            }




            if (keyboardState.IsKeyDown(Keys.O))
            {
                GreaterSplitVertical.AnimationRunning = true;
                GameFrozen = true;
            }
            if (keyboardState.IsKeyDown(Keys.P))
            {
                GreaterSplitHorizontal.AnimationRunning = true;
                GameFrozen = true;
            }
            if (keyboardState.IsKeyDown(Keys.Z))
            {
                hSplitSwing.AnimationRunning = true;
                GameFrozen = true;
            }
            if (keyboardState.IsKeyDown(Keys.X))
            {
                vSplitSwing.AnimationRunning = true;
                GameFrozen = true;
            }

            //test
        }

        protected override void Draw(GameTime gameTime)
        {
            int width = GraphicsDevice.Viewport.Width;
            int height = GraphicsDevice.Viewport.Height;

            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            foreach (var asteroid in Asteroids)
            {
                asteroid.Draw(_spriteBatch);
            }
            foreach (var UFO in UFOs)
            {
                UFO.Draw(_spriteBatch);
            }

            //for testing sprites
            //ship.DisplayImage = ContentLoad2D("Upgrades/frame6");

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
            }
            counter = 0;
            foreach (var gun in ActiveGuns)
            {
                gun.EnergyDraw(_spriteBatch);
            }


            //Ability and Upgrade Effects

            foreach (var upgrade in ActiveUpgrades)
            {
                if (upgrade.UpgradeType >= StatUpgradeType.Drones1 && upgrade.UpgradeType <= StatUpgradeType.DronesFinal && upgrade.inEffect)
                {
                    for (int i = 0; i < DroneList.Count; i++)
                    {
                        switch (i)
                        {
                            case 0: DroneList[i].Position = new Vector2(ship.Position.X + 22, ship.Position.Y - 12); break;
                            case 1: DroneList[i].Position = new Vector2(ship.Position.X - 22, ship.Position.Y - 12); break;
                            case 2: DroneList[i].Position = new Vector2(ship.Position.X + 00, ship.Position.Y + 32); break;
                        }

                        DroneList[i].Draw(_spriteBatch);
                    }
                }
            }

            foreach (var ability in ActiveAbilities)
            {
                if (ability.AbilityType >= AbilityUpgradeType.Shield1 && ability.AbilityType <= AbilityUpgradeType.ShieldFinal && ability.inEffect)
                {
                    ShieldSprite.Position.X = ship.Position.X;
                    ShieldSprite.Position.Y = ship.Position.Y;
                    ShieldSprite.Rotation = ship.Rotation;
                    if (ability.AbilityType == AbilityUpgradeType.ShieldFinal)
                        ShieldSprite.Color = Color.OrangeRed;
                    ShieldSprite.Draw(_spriteBatch);
                }
                if (ability.AbilityType >= AbilityUpgradeType.TimeStop1 && ability.AbilityType <= AbilityUpgradeType.TimeStopFinal && ability.inEffect)
                {
                    _spriteBatch.FillRectangle(0, 0, 800, 480, new Color(Color.Black, 75));
                }
            }


            //Ability and Upgrade Effects


            foreach (var upgrade in UpgradesToDraw)
            {
                upgrade.Draw(upgradeTitleFont, upgradeDescFont, _spriteBatch);
            }
            UpgradeSkip.Draw(_spriteBatch);


            //test
            if (vSplitPostSwing>TimeSpan.Zero)
            {
                vSplitPostSwing -= gameTime.ElapsedGameTime;
                vSplitSwing.Draw(_spriteBatch);
                vSplitSwingEffect.Position.X = vSplitSwing.Position.X - 65;
                vSplitSwingEffect.Position.Y = vSplitSwing.Position.Y - 150;
                vSplitSwingEffect.Draw(_spriteBatch);
                if (vSplitPostSwing-TimeFromMilli(5)<=TimeSpan.Zero)
                {
                    vSplitSwing.Rotation = Angle(90);
                    GameFrozen = false;
                }
            }
            if (hSplitPostSwing>TimeSpan.Zero)
            {
                hSplitPostSwing -= gameTime.ElapsedGameTime;
                hSplitSwingEffect.Draw(_spriteBatch);
                if (hSplitPostSwing - TimeFromMilli(5) <= TimeSpan.Zero)
                {
                    GameFrozen = false;
                }
            }


            if (splitBack>TimeSpan.Zero)
            {
                splitBack -= gameTime.ElapsedGameTime;
                _spriteBatch.FillRectangle(new Rectangle(0, 0, width, height), splitBackColor);
            }


            if (GreaterSplitVertical.AnimationRunning && GameFrozen)
            {
                splitBackColor = Color.DarkRed;
                splitBack = new TimeSpan(0, 0, 0, 0, 300);
                vSplitPostSwing = new TimeSpan(0, 0, 0, 1, 750);
                vSplitSwing.Rotation = Angle(-30);
                GreaterSplitVertical.AnimateWhileFrozen(_spriteBatch, ref animBurnBool);
                GameFrozen = true;
            }

            if (vSplitSwing.AnimationRunning && GameFrozen)
            {
                vSplitSwing.Position = new Vector2(ship.Position.X - vSplitSwing.Image.Width/2 + 80, ship.Position.Y - vSplitSwing.Image.Height/2 + 20);
                vSplitSwing.Origin = new Vector2(vSplitSwing.Image.Width, vSplitSwing.Image.Height/2);
                vSplitSwing.MovementAnimate(Angle(-90), new Vector2(0, 0), _spriteBatch, ref GreaterSplitVertical.AnimationRunning);
                GameFrozen = true;
            }


            if (hSplitSwing.AnimationRunning && GameFrozen)
            {
                hSplitBack.MovementAnimate(0, new Vector2(-224, 0), _spriteBatch, ref animBurnBool);
                hSplitSwing.MovementAnimate(Angle(-80), new Vector2(40, -40), _spriteBatch, ref GreaterSplitHorizontal.AnimationRunning);
                hSplitSwingToEffect = new TimeSpan(0, 0, 0, 0, 150);
                splitBackColor = Color.Black;
                splitBack = new TimeSpan(0, 0, 0, 0, 600);
                GameFrozen = true;
            }

            if (GreaterSplitHorizontal.AnimationRunning && GameFrozen)
            {
                hSplitSwingToEffect -= gameTime.ElapsedGameTime;
                if (hSplitSwingToEffect<=TimeSpan.Zero)
                {
                    GreaterSplitHorizontal.AnimateWhileFrozen(_spriteBatch, ref animBurnBool);
                }
                hSplitPostSwing = new TimeSpan(0, 0, 0, 1, 000);
                GameFrozen = true;
            }


            //test
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
