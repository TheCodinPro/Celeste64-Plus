
using System;
using System.Security.Cryptography.X509Certificates;

namespace Celeste64;

public class CassetteTrafficBlock(bool startOn, string type, string group, Vec3 end) : CassetteBlock(startOn, type, group), IListenToAudioCallback, IHaveModels
{
	public SkinnedModel? FrameModel;

	public const float Acceleration = 400;
	public const float MaxSpeed = 600;
	public const float ReturnSpeed = 50;

	public Vec3 Start;
	public Vec3 End = end;

	private readonly Routine routine = new();
	private Sound? sfxMove;
	private Sound? sfxRetract;

	public override void Added()
	{
		base.Added();
		Start = Position;
		routine.Run(Sequence());
		sfxMove = World.Add(new Sound(this, Sfx.sfx_zipmover_loop));
		sfxRetract = World.Add(new Sound(this, Sfx.sfx_zipmover_retract_loop));
		FrameModel = new SkinnedModel(Assets.Models["trafficblockframe"]);
		FrameModel.Transform = Matrix.CreateScale(LocalBounds.Size.X / 1.9f, LocalBounds.Size.Y / 1.9f, LocalBounds.Size.Z / 1.9f);
		FrameModel.MakeMaterialsUnique();
	}

	public override void Update()
	{
		base.Update();
		routine.Update();

		if (!Collidable)
		{
			FrameModel.Flags = ModelFlags.Transparent;

			foreach (var mat in FrameModel.Materials)
				switch (BlockType)
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
		}
		else
		{
			FrameModel.Flags = ModelFlags.Terrain;

			foreach (var mat in FrameModel.Materials)
				switch (BlockType)
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
		}

		//if (TShake > 0)
		//{
		//	if (TShake <= 0)
		//	{
		//		FrameModel.Transform = Matrix.Identity * Matrix.CreateScale(LocalBounds.Size.X / 1.9f, LocalBounds.Size.Y / 1.9f, LocalBounds.Size.Z / 1.9f);
		//	}
		//	else if (Time.OnInterval(.02f))
		//	{
		//		Matrix matrix =
		//			Matrix.CreateScale(LocalBounds.Size.X / 1.9f, LocalBounds.Size.Y / 1.9f, LocalBounds.Size.Z / 1.9f) *
		//			Matrix.CreateTranslation(World.Rng.Float(-1, 1), World.Rng.Float(-1, 1), 0);
		//		FrameModel.Transform = matrix;
		//	}
		//}
		float scaleoffset = 1f;
		FrameModel.Transform = Model.Transform + Matrix.CreateScale(LocalBounds.Size.X / scaleoffset, LocalBounds.Size.Y / scaleoffset, LocalBounds.Size.Z / scaleoffset);
	}

	private CoEnumerator Sequence()
	{
		while (true)
		{
			while (!HasPlayerRider())
				yield return Co.SingleFrame;

			Audio.Play(Sfx.sfx_zipmover_start, Position);
			TShake = .15f;
			UpdateOffScreen = true;
			yield return .15f;

			// move to target
			{
				sfxMove?.Resume();
				var target = End;
				var normal = (target - Position).Normalized();
				while (Position != target && Vec3.Dot((target - Position).Normalized(), normal) >= 0)
				{
					Velocity = Utils.Approach(Velocity, MaxSpeed * normal, Acceleration * Time.Delta);
					yield return Co.SingleFrame;
				}

				sfxMove?.Stop();
				Velocity = Vec3.Zero;
				MoveTo(target);
			}

			Audio.Play(Sfx.sfx_zipmover_stop, Position);
			TShake = .2f;
			yield return .8f;

			// Move back to start
			{
				Audio.Play(Sfx.sfx_zipmover_retract_start, Position);
				sfxRetract?.Resume();
				var target = Start;
				var normal = (target - Position).Normalized();
				while (Vec3.Dot((target - Position).Normalized(), normal) >= 0)
				{
					Velocity = normal * ReturnSpeed;
					yield return Co.SingleFrame;
				}

				sfxRetract?.Stop();
				Velocity = Vec3.Zero;
				MoveTo(target);
			}

			//Reactivate
			{
				Audio.Play(Sfx.sfx_zipmover_retract_stop, Position);
				TShake = .1f;
				UpdateOffScreen = false;
				yield return .5f;
			}
		}
	}

	public virtual void CollectModels(List<(Actor Actor, Model Model)> populate)
	{
		if (FrameModel != null)
			populate.Add((this, FrameModel));
		if (Model != null)
			populate.Add((this, Model));
	}
}
