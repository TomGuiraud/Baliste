

using UnityEngine;
using System.Collections;

public class ProjectileBehaviour : MonoBehaviour {

	public enum ProjectileState {Alive,Dead}
	public ProjectileState _currentProjectileState;

	public Transform _projectileHeadTransform;

	public MeshRenderer[] _meshRenrederers; 

    public Vector3 _initDirection;
    public float _initFiringAngle;
    public float _initForce;
    public float _gravityValue;

    public Vector3 _initPosition;
    public float _projectileProgression;

    private float _Vox;
    private float _Voy;
    private float _lifeTime = 0.0f;

	public ParticleSystem _explosionFX;

	public float _explosionRange;

    // Use this for initialization
    void Start () {
        _initPosition = this.transform.position;
        _Vox = Mathf.Cos(_initFiringAngle) * _initForce;
        _Voy = Mathf.Sin(_initFiringAngle) * _initForce;

		_currentProjectileState = ProjectileState.Alive;
    }
	
	// Update is called once per frame
	void Update () {
		
		_lifeTime += Time.deltaTime;

		if (_currentProjectileState != ProjectileState.Dead){
			ProjectileMovement(_lifeTime);
		}

		if (_lifeTime > 1.0f && _currentProjectileState != ProjectileState.Dead){
			ProjectileExplosion();
		}


		if (this.transform.position.y < -10)
		{
			Destroy(this.gameObject);
			print ("Fail to collide ground | >Emergency destroy");
		}
	}

	///////////////////////////////////////////////////////////////////////PROJECTILE STATE MANAGER/////////////////////////////////////////////////////////////////////////////////////////
	void SetProjectileState (ProjectileState targetState){
		_currentProjectileState = targetState;
		if (targetState == ProjectileState.Dead){
			//Disable all meshes
			for (int i = 0 ; i < _meshRenrederers.Length ; i++){
				_meshRenrederers[i].enabled = false;
			}
		}
	}


	///////////////////////////////////////////////////////////////////////MOVEMENT AND ORIENTATION/////////////////////////////////////////////////////////////////////////////////////////

	void ProjectileMovement (float progression) {

        Vector3 speed = ((_initDirection * _Vox) + (Vector3.up * _Voy + (Vector3.up * _gravityValue * _lifeTime)));
        Vector3 pos = _initPosition + (speed * _lifeTime);

        Vector3 nextSpeed = ((_initDirection * _Vox) + (Vector3.up * _Voy + (Vector3.up * _gravityValue * (_lifeTime + 0.1f))));
        Vector3 nextPos = _initPosition + (nextSpeed * (_lifeTime + 0.1f));

        this.transform.position = pos;

        ProjectileOrientation(nextPos - pos);
    }

    void ProjectileOrientation(Vector3 direction)
    {
        transform.LookAt(this.transform.position + direction);
    }

	///////////////////////////////////////////////////////////////////////EXPLOSION/////////////////////////////////////////////////////////////////////////////////////////

	void ProjectileExplosion ()
	{
		Vector3 tmpRandom = new Vector3(Random.Range(0.0f,3.0f), Random.Range(0.0f,3.0f), Random.Range(0.0f,3.0f));

		Vector3[] tmpRayDirectionArray = new Vector3[3];
		tmpRayDirectionArray[0] = Vector3.Cross(transform.forward, tmpRandom);
		tmpRayDirectionArray[1] = Vector3.Cross(transform.right, tmpRandom);
		tmpRayDirectionArray[2] = Vector3.Cross(transform.up, tmpRandom);

		int[] tmpMultiplier = new int[2];
		tmpMultiplier[0] = 1;
		tmpMultiplier[1] = -1;

		for (int i = 0; i < tmpRayDirectionArray.Length; i++)
		{
			for (int j = 0; j < tmpMultiplier.Length; j++)
			{
				RaycastHit hit;
				Debug.DrawRay(_projectileHeadTransform.position, tmpRayDirectionArray[i] * tmpMultiplier[j] * 0.5f, Color.red);
				if (Physics.Raycast(_projectileHeadTransform.position, tmpRayDirectionArray[i] * tmpMultiplier[j], out hit, 0.5f))
				{
					SetProjectileState (ProjectileState.Dead);
					_explosionFX.Play();
					ComputeDamage();
					print ("Collide " + hit.collider.name);
				}
			}
		}

	}

	void ComputeDamage (){
		for (int i = 0; i < GameManager._singleton._playerTransformArray.Length; i++){
			if (Vector3.Distance(this.transform.position,GameManager._singleton._playerTransformArray[i].position) < _explosionRange){
				GameManager._singleton._balistBehaviorArray[i].SetDamage(1);
				CameraBehaviour._singleton.Shake(10.0f,2.0f);
			}
		}
	}

}
