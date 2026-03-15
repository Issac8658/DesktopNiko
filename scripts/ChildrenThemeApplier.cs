using Godot;

[Tool, GlobalClass]
public partial class ChildrenThemeApplier : Node
{
	public override void _EnterTree()
	{
		Node Parent = GetParent();
		if (Parent is CanvasItem)
		{
			foreach (Node child in Parent.GetChildren(true))
				if (child is CanvasItem CanvasNode)
				{
					CanvasNode.SetMeta("_recursiveItemParentMaterial", CanvasNode.UseParentMaterial);
					CanvasNode.UseParentMaterial = true;
				}
		}
	}
	public override void _ExitTree()
	{
		Node Parent = GetParent();
		if (Parent is CanvasItem)
		{
			foreach (Node child in Parent.GetChildren(true))
				if (child is CanvasItem CanvasNode)
				{
					Variant Meta = CanvasNode.GetMeta("_recursiveItemParentMaterial", CanvasNode.UseParentMaterial);
					if (Meta.VariantType == Variant.Type.Bool)
					{
						CanvasNode.UseParentMaterial = Meta.AsBool();
						CanvasNode.RemoveMeta("_recursiveItemParentMaterial");
					}
				}
		}
	}
}
