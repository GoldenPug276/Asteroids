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
            Level newLevel = new Level(lLev.SmallAsteroidTotal, lLev.LargeAsteroidTotal, lLev.LargeUFOTotal, lLev.UFOTotal, lLev.GlobalSpawnTimer, lLev.UFORange, lLev.UFOShotSpeed, lLev.LevelNum);

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
        public void Update(TimeSpan gameTime, Rectangle playSpace, List<Asteroid> roids, List<UFO> flys)
        {
            GlobalSpawnTimer -= gameTime;
            smallTimer -= gameTime;
            largeUTimer -= gameTime;
            smallUTimer -= gameTime;

            if (GlobalSpawnTimer<=TimeSpan.Zero && !LargeAssMax)
            {
                //whatSpawns = Game1.WhatSpawned.LargeAss;
                roids.Add(Asteroid.NaturalSpawn(playSpace, largevelocity, largeAss, Game1.Size.LeChonk));
                GlobalSpawnTimer = reserveSpawnTimer;
                LargeSpawned++;
            }
            else if (LargeSpawned>=LargeAsteroidTotal)
            {
                LargeAssMax = true;
            }

            if (smallTimer<=TimeSpan.Zero && !SmallAssMax)
            {
                //whatSpawns = Game1.WhatSpawned.SmallAss;
                roids.Add(Asteroid.NaturalSpawn(playSpace, smallvelocity, smallAss, Game1.Size.Normal));
                smallTimer = reserveSpawnTimer * 0.5;
                SmallSpawned++;
            }
            else if (SmallSpawned>=SmallAsteroidTotal)
            {
                SmallAssMax = true;
            }

            if (largeUTimer<=TimeSpan.Zero && !LargeSauceMax)
            {
                //whatSpawns = Game1.WhatSpawned.LargeSauce;
                flys.Add(UFO.NaturalSpawn(playSpace, largevelocity, largeUFO, Game1.Size.Normal, UFOShotTimer));
                largeUTimer = reserveSpawnTimer * 0.80;
                LargeUSpawned++;
            }
            else if (LargeUSpawned >= LargeUFOTotal)
            {
                LargeSauceMax = true;
            }

            if (smallUTimer<=TimeSpan.Zero && !SmallSauceMax)
            {
                //whatSpawns = Game1.WhatSpawned.SmallSauce;
                flys.Add(UFO.NaturalSpawn(playSpace, tinyvelocity, smallUFO, Game1.Size.Baby, SmallUFOShotTimer));
                smallUTimer = reserveSpawnTimer * 1.50;
                SmallUSpawned++;
            }
            else if (SmallUSpawned >= UFOTotal)
            {
                SmallSauceMax = true;
            }

            if (SmallSauceMax && LargeSauceMax && SmallAssMax && LargeAssMax)
            {
                Finished = true;
            }
        }
    }
}
