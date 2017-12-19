using Photon.SocketServer.Rpc;
using Photon.SocketServer;
using GameServer.Common;


namespace GameServer.Operations
{
    class MobAttackedByPlayer : BaseOperation
    {
        public MobAttackedByPlayer(IRpcProtocol protocol, OperationRequest request)
            : base(protocol, request)
        {
        }

        [DataMember(Code = (byte)ParameterCode.MobID)]
        public int MobID { get; set; }
        [DataMember(Code = (byte)ParameterCode.Damage)]
        public int Damage { get; set; }
    }
}
