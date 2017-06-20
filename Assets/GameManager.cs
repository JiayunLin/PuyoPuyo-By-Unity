using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	static GameManager _instance;
	enum PlayState {
		ready,
		playing,
		end,
	}

	public PuyoCreator puyoCreator;
	public GameObject[] pads;

	public static int playerNum = 1;
	private PlayState state;
	public int padWidth = 6;
	public int padHeigh = 12;
	public Dictionary<int, Puyo.PuyoColor[][]> padMaps = new Dictionary<int, Puyo.PuyoColor[][]>();
	public Dictionary<int, GameObject[][]> settlePuyos = new Dictionary<int, GameObject[][]>();
	private uint frameInterval = 0;
	private uint interval = 15;

	public Dictionary<Puyo.PuyoColor, GameObject> puyos = new Dictionary<Puyo.PuyoColor, GameObject> ();

	private GameObject[] currentPuyos = new GameObject[playerNum];

	public Action CheckSettle;
	public Action Down;

	void Awake() {
		_instance = this;
	}

	public static GameManager Instance{
		get {
			return _instance;
		}
	}

	// Use this for initialization
	void Start () {
		for (int i = 0; i < playerNum; i++) {
			padMaps[i] = new Puyo.PuyoColor[padWidth][];
			settlePuyos[i] = new GameObject[padWidth][];
			for (int j = 0; j < padMaps [i].Length; j++) {
				padMaps [i][j] = new Puyo.PuyoColor[padHeigh + 1];
				settlePuyos[i][j] = new GameObject[padHeigh];
			}
		}

		puyos [Puyo.PuyoColor.blue] = Resources.Load ("prefab/blue") as GameObject;
		puyos [Puyo.PuyoColor.green] = Resources.Load ("prefab/green") as GameObject;
		puyos [Puyo.PuyoColor.red] = Resources.Load ("prefab/red") as GameObject;
		puyos [Puyo.PuyoColor.yellow] = Resources.Load ("prefab/yellow") as GameObject;

		puyoCreator = GetComponent<PuyoCreator> ();
		state = PlayState.ready;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			StartGame ();
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			CheckClean (0);
		}
	}

	void FixedUpdate() {
		frameInterval++;
		if (frameInterval >= interval) {
			frameInterval = 0;
			DoSomething ();
		}
	}

	void StartGame() {
		state = PlayState.playing;
	}

	bool isFail(int index) {
		for (int i = 0; i < padWidth; i++) {
			if (padMaps [index] [i] [padHeigh + 1] != Puyo.PuyoColor.none) {
				return true;
			}
		}
		return false;
	}

	void DoSomething() {
		for (int i = 0; i < currentPuyos.Length; i++) {
			if (currentPuyos [i] == null) {
				CreatePuyo (i);
			}
		}
			
		if (CheckSettle != null) {
			CheckSettle ();
		}
			
		if (Down != null) {
			Down ();
		}
	}

	void CreatePuyo(int index) {
		Puyo puyo = puyoCreator.GetNextPuyo (index);
		GameObject twoPuyos = new GameObject ();
		twoPuyos.name = "puyos";
		twoPuyos.transform.SetParent (pads[index].transform);
		twoPuyos.transform.position = pads [index].GetComponent<PadController> ().GetStartPos ();
		PuyoController controller = twoPuyos.AddComponent<PuyoController>();
		controller.index = index;
		GameObject puyo1 = Instantiate (puyos[puyo.colors[0]], Vector3.zero, Quaternion.identity) as GameObject;
		GameObject puyo2 = Instantiate (puyos[puyo.colors[1]], Vector3.zero, Quaternion.identity) as GameObject;
		puyo1.GetComponent<LittlePuyo>().color = puyo.colors[0];
		puyo2.GetComponent<LittlePuyo>().color = puyo.colors[1];
		controller.puyos [0] = puyo1;
		controller.puyos[1] = puyo2;
		puyo1.transform.SetParent (twoPuyos.transform);
		puyo1.transform.localPosition = Vector3.zero;
		puyo2.transform.SetParent (twoPuyos.transform);
		puyo2.transform.localPosition =new Vector3(0, 0, 1f);

		currentPuyos [index] = twoPuyos;
	}
		
	public void CurrentPuyoSettle(int index) {
		currentPuyos [index] = null;
	}

	public void CheckClean(int index) {
		Dictionary<string, Chain> chains = new Dictionary<string, Chain> ();

		for (int i = 0; i < padWidth; i++) {
			for (int j = 0; j < padHeigh; j++) {
				Debug.Log ("a1");
				if (padMaps[index][i][j] != Puyo.PuyoColor.none) {
					Debug.Log ("a2");
					string key = i.ToString () + j.ToString();
					string leftKey = (i - 1).ToString () + j.ToString();
					string downKey = (i).ToString () + (j - 1).ToString();
					if (i > 0 && j > 0) {
						if (padMaps [index] [i] [j] == padMaps [index] [i - 1] [j]) {
							chains [leftKey].Add (settlePuyos [index] [i] [j]);
							chains [key] = chains [leftKey];
							if (padMaps [index] [i] [j] == padMaps [index] [i] [j - 1]) {
								for (int k = 0; k < chains[downKey].Count(); k++) {
									Debug.Log (1);
									GameObject obj = chains[downKey].puyos[k];
									chains [leftKey].Add (obj);
									chains [obj.GetComponent<LittlePuyo> ().x.ToString () + obj.GetComponent<LittlePuyo> ().y] = chains [leftKey];
								}
								chains [downKey].puyos.Clear ();
							}
						} else if (padMaps [index] [i] [j] == padMaps [index] [i] [j - 1]) {
							chains [downKey].Add (settlePuyos [index] [i] [j]);
							chains [key] = chains [downKey];
						} else {
							chains [key] = new Chain ();
							chains [key].Add (settlePuyos [index] [i] [j]);
						}
					} else if (i > 0) {
						if (padMaps [index] [i] [j] == padMaps [index] [i - 1] [j]) {
							chains [leftKey].Add (settlePuyos [index] [i] [j]);
							chains [key] = chains [leftKey];
						} else {
							chains [key] = new Chain ();
							chains [key].Add (settlePuyos [index] [i] [j]);
						}
					} else if (j > 0) {
						if (padMaps [index] [i] [j] == padMaps [index] [i] [j - 1]) {
							chains [downKey].Add (settlePuyos [index] [i] [j]);
							chains [key] = chains [downKey];
						} else {
							chains [key] = new Chain ();
							chains [key].Add (settlePuyos [index] [i] [j]);
						}
					} else {
						chains [key] = new Chain ();
						chains [key].Add (settlePuyos [index] [i] [j]);
					}
				}
			}
		}
		Debug.Log ("b1");
		string s = "";
		foreach (var kv in chains) {
			s += kv.Value.puyos.Count + "_";
			if (kv.Value.puyos.Count >= 4) {
				for (int i = 0; i < kv.Value.puyos.Count; i++) {
					Debug.Log (2);
					//Debug.Log ();
					GameObject puyo = kv.Value.puyos[i];
					padMaps [index] [puyo.GetComponent<LittlePuyo> ().x] [puyo.GetComponent<LittlePuyo> ().y] = Puyo.PuyoColor.none;
					settlePuyos [index] [puyo.GetComponent<LittlePuyo> ().x] [puyo.GetComponent<LittlePuyo> ().y] = null;
					Destroy (kv.Value.puyos[i]);
				}
				kv.Value.puyos.Clear ();
			}
		}

		reSettle (index);
			
		Debug.Log (s);
	}

	private void reSettle(int index) {
		for (int i = 0; i < padWidth; i++) {
			int bottom = -1;
			for (int j = 0; j < padHeigh; j++) {
				if (bottom == -1 && padMaps [index] [i] [j] == Puyo.PuyoColor.none) {
					bottom = j;
				}
				if (bottom != -1 && padMaps [index] [i] [j] != Puyo.PuyoColor.none) {
					padMaps [index] [i] [bottom] = padMaps [index] [i] [j];
					settlePuyos [index] [i] [bottom] = settlePuyos [index] [i] [j];
					settlePuyos [index] [i] [j].transform.position -= new Vector3 (0, 0, j - bottom);
					padMaps [index] [i] [j] = Puyo.PuyoColor.none;
					settlePuyos [index] [i] [j] = null;
					bottom++;
				}
			}
		}
	}
}
