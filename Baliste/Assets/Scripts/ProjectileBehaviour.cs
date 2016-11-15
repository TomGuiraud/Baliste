

using UnityEngine;
using System.Collections;

public class ProjectileBehaviour : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		ProjectileMovement();
		if (this.transform.position.y < -10)
		{
			Destroy(this.gameObject);
		}
	}

	void ProjectileMovement () {

	}

}
