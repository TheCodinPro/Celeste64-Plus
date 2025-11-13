
namespace Celeste64;

public class WingedBerry : Strawberry
{
    private string DetectionAreaName;
    private bool escaping = false;
    private float shadow_alpha = 1.0f;
    private Vec3 Velocity = new(0.0f, 0.0f, -1.0f);
    private Vec3 Acceleration = new(0.0f, 0.0f, 0.1f);

    public WingedBerry(string id, bool isLocked, string? unlockCondition, bool unlockSound, Vec3? bubbleTo, string detectionAreaName)
        : base(id, isLocked, unlockCondition, unlockSound, bubbleTo)
    {
        Model = new(Assets.Models["winged_berry"]);
        DetectionAreaName = detectionAreaName;
    }

    public void OnPlayerDash(Player player)
    {
        if ((DetectionAreaName == string.Empty || (World.OverlapsFirst<WingedBerryArea>(player.Position + Vec3.UnitZ * 3) is { } detectionArea && detectionArea.Name == DetectionAreaName)) && !IsCollected && !IsLocked)
        {
            escaping = true;
        }
    }

    public override void Update()
    {
        base.Update();
        if (escaping && !Model.Flags.HasFlag(ModelFlags.StrawberryGetEffect))
        {
            Position += Velocity;
            Velocity += Acceleration;
            shadow_alpha *= 0.9f;
            PointShadowAlpha = shadow_alpha;
        }
    }
}

