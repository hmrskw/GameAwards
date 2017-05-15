using UnityEngine;

public class GrassObject {
	public GameObject Object { get; private set; }
	public GrassesController Controller { get; private set; }

	public GrassObject(GameObject _obj, GrassesController _ctrl)
	{
		Object = _obj;
		Controller = _ctrl;
	}
}
