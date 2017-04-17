using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MarumotoGrassTest : MonoBehaviour
{
	[SerializeField]
	private GameObject _grassObject;
	[SerializeField]
	private Transform _parent;
	[SerializeField]
	private int _numOfCreateWidth;
	[SerializeField]
	private int _numOfCreateHeight;

	private List<Grass> _grasses;

	void Start ()
	{
		for(int i = 0; i < _numOfCreateHeight; i++)
		{
			for(int j = 0; j < _numOfCreateWidth; j++)
			{
				Instantiate(_grassObject, new Vector3(j, 0, i), Quaternion.identity, _parent);
			}
		}
		_grasses = GetComponentsInChildren<Grass>().ToList();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!Input.GetKeyDown(KeyCode.G)) return;

		foreach(Grass _grass in _grasses)
		{
			StartCoroutine(_grass.Growth());
		}
	}
}
