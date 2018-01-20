using UnityEngine;
using System.Collections;

namespace OcTreeProjector
{
    public interface ITrigger
    {
        bool IsDetected(Bounds bounds);

    }
}