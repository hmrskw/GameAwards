using UnityEngine;
using UnityEditor;

public class CreateGrassData : MonoBehaviour
{
	[MenuItem("ScriptableObject/Instantiate GrassDatas")]
	public static void CreateInstance()
	{
		var _asset = ScriptableObject.CreateInstance<GrassData>();

		AssetDatabase.CreateAsset(_asset, "Assets/Resouces/ScriptableObjects/GrassData.asset");
		AssetDatabase.Refresh();
	}
}
