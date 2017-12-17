using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Common
{
    public enum OperationCode:byte
    {
        EnterServer,
        PlayerMove,
        PlayerJoinToGame,
        PlayerLeftGame,
        GetPlayersList,
        GetCurrentPlayerInfo,
        GetMobsList,
    }
}
