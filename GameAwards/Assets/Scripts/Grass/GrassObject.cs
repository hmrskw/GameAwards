using UnityEngine;

/// <summary>
/// ******************************************************
/// 制作者：丸本慶大
/// ******************************************************
/// 草オブジェクトに対してGetComponentなどをしないように
/// よく使うコンポーネントや値をあらかじめ保持しておくクラス。
/// </summary>
public class GrassObject {
	public GameObject Object { get; private set; }
	public GrassesController Controller { get; private set; }
	public Quaternion Rotation { get; private set; }
	
	public GrassObject(GameObject _obj, GrassesController _ctrl, Quaternion _rot)
	{
		Object = _obj;
		Controller = _ctrl;
		Rotation = _rot;
	}
}
