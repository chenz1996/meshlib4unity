using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using System.Threading;

//半边数据结构 命名空间
namespace MemLib.HalfEdgeStructure {
    //[ExecuteInEditMode]
    /// <summary>
    /// 半边数据结构类
    /// <para >HalfEdgeMesh HEMesh(this Mesh mesh)为扩展方法，可以返回Mesh对应的HalfEdgeMesh</para>
    /// </summary>
    public static class HalfEdgeStructure {

        private static ConditionalWeakTable<Mesh, HalfEdgeMesh> MeshManager=new ConditionalWeakTable<Mesh, HalfEdgeMesh>();


        private static List<Mesh> buildQueues = new List<Mesh>();
        /// <summary>
        /// 若返回null,表示尚未（正在）进行初始化
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HalfEdgeMesh HEMesh(this Mesh mesh) {
            HalfEdgeMesh heMesh = null;

            if (MeshManager.TryGetValue(mesh, out heMesh)) {
                return heMesh;
            } else {
                //heMesh = new HalfEdgeMesh(mesh);
                //MeshManager.Add(mesh, heMesh);
                //getHEMesh(mesh,(_heMesh)=> {
                //    heMesh = _heMesh;
                //});
                //Debug.LogWarning(mesh.name + "的半边数据结构正在构建中...");
                return null;
            }
        }

        public static void startGetHEMesh(this Mesh mesh,UnityAction<bool,string>resultCallback) {

            Debug.Log("startGetHEMesh...");

            HalfEdgeMesh heMesh = null;
            //任务已在队列中
            if (buildQueues.Contains(mesh)) {
                resultCallback(false, mesh.name+"的半边数据结构正在构建中,请勿重复发起任务");
                return;
            }
            //已经构建完成
            if (MeshManager.TryGetValue(mesh, out heMesh)) {
                resultCallback(true, mesh.name + "的半边数据结构已存在，请直接访问");
                return;
            }

            buildQueues.Add(mesh);//表示mesh正在构建半边数据结构；

            Vector3[] verticies = mesh.vertices;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;

            //进程通信控制器
            UnityAction<string> messageHandler = (message) => { Debug.Log(message); };
            //子进程
            Thread initThread = new Thread(() => {
                heMesh = new HalfEdgeMesh(mesh);
                heMesh.Init(verticies, normals, triangles,messageHandler);
                MeshManager.Add(mesh, heMesh);
                buildQueues.Remove(mesh);
                resultCallback(true,"半边数据结构构建完成");
            });
            //开启子进程
            initThread.Start();

            Debug.Log("GetHEMesh Thread is Started...");
        }
        
    }
    //[System.Serializable]
    /// <summary>
    /// 半边Mesh
    /// </summary>
    public class HalfEdgeMesh {
        public List<Edge> edges=new List<Edge>();
        public List<Face> faces = new List<Face>();
        public List<HalfEdge> halfEdges = new List<HalfEdge>();
        public List<Vertex> vertices = new List<Vertex>();

        public Mesh mesh;

        public bool Inited { get; private set; }

        /// <summary>
        /// 实例化方法1，需要配合Init方法进行初始化；
        /// </summary>
        /// <param name="mesh">原始UnityMesh</param>
        public HalfEdgeMesh(Mesh mesh) {
            Inited = false;//初始状态
            this.mesh = mesh;
        }
        
