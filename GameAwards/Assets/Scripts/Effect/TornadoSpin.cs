using System.Collections;
using UnityEngine;

/// <summary>
/// ******************************************************
/// 制作者：丸本慶大
/// ******************************************************
/// 暴風壁渦巻かせるスクリプト。
/// </summary>
public class TornadoSpin : MonoBehaviour {
	[SerializeField]
	Vector3 _angleSpeed;

	private void FixedUpdate()
	{
		transform.Rotate(new Vector3(_angleSpeed.x, _angleSpeed.y, _angleSpeed.z));
	}
}
