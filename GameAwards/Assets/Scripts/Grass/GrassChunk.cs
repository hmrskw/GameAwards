using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassChunk
{
	public int HalfWidth { get; private set; }
	public int HalfDepth { get; private set; }

	public int TipSize { get; private set; }
	public int OneLinePerChunkTipNum { get; private set; }
	public int OneChunkTipNum { get; private set; }
	public int OneChunkSize { get; private set; }
	public int DummyPointElements { get; private set; }
	public int MaptipIndicesElements { get; private set; }

	public GrassChunk(GrassData _data)
	{
		TipSize = _data.TipSize;
		OneLinePerChunkTipNum = _data.OneLinePerChunkTipNum;
		OneChunkTipNum = OneLinePerChunkTipNum * OneLinePerChunkTipNum;
		OneChunkSize = TipSize * OneLinePerChunkTipNum;
		DummyPointElements = _data.TerrainSize / TipSize;
		MaptipIndicesElements = _data.TerrainSize / OneChunkSize;

		if (_data.ChunkWidth % 2 == 1) HalfWidth = (_data.ChunkWidth - 1) / 2;
		else HalfWidth = _data.ChunkWidth / 2;
		if (_data.ChunkDepth % 2 == 1) HalfDepth = (_data.ChunkDepth - 1) / 2;
		else HalfDepth = _data.ChunkDepth / 2;
	}
}
