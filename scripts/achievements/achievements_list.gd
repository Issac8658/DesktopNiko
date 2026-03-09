extends VBoxContainer

@export var locked_icon : Texture2D
@export var achievement_template : PackedScene
@export var achievement_sepatator_template : PackedScene
var panels = {}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var achiements : Array = AchievementsController.GetAchievementsList();
	for i in range(len(achiements)):
		var achievement_id = achiements[i].Id
		if i > 0:
			var separator = achievement_sepatator_template.instantiate()
			add_child(separator)
		var achievement_panel = achievement_template.instantiate()
		panels[achievement_id] = achievement_panel
		update_panel(achievement_panel, achievement_id)
		add_child(achievement_panel)
	
	AchievementsController.AchievementTaked.connect(func (achievement_id):
		update_panel(panels[achievement_id], achievement_id)
	)

func update_panel(panel : Node, achievement_id : String):
	var data : Achievement = AchievementsController.GetAchievementById(achievement_id)
	if AchievementsController.IsAchievementTaked(achievement_id):
		panel.get_node("TextInfo/AchievementName").text = data.Title
		panel.get_node("TextInfo/AchievementDesc").text = data.Description
		panel.get_node("Icon").texture = data.Icon
	else:
		panel.get_node("Icon").texture = locked_icon
		if data.Hidden:
			panel.get_node("TextInfo/AchievementName").text = "???"
			panel.get_node("TextInfo/AchievementDesc").text = "ACHIEVEMENT_HIDDEN_TEXT"
		else:
			panel.get_node("TextInfo/AchievementName").text = data.Title
			panel.get_node("TextInfo/AchievementDesc").text = data.Description
