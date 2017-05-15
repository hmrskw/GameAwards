using UnityEngine;
using UnityEditor;

public class CreateGrassData : ScriptableObject
{
	[Header("テラインの大きさ")]
	public int TerrainSize;

	[Space(5)]

	[Header("生成するチャンク数(奇数のみ)")]
	public int ChunkDepth;
	public int ChunkWidth;

	[System.NonSerialized, Tooltip("草オブジェクト1つあたりのサイズ")]
	public int ObjectSize = 4;

	[MenuItem("ScriptableObject/Instantiate GrassDatas")]
	public static void CreateInstance()
	{
		var _asset = CreateInstance<CreateGrassData>();

		AssetDatabase.CreateAsset(_asset, "Assets/Resources/ScriptableObjects/GrassData.asset");
		AssetDatabase.Refresh();
	}
}
