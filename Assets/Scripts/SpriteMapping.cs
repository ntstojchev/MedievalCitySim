using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpriteMapping
{
	public List<SpriteMappingEntry> Sprites;

	public Sprite GetSpriteForCellType(CellType type)
	{
		foreach (SpriteMappingEntry entry in Sprites) {
			if (entry.Type == type) {
				return entry.Sprite;
			}
		}

		return null;
	}
}
