using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class GrassObject {
	public GameObject Object { get; private set; }
	public GrassesController Controller { get; private set; }
	public Quaternion Rotation { get; private set; }
	public List<MeshRenderer> Renderers { get; set; }
	
	public GrassObject(GameObject _obj, GrassesController _ctrl, Quaternion _rot)
	{
		Object = _obj;
		Controller = _ctrl;
		Rotation = _rot;
		Renderers = _obj.GetComponentsInChildren<MeshRenderer>().ToList();
	}
}
