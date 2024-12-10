using UnityEngine;
using System.Collections;
using Games.LogicObj;
public class followCamera : MonoBehaviour
{
	    public Transform  target;
	    public  float  distance = -8.74f;
        public float height= 3.0f;
        public  float damping= 5.0f;
    	public bool smoothRotation  = true;
	    public float rotationDamping    = 10.0f;
	
	    public Vector3 targetLookAtOffset ;     // allows offsetting of camera lookAt, very useful for low bumper heights
	
	    public float bumperDistanceCheck= 2.5f;  // length of bumper ray
     	public float bumperCameraHeight  = 1.0f;   // adjust camera height while bumping
	    public Vector3 bumperRayOffset;    //
    	public bool  isEnable=false;
		// Use this for initialization
		void Start ()
		{
		targetLookAtOffset = new Vector3 (0.0f,1.3f,0.0f);
		}
	
		// Update is called once per frame
		void Update ()
		{
		if (isEnable == false)
						return;
		var wantedPosition = target.TransformPoint(0, height, -distance);
		
		// check to see if there is anything behind the target
		RaycastHit hit ;
		var back = target.transform.TransformDirection(-1 * Vector3.forward);  
		
		// cast the bumper ray out from rear and check to see if there is anything behind
		if (Physics.Raycast(target.TransformPoint(bumperRayOffset), back, out hit, bumperDistanceCheck)) {
			// clamp wanted position to hit position
			wantedPosition.x = hit.point.x;
			wantedPosition.z = hit.point.z;
			wantedPosition.y = Mathf.Lerp(hit.point.y + bumperCameraHeight, wantedPosition.y, Time.deltaTime * damping);
		}
		
		transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);
		
		var lookPosition  = target.TransformPoint(targetLookAtOffset);
		
		if (smoothRotation) {
			var wantedRotation  = Quaternion.LookRotation(lookPosition - transform.position, target.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
		} else {
			transform.rotation = Quaternion.LookRotation(lookPosition - transform.position, target.up);
		}
	

		
	}
}

