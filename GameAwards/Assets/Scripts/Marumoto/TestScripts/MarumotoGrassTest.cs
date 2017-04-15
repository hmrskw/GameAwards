using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MarumotoGrassTest : MonoBehaviour
{
	private List<Grass> _grasses;

	void Start ()
	{
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
