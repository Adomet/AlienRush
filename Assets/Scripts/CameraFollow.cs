using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform followTransform;
	[Range(1, 10)] public float followSpeed = 2;
	[Range(1, 10)] public float lookSpeed = 5;
	
	Vector3 initialCameraPosition;
	Vector3 initialCarPosition;
	Vector3 absoluteInitCameraPosition;

	void Start(){
		initialCameraPosition = gameObject.transform.position;
		initialCarPosition = followTransform.position;
		absoluteInitCameraPosition = initialCameraPosition - initialCarPosition;
	}

	void Update()
	{
		if(!followTransform)
			return;
		
		//Look
		Vector3 _lookDirection = (new Vector3(followTransform.position.x, followTransform.position.y, followTransform.position.z)) - transform.position;
		Quaternion _rot = Quaternion.LookRotation(_lookDirection, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);

		//Move
		Vector3 _targetPos = absoluteInitCameraPosition + followTransform.transform.position;
		transform.position = Vector3.Lerp(transform.position, _targetPos, followSpeed * Time.deltaTime);

	}

}
