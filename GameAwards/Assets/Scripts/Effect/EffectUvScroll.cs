using UnityEngine;

/// <summary>
/// ******************************************************
/// 制作者：丸本慶大
/// ******************************************************
/// スクリプトからUVスクロールさせます。
/// </summary>
public class EffectUvScroll : MonoBehaviour {
	[SerializeField]
	Vector2 _tiling = new Vector2(1, 1);

	[SerializeField]
	Vector2 _offSet = new Vector2(0, 0);
	[SerializeField]
	Vector2 _autoScrollSpeed = new Vector2(1, 1);

	[SerializeField]
	bool _autoUvScroll = true;

	[SerializeField]
	Color _color;
	[SerializeField]
	Color _emissionColor;

	//歪み数値定義
	[SerializeField, Range(0, 1)]
	float _heatTime = 0;
	[SerializeField, Range(0, 4)]
	float _heatForce = 0;

	private Renderer _rend;

	private float _offsetX = 0;
	private float _offsetY = 0;

	void Start()
	{
		_rend = GetComponent<Renderer>();

		_rend.sharedMaterial.SetTextureOffset("_MainTex", new Vector2(_offsetX, _offsetY));
		_rend.sharedMaterial.SetTextureScale("_MainTex", new Vector2(_tiling.x, _tiling.y));

		_rend.sharedMaterial.SetColor("_TintColor", _color);
		_rend.sharedMaterial.SetColor("_Color", _color);
		_rend.sharedMaterial.SetColor("_EmissionColor", _emissionColor);

		//歪み量設定
		_rend.sharedMaterial.SetFloat("_HeatTime", _heatTime);
		_rend.sharedMaterial.SetFloat("_HeatForce", _heatForce);

	}

	void FixedUpdate()
	{
		if (_autoUvScroll == true)
		{
			_offsetX += Time.fixedDeltaTime * _autoScrollSpeed.x;
			_offsetY += Time.fixedDeltaTime * _autoScrollSpeed.y;
		}
		else
		{
			_offsetX = _offSet.x;
			_offsetY = _offSet.y;
		}

		_rend.material.SetTextureOffset("_MainTex", new Vector2(_offsetX, _offsetY));
		_rend.material.SetTextureScale("_MainTex", new Vector2(_tiling.x, _tiling.y));

		_rend.material.SetColor("_TintColor", _color);
		_rend.material.SetColor("_Color", _color);
		_rend.material.SetColor("_EmissionColor", _emissionColor);

		//歪み量設定
		_rend.material.SetFloat("_HeatTime", _heatTime);
		_rend.material.SetFloat("_HeatForce", _heatForce);

	}
}
