using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BalistBehaviour : MonoBehaviour {


	//Balist Components
	public int balistIndex;
	public Rigidbody balistRB;
	public Collider balistColl;
	public Color balistColor;

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
	public float _timeToReachMaxSpeed;
	private float _timeSinceBalistIsMoving;
	public float _balistVerticalSpeed = 1.0f;
	public float _balistZCorrection = 0.35f;

	public Transform[] _wheelsTranformArray;

	// Projectile Variables
	public GameObject projectile;
	public Transform projectileLaunchPoint;
	public float _projectileStrenght;
	public float _projectileGravityValue;

	//Trajectory Feedback
	public LineRenderer _trajFeedback;
	public int _feedbackLength;
	public float _feedbackStep = 0.016f;

	//Attack Feedback
	public ParticleSystem _attackFeedback;

	//Wheel Feedbacks
	public ParticleSystem[] _wheelBrakingFeedbacks;
	public bool _isWheelBrakingFeedbackActivated;

	//Mesh
	public MeshRenderer[] _mesheRendererArray;

	//Light
	public Light _balistLight;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////FUNCTIONS///////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// Use this for initialization
	void Start () {
		_isWheelBrakingFeedbackActivated = false;
	
	}
	
	// Update is called once per frame
	void Update () {
		Attack();
		BalistMovement ();
		ComputeFeedback ();
	}

	void FixedUpdate () {
		
	}

	//SET UP
	public void SetBalistColor (Color targetColor){
		balistColor = targetColor;
		for (int i = 0 ; i < _mesheRendererArray.Length ; i++){
			_mesheRendererArray[i].material.color = targetColor;
		}
		_balistLight.color = targetColor;

		_trajFeedback.SetColors(balistColor,balistColor);
	}

	//////////////////////////////////////////////////////////////////////////////////////////MOVEMENT//////////////////////////////////////////////////////////////////////////////////////////
    public void BalistMovement (){
    	//ZPosition
    	RaycastHit hit;
    	if (Physics.Raycast(transform.position, -Vector3.up, out hit))
    	{
    	    Vector3 tmpCurrentPosition = this.transform.position;
    	    tmpCurrentPosition.y = hit.point.y + _balistZCorrection;
    	    this.transform.position = tmpCurrentPosition;
    	}
		
    	//Balist Orientation
    	Vector3[] tmpWheelTargetPosArray = new Vector3[4];
    	for (int i = 0; i < _wheelsTranformArray.Length; i++)
    	{
    	    RaycastHit whit;
    	    if (Physics.Raycast(_wheelsTranformArray[i].position + (Vector3.up * 5.0f), -Vector3.up, out whit, 10.0f ,(LayerMask.GetMask("Ground"))))
    	    {
    	        tmpWheelTargetPosArray[i] = whit.point;
    	    }
    	}
			
    	Vector3 side1 = tmpWheelTargetPosArray[0] - tmpWheelTargetPosArray[1];
    	Vector3 side2 = tmpWheelTargetPosArray[0] - tmpWheelTargetPosArray[2];
    	Vector3 perp1 = Vector3.Cross(side1, side2) * -1;
    	Vector3 tmpMiddlePoint1 = ((tmpWheelTargetPosArray[0] + tmpWheelTargetPosArray[1] + tmpWheelTargetPosArray[2]) / 3);
    	Debug.DrawRay(tmpMiddlePoint1, perp1, Color.blue);
		
    	Vector3 side3 = tmpWheelTargetPosArray[0] - tmpWheelTargetPosArray[1];
    	Vector3 side4 = tmpWheelTargetPosArray[0] - tmpWheelTargetPosArray[3];
    	Vector3 perp2 = Vector3.Cross(side3, side4) * -1;
    	Vector3 tmpMiddlePoint2 = ((tmpWheelTargetPosArray[0] + tmpWheelTargetPosArray[1] + tmpWheelTargetPosArray[3]) / 3);
    	Debug.DrawRay(tmpMiddlePoint2, perp2, Color.red);
		
    	Vector3 side5 = tmpWheelTargetPosArray[1] - tmpWheelTargetPosArray[2];
    	Vector3 side6 = tmpWheelTargetPosArray[1] - tmpWheelTargetPosArray[3];
    	Vector3 perp3 = Vector3.Cross(side5, side6);
    	Vector3 tmpMiddlePoint3 = ((tmpWheelTargetPosArray[1] + tmpWheelTargetPosArray[2] + tmpWheelTargetPosArray[3]) / 3);
    	Debug.DrawRay(tmpMiddlePoint3, perp3, Color.green);
		
    	Vector3 side7 = tmpWheelTargetPosArray[0] - tmpWheelTargetPosArray[2];
    	Vector3 side8 = tmpWheelTargetPosArray[0] - tmpWheelTargetPosArray[3];
    	Vector3 perp4 = Vector3.Cross(side7, side8);
    	Vector3 tmpMiddlePoint4 = ((tmpWheelTargetPosArray[0] + tmpWheelTargetPosArray[2] + tmpWheelTargetPosArray[3]) / 3);
    	Debug.DrawRay(tmpMiddlePoint4, perp4, Color.yellow);
		
    	Vector3 tmpFinalRotationVector = ((perp1 + perp2 + perp3 + perp4) / 4);
    	this.transform.rotation = Quaternion.FromToRotation(this.transform.up, tmpFinalRotationVector) * this.transform.rotation;
		
		
    	// Direction
    	Vector3 tmpDirection = Vector3.zero;

		int tmpSign = Math.Sign(Input.GetAxis(_controls.BalistVertical));
		if (Mathf.Abs(Input.GetAxis(_controls.BalistVertical)) >= 0.1f)
		{
			if (Mathf.Abs(_timeSinceBalistIsMoving) < _timeToReachMaxSpeed)
			{
				_timeSinceBalistIsMoving += Time.deltaTime * tmpSign;
				if (Mathf.Abs(_timeSinceBalistIsMoving) > _timeToReachMaxSpeed)
				{
					_timeSinceBalistIsMoving = _timeToReachMaxSpeed * tmpSign;
				}
			}

			//BrakingFeedback
			if ((Mathf.Sign(Input.GetAxis(_controls.BalistVertical)) * -1) == Mathf.Sign(_timeSinceBalistIsMoving))
			{
				SetWheelBrackingFeedback(true);
				print ("Activate Braking Feedback");
			}else{
				print ("Disable Braking Feedback");
				SetWheelBrackingFeedback(false);
			}

		}else{
			_timeSinceBalistIsMoving += Time.deltaTime * Math.Sign(_timeSinceBalistIsMoving) * -1;
		}

		//print (_timeSinceBalistIsMoving);

		tmpDirection.z = (Input.GetAxis(_controls.BalistVertical) * (_balistVerticalSpeed/_timeToReachMaxSpeed)) + (_balistVerticalSpeed * (_timeSinceBalistIsMoving/ _timeToReachMaxSpeed));
		print (tmpDirection.z);
		this.transform.Translate(tmpDirection);


		
    	//Orientation
    	Vector3 tmpRotation = Vector3.zero;
    	tmpRotation.y = (Input.GetAxis(_controls.BalistHorizontal)) * _balistHorizontalSpeed;
    	this.transform.Rotate(tmpRotation);
	}

	public void SetWheelBrackingFeedback (bool shoulBeActivated){
		for (int i = 0 ; i < _wheelBrakingFeedbacks.Length ; i++){
			if (shoulBeActivated){
				_wheelBrakingFeedbacks[i].Play();
			}else{
				_wheelBrakingFeedbacks[i].Stop();
			}
		}

	}

	//////////////////////////////////////////////////////////////////////////////////////////ATTACK//////////////////////////////////////////////////////////////////////////////////////////

	public void Attack (){
		//if (Input.GetKeyDown(KeyCode.Space)){
		if (Input.GetButtonDown(_controls.BalistInput)){
			GameObject tmpProj = Instantiate(projectile,projectileLaunchPoint.transform.position, projectileLaunchPoint.transform.rotation) as GameObject;
            ProjectileBehaviour tmpProjBH = tmpProj.GetComponent<ProjectileBehaviour>();

            Vector3 tmpTargetDirection = projectileLaunchPoint.forward;
            tmpTargetDirection.y = 0.0f;
            tmpTargetDirection.Normalize();
            tmpProjBH._initDirection = tmpTargetDirection;

            tmpProjBH._initFiringAngle = Vector3.Angle(projectileLaunchPoint.forward, tmpTargetDirection) * Mathf.Deg2Rad;
			tmpProjBH._initForce = _projectileStrenght;
			tmpProjBH._gravityValue = _projectileGravityValue;

			CameraBehaviour._singleton.Shake(3.0f,0.5f);
			_attackFeedback.Play();
        }
	}

    public void ComputeFeedback()
    {
        Vector3 tmpTargetDirection = projectileLaunchPoint.forward;
        tmpTargetDirection.y = 0.0f;
        tmpTargetDirection.Normalize();

        float tmpFiringAngle = Vector3.Angle(projectileLaunchPoint.forward, tmpTargetDirection) * Mathf.Deg2Rad;
		float tmpVox = Mathf.Cos(tmpFiringAngle) * _projectileStrenght;
		float tmpVoy = Mathf.Sin(tmpFiringAngle) * _projectileStrenght;

        Vector3 initialPos = projectileLaunchPoint.position;
        Vector3 pos = projectileLaunchPoint.position;
		_trajFeedback.SetVertexCount(_feedbackLength);
        _trajFeedback.SetPosition(0, pos);

		for (int i = 1; i < _feedbackLength; i++)
        {
			pos = projectileLaunchPoint.transform.position + (((tmpTargetDirection * tmpVox) + (Vector3.up * tmpVoy + (Vector3.up * _projectileGravityValue * i * _feedbackStep))) * i * _feedbackStep);
            /*pos += tmpTargetDirection * tmpVox * step; // Deplacement horizontal
            pos += Vector3.up * tmpVoy * step; // Deplacement vertical*/

            _trajFeedback.SetPosition(i, pos);
        }
    }

	//////////////////////////////////////////////////////////////////////////////////////////DAMAGE//////////////////////////////////////////////////////////////////////////////////////////

	public void SetDamage (int damageAmount){
		GameManager._singleton.DestroyPlayer(balistIndex);
	}

}
