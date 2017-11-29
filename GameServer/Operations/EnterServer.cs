using Photon.SocketServer.Rpc;
using Photon.SocketServer;
using GameServer.Common;

namespace GameServer.Operations
{
    public class EnterServer : BaseOperation
    {
        public EnterServer(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.PlayerName)]
        public string PlayerName { get; set; }
        [DataMember(Code = (byte)ParameterCode.PlayerID)]
        public int PlayerID { get; set; }
    }
}
