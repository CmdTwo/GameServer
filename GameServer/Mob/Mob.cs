﻿using System;
using GameServer.Common;

namespace GameServer.Mob
{
    public class Mob
    {
        public int Type { get; private set; }
        public Vector3Net Position { get; private set; }
        public int MobID { get; private set; }
        public int Coins { get; set; }
        public int StartCoins;

        public Mob(Vector3Net position, int type, int mobID)
        {
            Position = position;
            Type = type;
            MobID = mobID;
            StartCoins = 0;
            Coins = 0;
        }

        public bool IsDead { get { return Coins <= 0 ? true : false; } }
    }
}
