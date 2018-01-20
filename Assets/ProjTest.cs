using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjTest : MonoBehaviour
{

    public Transform point;

    private Camera m_Camera;

	void Start ()
	{
	    m_Camera = gameObject.GetComponent<Camera>();
	}

    void OnGUI()
    {
        Matrix4x4 worldToProj = m_Camera.projectionMatrix*transform.worldToLocalMatrix;

        Vector4 pos = new Vector4(point.position.x, point.position.y, point.position.z, 1);
        pos = worldToProj * pos;

        GUILayout.Label(pos.ToString("f4"));

        Vector3 p = new Vector3(pos.x/pos.w, pos.y/pos.w, pos.z/pos.w);
        GUILayout.Label(p.ToString("f4"));
    }
}
