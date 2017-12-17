using System.Collections.Generic;
using GameServer.Common;

namespace GameServer.Territory
{
    public abstract class OccupiedTerritory
    {
        public static List<OccupiedTerritory> OccupiedTerrotpryes = new List<OccupiedTerritory>();

        static OccupiedTerritory()
        {
            OccupiedTerrotpryes.Add(new OccuipedShape(new Vector3Net(500, 0, 0, Vector3Net.RotationNet.Zero), new Vector3Net(700, 0, 0, Vector3Net.RotationNet.Zero), new Vector3Net(500, 0, 200, Vector3Net.RotationNet.Zero)));
        }

        public abstract bool IsContain(Vector3Net position);
    }
}
