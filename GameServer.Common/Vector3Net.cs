using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Common
{
    public struct Vector3Net
    {
        public struct RotationNet
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float W { get; set; }

            public RotationNet(float x, float y, float z, float w)
            {
                X = x;
                Y = y;
                Z = z;
                W = w;
            }
            public static RotationNet Zero = new RotationNet(0, 0, 0, 0);
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public RotationNet Rotation;

        public Vector3Net(float x, float y, float z, RotationNet rotation)
        {
            X = x;
            Y = y;
            Z = z;
            Rotation = rotation;
        }
    }
}
