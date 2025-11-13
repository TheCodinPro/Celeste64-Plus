
using System.Security.Cryptography.X509Certificates;

namespace Celeste64;

public class CassetteBlock : Solid, IListenToAudioCallback, IListenToBeat
{
	public enum CassetteBlockType
	{
		Blue,
		Red,
		Yellow,
		Green,
		None
	}

	private int beatCount = 0;

	private CassetteBlockType blockType;

	public CassetteBlockType? BlockType
	{
		get => blockType;
		set
		{
			if (blockType != value)
			{
				blockType = (CassetteBlockType)value;
			}
		}
	}

	private string group;

	public CassetteBlock(bool startOn, string type, string group)
	{
		Model.MakeMaterialsUnique();
		Transparent = true;
		this.group = group;
		if (Enum.TryParse<CassetteBlockType>(type, true, out CassetteBlockType parsedType))
		{
			blockType = parsedType;
		}
		else
		{
			// Default value for handling an invalid enum string
			blockType = CassetteBlockType.None;
		}
		if (blockType == CassetteBlockType.None) 
		{
			SetOn(startOn);
		}
	}

	public void SetOn(bool enabled)
	{
		Collidable = enabled;
		this.Model.Flags = enabled ? ModelFlags.Terrain : ModelFlags.Transparent;

		foreach (var mat in this.Model.Materials)
			mat.Color = enabled ? Color.White : Color.White * 0.30f;
	}

	public void AudioCallbackEvent(int index)
	{
		if (blockType == CassetteBlockType.None)
		{
			if (index % 2 == 0)
			{
				SetOn(!Collidable);
			}
		} 
		//else if (blockType == CassetteBlockType.Red)
		//{
		//	if (index % 2 == 0)
		//	{
		//		if (beatCount % 1 == 0)
		//		{
		//			SetOn(!Collidable);
		//		}
		//		beatCount++;
		//	}
		//} 
		//else if (blockType == CassetteBlockType.Blue)
		//{
		//	if (index % 2 == 0)
		//	{
		//		if (beatCount % 2 == 0)
		//		{
		//			SetOn(!Collidable);
		//		}
		//		beatCount++;
		//	}
		//} 
		//else if (blockType == CassetteBlockType.Yellow)
		//{
		//	if (index % 2 == 0)
		//	{
		//		if (beatCount % 3 == 0)
		//		{
		//			SetOn(!Collidable);
		//		}
		//		beatCount++;
		//	}
		//} 
		//else if (blockType == CassetteBlockType.Green)
		//{
		//	if (index % 2 == 0)
		//	{
		//		if (beatCount % 4 == 0)
		//		{
		//			SetOn(!Collidable);
		//		}
		//		beatCount++;
		//	}
		//}

	}

	public void BeatEvent(int beatIndex, CassetteBlock.CassetteBlockType type, CassetteBlockManager manager)
	{
		if (manager.group == group)
		{
			if (blockType == type)
			{
				SetOn(true);
			} else
			{
				SetOn(false);
			}
		}
	}
}
