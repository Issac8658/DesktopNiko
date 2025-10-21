using Godot;
using System.Collections.Generic;

public partial class NPAController : Node
{
	public float NikoPower = 100f;
	public int CollectedPancakes = 0;

	[Export]
	public int NikoTrailMaxSegments = 10;

	[Export]
	public Range NikoPowerRange;
	[Export]
	public RigidBody2D NikoBody;
	[Export]
	public AnimatedSprite2D NikoSprite;
	[Export]
	public Camera2D Camera;
	[Export]
	public Line2D NikoTrail;
	[Export]
	public Label PancakeCountLabel;

	private bool NikoCanFly = false;
	private float CurrentNikoRotation = 0;
	private List<Vector2> TrailQueue = [];
	private Vector2 LastPoint = new();

	public override void _Input(InputEvent Event)
	{
		if (Event is InputEventMouseButton)
		{
			NikoCanFly = Event.IsPressed();
		}
	}

	public void PancakeCollected()
	{
		NikoPower += 5;
		CollectedPancakes += 1;
		PancakeCountLabel.Text = CollectedPancakes.ToString();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (NikoCanFly && NikoPower > 0f)
		{
			NikoBody.LinearVelocity *= 1.1f;
			NikoPower -= 2f;
		}

		if (TrailQueue.Count > NikoTrailMaxSegments)
		{
			TrailQueue.RemoveAt(0);
		}
		if (LastPoint.DistanceTo(NikoBody.Position) >= 96)
		{
			TrailQueue.Add(NikoBody.Position);
			LastPoint = NikoBody.Position;
		}
		NikoTrail.Points = TrailQueue.ToArray();
		NikoTrail.AddPoint(NikoBody.Position);

		NikoPower = Mathf.Clamp(NikoPower, 0f, 100f);
	}


	public override void _Ready()
	{
		LastPoint = NikoBody.Position;
		TrailQueue.Add(LastPoint);
		NikoSprite.Play();
	}

	public override void _Process(double delta)
	{
		//NikoBody.Rotation = 0;
		Vector2 NormalizedVelocity = NikoBody.LinearVelocity.Normalized();

		if (NikoBody.LinearVelocity.X < 0)
		{
			NikoSprite.FlipH = true;
			NormalizedVelocity = -NormalizedVelocity;
		}
		else
			NikoSprite.FlipH = false;

		CurrentNikoRotation = Mathf.Atan2(NormalizedVelocity.Y, NormalizedVelocity.X);
		NikoSprite.Rotation = Lerp(NikoSprite.Rotation, CurrentNikoRotation, 0.15f);

		NikoSprite.SpeedScale = Mathf.Clamp(Mathf.Abs(NikoBody.LinearVelocity.X) / 500, 0f, 1f);

		NikoPowerRange.Value = NikoPower;
	}

	private static float Lerp(float a, float b, float t)
	{
		return a * (1 - t) + t * b;
	}
	private static Vector2 LerpVec2(Vector2 a, Vector2 b, float t)
	{
		return a * new Vector2(1 - t, 1 - t) + new Vector2(t, t) * b;
	}
}