        /// <summary>
        /// 配合构造方法1使用
        /// </summary>
        /// <param name="verticies">顶点信息</param>
        /// <param name="normals">法线信息</param>
        /// <param name="triangles">三角网格面信息</param>
        public void Init(Vector3[] verticies, Vector3[] normals, int[] triangles,UnityAction<string>handler=null) {
            //构造顶点
            for (int i = 0; i < verticies.Length; i++) {
                Vertex vertex = new Vertex(i, verticies[i]);
                vertex.Normal = normals[i];
                vertices.Add(vertex);
            }

            if (handler != null) {
                handler("顶点构造完成");
            }

            //构造面、半边、边
            int faceNumber = triangles.Length / 3;
            for (int i = 0; i < faceNumber; i++) {
                Face face = new Face(i);
                faces.Add(face);

                face.HalfEdges = new List<HalfEdge>();

                for (int j = 0; j < 3; j++) {
                    HalfEdge hEdge = new HalfEdge(3 * i + j);
                    //第一条边与面建立联系
                    if (j == 0) {
                        face.halfEdge = hEdge;
                    }
                    //半边与面建立联系
                    face.HalfEdges.Add(hEdge);
                    hEdge.face = face;

                    //半边的指向顶点
                    hEdge.FromVertex = vertices[triangles[3 * i + j]];
                    hEdge.ToVertex= vertices[triangles[3 * i + (j + 1) % 3]];
                    //hEdge.toPosition = verticies[triangles[3 * i + (j + 1) % 3]];

                    //离开顶点的半边
                    if (!hEdge.FromVertex.HalfEdges.Contains(hEdge)) {
                        hEdge.FromVertex.HalfEdges.Add(hEdge);
                    }
                    if (hEdge.FromVertex.halfEdge == null) {
                        hEdge.FromVertex.halfEdge = hEdge;
                    }

                    bool hasOpposite = false;
                    //半边与反向建立联系
                    foreach (HalfEdge _hedge in hEdge.ToVertex.HalfEdges) {

                        if (_hedge.Opposite != null) {
                            continue;
                        }

                        if (_hedge.ToVertex==hEdge.FromVertex) {
                            _hedge.Opposite = hEdge;
                            hEdge.Opposite = _hedge;
                            hasOpposite = true;
                            break;
                        }

                    }
                    if (!hasOpposite) {
                        Edge edge = new Edge(edges.Count);
                        edge.halfEdge = hEdge;
                        edges.Add(edge);
                    }
                    halfEdges.Add(hEdge);
                }


                //构建半边顺序
                for (int j = 0; j < 3; j++) {
                    halfEdges[3 * i + j].Next = halfEdges[3 * i + (1 + j) % 3];
                    halfEdges[3 * i + j].Previous = halfEdges[3 * i + (2 + j) % 3];
                }

                face.UpdateNormal();
            }

            if (handler != null) {
                handler("构造面、半边、边");
            }

            //给没有反向的半边增加反向
            int heCount = halfEdges.Count;
            int totalCount = heCount;
            for (int i = 0; i < heCount; i++) {
                if (halfEdges[i].Opposite == null) {

                    HalfEdge opHalfEdge = new HalfEdge(totalCount++);
                    opHalfEdge.FromVertex = halfEdges[i].ToVertex;
                    opHalfEdge.ToVertex = halfEdges[i].FromVertex;

                    opHalfEdge.Opposite = halfEdges[i];
                    halfEdges[i].Opposite = opHalfEdge;

                    halfEdges.Add(opHalfEdge);
                }
            }
            Inited = true;
        }

        /// <summary>
        /// 构造方法2，推荐构造方式
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="verticies"></param>
        /// <param name="normals"></param>
        /// <param name="triangles"></param>
        public HalfEdgeMesh(Mesh mesh, Vector3[] verticies, Vector3[] normals, int[] triangles) {
            
            Inited = false;//初始状态
            this.mesh = mesh;
            //TODO:mesh的get方法只能在主线程中调用，因此点面法线信息要单独传参；
            //Vector3[] verticies = mesh.vertices;
            //Vector3[] normals = mesh.normals;
            //int[] triangles = mesh.triangles;
            //Vector2[] uvs = mesh.uv;
            //初始化
            Init(verticies, normals, triangles);
        }

        public void update() {
            Vector3[] verts = mesh.vertices;
            int len = verts.Length;
            for (int i = 0; i < len; i++) {
                vertices[i].Position = verts[i];
            }
            mesh.RecalculateNormals();
            //mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }
    }

    /// <summary>
    /// 半边
    /// </summary>
    public class HalfEdge {
        //
        public int index;//id
        public Vertex FromVertex;//起点
        public Vertex ToVertex;//终点
        public Edge edge;//边
        public Face face;//面
        //法线
        public Vector3 Normal;
        //前、后、反向
        public HalfEdge Previous;
        public HalfEdge Next;
        public HalfEdge Opposite;

