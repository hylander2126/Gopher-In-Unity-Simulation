using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject r_notify;
    public GameObject l_notify;




    //right part of sensor
    public float targetVelocity=10.0f;
    public int numberOfRays=9;
    public float angle=90;
    public float rayRange=1.3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDrawGizmos(){
	for(int i=0; i<numberOfRays;i++){
		var rotation = this.transform.rotation;
		var rotationMod= Quaternion.AngleAxis(-10+(i/(float)numberOfRays)*-angle, this.transform.up);
		var direction=rotation*rotationMod*Vector3.forward;
		Gizmos.DrawRay(this.transform.position, direction*rayRange);
	}

	for(int i=0; i<numberOfRays;i++){
		var rotation = this.transform.rotation;
		var rotationMod= Quaternion.AngleAxis(10+(i/(float)numberOfRays)*angle, this.transform.up);
		var direction=rotation*rotationMod*Vector3.forward;
		Gizmos.DrawRay(this.transform.position, direction*rayRange);
	}
 }
}
