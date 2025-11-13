
namespace Celeste64;

public class Spring : Attacher, IHaveModels, IPickup
{
	public SkinnedModel Model;

	public float PickupRadius => 16;

	private float tCooldown = 0;

	private bool active = true;

	public Spring()
	{
		Model = new SkinnedModel(Assets.Models["spring_board"]);
		Model.Transform = Matrix.CreateScale(8.0f);
		Model.SetLooping("Spring", false);
		Model.Play("Idle");
		Model.MakeMaterialsUnique();

		//if (AttachedTo is CassetteBlock cassetteBlock)
		//{
		//	foreach (var mat in this.Model.Materials)
		//		mat.Color = Color.White * 0.30f;
		//}

		LocalBounds = new(Position + Vec3.UnitZ * 4, 16);
	}

	public override void Update()
	{
		Model.Update();

		if (tCooldown > 0)
		{
			tCooldown -= Time.Delta;
			if (tCooldown <= 0)
				UpdateOffScreen = false;
		}

		if (AttachedTo is CassetteBlock cassetteBlock)
		{
			if (!cassetteBlock.Collidable)
			{
				this.Model.Flags = ModelFlags.Transparent;

				foreach (var mat in this.Model.Materials)
					switch (cassetteBlock.BlockType)
					{
						case CassetteBlock.CassetteBlockType.Blue:
							mat.Color = Color.Blue * 0.30f;
							break;
						case CassetteBlock.CassetteBlockType.Red:
							mat.Color = Color.Red * 0.30f;
							break;
						case CassetteBlock.CassetteBlockType.Green:
							mat.Color = Color.Green * 0.30f;
							break;
						case CassetteBlock.CassetteBlockType.Yellow:
							mat.Color = Color.Yellow * 0.30f;
							break;
					}

				active = false;
			}
			else
			{
				this.Model.Flags = ModelFlags.Terrain;

				foreach (var mat in this.Model.Materials)
					switch (cassetteBlock.BlockType)
					{
						case CassetteBlock.CassetteBlockType.Blue:
							mat.Color = Color.Blue;
							break;
						case CassetteBlock.CassetteBlockType.Red:
							mat.Color = Color.Red;
							break;
						case CassetteBlock.CassetteBlockType.Green:
							mat.Color = Color.Green;
							break;
						case CassetteBlock.CassetteBlockType.Yellow:
							mat.Color = Color.Yellow;
							break;
					}
				active = true;
			}
		}
		else
		{
			this.Model.Flags = ModelFlags.Terrain;

			foreach (var mat in this.Model.Materials)
				mat.Color = Color.White;
			active = true;
		}
	}

	public void CollectModels(List<(Actor Actor, Model Model)> populate)
	{
		populate.Add((this, Model));
	}

	public void Pickup(Player player)
	{
		if (tCooldown <= 0 && active)
		{
			UpdateOffScreen = true;
			Audio.Play(Sfx.sfx_springboard, Position);
			tCooldown = 1.0f;
			Model.Play("Spring", true);
			player.Spring(this);

			if (AttachedTo is FallingBlock fallingBlock)
				fallingBlock.Trigger();
			if (AttachedTo is TrafficBlock trafficBlock)
				trafficBlock.Trigger();
		}
	}
}
