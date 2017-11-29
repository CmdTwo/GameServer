using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Common
{
    public enum ErrorCode:byte
    {
        Ok,
        InvaildParameters,
        PlayerIsExist,
        RequestNotImplemented
    }
}
