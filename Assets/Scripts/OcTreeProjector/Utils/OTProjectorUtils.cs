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

        public static Bounds OrthoBounds(Vector3 position, Quaternion rotation, float size, float aspect, float near, float far)
        {
            Vector3 p1 = position + rotation * new Vector3(-size * aspect, -size, near);
            Vector3 p2 = position + rotation * new Vector3(size * aspect, -size, near);
            Vector3 p3 = position + rotation * new Vector3(size * aspect, size, near);
            Vector3 p4 = position + rotation * new Vector3(-size * aspect, size, near);
            Vector3 p5 = position + rotation * new Vector3(-size * aspect, -size, far);
            Vector3 p6 = position + rotation * new Vector3(size * aspect, -size, far);
            Vector3 p7 = position + rotation * new Vector3(size * aspect, size, far);
            Vector3 p8 = position + rotation * new Vector3(-size * aspect, size, far);

            return GetBounds(p1, p2, p3, p4, p5, p6, p7, p8);
        }

        public static Bounds PerspectiveBounds(Vector3 position, Quaternion rotation, float fov, float aspect,
            float near, float far)
        {
            float tfov = Mathf.Tan(fov*Mathf.Deg2Rad/2);
            float ny = tfov*near;
            float fy = tfov*far;

            Vector3 p1 = position + rotation*new Vector3(-aspect*ny, -ny, near);
            Vector3 p2 = position + rotation*new Vector3(-aspect*ny, ny, near);
            Vector3 p3 = position + rotation*new Vector3(aspect*ny, ny, near);
            Vector3 p4 = position + rotation*new Vector3(aspect*ny, -ny, near);

            Vector3 p5 = position + rotation*new Vector3(-aspect* fy, -fy, far);
            Vector3 p6 = position + rotation*new Vector3(-aspect* fy, fy, far);
            Vector3 p7 = position + rotation*new Vector3(aspect* fy, fy, far);
            Vector3 p8 = position + rotation*new Vector3(aspect* fy, -fy, far);

            return GetBounds(p1, p2, p3, p4, p5, p6, p7, p8);
        }

        private static Bounds GetBounds(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p5, Vector3 p6,
            Vector3 p7, Vector3 p8)
        {
            Vector3 min = OTProjectorUtils.GetMinVector(p1, p2);
            min = OTProjectorUtils.GetMinVector(min, p3);
            min = OTProjectorUtils.GetMinVector(min, p4);
            min = OTProjectorUtils.GetMinVector(min, p5);
            min = OTProjectorUtils.GetMinVector(min, p6);
            min = OTProjectorUtils.GetMinVector(min, p7);
            min = OTProjectorUtils.GetMinVector(min, p8);
            Vector3 max = OTProjectorUtils.GetMaxVector(p1, p2);
            max = OTProjectorUtils.GetMaxVector(max, p3);
            max = OTProjectorUtils.GetMaxVector(max, p4);
            max = OTProjectorUtils.GetMaxVector(max, p5);
            max = OTProjectorUtils.GetMaxVector(max, p6);
            max = OTProjectorUtils.GetMaxVector(max, p7);
            max = OTProjectorUtils.GetMaxVector(max, p8);

            Vector3 si = max - min;
            Vector3 ct = min + si / 2;
            return new Bounds(ct, si);
        }
    }
}