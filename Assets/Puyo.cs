using UnityEngine;
using System.Collections;

public class Puyo {
	public enum PuyoColor {
		none,
		red,
		blue,
		green,
		yellow,
	}

	public PuyoColor[] colors = new PuyoColor[2];

	public Puyo() {
		colors [0] = (PuyoColor)Random.Range (1, 4);
		colors [1] = (PuyoColor)Random.Range (1, 4);
	}
}
