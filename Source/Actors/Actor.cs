
namespace Celeste64;

public class Actor
{
	private World? world = null;
	private Vec3 position;
	private Vec3 scale = new Vector3(1, 1, 1);
	private Vec2 rotationX = -Vec2.UnitY;
	private Vec2 rotationY = -Vec2.UnitY;
	private Vec2 rotationZ = -Vec2.UnitY;
	private Vec3 forward;
	private Vec3 backward;
	private Vec3 right;
	private Vec3 left;
	private Vec3 up;
	private Vec3 down;
	private Matrix matrix;
	private BoundingBox localBounds;
	private BoundingBox worldBounds;
	private bool dirty = true;

	/// <summary>
	/// Optional GroupName, used by Strawberries to check what unlocks them. Can
	/// be used by other stuff for whatever.
	/// </summary>
	public string GroupName = string.Empty;

	/// <summary>
	/// The World we belong to - asserts if Destroyed
	/// </summary>
	public World World => world ?? throw new Exception("Actor not added to the World");

	/// <summary>
	/// If we're currently alive
	/// </summary>
	public bool Alive => world != null;

	/// <summary>
	/// If we're being destroyed
	/// </summary>
	public bool Destroying = false;

	/// <summary>
	/// If we should Update while off-screen
	/// </summary>
	public bool UpdateOffScreen = false;

	public BoundingBox LocalBounds
	{
		get => localBounds;
		set
		{
			if (localBounds != value)
			{
				localBounds = value;
				dirty = true;
			}
		}
	}

	public Vec3 Position
	{
		get => position;
		set
		{
			if (position != value)
			{
				position = value;
				dirty = true;
			}
		}
	}

	public Vec3 Scale
	{
		get => scale;
		set
		{
			if (scale != value)
			{
				scale = value;
				dirty = true;
			}
		}
	}

	public Vec2 RotationZ
	{
		get => rotationZ;
		set
		{
			if (rotationZ != value)
			{
				rotationZ = value;
				dirty = true;
			}
		}
	}

	public Vec2 RotationX
	{
		get => rotationX;
		set
		{
			if (rotationX != value)
			{
				rotationX = value;
				dirty = true;
			}
		}
	}

	public Vec2 RotationY
	{
		get => rotationY;
		set
		{
			if (rotationY != value)
			{
				rotationY = value;
				dirty = true;
			}
		}
	}

	public Vec3 Forward
	{
		get
		{
			ValidateTransformations();
			return forward;
		}
	}
	public Vec3 Backward
	{
		get
		{
			ValidateTransformations();
			return backward;
		}
	}

	public Vec3 Right
	{
		get
		{
			ValidateTransformations();
			return right;
		}
	}

	public Vec3 Left
	{
		get
		{
			ValidateTransformations();
			return left;
		}
	}

	public Vec3 Up
	{
		get
		{
			ValidateTransformations();
			return up;
		}
	}

	public Vec3 Down
	{
		get
		{
			ValidateTransformations();
			return down;
		}
	}

	public Matrix Matrix
	{
		get
		{
			ValidateTransformations();
			return matrix;
		}
	}

	public BoundingBox WorldBounds
	{
		get
		{
			ValidateTransformations();
			return worldBounds;
		}
	}

	public void SetWorld(World? world)
	{
		if (world != null && this.world != null)
			throw new Exception("Actor is already assigned to a World");
		this.world = world;
	}

	protected void ValidateTransformations()
	{
		if (!dirty)
			return;
		dirty = false;

		matrix =
			Matrix.CreateScale(scale) *
			Matrix.CreateRotationX(rotationX.Angle() + MathF.PI / 2) *
			Matrix.CreateRotationY(rotationY.Angle() + MathF.PI / 2) *
			Matrix.CreateRotationZ(rotationZ.Angle() + MathF.PI / 2) *
			Matrix.CreateTranslation(position);
		worldBounds = BoundingBox.Transform(localBounds, matrix);
		forward = Vec3.TransformNormal(-Vec3.UnitY, matrix);
		backward = Vec3.TransformNormal(Vec3.UnitY, matrix);
		right = Vec3.TransformNormal(-Vec3.UnitX, matrix);
		left = Vec3.TransformNormal(Vec3.UnitX, matrix);
		up = Vec3.TransformNormal(Vec3.UnitZ, matrix);
		down = Vec3.TransformNormal(-Vec3.UnitZ, matrix);

		Transformed();
	}

	public virtual void Created() {}
	public virtual void Added() { }
	public virtual void Update() {}
	public virtual void LateUpdate() {}
	public virtual void Destroyed() {}

	/// <summary>
	/// Called when we move
	/// </summary>
	protected virtual void Transformed() {}
}