using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	[SerializeField] private Transform _cameraMaxZoom;
	[SerializeField] private float _cameraSpeed;
	[SerializeField] private float _CamDistRef;
	private Vector3 _cameraTrajectory; 

	//Shaky Cam
	private float _timeLeftToShake;
	private float _shakeDuration;
	private float _shakeStrength;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		CameraPositionner();
	}

	void CameraPositionner (){
		// Initial Position Computing
		Vector3 tmpGravityCenter = Vector3.zero;
		for (int i = 0 ; i < GameManager._singleton._playerNumbers ; i ++){
			Vector3 tmpBalistPosition = GameManager._singleton._playerTransformArray[i].position;
			tmpGravityCenter += tmpBalistPosition;
		}

		tmpGravityCenter = tmpGravityCenter/GameManager._singleton._playerNumbers;
		this.transform.LookAt(tmpGravityCenter);

		float tmpFarestPlayerDistFromGC = 0.0f;
		for (int j = 0 ; j < GameManager._singleton._playerNumbers ; j ++){
			float tmpDistWithGC = Vector3.Distance(GameManager._singleton._playerTransformArray[j].position, tmpGravityCenter);
			if (tmpDistWithGC > tmpFarestPlayerDistFromGC){
				tmpFarestPlayerDistFromGC = tmpDistWithGC;
			}
		}

		_cameraTrajectory = (_cameraMaxZoom.position - tmpGravityCenter);
		Vector3 tmpTargetCamPosition = tmpGravityCenter + (_cameraTrajectory * (tmpFarestPlayerDistFromGC/_CamDistRef));
		float tmpCamSpeed = _cameraSpeed;
		//ShakyCam Modifier
		if (_timeLeftToShake > 0.0f ){
			_timeLeftToShake -= Time.deltaTime;
			float tmpStrengthAttenuation = (_timeLeftToShake/_shakeDuration);
			print (_shakeStrength);
			tmpTargetCamPosition += Random.insideUnitSphere * (_shakeStrength * tmpStrengthAttenuation);
			tmpCamSpeed *= (_shakeStrength/3.0f);
		}
		this.transform.position = Vector3.Lerp(this.transform.position, tmpTargetCamPosition, Time.smoothDeltaTime * tmpCamSpeed);
	}

	public void Shake (float strength, float duration)
	{
		_timeLeftToShake = duration;
		_shakeDuration = duration;
		_shakeStrength = strength;
	}
		
}
