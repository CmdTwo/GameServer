using Photon.SocketServer.Rpc;
using Photon.SocketServer;
using GameServer.Common;

namespace GameServer.Operations
{
    public class PlayerMove : BaseOperation
    {
        public PlayerMove(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.PlayerPosX)]
        public float X { get; set; }

        [DataMember(Code = (byte)ParameterCode.PlayerPosY)]
        public float Y { get; set; }

        [DataMember(Code = (byte)ParameterCode.PlayerPosZ)]
        public float Z { get; set; }


        [DataMember(Code = (byte)ParameterCode.PlayerRotX)]
        public float RotX { get; set; }

        [DataMember(Code = (byte)ParameterCode.PlayerRotY)]
        public float RotY { get; set; }

        [DataMember(Code = (byte)ParameterCode.PlayerRotZ)]
        public float RotZ { get; set; }

        [DataMember(Code = (byte)ParameterCode.PlayerRotW)]
        public float RotW { get; set; }
    }
}
