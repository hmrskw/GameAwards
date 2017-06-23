using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour {
	[SerializeField]
	Image _background;

	[SerializeField]
	Image _fillArea;

	[SerializeField]
	Text _fillAmount;

	[SerializeField]
	Outline _outline;

	public void ChangeBarAlpha(float _alpha)
	{
		_background.color = new Color(_background.color.r, _background.color.g, _background.color.b, _alpha);
		_fillArea.color = new Color(_fillArea.color.r, _fillArea.color.g, _fillArea.color.b, _alpha);
		_fillAmount.color = new Color(_fillAmount.color.r, _fillAmount.color.g, _fillAmount.color.b, _alpha);
		_outline.effectColor = new Color(_outline.effectColor.r, _outline.effectColor.g, _outline.effectColor.b, _alpha);
	}
}
