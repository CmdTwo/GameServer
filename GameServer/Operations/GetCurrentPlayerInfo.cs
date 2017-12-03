using Photon.SocketServer.Rpc;
using Photon.SocketServer;
using GameServer.Common;

namespace GameServer.Operations
{

    class GetCurrentPlayerInfo : BaseOperation
    {
        public GetCurrentPlayerInfo(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.PlayerID)]
        public int PlayerID { get; set; }
    }
}
