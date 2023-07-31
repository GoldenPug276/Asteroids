using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Asteroid
{
    class Level
    {
        /* Old System Archive
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

        private TimeSpan smallTimer;
        private TimeSpan largeUTimer;
        private TimeSpan smallUTimer;
        */

        public float SmallAsteroidSpawnChance = 2.5f;
        public float LargeAsteroidSpawnChance = 5;
        public float LargeUFOSpawnChance = 0;
        public float SmallUFOSpawnChance = 0;
        public TimeSpan LevelSpawnTimer = new TimeSpan(0, 0, 30);
        public TimeSpan SpawnOpportunityTimer = new TimeSpan(0, 0, 1);
        public TimeSpan reserveSpawnOppTimer = new TimeSpan(0, 0, 1);

        public int LevelNum;
        public int UFORange;
        public int UFOMoveExtra;
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

        private float tinyvelocity;
        private float smallvelocity;
        private float largevelocity;
        private Texture2D smallAss;
        private Texture2D largeAss;
        private Texture2D smallUFO;
        private Texture2D largeUFO;

        public Level(int range, int movePlus, float speed, int level)//int smallCount, int largeCount, int largeUCount, int uCount, TimeSpan spawnTimer
        {
            //SmallAsteroidTotal = smallCount;
            //LargeAsteroidTotal = largeCount;
            //LargeUFOTotal = largeUCount;
            //UFOTotal = uCount;

            //GlobalSpawnTimer = spawnTimer;

            //reserveSpawnTimer = GlobalSpawnTimer;
            //smallTimer = reserveSpawnTimer * 0.5;

            //largeUTimer = reserveSpawnTimer * 0.85;
            //smallUTimer = reserveSpawnTimer * 1.5;

            UFORange = range;
            UFOMoveExtra = movePlus;
            LevelNum = level;

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
            Level newLevel = new Level(lLev.UFORange, lLev.UFOMoveExtra, lLev.UFOShotSpeed, lLev.LevelNum);

            newLevel.LevelNum++;
            if (newLevel.LevelNum==2)
            {
                //newLevel.LargeUFOTotal = 12;
                //newLevel.UFOTotal = 3;
                //newLevel.LargeAsteroidTotal = 3;

                newLevel.LargeAsteroidSpawnChance = 7.5f;
                newLevel.SmallAsteroidSpawnChance = 15;
                newLevel.LargeUFOSpawnChance = 7.5f;
                newLevel.SmallUFOSpawnChance = 5f;
            }
            else if (newLevel.LevelNum==3)
            {
                //newLevel.LargeUFOTotal = (int)(newLevel.LargeUFOTotal * 0.5);
                //newLevel.UFOTotal *= 2;

                newLevel.UFOShotTimer *= 0.9f;
                newLevel.SmallUFOShotTimer *= 0.85f;
                newLevel.UFORange -= 10;
                newLevel.UFOMoveExtra += 5;

                newLevel.LargeAsteroidSpawnChance = 15;
                newLevel.SmallAsteroidSpawnChance = 22.5f;
                newLevel.LargeUFOSpawnChance = 15;
                newLevel.SmallUFOSpawnChance = 7.5f;
            }
            else if (newLevel.LevelNum>=5 && newLevel.LevelNum<9)
            {
                //newLevel.LargeUFOTotal = 0;
                //newLevel.UFOTotal = (int)(newLevel.UFOTotal * 1.75);

                newLevel.SmallUFOShotTimer *= 0.75f;
                newLevel.UFORange -= 20;
                newLevel.UFOMoveExtra += 10;

                newLevel.LargeAsteroidSpawnChance += 7.5f;
                newLevel.SmallAsteroidSpawnChance += 7.5f;
                newLevel.LargeUFOSpawnChance += 5;
                newLevel.SmallUFOSpawnChance += 10;
            }
            else if (newLevel.LevelNum>=9)
            {
                //newLevel.LargeUFOTotal = 0;
                //newLevel.UFOTotal = (int)(newLevel.UFOTotal * 2.5);

                newLevel.SmallUFOShotTimer *= 0.5f;
                newLevel.UFORange = 10;
                newLevel.UFOMoveExtra = 50;

                newLevel.LargeAsteroidSpawnChance += 7.5f;
                newLevel.SmallAsteroidSpawnChance += 10;
                newLevel.LargeUFOSpawnChance -= 5;
                newLevel.SmallUFOSpawnChance += 7.5f;

                //add some (+=(x+(level.Num*x)) eventually
            }

            newLevel.LevelSpawnTimer = new TimeSpan(0, 0, 0, 30, 0);


            newLevel.UFOShotSpeed *= 1.25f;

            //newLevel.LargeAsteroidTotal = (int)(newLevel.LargeAsteroidTotal * 1.5);
            //newLevel.SmallAsteroidTotal = (int)(newLevel.SmallAsteroidTotal * 1.75);

            //newLevel.GlobalSpawnTimer = newLevel.GlobalSpawnTimer * 0.9;
            //newLevel.reserveSpawnTimer = newLevel.GlobalSpawnTimer;


            //Old Spawn Info

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

            LevelSpawnTimer -= Game1.gameTime.ElapsedGameTime;
            SpawnOpportunityTimer -= Game1.gameTime.ElapsedGameTime;

            armorMax = (float)Math.Floor((float)LevelNum / 3);
            if (armorMax > 6) { armorMax = 6; }

            if (armorMax > 2) { armorMin = armorMax - 3; }

            float UFOArmor = (float)Math.Floor(GlobalArmorValue / 2);


            if (SpawnOpportunityTimer<=TimeSpan.Zero && LevelSpawnTimer>TimeSpan.Zero)
            {
                if (rand.Next(1, 101)<=LargeAsteroidSpawnChance)
                {
                    GlobalArmorValue = rand.Next((int)armorMin, (int)armorMax + 1);
                    if (GlobalArmorValue==0) { noArmorChain++; }
                    if (noArmorChain==3) { GlobalArmorValue = rand.Next(1, (int)armorMax + 1); noArmorChain = 0; }

                    roids.Add(Enemy.NaturalSpawn(Enemy.Type.Asteroid, playSpace, largevelocity, largeAss, Game1.Size.LeChonk, TimeSpan.Zero, GlobalArmorValue));
                }
                if (rand.Next(1, 101)<=SmallAsteroidSpawnChance)
                {
                    roids.Add(Enemy.NaturalSpawn(Enemy.Type.Asteroid, playSpace, smallvelocity, smallAss, Game1.Size.Normal, TimeSpan.Zero, 0));
                }
                if (rand.Next(1, 101)<=LargeUFOSpawnChance)
                {
                    GlobalArmorValue = rand.Next((int)armorMin, (int)armorMax + 1);
                    if (GlobalArmorValue==0) { noArmorChain++; }
                    if (noArmorChain==3) { GlobalArmorValue = rand.Next(1, (int)armorMax + 1); noArmorChain = 0; }
                    UFOArmor = (float)Math.Floor(GlobalArmorValue / 2);

                    flys.Add(Enemy.NaturalSpawn(Enemy.Type.UFO, playSpace, smallvelocity, largeUFO, Game1.Size.Normal, UFOShotTimer, UFOArmor));
                }
                if (rand.Next(1, 101)<=SmallUFOSpawnChance)
                {
                    GlobalArmorValue = rand.Next((int)armorMin, (int)armorMax + 1);
                    if (GlobalArmorValue==0) { noArmorChain++; }
                    if (noArmorChain==3) { GlobalArmorValue = rand.Next(1, (int)armorMax + 1); noArmorChain = 0; }
                    UFOArmor = (float)Math.Floor(GlobalArmorValue / 2);

                    flys.Add(Enemy.NaturalSpawn(Enemy.Type.UFO, playSpace, tinyvelocity, smallUFO, Game1.Size.Baby, SmallUFOShotTimer, UFOArmor));
                }

                SpawnOpportunityTimer = reserveSpawnOppTimer;
            }

            if (LevelSpawnTimer<=TimeSpan.Zero)
                { Finished = true; }


        }
            /* Old Spawn Archive

            GlobalSpawnTimer -= Game1.gameTime.ElapsedGameTime;
            smallTimer -= Game1.gameTime.ElapsedGameTime;
            largeUTimer -= Game1.gameTime.ElapsedGameTime;
            smallUTimer -= Game1.gameTime.ElapsedGameTime;

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
                flys.Add(Enemy.NaturalSpawn(Enemy.Type.UFO, playSpace, smallvelocity, largeUFO, Game1.Size.Normal, UFOShotTimer, UFOArmor));
                LargeUSpawned++;
                largeUTimer = reserveSpawnTimer * 0.8;

                if (LargeUSpawned >= LargeUFOTotal) { LargeSauceMax = true; }
            }

            if (smallUTimer<=TimeSpan.Zero && !SmallSauceMax)
            {
                //whatSpawns = Game1.WhatSpawned.SmallSauce;
                flys.Add(Enemy.NaturalSpawn(Enemy.Type.UFO, playSpace, tinyvelocity, smallUFO, Game1.Size.Baby, SmallUFOShotTimer, UFOArmor));
                SmallUSpawned++;
                smallUTimer = reserveSpawnTimer * 1.5;

                if (SmallUSpawned>=UFOTotal) { SmallSauceMax = true; }
            }

            if (SmallSauceMax && LargeSauceMax && SmallAssMax && LargeAssMax)
                { Finished = true; }

            */
    }
}
