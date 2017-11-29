﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Common
{
    public enum EventCode:byte
    {
        PlayerMove,
        PlayerJoinToGame,
        PlayerLeftGame,
    }
}
