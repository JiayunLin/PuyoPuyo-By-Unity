using UnityEngine;
using System.Collections;

public class PuyoController : MonoBehaviour {
	private bool[] isSettle = new bool[2] {false, false};
	private Vector3[] offset = new Vector3[4] {
		new Vector3(0, 0, 1f),
		new Vector3(1, 0, 0f),
		new Vector3(0, 0, -1f),
		new Vector3(-1, 0, 0f)
	};
	private int state = 0;

	public int index = 0;
	public GameObject[] puyos = new GameObject[2];

	// Use this for initialization
	void Start () {
		GameManager.Instance.CheckSettle += CheckSettle;
		GameManager.Instance.Down += Down;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isSettle[0] && !isSettle[1] && Input.GetKeyDown (KeyCode.J)) {
			state = (state + 1) % 4;
			puyos[1].transform.position = transform.position + offset[state];
		}
		if (!isSettle [0] && !isSettle [1] && Input.GetKeyDown (KeyCode.LeftArrow)) {
			PadController pad = GameManager.Instance.pads [index].GetComponent<PadController> ();
			if (pad.GetPosOnPad (GetPuyo1Pos ()).x > 0 && pad.GetPosOnPad (GetPuyo2Pos ()).x > 0) {
				transform.position += new Vector3 (-1, 0, 0);
			}
		}
		if (!isSettle [0] && !isSettle [1] && Input.GetKeyDown (KeyCode.RightArrow)) {
			PadController pad = GameManager.Instance.pads [index].GetComponent<PadController> ();
			if (pad.GetPosOnPad (GetPuyo1Pos ()).x < GameManager.Instance.padWidth - 1 && pad.GetPosOnPad (GetPuyo2Pos ()).x < GameManager.Instance.padWidth - 1) {
				transform.position += new Vector3 (1, 0, 0);
			}
		}
		if (!isSettle [0] && !isSettle [1] && Input.GetKey(KeyCode.DownArrow)) {
			CheckSettle ();
			Down ();
		}
	}

	public void Down(){
		transform.position = transform.position - new Vector3 (0, 0, 1);
	}

	public void CheckSettle() {
		bool anyOneSettle = false;
		PadController pad = GameManager.Instance.pads [index].GetComponent<PadController> ();
		PadController.PuyoPos pos1 = pad.GetPosOnPad(GetPuyo1Pos ());
		if (!isSettle[0] && (pos1.y - 1 == -1 || GameManager.Instance.padMaps [index] [pos1.x] [pos1.y - 1] != Puyo.PuyoColor.none)) {
			anyOneSettle = true;
			PuyoSettle (0, pos1.x, pos1.y);
			GameManager.Instance.padMaps [index] [pos1.x] [pos1.y] = puyos [0].GetComponent<LittlePuyo> ().color;
			GameManager.Instance.settlePuyos [index] [pos1.x] [pos1.y] = puyos [0];
		}
		PadController.PuyoPos pos2 = pad.GetPosOnPad(GetPuyo2Pos ());
		if (!isSettle[1] && (pos2.y - 1 == -1 || GameManager.Instance.padMaps [index] [pos2.x] [pos2.y - 1] != Puyo.PuyoColor.none)) {
			anyOneSettle = true;
			PuyoSettle (1, pos2.x, pos2.y);
			GameManager.Instance.padMaps [index] [pos2.x] [pos2.y] = puyos [1].GetComponent<LittlePuyo> ().color;
			GameManager.Instance.settlePuyos [index] [pos2.x] [pos2.y] = puyos [1];
		}
		if (!isSettle[0] && (pos1.y - 1 == -1 || GameManager.Instance.padMaps [index] [pos1.x] [pos1.y - 1] != Puyo.PuyoColor.none)) {
			anyOneSettle = true;
			PuyoSettle (0, pos1.x, pos1.y);
			GameManager.Instance.padMaps [index] [pos1.x] [pos1.y] = puyos [0].GetComponent<LittlePuyo> ().color;
			GameManager.Instance.settlePuyos [index] [pos1.x] [pos1.y] = puyos [0];
		}

		if (anyOneSettle) {
			CheckClean ();
			if (!(isSettle[0] ^ isSettle[1])) {
				GameManager.Instance.CurrentPuyoSettle (index);
			}
		}
	}

	public Vector3 GetPuyo1Pos() {
		return puyos[0].transform.position;
	}

	public Vector3 GetPuyo2Pos() {
		return puyos[1].transform.position;
	}

	public void PuyoSettle(int i, int x, int y) {
		puyos [i].transform.SetParent (null);
		puyos [i].GetComponent<LittlePuyo> ().x = x;
		puyos [i].GetComponent<LittlePuyo> ().y = y;
		isSettle[i] = true;

		if (isSettle [0] && isSettle [1]) {
			GameManager.Instance.CheckSettle -= CheckSettle;
			Invoke ("DestroySlef", 30);
		}
	}

	public void CheckClean() {
		GameManager.Instance.CheckClean (index);
	}

	private void DestroySelf(){
		GameManager.Instance.CheckSettle -= CheckSettle;
		GameManager.Instance.Down -= Down;
		Destroy (gameObject);
	}
}
