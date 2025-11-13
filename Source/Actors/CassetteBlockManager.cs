
namespace Celeste64;

public class CassetteBlockManager(string group, int beatOffset = 2) : Actor, IListenToAudioCallback
{
	public CassetteBlock.CassetteBlockType currentType = CassetteBlock.CassetteBlockType.Blue;

	public string group = group;

	public void AudioCallbackEvent(int beatIndex)
	{
		if (beatIndex % beatOffset == 0)
		{
			if (currentType == CassetteBlock.CassetteBlockType.Blue)
			{
				foreach (var listener in World.All<IListenToBeat>())
				{
					(listener as IListenToBeat)?.BeatEvent(beatIndex, currentType, this);
				}
				currentType = CassetteBlock.CassetteBlockType.Red;
			}
			else if (currentType == CassetteBlock.CassetteBlockType.Red)
			{
				foreach (var listener in World.All<IListenToBeat>())
				{
					(listener as IListenToBeat)?.BeatEvent(beatIndex, currentType, this);
				}
				currentType = CassetteBlock.CassetteBlockType.Yellow;
			}
			else if (currentType == CassetteBlock.CassetteBlockType.Yellow)
			{
				foreach (var listener in World.All<IListenToBeat>())
				{
					(listener as IListenToBeat)?.BeatEvent(beatIndex, currentType, this);
				}
				currentType = CassetteBlock.CassetteBlockType.Green;
			}
			else if (currentType == CassetteBlock.CassetteBlockType.Green)
			{
				foreach (var listener in World.All<IListenToBeat>())
				{
					(listener as IListenToBeat)?.BeatEvent(beatIndex, currentType, this);
				}
				currentType = CassetteBlock.CassetteBlockType.Blue;
			}
		}
	}
}
