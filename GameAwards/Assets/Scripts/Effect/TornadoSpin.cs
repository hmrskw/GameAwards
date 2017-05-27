using System.Collections;
using UnityEngine;

public class TornadoSpin : MonoBehaviour {
	[SerializeField]
	float _angleSpeed;

	IEnumerator Start ()
	{
		while (true)
		{
			transform.Rotate(new Vector3(0, _angleSpeed, 0));
			yield return null;
		}
	}
}
