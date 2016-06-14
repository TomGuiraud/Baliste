

using UnityEngine;
using System.Collections;

public class ProjectileBehaviour : MonoBehaviour {

	private Transform _transform;
	[SerializeField] private Rigidbody _rigidbody;
	[SerializeField] private Transform _testSphereTransform;

	// Use this for initialization
	void Start () {
		_transform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (_rigidbody.velocity.magnitude > 5.0f){
			AdjustOrientation();
		}

	}

	void AdjustOrientation (){
		Vector3 tmpVectToLook = ((_rigidbody.velocity).normalized + _transform.position);
		//_testSphereTransform.position = tmpVectToLook;
		//print ("Old Rot : " + _transform.rotation.eulerAngles);
		//_transform.Rotate(Vector3.RotateTowards( _transform.forward, tmpVectToLook,1.0F, 0.0f),Space.Self);
		_transform.LookAt(tmpVectToLook);
		//print ("New Rot : " + _transform.rotation.eulerAngles);
	}
}
