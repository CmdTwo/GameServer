using ExitGames.Threading;
using System.Collections.Generic;
using System.Threading;

namespace GameServer
{
    public class World
    {
        private readonly ReaderWriterLockSlim readerWriterLockSlim;
        public List<UnityClient> Clients { get; private set; }
        public static readonly World Instance = new World();
        public World()
        {
            Clients = new List<UnityClient>();
            readerWriterLockSlim = new ReaderWriterLockSlim();
        }

        public bool IsContain(string playerName)
        {
            using (ReadLock.TryEnter(this.readerWriterLockSlim, 1000))
            {
                return Clients.Exists(x => x.PlayerName == playerName);
            }
        }

        public void AddClient(UnityClient client)
        {
            using (WriteLock.TryEnter(this.readerWriterLockSlim, 1000))
            {
                Clients.Add(client);
            }
        }

        public void RemoveClient(UnityClient client)
        {
            using (WriteLock.TryEnter(this.readerWriterLockSlim, 1000))
            {
                Clients.Remove(client);
            }
        }

        ~World()
        {
            //Clients.Clear();
            readerWriterLockSlim.Dispose();
        }

    }
}
