using Photon.SocketServer;
using System.Linq;
using System.Collections.Generic;
using PhotonHostRuntimeInterfaces;
using ExitGames.Logging;
using GameServer.Common;
using GameServer.Mob;

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

            World.Instance.RemoveClient(PlayerID);
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
                case (byte)OperationCode.GetCurrentPlayerInfo:
                    {
                        GetCurrentPlayerInfo(sendParameters);
                        break;
                    }
                case (byte)OperationCode.GetMobsList:
                    {
                        GetMobsListHandler(sendParameters);
                        break;
                    }
                case (byte)OperationCode.MobAttackedByPlayer:
                    {
                        MobAttackedByPlayerHandler(operationRequest, sendParameters);
                        break;
                    }
                case (byte)OperationCode.UpdateMobInfo:
                    UpdateMobInfoHandler(operationRequest, sendParameters);
                    break;
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
            PlayerID = World.Instance.AddClient(this);

            OperationResponse response = new OperationResponse(operationRequest.OperationCode);
            response.ReturnCode = (short)ErrorCode.Ok;
            SendOperationResponse(response, sendParameters);

            //OperationResponse response = new OperationResponse(operationRequest.OperationCode);

            //SendOperationResponse(response, sendParameters);

            Log.Info("Player " + PlayerName + "(id:" + PlayerID + ") enter to server!");
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
                            { (byte)ParameterCode.PlayerID, PlayerID }
                        };
            eventData.SendTo(World.Instance.GetClientsList(PlayerID), sendParameters);

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
                            { (byte)ParameterCode.PlayerName, PlayerName },
                            { (byte)ParameterCode.PlayerID, PlayerID }
                        };
            eventData.SendTo(World.Instance.GetClientsList(PlayerID), sendParameters);

            Log.Info("Player " + PlayerName + " join to game");
        }
        private void PlayerLeftGameHandler(SendParameters sendParameters)
        {
            EventData eventData = new EventData((byte)EventCode.PlayerLeftGame);
            eventData.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.PlayerID, PlayerID } };
            eventData.SendTo(World.Instance.GetClientsList(), sendParameters);

            Log.Info("Player " + PlayerName + "(id:" + PlayerID + ") left game");
        }
        private void GetPlayersListHandler(SendParameters sendParameters)
        {
            OperationResponse response = new OperationResponse((byte)OperationCode.GetPlayersList);

            List<UnityClient> players = World.Instance.GetClientsList(PlayerID);
            Dictionary<int, object[]> dPlayers = players.ToDictionary(x => x.PlayerID, x => new object[] { x.PlayerName, x.Position.X, x.Position.Y, x.Position.Z, x.Position.Rotation.X, x.Position.Rotation.Y, x.Position.Rotation.Z, x.Position.Rotation.W });

            response.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.PlayersList, dPlayers } };
            SendOperationResponse(response, sendParameters);
        }
        private void GetCurrentPlayerInfo(SendParameters sendParameters)
        {
            OperationResponse response = new OperationResponse((byte)OperationCode.GetCurrentPlayerInfo);

            response.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.PlayerID, PlayerID } };
            SendOperationResponse(response, sendParameters);
        }
        private void GetMobsListHandler(SendParameters sendParameters)
        {
            OperationResponse response = new OperationResponse((byte)OperationCode.GetMobsList);

            List<Mob.Mob> mobs = World.Instance.GetMobsList();
            Dictionary<int, object[]> dMobs = mobs.ToDictionary(x => x.MobID, x => new object[] { x.Position.X, x.Position.Y, x.Position.Z, x.Position.Rotation.X, x.Position.Rotation.Y, x.Position.Rotation.Z, x.Position.Rotation.W, x.Type });

            response.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.MobsList, dMobs } };
            SendOperationResponse(response, sendParameters);
        }
        private void MobAttackedByPlayerHandler(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Operations.MobAttackedByPlayer mobAttackedRequest = new Operations.MobAttackedByPlayer(Protocol, operationRequest);

            if (!mobAttackedRequest.IsValid)
            {
                SendOperationResponse(mobAttackedRequest.GetResponse(ErrorCode.InvaildParameters), sendParameters);
                return;
            }

            //if (World.Instance.Mobs.ContainsKey(mobAttackedRequest.MobID))
            //{

            //}
            Log.Debug("Mob#" + mobAttackedRequest.MobID + " attaked by player#" + PlayerID + " (-" + mobAttackedRequest.Damage + ")");
            Mob.Mob mob = World.Instance.Mobs[mobAttackedRequest.MobID];
            mob.Coins -= mobAttackedRequest.Damage;

            //Log.Debug("Mob coins:" + mob.Coins + " | " + mob.StartCoins + " | isDead? " + mob.IsDead);

            if (mob.IsDead)
            {
                MobDiedEvent(mob.MobID);
                OperationResponse response = new OperationResponse((byte)OperationCode.MobDefeatedByPlayer);
                response.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.MobID, mob.MobID }, { (byte)ParameterCode.Coins, mob.StartCoins } };
                SendOperationResponse(response, sendParameters);
            }
            else
            {
                EventData eventData = new EventData((byte)EventCode.MobAttackedByPlayer);
                eventData.Parameters = new Dictionary<byte, object> {
                { (byte)ParameterCode.MobID, mobAttackedRequest.MobID },
                { (byte)ParameterCode.Damage, mobAttackedRequest.Damage },
                { (byte)ParameterCode.PlayerID, PlayerID }
                        };
                eventData.SendTo(World.Instance.GetClientsList(), sendParameters);
            }
        }
        public void UpdateMobInfoHandler(OperationRequest operationRequest, SendParameters sendParameters)
        {
            Operations.UpdateMobInfo updateMobInfoRequest = new Operations.UpdateMobInfo(Protocol, operationRequest);

            if (!updateMobInfoRequest.IsValid)
            {
                SendOperationResponse(updateMobInfoRequest.GetResponse(ErrorCode.InvaildParameters), sendParameters);
                return;
            }

            World.Instance.Mobs[updateMobInfoRequest.MobID].StartCoins = updateMobInfoRequest.Coins;
            World.Instance.Mobs[updateMobInfoRequest.MobID].Coins = updateMobInfoRequest.Coins;

           // Log.Debug("Add coins: " + updateMobInfoRequest.Coins + " | Current: " + World.Instance.Mobs[updateMobInfoRequest.MobID].StartCoins);
        }
        #endregion

        #region Server Events
        public void MobDiedEvent(int MobID)
        {
            EventData eventData = new EventData((byte)EventCode.MobDied);
            eventData.Parameters = new Dictionary<byte, object> {
                { (byte)ParameterCode.MobID, MobID } };
            eventData.SendTo(World.Instance.GetClientsList(), new SendParameters() { Unreliable = true });
        }
        #endregion
    }
}
