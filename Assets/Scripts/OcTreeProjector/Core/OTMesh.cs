using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace OcTreeProjector
{
    public class OTMesh
    {

        public readonly List<Vector3> vertexCache = new List<Vector3>();
        public readonly List<int> indexesCache = new List<int>();
        public int currentIndex;
    }
}