extends VBoxContainer

@export var locked_icon : Texture2D
@export var achievement_template : PackedScene
@export var achievement_sepatator_template : PackedScene
var panels = {}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	for i in range(len(AchievementsGlobalConroller.achievements)):
		var achievement_id = AchievementsGlobalConroller.achievements.keys()[i]
		if i > 0:
			var separator = achievement_sepatator_template.instantiate()
			add_child(separator)
		var achievement_panel = achievement_template.instantiate()
		panels[achievement_id] = achievement_panel
		update_panel(achievement_panel, achievement_id)
		add_child(achievement_panel)
	
	AchievementsGlobalConroller.achievement_taked.connect(func (achievement_id):
		update_panel(panels[achievement_id], achievement_id)
	)

func update_panel(panel : Node, achievement_id : String):
	var data = AchievementsGlobalConroller.get_achievement_data(achievement_id)
	if AchievementsGlobalConroller.is_achievement_taked(achievement_id):
		panel.get_node("TextInfo/AchievementName").text = data[0]
		panel.get_node("TextInfo/AchievementDesc").text = data[1]
		panel.get_node("Icon").texture = load(data[2])
	else:
		panel.get_node("Icon").texture = locked_icon
		if data[3]:
			panel.get_node("TextInfo/AchievementName").text = "???"
			panel.get_node("TextInfo/AchievementDesc").text = "ACHIEVEMENT_HIDDEN_TEXT"
		else:
			panel.get_node("TextInfo/AchievementName").text = data[0]
			panel.get_node("TextInfo/AchievementDesc").text = data[1]
