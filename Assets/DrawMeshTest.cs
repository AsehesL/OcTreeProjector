using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawMeshTest : MonoBehaviour
{

    public Material material;
    public Mesh mesh;

	void Start () {
		
	}
	
	void Update ()
	{
	    Graphics.DrawMesh(mesh, transform.localToWorldMatrix, material, gameObject.layer);
	}
}
