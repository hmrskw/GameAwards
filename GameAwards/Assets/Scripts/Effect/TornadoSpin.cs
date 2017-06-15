using System.Collections;
using UnityEngine;

public class TornadoSpin : MonoBehaviour {
	[SerializeField]
	Vector3 _angleSpeed;

	private void FixedUpdate()
	{
		transform.Rotate(new Vector3(_angleSpeed.x, _angleSpeed.y, _angleSpeed.z));
	}
}
