

using UnityEngine;
using System.Collections;

public class ProjectileBehaviour : MonoBehaviour {

    public Vector3 _initDirection;
    public float _initFiringAngle;
    public float _initForce;
    public float _gravityValue;

    public Vector3 _initPosition;
    public float _projectileProgression;

    float _Vox;
    float _Voy;
    float _lifeTime = 0.0f;

    // Use this for initialization
    void Start () {
        _initPosition = this.transform.position;
        _Vox = Mathf.Cos(_initFiringAngle) * _initForce;
        _Voy = Mathf.Sin(_initFiringAngle) * _initForce;
    }
	
	// Update is called once per frame
	void Update () {
        ProjectileMovement(Time.deltaTime);

		if (this.transform.position.y < -10)
		{
			Destroy(this.gameObject);
		}
	}

	void ProjectileMovement (float progression) {

        _lifeTime += progression;
        Vector3 speed = ((_initDirection * _Vox) + (Vector3.up * _Voy + (Vector3.up * _gravityValue * _lifeTime)));
        Vector3 pos = _initPosition + (speed * _lifeTime);

        Vector3 nextSpeed = ((_initDirection * _Vox) + (Vector3.up * _Voy + (Vector3.up * _gravityValue * (_lifeTime + 0.1f))));
        Vector3 nextPos = _initPosition + (nextSpeed * (_lifeTime + 0.1f));

        /*_Voy += _gravityValue * progression;
         
        pos += _Vox * _initDirection * progression; // Deplacement horizontal
        pos += Vector3.up * _Voy * progression; // Deplacement vertical*/

        this.transform.position = pos;

        ProjectileOrientation(nextPos - pos);
    }

    void ProjectileOrientation(Vector3 direction)
    {
        transform.LookAt(this.transform.position + direction);
    }

}
