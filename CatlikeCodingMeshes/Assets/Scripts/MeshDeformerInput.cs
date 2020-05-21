using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{	
	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices, vertexVelocities;
	public float force = 10f;
	public float forceOffset = .1f;
	
    // Start is called before the first frame update
    void Start(){
        deformingMesh = GetComponent<MeshFilter>().mesh;
		originalVertices = deformingMesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
		
		for(int i = 0; i < originalVertices.Length; i++){
			displacedVertices[i] = originalVertices[i];
		}
		
		vertexVelocities = new Vector3[originalVertices.Length];
    }

    // Update is called once per frame
    void Update(){
        if(Input.GetMouseButton(0)){
			HandleInput();
		}
    }
	
	void HandleInput(){
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(inputRay, out hit)){
			MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
			if(deformer){
				Vector3 point = hit.point;
				point += hit.normal * forceOffset;
				deformer.AddDeformingForce(point, force);
			}
		}
	}
	
}
