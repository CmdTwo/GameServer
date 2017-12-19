using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Common
{
    public enum ParameterCode:byte
    {
        PlayerName,
        PlayerID,
        PlayerPosX,
        PlayerPosY,
        PlayerPosZ,
        PlayerRotX,
        PlayerRotY,
        PlayerRotZ,
        PlayerRotW,
        PlayersList,
        MobsList,
        MobID,
        MobType,
        Damage,
        Coins,
    }
}
