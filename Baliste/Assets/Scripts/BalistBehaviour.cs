using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BalistBehaviour : MonoBehaviour {


	//Balist Components
	public Rigidbody balistRB;
	public Collider balistColl;


	//Movement Variables
	[System.Serializable]
	public class BalistControllerSet {
		public string BalistHorizontal;
		public string BalistVertical;
		public string BalistInput;

		public BalistControllerSet (string horInput, string verInput, string inInput){
			BalistHorizontal = horInput;
			BalistVertical = verInput;
			BalistInput = inInput;
		}
	}

	public BalistControllerSet _controls;
	public float _balistHorizontalSpeed = 1.0f;
	public float _balistVerticalSpeed = 1.0f;
	public float _balistZCorrection = 0.35f;

	public Transform[] _wheelsTranformArray;

	// Projectile Variables
	public GameObject projectile;
	public Transform projectileLaunchPoint;
	public float projectileStrenght;

	//Mesh
	public MeshRenderer[] _mesheRendererArray;

	//Light
	public Light _balistLight;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Attack();
		BalistMovement ();
	}

	void FixedUpdate () {
		
	}

	//SET UP
	public void SetBalistColor (Color targetColor){
		for (int i = 0 ; i < _mesheRendererArray.Length ; i++){
			_mesheRendererArray[i].material.color = targetColor;
		}
		_balistLight.color = targetColor;
	}

	// MOVEMENT
	public void BalistMovement (){
	/*	if (Input.GetButton(_controls.BalistHorizontal)){
			print ("Move Forward");
		}else if (Input.GetButton(_controls.BalistHorizontal)){
			print ("Move Backward");
		}
		if (Input.GetButton(_controls.BalistHorizontal)){
			print ("Move Left");
		}else if (Input.GetButton(_controls.BalistHorizontal)){
			print ("Move Right");

		}*/



		//ZPosition
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit)){
			Vector3 tmpCurrentPosition = this.transform.position;
			tmpCurrentPosition.y = hit.point.y + _balistZCorrection;
			this.transform.position = tmpCurrentPosition;
		}

		//Balist Orientation
		Vector3[] tmpWheelTargetPosArray = new Vector3[4];
		for (int i = 0 ; i < _wheelsTranformArray.Length ; i++){
			//Debug.DrawRay(_wheelsTranformArray[i].position, -Vector3.up,Color.white);
			RaycastHit whit;
			if (Physics.Raycast(_wheelsTranformArray[i].position + Vector3.up, -Vector3.up * 10, out whit)){
				Vector3 tmpWheelTargetPos = whit.point;
				tmpWheelTargetPosArray[i] = tmpWheelTargetPos;
			}
		}

		//print ("ArrayLenght : " + tmpWheelTargetPosArray.Length);

		if (tmpWheelTargetPosArray.Length == 4) {
			Vector3 side1 = tmpWheelTargetPosArray [0] - tmpWheelTargetPosArray [1];
			Vector3 side2 = tmpWheelTargetPosArray [0] - tmpWheelTargetPosArray [2];
			Vector3 perp1 = Vector3.Cross(side1, side2) * -1 ;
			Vector3 tmpMiddlePoint1 = ((tmpWheelTargetPosArray [0] + tmpWheelTargetPosArray [1] + tmpWheelTargetPosArray [2])/3);
			//Debug.DrawRay(tmpMiddlePoint1,perp1,Color.blue);

			Vector3 side3 = tmpWheelTargetPosArray [0] - tmpWheelTargetPosArray [1];
			Vector3 side4 = tmpWheelTargetPosArray [0] - tmpWheelTargetPosArray [3];
			Vector3 perp2 = Vector3.Cross(side3, side4) * -1;
			Vector3 tmpMiddlePoint2 = ((tmpWheelTargetPosArray [0] + tmpWheelTargetPosArray [1] + tmpWheelTargetPosArray [3])/3);
			//Debug.DrawRay(tmpMiddlePoint2, perp2,Color.red);

			Vector3 side5 = tmpWheelTargetPosArray [1] - tmpWheelTargetPosArray [2];
			Vector3 side6 = tmpWheelTargetPosArray [1] - tmpWheelTargetPosArray [3];
			Vector3 perp3 = Vector3.Cross(side5, side6);
			Vector3 tmpMiddlePoint3 = ((tmpWheelTargetPosArray [1] + tmpWheelTargetPosArray [2] + tmpWheelTargetPosArray [3])/3);
			//Debug.DrawRay(tmpMiddlePoint3, perp3,Color.green);

			Vector3 side7 = tmpWheelTargetPosArray [0] - tmpWheelTargetPosArray [2];
			Vector3 side8 = tmpWheelTargetPosArray [0] - tmpWheelTargetPosArray [3];
			Vector3 perp4 = Vector3.Cross(side7, side8);
			Vector3 tmpMiddlePoint4 = ((tmpWheelTargetPosArray [0] + tmpWheelTargetPosArray [2] + tmpWheelTargetPosArray [3])/3);
			//Debug.DrawRay(tmpMiddlePoint4, perp4,Color.yellow);

			Vector3 tmpFinalRotationVector = ((perp1 + perp2 + perp3 + perp4) / 4);
			Debug.DrawRay(this.transform.position, tmpFinalRotationVector ,Color.green);
			Debug.DrawRay(this.transform.position, this.transform.up ,Color.red);

			Quaternion tmpTargetRotation = Quaternion.FromToRotation(this.transform.up, tmpFinalRotationVector);
			tmpTargetRotation.Set(tmpTargetRotation.x, this.transform.rotation.y, tmpTargetRotation.z, tmpTargetRotation.w);
			//this.transform.up = Vector3.Lerp(this.transform.rotation.eulerAngles, tmpTargetRotation, Time.smoothDeltaTime * 50);
			this.transform.rotation = Quaternion.LerpUnclamped(this.transform.rotation, tmpTargetRotation, 1);
		}

		// Direction
		Vector3 tmpDirection = Vector3.zero;
		tmpDirection.z = (Input.GetAxis(_controls.BalistVertical)) * _balistVerticalSpeed;
		this.transform.Translate (tmpDirection);

		//Orientation
		Vector3 tmpRotation = Vector3.zero;
		tmpRotation.y = (Input.GetAxis(_controls.BalistHorizontal)) * _balistHorizontalSpeed;
		this.transform.Rotate (tmpRotation);
	}

	// ATTACK
	public void Attack (){
		if (Input.GetKeyDown(KeyCode.Space)){
			GameObject tmpProj = Instantiate(projectile,projectileLaunchPoint.transform.position, projectileLaunchPoint.transform.rotation) as GameObject;
			Physics.IgnoreCollision(tmpProj.GetComponent<Collider>(), balistColl);
			tmpProj.GetComponent<Rigidbody>().AddForce(tmpProj.transform.forward * projectileStrenght);

			//Back Fire

			//Test Attack
			print (Camera.main);
			//Camera.main.GetComponent<CameraBehaviour>().Shake(10.0f, 5.0f);
		}
	}

}