        /// <summary>
        /// 构造方法
        /// </summary>
        public HalfEdge(int id) {
            index = id;
        }

        /// <summary>
        ///半边是否在边界上
        /// </summary>
        public bool OnBoundary {
            get {
                return (face==null||Opposite.face==null);
            }
        }
    }

    /// <summary>
    /// 顶点
    /// </summary>
    public class Vertex {
        //坐标，法线
        public Vector3 Position;
        public Vector3 Normal;
        
        //半边数据
        public HalfEdge halfEdge;//一条离开该顶点的半边
        //private HalfEdgeMesh mesh;
        public int index { private set; get; }//索引
        
        public Vector3 delta { get; private set; }//微分坐标
        public float[] omegas;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="position">顶点坐标</param>
        public Vertex(int id,Vector3 position) {
            index = id;
            Position.x = position.x;
            Position.y = position.y;
            Position.z = position.z;

            HalfEdges = new List<HalfEdge>();
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vertex(int id,float x,float y,float z) {
            index = id;
            Position.x = x;
            Position.y = y;
            Position.z = z;

            HalfEdges = new List<HalfEdge>();
        }

        #region AdjacentsOfVertex
        //所有相邻的半边
        public List<HalfEdge> HalfEdges { get;internal set; }
        //以下方式对于边界点不适用，边界点的半边不一定存在Next半边，会导致程序崩溃
        //public IEnumerable<HalfEdge> HalfEdges {
        //    get {
        //        HalfEdge hEdge = this.halfEdge;  
        //        if (hEdge != null) {
        //            do {
        //                yield return hEdge;
        //                hEdge = hEdge.Opposite.Next;
        //            } while (hEdge != this.halfEdge&& hEdge!=null);
        //        }
        //    }
        //}
        //所有相邻的边
        public IEnumerable<Edge> Edges {
            get {
                foreach(HalfEdge hEdge in HalfEdges) {
                    yield return hEdge.edge;
                }
            }
        }
        //所有相邻的面
        public IEnumerable<Face> Faces {
            get {
                foreach (HalfEdge hEdge in HalfEdges) {
                    yield return hEdge.face;
                }
            }
        } 
        /// <summary>
        /// 该顶点的所有相邻顶点
        /// </summary>
        public IEnumerable<Vertex> Vertices {
            get {
                foreach (HalfEdge hEdge in HalfEdges) {
                    yield return hEdge.ToVertex;
                }
            }
        }
#endregion

#region CountOfAdjacents
        //相邻半边个数
        public int HalfEdgeCount {
            get {
                int Count = 0;
                foreach (HalfEdge hEdge in HalfEdges) {
                    Count++;
                }
                return Count;
            }
        }
        //相邻边个数
        public int EdgeCount {
            get {
                int Count = 0;
                foreach (Edge edge in Edges) {
                    Count++;
                }
                return Count;
            }
        }
        //相邻面个数
        public int FaceCount {
            get {
                int Count = 0;
                foreach (Edge edge in Edges) {
                    Count++;
                }
                return Count;
            }
        }
        //相邻顶点个数
#endregion
        /// <summary>
        /// 顶点是否在边界
        /// </summary>
        public bool OnBoundary {
            get {
                if (halfEdge == null) {
                    return true;
                }

                if (halfEdge.OnBoundary) {
                    return true;
                }
                //return HalfEdges.Count == Faces.Count();
                //foreach (HalfEdge half in HalfEdges) {
                //    if (half.OnBoundary) {
                //        return true;
                //    }
                //}
                return false;
            }
        }
        
        /// <summary>
        /// 更新微分坐标和系数omega
        /// </summary>
        public void UpdateDelta() {
            //获取邻点及其数目
            var nVs = Vertices;
            int number = nVs.Count();
            Vector3[] nVPs = new Vector3[number];//邻点坐标
            //定义系数，邻点的距离
            omegas = new float[number];
            float[] lengthes = new float[number];//顶点与邻点的距离
            
            float mRecip = 0;//m分之1
            //计算邻点的距离以及m分之1的值
            int j = 0;
            foreach(Vertex nV in nVs) {
                nVPs[j] = nV.Position;
                lengthes[j] = Vector3.Distance(Position, nVPs[j]);
                mRecip += 1f / lengthes[j];
                j++;
            }
            //计算m
            float m = 1f / mRecip;
            //计算系数
            for(int i = 0; i < number; i++) {
                omegas[i] = m / lengthes[i];
            }

            Vector3 _delta = Position;
            for (int i = 0; i < number; i++) {
                _delta -= omegas[i] * nVPs[i];
            }
            delta = _delta;
        }
    }

