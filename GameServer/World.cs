using ExitGames.Threading;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System;
using GameServer.Common;
using GameServer.Territory;
using GameServer.Mob;
using ExitGames.Logging;

namespace GameServer
{
    public class World
    {
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly ReaderWriterLockSlim readerWriterLockSlim;

        private const int SpawnOnMapX_Min = 20;
        private const int SpawnOnMapZ_Min = 20;

        private const int SpawnOnMapX_Max = 50;
        private const int SpawnOnMapY = 1;
        private const int SpawnOnMapZ_Max = 50;

        private Stack<int> AvailableIDs;
        public Dictionary<int, UnityClient> Clients { get; private set; }
        public Dictionary<int, Mob.Mob> Mobs { get; private set; }
        public static readonly World Instance = new World();
        public World()
        {
            Clients = new Dictionary<int, UnityClient>();
            Mobs = new Dictionary<int, Mob.Mob>();
            readerWriterLockSlim = new ReaderWriterLockSlim();
            AvailableIDs = new Stack<int>();
        }

        #region Client
        public bool IsContain(int ID)
        {
            using (ReadLock.TryEnter(this.readerWriterLockSlim, 1000))
            {
                return Clients.ContainsKey(ID);
            }
        }

        public void AddClient(UnityClient client, int ID)
        {
            using (WriteLock.TryEnter(this.readerWriterLockSlim, 1000))
            {
                Clients.Add(ID, client);
            }
        }

        public int AddClient(UnityClient client)
        {
            using (WriteLock.TryEnter(this.readerWriterLockSlim, 1000))
            {
                int ID = (AvailableIDs.Count == 0) ? Clients.Count + 1 : AvailableIDs.Pop();
                Clients.Add(ID, client);
                return ID;
            }
        }

        public void RemoveClient(int ID)
        {
            using (WriteLock.TryEnter(this.readerWriterLockSlim, 1000))
            {
                Clients.Remove(ID);
                AvailableIDs.Push(ID);
            }
        }

        public List<UnityClient> GetClientsList()
        {
            return Clients.Select(x => x.Value).ToList();
        }

        public List<UnityClient> GetClientsList(int exceptID)
        {
            return GetClientsList().Except(new List<UnityClient> { Clients[exceptID] }).ToList();
        }
        #endregion

        #region Mob
        public void SpawnMobs()
        {
            Log.Debug("Star spawning mobs");
            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                Vector3Net position = new Vector3Net(rand.Next(SpawnOnMapX_Min, SpawnOnMapX_Max), SpawnOnMapY, rand.Next(SpawnOnMapZ_Min, SpawnOnMapZ_Max), Vector3Net.RotationNet.Zero);
               // Vector3Net position = new Vector3Net(20, MapY, 20, Vector3Net.RotationNet.Zero);
                foreach (OccupiedTerritory territory in OccupiedTerritory.OccupiedTerrotpryes)
                {
                    if (!territory.IsContain(position))
                    {
                        Mob.Mob mob = new Mob.Mob(position, rand.Next(1, 4), Mobs.Count + 1);
                        Mobs.Add(Mobs.Count + 1, mob);
                        Log.Debug("Mob: " + Mobs.Count + " | created on | " + mob.Position.X + " : " + mob.Position.Z);
                    }
                    else
                    {
                        position = new Vector3Net(rand.Next(SpawnOnMapX_Min, SpawnOnMapX_Max), SpawnOnMapY, rand.Next(SpawnOnMapZ_Min, SpawnOnMapZ_Max), Vector3Net.RotationNet.Zero);
                        //position = new Vector3Net(20, MapY, 20, Vector3Net.RotationNet.Zero);
                    }
                }
            }
        }
        public List<Mob.Mob> GetMobsList()
        {
            return Mobs.Select(x => x.Value).ToList();
        }
        #endregion

        ~World()
        {
            Clients.Clear();
            readerWriterLockSlim.Dispose();
        }

    }
}
