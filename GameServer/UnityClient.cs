using Photon.SocketServer;
using System.Linq;
using System.Collections.Generic;
using PhotonHostRuntimeInterfaces;
using ExitGames.Logging;
using GameServer.Common;

namespace GameServer
{
    public class UnityClient : ClientPeer
    {
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();
        public Vector3Net Position { get; private set; }
        public string PlayerName { get; private set; }
        public int PlayerID { get; private set; }
        public UnityClient(InitRequest initRequest) : base(initRequest)
        {
            Position = new Vector3Net(22, 1, 22, new Vector3Net.RotationNet(0, 0, 0, 0));
            PlayerName = "";
            PlayerID = 0;
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            Log.Info(this.PlayerName + " try to disconnect...");

            World.Instance.RemoveClient(this);
            PlayerLeftGameHandler(new SendParameters() { Unreliable = true });
        }
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                case (byte)OperationCode.EnterServer:
                    {
                        EnterServerHandler(operationRequest, sendParameters);
                        break;
                    }
                case (byte)OperationCode.PlayerMove:
                    {
                        PlayerMoveHandler(operationRequest, sendParameters);
                        break;
                    }
                case (byte)OperationCode.PlayerJoinToGame:
                    {
                        PlayerJoinToGameHandler(sendParameters);
                        break;
                    }
                case (byte)OperationCode.PlayerLeftGame:
                    {
                        PlayerLeftGameHandler(sendParameters);
                        break;
                    }
                case (byte)OperationCode.GetPlayersList:
                    {
                        GetPlayersListHandler(sendParameters);
                        break;
                    }
                default:
                    {
                        Log.Debug("Unknow OperationRequest received: " + operationRequest.OperationCode);
                        break;
                    }
            }
        }

        #region Operation Request Handlers

        private void EnterServerHandler(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Operations.EnterServer enterGameRequest = new Operations.EnterServer(Protocol, operationRequest);

            if (!enterGameRequest.IsValid)
            {
                SendOperationResponse(enterGameRequest.GetResponse(ErrorCode.InvaildParameters), sendParameters);
                return;
            }

            PlayerName = enterGameRequest.PlayerName;
            PlayerID = enterGameRequest.PlayerID;

            if (World.Instance.IsContain(PlayerName))
            {
                SendOperationResponse(enterGameRequest.GetResponse(ErrorCode.PlayerIsExist), sendParameters);
                return;
            }

            World.Instance.AddClient(this);

            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            response.ReturnCode = (short)ErrorCode.Ok;
            SendOperationResponse(response, sendParameters);

            Log.Info("Player " + PlayerName + " enter to server!");
        }
        private void PlayerMoveHandler(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Operations.PlayerMove playerMoveRequest = new Operations.PlayerMove(Protocol, operationRequest);

            if (!playerMoveRequest.IsValid)
            {
                SendOperationResponse(playerMoveRequest.GetResponse(ErrorCode.InvaildParameters), sendParameters);
                return;
            }

            Position = new Vector3Net(playerMoveRequest.X, playerMoveRequest.Y, playerMoveRequest.Z, new Vector3Net.RotationNet(playerMoveRequest.RotX, playerMoveRequest.RotY, playerMoveRequest.RotZ, playerMoveRequest.RotW));

            EventData eventData = new EventData((byte)EventCode.PlayerMove);
            eventData.Parameters = new Dictionary<byte, object> {
                            { (byte)ParameterCode.PlayerPosX, Position.X },
                            { (byte)ParameterCode.PlayerPosY, Position.Y },
                            { (byte)ParameterCode.PlayerPosZ, Position.Z },
                            { (byte)ParameterCode.PlayerRotX, Position.Rotation.X },
                            { (byte)ParameterCode.PlayerRotY, Position.Rotation.Y },
                            { (byte)ParameterCode.PlayerRotZ, Position.Rotation.Z },
                            { (byte)ParameterCode.PlayerRotW, Position.Rotation.W },
                            { (byte)ParameterCode.PlayerName, PlayerName }
                        };
            eventData.SendTo(World.Instance.Clients.Except(new List<UnityClient> { this }), sendParameters);

            //Log.Info("Player " + PlayerName + " move | x:" + Position.X + " y:" + Position.Y + " z:" + Position.Z);
        }
        private void PlayerJoinToGameHandler(SendParameters sendParameters)
        {
            EventData eventData = new EventData((byte)EventCode.PlayerJoinToGame);

            eventData.Parameters = new Dictionary<byte, object> {
                            { (byte)ParameterCode.PlayerPosX, Position.X },
                            { (byte)ParameterCode.PlayerPosY, Position.Y },
                            { (byte)ParameterCode.PlayerPosZ, Position.Z },
                            { (byte)ParameterCode.PlayerRotX, Position.Rotation.X },
                            { (byte)ParameterCode.PlayerRotY, Position.Rotation.Y },
                            { (byte)ParameterCode.PlayerRotZ, Position.Rotation.Z },
                            { (byte)ParameterCode.PlayerRotW, Position.Rotation.W },
                            { (byte)ParameterCode.PlayerName, PlayerName }
                        };
            eventData.SendTo(World.Instance.Clients.Except(new List<UnityClient> { this }), sendParameters);

            Log.Info("Player " + PlayerName + " join to game");
        }
        private void PlayerLeftGameHandler(SendParameters sendParameters)
        {
            EventData eventData = new EventData((byte)EventCode.PlayerLeftGame);
            eventData.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.PlayerName, PlayerName } };
            eventData.SendTo(World.Instance.Clients, sendParameters);

            Log.Info("Player " + PlayerName + " left game");
        }
        private void GetPlayersListHandler(SendParameters sendParameters)
        {
            OperationResponse response = new OperationResponse((byte)OperationCode.GetPlayersList);

            List<UnityClient> players = World.Instance.Clients.Except(new List<UnityClient> { this }).ToList();
            Dictionary<string, object[]> dPlayers = players.ToDictionary(x => x.PlayerName, x => new object[] { x.Position.X, x.Position.Y, x.Position.Z, x.Position.Rotation.X, x.Position.Rotation.Y, x.Position.Rotation.Z, x.Position.Rotation.W });

            response.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.PlayersList, dPlayers } };
            SendOperationResponse(response, sendParameters);
        }

        #endregion
        
    }
}