    /// <summary>
    /// 边
    /// </summary>
    public class Edge {
        public int index;//id
        public HalfEdge halfEdge;//半边
        //public Color color;//颜色
        public double Angle;//角度

        public Edge(int id) {
            index = id;
        }
        public Vertex[] TwoVertices {
            get {
                Vertex[] v = new Vertex[2];
                v[0] = halfEdge.ToVertex;
                v[1] = halfEdge.FromVertex;
                return v;
            }
        }

        public List<HalfEdge> TwoHalfEdges {
            get {
                List<HalfEdge> hs = new List<HalfEdge>();
                hs.Add(halfEdge);
                if (halfEdge.Opposite != null) {
                    hs.Add(halfEdge);
                }
                return hs;
            }
        }
        public Face[] TwoFaces {
            get {
                Face[] fs = new Face[2];
                fs[0] = halfEdge.face;
                fs[1] = halfEdge.Opposite.face;
                return fs;
            }
        }
        /// <summary>
        ///边是否在边界上
        /// </summary>
        public bool OnBoundary {
            get {
                return halfEdge.OnBoundary||halfEdge.Opposite.OnBoundary;
            }
        }
    }

    /// <summary>
    /// 面
    /// </summary>
    public class Face {
        public int index;//id
        public HalfEdge halfEdge;//半边
        //public Color color;//颜色
        public Vector3 Normal;
        
        public Face(int id) {
            index = id;

            HalfEdges = new List<HalfEdge>();
        }
   
        //所有相邻的半边
        public List<HalfEdge> HalfEdges { get; internal set; }
        //所有相邻的边
        public IEnumerable<Edge> Edges {
            get {
                foreach (HalfEdge hEdge in HalfEdges) {
                    yield return hEdge.edge;
                }
            }
        }
        //所有相邻的面
        public IEnumerable<Face> Faces {
            get {
                foreach (HalfEdge hEdge in HalfEdges) {
                    if (hEdge.Opposite != null && hEdge.Opposite.face != null) {
                        yield return hEdge.Opposite.face;
                    }
                }
            }
        }
        //所有相邻的顶点
        public IEnumerable<Vertex> Vertices {
            get {
                foreach (HalfEdge hEdge in HalfEdges) {
                    yield return hEdge.FromVertex;
                }
            }
        }
        /// <summary>
        ///面是否在边界上
        /// </summary>
        public bool OnBoundary {
            get {
                foreach (HalfEdge hEdge in HalfEdges) {
                    if (hEdge.Opposite.OnBoundary) {
                        return true;
                    }
                }
                return false;
            }
        }

        //更新面法线
        public void UpdateNormal() {
            int i = 0;
            IEnumerable<Vertex> vs = Vertices;
            Vector3[] vt=new Vector3[3];
            foreach (Vertex v in vs) {
                vt[i++] = v.Position;
            }
            Normal = Vector3.Cross(vt[2] - vt[0], vt[1] - vt[0]);
            Normal.Normalize();
        }
    }

    /// <summary>
    /// 扩展工具类
    /// <para>方法列表：</para>
    /// <para>Count：计算IEnumerable数组的元素个数</para>
    /// </summary>
    public static class Extensions {
        public static int Count<T>(this IEnumerable<T> ts) {
            int count = 0;
            foreach(T t in ts){
                count++;
            }
            return count;
        }
    }
}