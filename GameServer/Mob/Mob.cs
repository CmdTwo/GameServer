using System;
using GameServer.Common;

namespace GameServer.Mob
{
    public class Mob
    {
        private int _Coins;
        public int Coins
        {
            get
            { return _Coins; }
            set
            { if (value <= 0) { Dead = true; _Coins = 0; } else _Coins = value; }
        }
        public int StartCoins;
        public int Type { get; private set; }
        public Vector3Net Position { get; private set; }
        public int MobID { get; private set; }
        public bool Dead;
       
        public Mob(Vector3Net position, int type, int mobID)
        {
            Position = position;
            Type = type;
            MobID = mobID;
            StartCoins = 0;
            Coins = 100;
            Dead = false;
        }

        //public bool IsDead { get { return Coins <= 0 ? true : false; } }
    }
}
