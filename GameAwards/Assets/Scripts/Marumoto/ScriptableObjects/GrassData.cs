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
	[Header("1チャンクの1辺に並べる数")]
	public int OneLinePerChunkTipNum;

	[System.NonSerialized, Tooltip("草オブジェクト1つあたりのサイズ")]
	public int TipSize = 10;
}
