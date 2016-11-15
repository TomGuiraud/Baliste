using UnityEngine;
using System;
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
    public float _MaxWheelHeightValue = 1.0f;

	public Transform[] _wheelsTranformArray;

	// Projectile Variables
	public GameObject projectile;
	public Transform projectileLaunchPoint;

	//Trajectory Feedback
	public LineRenderer _trajFeedback;

	//Mesh
	public MeshRenderer[] _mesheRendererArray;

	//Light
	public Light _balistLight;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////FUNCTIONS///////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Attack();
		BalistMovement ();
		ComputeTrajectory ();
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
    struct Wheel
    {

        public float distance;
        public Vector3 point;
    };

    public bool lol = true;

    public void BalistMovement (){
        //for (int u = 0; u < 12; u++)
        {

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
            Wheel[] tmpRayDistanceArray = new Wheel[4];
            for (int i = 0; i < _wheelsTranformArray.Length; i++)
            {
                RaycastHit whit;
                if (Physics.Raycast(_wheelsTranformArray[i].position + (Vector3.up * 5.0f), -Vector3.up, out whit, 10.0f ,(LayerMask.GetMask("Ground"))))
                {
                    tmpWheelTargetPosArray[i] = whit.point;
                    tmpRayDistanceArray[i] = new Wheel();
                    tmpRayDistanceArray[i].distance = whit.distance;

                    tmpRayDistanceArray[i].point = whit.point;
                }
            }

            Vector3[] tmpWheelPointToProcess = new Vector3[3];
            tmpWheelPointToProcess[0] = tmpRayDistanceArray[1].point;
            tmpWheelPointToProcess[1] = tmpRayDistanceArray[2].point;
            if (tmpRayDistanceArray[0].distance < tmpRayDistanceArray[3].distance)
            {
                tmpWheelPointToProcess[2] = tmpRayDistanceArray[0].point;
                //Debug.DrawRay(tmpRayDistanceArray[3].point, Vector3.up, Color.red);
            }
            else
            {
                tmpWheelPointToProcess[2] = tmpRayDistanceArray[3].point;
                //Debug.DrawRay(tmpRayDistanceArray[0].point, Vector3.up, Color.red);
            }



            bool tmpThreeWheelsCompute = false;
            if (tmpThreeWheelsCompute)
            {
                Vector3 side1 = tmpWheelPointToProcess[0] - tmpWheelPointToProcess[1];
                Vector3 side2 = tmpWheelPointToProcess[0] - tmpWheelPointToProcess[2];
                Vector3 perp1 = Vector3.Cross(side1, side2) * -1;
                if (perp1.y < 0.0f)
                {
                    perp1 = -perp1;
                }

                Vector3 tmpMiddlePoint = ((tmpWheelPointToProcess[0] + tmpWheelPointToProcess[1] + tmpWheelPointToProcess[2]) / 3);
                Debug.DrawRay(tmpMiddlePoint, perp1, Color.blue);

                Quaternion tmpCurrentRotation = this.transform.rotation;
                this.transform.rotation = Quaternion.FromToRotation(this.transform.up, perp1) * this.transform.rotation;
            }

            bool tmpFourWheelsCompute = true;
            if (tmpFourWheelsCompute)
            {
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
                //Debug.DrawRay(this.transform.position, tmpFinalRotationVector ,Color.green);
                //Debug.DrawRay(this.transform.position, this.transform.up ,Color.red);

                //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.FromToRotation(this.transform.up, tmpFinalRotationVector) * this.transform.rotation, _MaxWheelHeightValue);
                this.transform.rotation = Quaternion.FromToRotation(this.transform.up, tmpFinalRotationVector) * this.transform.rotation;

                //print("Old rotation : " + tmpCurrentRotation + "| New Rotation : " + this.transform.rotation);

            }

            // Direction
            Vector3 tmpDirection = Vector3.zero;
            tmpDirection.z = (Input.GetAxis(_controls.BalistVertical)) * _balistVerticalSpeed;
            this.transform.Translate(tmpDirection);

            //Orientation
            if (lol)
            {
                Vector3 tmpRotation = Vector3.zero;
                tmpRotation.y = (Input.GetAxis(_controls.BalistHorizontal)) * _balistHorizontalSpeed;
                this.transform.Rotate(tmpRotation);
            }
        }
	}

	// ATTACK
	public void Attack (){
		if (Input.GetKeyDown(KeyCode.Space)){
			GameObject tmpProj = Instantiate(projectile,projectileLaunchPoint.transform.position, projectileLaunchPoint.transform.rotation) as GameObject;
		}
	}

    public void ComputeTrajectory()
    {
        Vector3 tmpTargetDirection = transform.forward;
        tmpTargetDirection.y = 0.0f;
        tmpTargetDirection.Normalize();

        float tmpFiringAngle = Vector3.Angle(projectileLaunchPoint.forward, tmpTargetDirection) * Mathf.Deg2Rad;
        float tmpProjectileForce = 13.0f;
        float gravity = -10.0f;
        float tmpVoy = Mathf.Sin(tmpFiringAngle) * tmpProjectileForce;
        float tmpVox = Mathf.Cos(tmpFiringAngle) * tmpProjectileForce;

        Vector3 pos = projectileLaunchPoint.position;
        float step = 0.1f;
        _trajFeedback.SetVertexCount(40);
        _trajFeedback.SetPosition(0, pos);
        for (int i = 1; i < 40; i++)
        {
            tmpVoy += gravity * step;

            pos += tmpTargetDirection * tmpVox * step; // Deplacement horizontal
            pos += Vector3.up * tmpVoy * step; // Deplacement vertical

            _trajFeedback.SetPosition(i, pos);
        }


    }
}
