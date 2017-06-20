using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chain{
	public List<GameObject> puyos = new List<GameObject>();

	public void Add(GameObject obj) {
		puyos.Add (obj);
	}

	public int Count() {
		return puyos.Count;
	}

	public void Clean() {
		foreach (GameObject obj in puyos) {
			//Destroy (obj);
		}
		puyos.Clear ();
	}
}
