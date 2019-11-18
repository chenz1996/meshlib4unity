using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///
namespace MemLib {
    public class MeshLib {
        private static Mesh _sphere;
        private static Mesh _cube;
        private static Mesh _tetrahedron;
        private static Mesh _hoof;
        public static Mesh Sphere {
            get {
                if (_sphere == null) {
                    _sphere = new Mesh();
                    Vector3[] vertices = new Vector3[] {
                    new Vector3(1,1,-1),
                    new Vector3(1,1,1),
                    new Vector3(1,-1,-1),
                    new Vector3(1,-1,1),
                    new Vector3(-1,1,-1),
                    new Vector3(-1,1,1),
                    new Vector3(-1,-1,-1),
                    new Vector3(-1,-1,1)
                };
                    int[] triangles = new int[] {
                    1,0,3,
                    3,2,1,
                    2,3,7,
                    7,6,2,
                    1,2,6,
                    6,5,1,
                    0,1,5,
                    5,4,0,
                    3,0,4,
                    4,7,3,
                    5,4,7,
                    7,6,5
                };
                    _sphere.vertices = vertices;
                    _sphere.triangles = triangles;
                    _sphere.RecalculateNormals();
                }
                return _sphere;

            }
        }

        public static Mesh Cube {
            get {
                if (_cube == null) {
                    _cube = new Mesh();
                    Vector3[] vertices = new Vector3[] {
                        new Vector3(1,-1,1),
                        new Vector3(1,1,1),
                        new Vector3(1,1,-1),
                        new Vector3(1,-1,-1),
                        new Vector3(-1,-1,1),
                        new Vector3(-1,1,1),
                        new Vector3(-1,1,-1),
                        new Vector3(-1,-1,-1)
                    };
                    int[] triangles = new int[] {
                        1,0,3,
                        3,2,1,
                        2,3,7,
                        7,6,2,
                        1,2,6,
                        6,5,1,
                        0,1,5,
                        5,4,0,
                        3,0,4,
                        4,7,3,
                        5,4,7,
                        7,6,5
                    };
                    _cube.vertices = vertices;
                    _cube.triangles = triangles;
                    _cube.RecalculateNormals();
                }
                return _cube;
            }
        }

        public static Mesh Tetrohedron {
            get {
                if (_tetrahedron == null) {
                    _tetrahedron = new Mesh();
                    Vector3[] vertices = new Vector3[] {
                        new Vector3(1,-1,1),
                        new Vector3(1,1,1),
                        new Vector3(1,1,-1),
                        new Vector3(1,-1,-1),
                        new Vector3(-1,-1,1),
                        new Vector3(-1,1,1),
                        new Vector3(-1,1,-1),
                        new Vector3(-1,-1,-1)
                    };
                    int[] triangles = new int[] {
                        1,0,3,
                        3,2,1,
                    };
                    _tetrahedron.vertices = vertices;
                    _tetrahedron.triangles = triangles;
                    _tetrahedron.RecalculateNormals();
                }
                return _tetrahedron;
            }
        }

        public static Mesh Hoof {
            get {
                if (_hoof == null) {
                    _hoof = new Mesh();
                    Vector3[] vertices = new Vector3[] {
                        new Vector3(0,1,0),
                        new Vector3(-1,0,1),
                        new Vector3(-1,0,-1),
                        new Vector3(1,0,-1),
                        new Vector3(1,0,1),
                        new Vector3(0,-1,0),
                    };
                    int[] triangles = new int[] {
                        2,0,1,
                        3,0,2,
                        4,0,3,
                        1,0,4,
                        1,2,5,
                        2,3,5,
                        3,4,5,
                        4,1,5
                    };
                    _hoof.vertices = vertices;
                    _hoof.triangles = triangles;
                    _hoof.RecalculateNormals();
                }
                return _hoof;
            }
        }
    }

    public class MaterialLib {
        private static Material _vertexMaterial;
        private static Material _edgeMaterial;
        public static Material VertexMaterial {
            get {
                if (_vertexMaterial == null) {
                    _vertexMaterial = new Material(Shader.Find("Unlit/Color"));
                    _vertexMaterial.color = Color.blue;
                    _vertexMaterial.doubleSidedGI = true;
                }
                return _vertexMaterial;
            }
        }

        public static Material EdgeMaterial {
            get {
                if (_edgeMaterial == null) {
                    _edgeMaterial = new Material(Shader.Find("Unlit/Color"));
                    _edgeMaterial.color = Color.green;
                    _edgeMaterial.doubleSidedGI = true;
                }
                return _edgeMaterial;
            }
        }
    }
}
