using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Common;

namespace GameServer.Territory
{
    public class OccuipedShape : OccupiedTerritory
    {
        private Vector3Net Point1;
        private Vector3Net Point2;
        private Vector3Net Point3;

        public OccuipedShape(Vector3Net point1, Vector3Net point2, Vector3Net point3)
        {
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;
        }

        public override bool IsContain(Vector3Net position)
        {
            if ((position.X > Point1.X && position.X < Point2.X) && (position.Z > Point1.Z && position.Z < Point3.Z))
                return true;
            else
                return false;
        }
    }
}
