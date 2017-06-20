using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuyoCreator : MonoBehaviour {
	//privte static volatile PuyoCreator _instance;
	//private static object _lock = new object();
	//public static PuyoCreator Instance {
	//	get {
	//		if (_)
	//	}
	//}
	Queue<Puyo>[] puyoQ = new Queue<Puyo>[GameManager.playerNum];

	// Use this for initialization
	void Start () {
		for (int i = 0; i < puyoQ.Length; i++) {
			puyoQ [i] = new Queue<Puyo> ();
		}
		AddPuyo ();
		AddPuyo ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	//当一方queue少于2个，添加一个到双方queue里
	public void AddPuyo() {
		Puyo newPuyo = new Puyo ();
		for (int i = 0; i < puyoQ.Length; i++) {
			puyoQ [i].Enqueue (newPuyo);
		}
	}

	public Puyo GetNextPuyo(int index) {
		if (puyoQ [index].Count > 0) {
			if (puyoQ [index].Count <= 3) {
				AddPuyo ();
			}
			return puyoQ [index].Dequeue ();
		} else {
			Debug.LogError ("Empty Queue of " + index);
		}
		return null;
	}
}
