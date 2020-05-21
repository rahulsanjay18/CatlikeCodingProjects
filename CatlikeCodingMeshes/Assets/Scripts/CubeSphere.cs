using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeSphere : MonoBehaviour
{	
	public int grid_size;
	public float radius;
	
	private Mesh mesh;
	private Vector3[] vertices;
	private Vector3[] normals;
	private Color32[] cubeUV;
	
	private void Awake(){
		Generate();
	}
	
	private void Generate(){
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Sphere";
		
		CreateVertices();
		CreateTriangles();
		CreateColliders();
	}
	
	private void CreateVertices(){
		int corner_vertices = 8;
		int edge_vertices = (grid_size + grid_size + grid_size - 3) * 4;
		int face_vertices = (
			(grid_size - 1) * (grid_size - 1) +
			(grid_size - 1) * (grid_size - 1) +
			(grid_size - 1) * (grid_size - 1)) * 2;
			
		vertices = new Vector3[corner_vertices + edge_vertices + face_vertices];
		normals = new Vector3[vertices.Length];
		cubeUV = new Color32[vertices.Length];
		
		int v = 0;
		for(int y = 0; y <= grid_size; y++){
			for(int x = 0; x <= grid_size; x++){
				SetVertex(v++, x, y, 0);
			}
		
			for(int z = 1; z <= grid_size; z++){
				SetVertex(v++, grid_size, y, z);
			}
		
			for(int x = grid_size - 1; x >= 0; x--){
				SetVertex(v++, x, y, grid_size);
			}
		
			for(int z = grid_size - 1; z > 0; z--){
				SetVertex(v++, 0, y, z);
			}
		}
		
		for(int z = 1; z < grid_size; z++){
			for(int x = 1; x < grid_size; x++){
				SetVertex(v++, x, grid_size, z);
			}
		}
		
		for(int z = 1; z < grid_size; z++){
			for(int x = 1; x < grid_size; x++){
				SetVertex(v++, x, 0, z);
			}
		}
		
		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.colors32 = cubeUV;
	}
	
	private void SetVertex(int i, int x, int y, int z){
		Vector3 v = new Vector3(x, y, z) * 2f/grid_size - Vector3.one;
		float x2 = v.x * v.x;
		float y2 = v.y * v.y;
		float z2 = v.z * v.z;
		
		Vector3	s;
		s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
		s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
		s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);
		
		normals[i] = s;
		vertices[i] = normals[i] * radius;
		cubeUV[i] = new Color32((byte) x, (byte) y, (byte) z, 0);
	}
	
	private void CreateTriangles(){
		int[] trianglesZ = new int[(grid_size * grid_size) * 12];
		int[] trianglesX = new int[(grid_size * grid_size) * 12];
		int[] trianglesY = new int[(grid_size * grid_size) * 12];
		int ring = (grid_size + grid_size) * 2;
		int tZ = 0, tX = 0, tY = 0, v = 0;
		
		for(int y = 0; y < grid_size; y++, v++){
			for(int q = 0; q < grid_size; q++, v++){
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			
			for(int q = 0; q < grid_size; q++, v++){
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
			
			for(int q = 0; q < grid_size; q++, v++){
				tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);
			}
			
			for(int q = 0; q < grid_size - 1; q++, v++){
				tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);
			}
		
			tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
		}
		
		tY = CreateTopFace(trianglesY, tY, ring);
		tY = CreateBottomFace(trianglesY, tY, ring);
		
		mesh.subMeshCount = 3;
		mesh.SetTriangles(trianglesZ, 0);
		mesh.SetTriangles(trianglesX, 1);
		mesh.SetTriangles(trianglesY, 2);
	}
	
	private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11){
		triangles[i] = v00;
		triangles[i+1] = triangles[i+4] = v01;
		triangles[i+2] = triangles[i+3] = v10;
		triangles[i+5] = v11;
		
		return i+6;
	}
	
	private int CreateTopFace(int[] triangles, int t, int ring){
		int v = ring * grid_size;
		for(int x = 0; x < grid_size - 1; x++, v++){
			t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
		}
		t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);
		
		int v_min = ring * (grid_size + 1) - 1;
		int v_mid = v_min + 1;
		int v_max = v + 2;
		
		for(int z = 1; z < grid_size - 1; z++, v_min--, v_mid++, v_max++){
			t = SetQuad(triangles, t, v_min, v_mid, v_min - 1, v_mid + grid_size - 1);
		
			for(int x = 1; x < grid_size - 1; x++, v_mid++){
				t = SetQuad(triangles, t, v_mid, v_mid + 1, v_mid + grid_size - 1, v_mid + grid_size);
			}
			t = SetQuad(triangles, t, v_mid, v_max, v_mid + grid_size - 1, v_max + 1);
		}
		
		int v_top = v_min - 2;
		t = SetQuad(triangles, t, v_min, v_mid, v_top + 1, v_top);
		for(int x = 1; x < grid_size - 1; x++, v_top--, v_mid++){
			t = SetQuad(triangles, t, v_mid, v_mid + 1, v_top, v_top - 1);
		}

		t = SetQuad(triangles, t, v_mid, v_top - 2, v_top, v_top - 1);
		return t;
	}
	
	private int CreateBottomFace(int[] triangles, int t, int ring){
		int v = 1;
		int v_mid = vertices.Length - (grid_size - 1) * (grid_size - 1);
		
		t = SetQuad(triangles, t, ring - 1, v_mid, 0, 1);
		for(int x = 1; x < grid_size - 1; x++, v++, v_mid++){
			t = SetQuad(triangles, t, v_mid, v_mid + 1, v, v + 1);
		}
		t = SetQuad(triangles, t, v_mid, v + 2, v, v + 1);
		
		int v_min = ring - 2;
		v_mid -= grid_size - 2;
		int v_max = v + 2;
		
		for(int z = 1; z < grid_size - 1; z++, v_min--, v_mid++, v_max++){
			t = SetQuad(triangles, t, v_min, v_mid + grid_size - 1, v_min + 1, v_mid);
		
			for(int x = 1; x < grid_size - 1; x++, v_mid++){
				t = SetQuad(triangles, t, v_mid + grid_size - 1, v_mid + grid_size, v_mid, v_mid + 1);
			}
			t = SetQuad(triangles, t, v_mid + grid_size - 1, v_max + 1, v_mid, v_max);
		}
		
		int v_top = v_min - 1;
		t = SetQuad(triangles, t, v_top + 1, v_top, v_top + 2, v_mid);
		for(int x = 1; x < grid_size - 1; x++, v_top--, v_mid++){
			t = SetQuad(triangles, t, v_top, v_top - 1, v_mid, v_mid + 1);
		}

		t = SetQuad(triangles, t, v_top, v_top - 1, v_mid, v_top - 2);
		return t;
	}
	
	private void OnDrawGizmos(){
		if(vertices == null){
			return;
		}
		
		for(int i = 0; i < vertices.Length; i++){
			Gizmos.color = Color.black;
			Gizmos.DrawSphere(vertices[i], .1f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(vertices[i], normals[i]);
		}
	}
	
	private void CreateColliders(){
		gameObject.AddComponent<SphereCollider>();
	}
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
