using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassData : ScriptableObject
{
	[Header("テラインの大きさ")]
	public int TerrainSize;

	[Space(5)]

	[Header("生成するチャンク数(奇数のみ)")]
	public int ChunkDepth;
	public int ChunkWidth;

	[System.NonSerialized, Tooltip("草オブジェクト1つあたりのサイズ")]
	public int ObjectSize = 4;
}
