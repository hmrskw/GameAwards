using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ******************************************************
/// 制作者：丸本慶大
/// ******************************************************
/// 草オブジェクトをいくつかまとめたブロックで管理するため
/// そのブロック内の草をまとめてコントロールするクラス。
/// </summary>
public class GrassesController : MonoBehaviour {
	[SerializeField]
	List<Grass> _grasses;

	[SerializeField]
	GrassData _grassData;

	[SerializeField]
	List<ParticleSystem> _particle;

	List<MeshRenderer> _renderers;

	private void Awake()
	{
		_renderers = GetComponentsInChildren<MeshRenderer>().ToList();

		foreach(var _grass in _grasses)
		{
			_grass.Setup(_grassData.RandomMin, _grassData.RandomMax, _grassData.GrowthBaseTime, _grassData.WitherBaseTime, _grassData.Curve, _grassData.ChangedTexCurve);
		}
	}

	public void Growth()
	{
		for (int i = 0; i < _grasses.Count; i++)
		{
			StartCoroutine(_grasses[i].Growth());
		}
	}

	public void Wither()
	{
		for (int i = 0; i < _grasses.Count; i++)
		{
			StartCoroutine(_grasses[i].Wither());
		}
	}

	public void ForceScaleZero()
	{
		for (int i = 0; i < _grasses.Count; i++)
		{
			_grasses[i].ForceScaleZero();
		}
	}

	public void GrowthChangedTexture(Action _act)
	{
		for (int i = 0; i < _grasses.Count; i++)
		{
			StartCoroutine(_grasses[i].GrowthChangedTexture(_act));
		}
	}

	public void ChangeMaterials(MaterialPropertyBlock _mat)
	{
		for(int i = 0; i < _renderers.Count; i++)
		{
			_renderers[i].SetPropertyBlock(_mat);
		}
	}

	public void PlayParticle()
	{
		foreach(var _part in _particle)
		{
			_part.Play();
		}
	}
}
