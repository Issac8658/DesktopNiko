extends Window

const MAIN_WINDOW_ID = DisplayServer.MAIN_WINDOW_ID
@export var image_rect : TextureRect
@export var animator : AnimationPlayer
@export var icon : TextureRect
@export var name_label : Label
@export var desc_label : Label
@export var sound : AudioStreamPlayer
var achievements_buffer : Array[String] = []
var buffer_playing = false

func _ready() -> void:
	update_pos()
	animator.play("RESET")
	animator.animation_finished.connect(func (anim):
		if anim == "showing":
			start_buffer_play()
	)
	
	AchievementsGlobalConroller.achievement_taked.connect(func (achievement_id : String):
		achievements_buffer.append(achievement_id)
		if not buffer_playing:
			start_buffer_play()
	)

func _process(_delta) -> void:
	if visible and image_rect.is_visible_in_tree():
		image_rect.texture = ImageTexture.create_from_image(DisplayServer.screen_get_image_rect(Rect2i(position,size)))

func start_buffer_play():
	if len(achievements_buffer) > 0:
		buffer_playing = true
		var achievement_id = achievements_buffer.pop_front()
		if GlobalControlls.show_achievements:
			var data = AchievementsGlobalConroller.get_achievement_data(achievement_id)
			name_label.text = data[0]
			desc_label.text = data[1]
			icon.texture = load(data[2])
			update_pos()
			animator.play("showing")
		else:
			sound.play()
	else:
		buffer_playing = false


func update_pos():
	var main_window_screen_id = DisplayServer.window_get_current_screen(MAIN_WINDOW_ID)
	position = DisplayServer.screen_get_position(main_window_screen_id) + Vector2i(DisplayServer.screen_get_size(main_window_screen_id).x - size.x, 0)
