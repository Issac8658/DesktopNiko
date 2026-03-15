using Godot;

[GlobalClass]
public partial class RecursiveThemeApplier : Node
{
	[Export]
	public ShaderMaterial ThemeMaterial;
	public override void _EnterTree()
	{
		Node Parent = GetParent();
		ApplyThemeRecursive(Parent);
	}
	public override void _Ready()
	{
		Node Parent = GetParent();
		ChildEnteredTree += (child) => ApplyThemeRecursive(child, true);
	}

	private void ApplyThemeRecursive(Node Parent, bool ChangeThis = false)
	{
		foreach (Node child in Parent.GetChildren(true))
		{
			if (child is CanvasItem CanvasNode)
			{
				CanvasNode.UseParentMaterial = true;
				CanvasNode.Material = ThemeMaterial;
			}
			ApplyThemeRecursive(child, true);
		}
		if (Parent is CanvasItem ThisItem)
		{
			if (ChangeThis)
			{
				ThisItem.UseParentMaterial = true;
				ThisItem.Material = ThemeMaterial;
				if (ThisItem is LineEdit lineEdit)
				{
					lineEdit.ContextMenuEnabled = false;
				}
			}
		}
	}
}
