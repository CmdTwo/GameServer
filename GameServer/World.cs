using ExitGames.Threading;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace GameServer
{
    public class World
    {
        private readonly ReaderWriterLockSlim readerWriterLockSlim;
        private Stack<int> AvailableIDs;
        public Dictionary<int, UnityClient> Clients { get; private set; }
        public static readonly World Instance = new World();
        public World()
        {
            Clients = new Dictionary<int, UnityClient>();
            readerWriterLockSlim = new ReaderWriterLockSlim();
            AvailableIDs = new Stack<int>();
        }

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

        ~World()
        {
            //Clients.Clear();
            readerWriterLockSlim.Dispose();
        }

    }
}
