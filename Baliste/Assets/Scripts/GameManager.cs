using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	//Singleton
	public static GameManager _singleton;

	// Players Variables
	public GameObject _balistPrefab;
	public Transform[] _spawningPositions;
	public int _playerNumbers;
	public GameObject[] _playersArray;
	public Transform[] _playerTransformArray;

	public Color[] _playerColors;

	void Awake () {
		if (_singleton == null){
			_singleton = this;
		}
	}

	// Use this for initialization
	void Start () {
	
		GameLaunch();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Game Launch
	public void GameLaunch (){

		if (_playerNumbers > _spawningPositions.Length){
			_playerNumbers = _spawningPositions.Length;
		}

		_playersArray = new GameObject[_playerNumbers];
		_playerTransformArray = new Transform[_playerNumbers];

		for (int i = 0 ; i < _playerNumbers ; i++){
			GameObject tmpBalist = Instantiate(_balistPrefab, _spawningPositions[i].position, Quaternion.identity) as GameObject;
			BalistBehaviour tmpBalistBehaviour = tmpBalist.GetComponent<BalistBehaviour>();
			_playersArray[i] = tmpBalist;
			_playerTransformArray[i] = tmpBalist.transform;
			tmpBalistBehaviour.SetBalistColor(_playerColors[i]);
			tmpBalistBehaviour._controls = new BalistBehaviour.BalistControllerSet(("Player" + (i+1) as string + "Horizontal"), ("Player" + (i+1) as string + "Vertical"), ("Player" + (i+1) as string + "Input"));
		}
	}
}
