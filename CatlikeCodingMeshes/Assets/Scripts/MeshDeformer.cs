using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
	Mesh deformingMesh;
	Vector3[] originalVertices, displacedVertices, vertexVelocities;
	public float spring_force = 20f;
	public float damping = 5f;
	float uniform_scale = 1f;
	
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

    void Update(){
		uniform_scale = transform.localScale.x;
		for(int i = 0; i < displacedVertices.Length; i++){
			UpdateVertex(i);
		}
		deformingMesh.vertices = displacedVertices;
		deformingMesh.RecalculateNormals();
	}
	
	void UpdateVertex(int i){
		Vector3 velocity = vertexVelocities[i];
		Vector3 displacement = displacedVertices[i] - originalVertices[i];
		displacement *= uniform_scale;
		velocity -= displacement * spring_force * Time.deltaTime;
		velocity *= 1f - damping * Time.deltaTime;
		vertexVelocities[i] = velocity;
		displacedVertices[i] += velocity * (Time.deltaTime / uniform_scale);
	}
	
	public void AddDeformingForce(Vector3 point, float force){
		point = transform.InverseTransformPoint(point);
		for(int i = 0; i < displacedVertices.Length; i++){
			AddForceToVertex(i, point, force);
		}
	}
	
	void AddForceToVertex(int i, Vector3 point, float force){
		Vector3 pointToVertex = displacedVertices[i] - point;
		pointToVertex *= uniform_scale;
		float attenuated_force = force / (1f + pointToVertex.sqrMagnitude);
		float velocity = attenuated_force * Time.deltaTime;
		vertexVelocities[i] += pointToVertex.normalized * velocity;
	}
}
