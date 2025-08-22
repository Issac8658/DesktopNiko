extends Control

@export var toggle_checkbox : CheckBox
@export var hours_spinbox : SpinBox
@export var minutes_spinbox : SpinBox
@export var seconds_spinbox : SpinBox

func _ready() -> void:
	seconds_spinbox.value_changed.connect(update)
	minutes_spinbox.value_changed.connect(update)
	hours_spinbox.value_changed.connect(update)
	toggle_checkbox.toggled.connect(update)
	if AchievementsGlobalConroller.is_achievement_taked("sweet_dreams"):
		visible = true
	else:
		AchievementsGlobalConroller.achievement_taked.connect(func (achievement_id):
			if achievement_id == "sweet_dreams":
				visible = true
		)

func update(_value):
	if toggle_checkbox.button_pressed:
		GlobalControlls.sleep_time = int(seconds_spinbox.value + minutes_spinbox.value * 60 + hours_spinbox.value * 3600)
		#print(GlobalControlls.sleep_time)
		if hours_spinbox.value <= 0 and minutes_spinbox.value <= 0:
			seconds_spinbox.min_value = 2
		else:
			seconds_spinbox.min_value = 0
	else:
		GlobalControlls.sleep_time = 2147483628
