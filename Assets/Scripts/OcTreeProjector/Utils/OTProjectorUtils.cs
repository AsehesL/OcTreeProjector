using UnityEngine;
using System.Collections;

namespace OcTreeProjector
{
    internal static class OTProjectorUtils
    {
        public static bool IsOutOfBounds(Vector3 pjpos)
        {
            if (pjpos.x < -1 || pjpos.x > 1 || pjpos.y < -1 || pjpos.y > 1 || pjpos.z < -1 || pjpos.z > 1)
                return true;
            return false;
        }

        public static bool IsOutOfCamera(Vector3 worldPos, Camera camera)
        {
            Vector3 pos = camera.worldToCameraMatrix.MultiplyPoint(worldPos);
            pos = camera.projectionMatrix.MultiplyPoint(pos);
            return IsOutOfBounds(pos);
        }

        public static Vector3 GetMaxVector(Vector3 v1, Vector3 v2)
        {
            Vector3 p = v1;
            if (v2.x > p.x)
                p.x = v2.x;
            if (v2.y > p.y)
                p.y = v2.y;
            if (v2.z > p.z)
                p.z = v2.z;
            return p;
        }

        public static Vector3 GetMinVector(Vector3 v1, Vector3 v2)
        {
            Vector3 p = v1;
            if (v2.x < p.x)
                p.x = v2.x;
            if (v2.y < p.y)
                p.y = v2.y;
            if (v2.z < p.z)
                p.z = v2.z;
            return p;
        }

        public static void DrawProjectorGizmos(this OTProjector projector)
        {
            Matrix4x4 mtx = default(Matrix4x4);
            if (projector.orthographic)
            {
                mtx = Matrix4x4.Ortho(-projector.orthographicSize * projector.aspect, projector.orthographicSize * projector.aspect, projector.orthographicSize, -projector.orthographicSize,
                    -projector.near, -projector.far).inverse;
            }
            else
            {
                mtx = Matrix4x4.Perspective(projector.fieldOfView, projector.aspect, -projector.near, -projector.far).inverse;
            }
            mtx = projector.transform.localToWorldMatrix * mtx;
            Vector3 p1 = new Vector3(-1, -1, -1);
            Vector3 p2 = new Vector3(-1, 1, -1);
            Vector3 p3 = new Vector3(1, 1, -1);
            Vector3 p4 = new Vector3(1, -1, -1);
            Vector3 p5 = new Vector3(-1, -1, 1);
            Vector3 p6 = new Vector3(-1, 1, 1);
            Vector3 p7 = new Vector3(1, 1, 1);
            Vector3 p8 = new Vector3(1, -1, 1);
            p1 = mtx.MultiplyPoint(p1);
            p2 = mtx.MultiplyPoint(p2);
            p3 = mtx.MultiplyPoint(p3);
            p4 = mtx.MultiplyPoint(p4);
            p5 = mtx.MultiplyPoint(p5);
            p6 = mtx.MultiplyPoint(p6);
            p7 = mtx.MultiplyPoint(p7);
            p8 = mtx.MultiplyPoint(p8);

            Gizmos.color = new Color(0.8f, 0.8f, 0.8f, 0.6f);

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);

            Gizmos.DrawLine(p5, p6);
            Gizmos.DrawLine(p6, p7);
            Gizmos.DrawLine(p7, p8);
            Gizmos.DrawLine(p8, p5);

            Gizmos.DrawLine(p1, p5);
            Gizmos.DrawLine(p2, p6);
            Gizmos.DrawLine(p3, p7);
            Gizmos.DrawLine(p4, p8);
        }
    }
}