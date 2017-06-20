using UnityEngine;
using System.Collections;

public class PadController : MonoBehaviour {
	Transform startPoint;
	Transform zero;

	public struct PuyoPos {
		public int x;
		public int y;

		public PuyoPos(float _x, float _y) {
			x = (int)_x;
			y = (int)_y;
		}
	}

	// Use this for initialization
	void Start () {
		startPoint = transform.Find ("StartPoint");
		zero = transform.Find ("Zero");
	}
	
	// Update is called once per frame
	//void Update () {
	
	//}

	public Vector3 GetStartPos() {
		return startPoint.position;
	}

	public PuyoPos GetPosOnPad(Vector3 pos) {
		return new PuyoPos(pos.x - zero.position.x, Mathf.Ceil(pos.z - zero.position.z));
	}
}