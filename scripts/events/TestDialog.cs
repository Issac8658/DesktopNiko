using Godot;
using System;

public partial class TestDialog : Node
{
	private NikoDialog nikoDialog;
	private short CurrentStep = 0;
	private short PressedButton = 0;
	private Callable StepCallable;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().CreateTimer(3).Connect("timeout", Callable.From(() =>
		{
			nikoDialog = GetNode("/root/NikoDialog") as NikoDialog;
			StepCallable = Callable.From(Step);

			nikoDialog.Connect("ShowNextRequest", StepCallable);
			nikoDialog.ShowDialog(new(400, 140));
		}));
	}
	
	private void Step()
	{
		switch (CurrentStep)
		{
			case 0:
				nikoDialog.ShowMoreText(new(400, 140), "\n\npapaset");
				break;
			case 1:
				nikoDialog.ShowNewText(new(200, 160), "1 or 2 or 3 or 4", ["a 1", "av1", "sgg", "sfs"]);
				nikoDialog.Connect("ButtonPressed", Callable.From((short ButtonId) =>
				{
					PressedButton = ButtonId;
					Step();
				}), (uint)ConnectFlags.OneShot);
				break;
			case 2:
				switch (PressedButton)
				{
					case 0:
						nikoDialog.ShowNewText(new(200, 140), "hehe 1");
						break;
					case 1:
						nikoDialog.ShowNewText(new(200, 140), "not hehe 2");
						break;
					case 2:
						nikoDialog.ShowNewText(new(200, 140), "haha 3");
						break;
					case 3:
						nikoDialog.ShowNewText(new(200, 140), "not haha 4");
						break;
				}
				break;
			case 3:
				nikoDialog.ShowNewText(new(114, 114), "End");
				break;
			case 4:
				nikoDialog.Disconnect("ShowNextRequest", StepCallable);
				nikoDialog.EndDialog();
				break;
		}

		CurrentStep++;
	}
}
