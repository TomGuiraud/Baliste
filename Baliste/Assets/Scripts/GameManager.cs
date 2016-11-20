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
	public BalistBehaviour[] _balistBehaviorArray;

	//In Game Variables
	public int _currentPlayerAliveNumber;

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
		_balistBehaviorArray = new BalistBehaviour[_playerNumbers];
		_currentPlayerAliveNumber = _playerNumbers;

		for (int i = 0 ; i < _playerNumbers ; i++){
			GameObject tmpBalist = Instantiate(_balistPrefab, _spawningPositions[i].position, Quaternion.identity) as GameObject;
			BalistBehaviour tmpBalistBehaviour = tmpBalist.GetComponent<BalistBehaviour>();
			_playersArray[i] = tmpBalist;
			_playerTransformArray[i] = tmpBalist.transform;
			_balistBehaviorArray[i] = tmpBalistBehaviour;
			tmpBalistBehaviour.SetBalistColor(_playerColors[i]);
			tmpBalistBehaviour.balistIndex = i;
			tmpBalistBehaviour._controls = new BalistBehaviour.BalistControllerSet(("Player" + (i+1) as string + "Horizontal"), ("Player" + (i+1) as string + "Vertical"), ("Player" + (i+1) as string + "Input"));
		}
	}


	// In Game Effects
	public void DestroyPlayer (int playerToDestroyIndex){

		Destroy(_playersArray[playerToDestroyIndex].gameObject);

		//Reconstruct array without the destroyed player
		_currentPlayerAliveNumber -= 1;
		GameObject[] tmpNewPlayersArray = new GameObject[_currentPlayerAliveNumber];
		Transform[] tmpNewPlayerTransformArray = new Transform[_currentPlayerAliveNumber];
		BalistBehaviour[] tmpNewBalistBehaviorArray = new BalistBehaviour[_currentPlayerAliveNumber];

		int tmpIndexToFill = 0;
		for (int i = 0 ; i < (_currentPlayerAliveNumber + 1) ; i ++){
			if (i != playerToDestroyIndex){
				tmpNewPlayersArray[tmpIndexToFill] = _playersArray[i];
				tmpNewPlayerTransformArray[tmpIndexToFill] = _playerTransformArray[i];
				tmpNewBalistBehaviorArray[tmpIndexToFill] = _balistBehaviorArray[i];
				_balistBehaviorArray[i].balistIndex = tmpIndexToFill;

				tmpIndexToFill ++;
			}
		}
		_playersArray = tmpNewPlayersArray;
		_playerTransformArray = tmpNewPlayerTransformArray;
		_balistBehaviorArray = tmpNewBalistBehaviorArray;


		if (_currentPlayerAliveNumber <= 1){
			Application.LoadLevel(0);
		}

	}
}
