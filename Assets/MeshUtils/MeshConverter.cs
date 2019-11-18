using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MemLib.HalfEdgeStructure;
[ExecuteInEditMode][System.Obsolete]
[RequireComponent(typeof(MeshFilter))]
public class MeshConverter : MonoBehaviour {
    public Mesh mesh;
    
    private HalfEdgeMesh heMesh;

    public HalfEdgeMesh HEMesh {
        get {
            if (heMesh == null) {
                heMesh = mesh.HEMesh();
            }
            return heMesh;
        }
    }
	// Use this for initialization
	void Start () {
        if (mesh == null) {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            if (!mesh.name.EndsWith("copy")) {
                mesh = GetComponent<MeshFilter>().sharedMesh.CopyMesh();
                mesh.name = name + "_copy";
                GetComponent<MeshFilter>().sharedMesh = mesh;
            }
        }
        if (heMesh == null) {
            heMesh = mesh.HEMesh();
            if(heMesh == null) {
                //mesh.startGetHEMesh();
            }
        }
    }

	void Update () {
		
	}
}
