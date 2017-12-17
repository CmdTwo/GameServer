using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using Photon.SocketServer;
using System.IO;

namespace GameServer
{
    public class Server : ApplicationBase
    {
        public readonly ILogger Log = LogManager.GetCurrentClassLogger();
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new UnityClient(initRequest);
        }

        protected override void Setup()
        {
            var file = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);
            }
            Log.Info("Start server");
            World.Instance.SpawnMobs();
        }

        protected override void TearDown()
        {
            Log.Info("Server was stoped");
        }
    }
}
