using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableButtonFcn : MonoBehaviour
{
    public GameObject follower;
	private FlyCamera script;
	
	void Start(){
		script = follower.GetComponent<FlyCamera>();
	}
	
	public void EnableMovement(){
		script.is_active = true;
	}
}
