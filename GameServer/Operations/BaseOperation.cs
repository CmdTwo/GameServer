using Photon.SocketServer.Rpc;
using Photon.SocketServer;
using GameServer.Common;

namespace GameServer.Operations
{
    public class BaseOperation : Operation
    {
        public BaseOperation(IRpcProtocol protocol, OperationRequest request) 
            : base(protocol, request)
        { }

        public virtual OperationResponse GetResponse(ErrorCode errorCode, string debugMessage = "")
        {
            OperationResponse response = new OperationResponse(OperationRequest.OperationCode);
            response.ReturnCode = (short)errorCode;
            response.DebugMessage = debugMessage;
            return response;
        }
    }
}
