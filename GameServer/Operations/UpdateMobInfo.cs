using Photon.SocketServer.Rpc;
using Photon.SocketServer;
using GameServer.Common;

namespace GameServer.Operations
{
    class UpdateMobInfo : BaseOperation
    {
        public UpdateMobInfo(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.MobID)]
        public int MobID { get; set; }
        [DataMember(Code = (byte)ParameterCode.Coins)]
        public int Coins { get; set; }
    }
}
