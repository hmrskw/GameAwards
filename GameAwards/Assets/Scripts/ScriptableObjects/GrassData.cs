using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassData : ScriptableObject
{
	[Header("★マップに関する設定値")]
	[Header("テラインの大きさ")]
	public int TerrainSize;

	[Space(5)]

	[Header("生成するチャンク数(奇数のみ)")]
	public int ChunkDepth;
	public int ChunkWidth;
	[Header("1チャンクの1辺に並べる数")]
	public int OneLinePerChunkTipNum;

	[Space(5)]

	[Header("草が生やせない判定にする対角点の高さの差")]
	public int Constraint;

	[Space(15)]

	[Header("★草オブジェクト1つについての設定")]

	[Header("草のアニメーションカーブ")]
	public AnimationCurve Curve;
	[Header("草のテクスチャが変わる時の生えなおしに使うカーブ")]
	public AnimationCurve ChangedTexCurve;
	[Header("生えるまでにかかる秒数")]
	public float GrowthBaseTime;
	[Header("枯れるまでにかかる秒数")]
	public float WitherBaseTime;
	[Header("スケールのランダム値の最小値")]
	public Vector3 RandomMin;
	[Header("スケールのランダム値の最大値")]
	public Vector3 RandomMax;

	[System.NonSerialized, Tooltip("草オブジェクト1つあたりのサイズ")]
	public int TipSize = 10;
}
