using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asteroid
{
    class Level
    {
        public int SmallAsteroidTotal;
        public int LargeAsteroidTotal;
        public int LargeUFOTotal;
        public int UFOTotal;
        public int SmallSpawned;
        public int LargeSpawned;
        public int SmallUSpawned;
        public int LargeUSpawned;
        public TimeSpan GlobalSpawnTimer = new TimeSpan();
        public TimeSpan reserveSpawnTimer;
        public int LevelNum;
        public int UFORange;
        public TimeSpan UFOShotTimer = new TimeSpan(0, 0, 5);
        public TimeSpan SmallUFOShotTimer = new TimeSpan(0, 0, 0, 4, 500);
        public float UFOShotSpeed;
        public bool Finished = false;
        public bool SmallAssMax = false;
        public bool LargeAssMax = false;
        public bool SmallSauceMax = false;
        public bool LargeSauceMax = false;
        public float GlobalArmorValue = 0;
        private float armorMin = 0;
        private float armorMax = 0;
        private float noArmorChain = 0;

        private TimeSpan smallTimer;
        private TimeSpan largeUTimer;
        private TimeSpan smallUTimer;

        private float tinyvelocity;
        private float smallvelocity;
        private float largevelocity;
        private Texture2D smallAss;
        private Texture2D largeAss;
        private Texture2D smallUFO;
        private Texture2D largeUFO;

        public Level(int smallCount, int largeCount, int largeUCount, int uCount, TimeSpan spawnTimer, int range, float speed, int level)
        {
            SmallAsteroidTotal = smallCount;
            LargeAsteroidTotal = largeCount;
            LargeUFOTotal = largeUCount;
            UFOTotal = uCount;

            GlobalSpawnTimer = spawnTimer;
            UFORange = range;
            LevelNum = level;

            reserveSpawnTimer = GlobalSpawnTimer;
            smallTimer = reserveSpawnTimer * 0.5;

            largeUTimer = reserveSpawnTimer * 0.85;
            smallUTimer = reserveSpawnTimer * 1.5;

            UFOShotSpeed = speed;
        }

        public void VariablePass(float tiny, float small, float large, Texture2D smallA, Texture2D largeA,Texture2D smallUF, Texture2D largeUF)
        {
            tinyvelocity = tiny;
            smallvelocity = small;
            largevelocity = large;
            smallAss = smallA;
            largeUFO = largeUF;
            largeAss = largeA;
            smallUFO = smallUF;
        }

        public static Level NextLevel(Level lLev)
        {
            Level newLevel = new Level(lLev.SmallAsteroidTotal, lLev.LargeAsteroidTotal, lLev.LargeUFOTotal, lLev.UFOTotal, 
                lLev.GlobalSpawnTimer, lLev.UFORange, lLev.UFOShotSpeed, lLev.LevelNum);

            newLevel.LevelNum++;
            if (newLevel.LevelNum==2)
            {
                newLevel.LargeUFOTotal = 12;
                newLevel.UFOTotal = 3;
                newLevel.LargeAsteroidTotal = 3;
            }
            else if (newLevel.LevelNum==3)
            {
                newLevel.LargeUFOTotal = (int)(newLevel.LargeUFOTotal * 0.5);
                newLevel.UFOTotal *= 2;
                newLevel.UFOShotTimer *= 0.9f;
                newLevel.SmallUFOShotTimer *= 0.85f;
                newLevel.UFORange -= 10;
            }
            else if (newLevel.LevelNum>=5 && newLevel.LevelNum<8)
            {
                newLevel.LargeUFOTotal = 0;
                newLevel.UFOTotal = (int)(newLevel.UFOTotal * 1.75);
                newLevel.SmallUFOShotTimer *= 0.75f;
                newLevel.UFORange -= 20;
            }
            else if (newLevel.LevelNum>=8)
            {
                newLevel.LargeUFOTotal = 0;
                newLevel.UFOTotal = (int)(newLevel.UFOTotal * 2.5);
                newLevel.SmallUFOShotTimer *= 0.5f;
                newLevel.UFORange = 10;
            }

            newLevel.UFOShotSpeed *= 1.25f;

            newLevel.LargeAsteroidTotal = (int)(newLevel.LargeAsteroidTotal * 1.5);
            newLevel.SmallAsteroidTotal = (int)(newLevel.SmallAsteroidTotal * 1.75);

            newLevel.GlobalSpawnTimer = newLevel.GlobalSpawnTimer * 0.9;
            newLevel.reserveSpawnTimer = newLevel.GlobalSpawnTimer;

            //Level newLevel= new Level();
            //1.75x as many smalls
            //1.5x as many larges
            //0.5x as many largeUFOs until level 4, after which x0 as many largeUFOs
            //+2 as many smallUFOs until level 4, after which 1.75x as many smallUFOs
            //0.8x the spawn timer
            //round down each time

            return newLevel;
        }
        public void Update(Rectangle playSpace, List<Enemy> roids, List<Enemy> flys)
        {
            Random rand = new Random();

            GlobalSpawnTimer -= Game1.gameTime.ElapsedGameTime;
            smallTimer -= Game1.gameTime.ElapsedGameTime;
            largeUTimer -= Game1.gameTime.ElapsedGameTime;
            smallUTimer -= Game1.gameTime.ElapsedGameTime;

            armorMax = (float)Math.Floor((float)LevelNum / 3);
            if (armorMax>6) { armorMax = 6; }

            if (armorMax>2) { armorMin = armorMax - 3; }

            if (GlobalSpawnTimer<=TimeSpan.Zero||largeUTimer<=TimeSpan.Zero||smallUTimer<=TimeSpan.Zero)
            {
                GlobalArmorValue = rand.Next((int)armorMin, (int)armorMax + 1);
                if (GlobalArmorValue==0)    { noArmorChain++; }
                if (noArmorChain==3)        { GlobalArmorValue = rand.Next(1, (int)armorMax + 1); noArmorChain = 0; }
            }

            float UFOArmor = (float)Math.Floor(GlobalArmorValue / 2);

            if (GlobalSpawnTimer<=TimeSpan.Zero && !LargeAssMax)
            {
                //whatSpawns = Game1.WhatSpawned.LargeAss;
                roids.Add(Enemy.NaturalSpawn(Enemy.Type.Asteroid, playSpace, largevelocity, largeAss, Game1.Size.LeChonk, TimeSpan.Zero, GlobalArmorValue));
                LargeSpawned++;
                GlobalSpawnTimer = reserveSpawnTimer;

                if (LargeSpawned>=LargeAsteroidTotal) { LargeAssMax = true; }
            }

            if (smallTimer<=TimeSpan.Zero && !SmallAssMax)
            {
                //whatSpawns = Game1.WhatSpawned.SmallAss;
                roids.Add(Enemy.NaturalSpawn(Enemy.Type.Asteroid, playSpace, smallvelocity, smallAss, Game1.Size.Normal, TimeSpan.Zero, 0));
                SmallSpawned++;
                smallTimer = reserveSpawnTimer * 0.5;

                if (SmallSpawned>=SmallAsteroidTotal) { SmallAssMax = true; }
            }

            if (largeUTimer<=TimeSpan.Zero && !LargeSauceMax)
            {
                //whatSpawns = Game1.WhatSpawned.LargeSauce;
                flys.Add(Enemy.NaturalSpawn(Enemy.Type.UFO, playSpace, largevelocity, largeUFO, Game1.Size.Normal, UFOShotTimer, UFOArmor));
                LargeUSpawned++;
                largeUTimer = reserveSpawnTimer * 0.8;

                if (LargeUSpawned >= LargeUFOTotal) { LargeSauceMax = true; }
            }

            if (smallUTimer<=TimeSpan.Zero && !SmallSauceMax)
            {
                //whatSpawns = Game1.WhatSpawned.SmallSauce;
                flys.Add(Enemy.NaturalSpawn(Enemy.Type.Asteroid, playSpace, tinyvelocity, smallUFO, Game1.Size.Baby, SmallUFOShotTimer, UFOArmor));
                SmallUSpawned++;
                smallUTimer = reserveSpawnTimer * 1.5;

                if (SmallUSpawned>=UFOTotal) { SmallSauceMax = true; }
            }

            if (SmallSauceMax && LargeSauceMax && SmallAssMax && LargeAssMax)
                { Finished = true; }
        }
    }
}
