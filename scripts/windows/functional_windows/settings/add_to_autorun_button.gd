extends Button

const AUTORUN_PATH = "HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run"


func _ready() -> void:
	if not OS.get_name() == "Windows":
		disabled = true
		text = "settings.general.actions.add_to_autorun.windows_only"
	else:
		if check():
			text = "settings.general.actions.remove_from_autorun"
		pressed.connect(func ():
			if check():
				remove_from_autorun()
				text = "settings.general.actions.add_to_autorun"
			else:
				add_to_autorun()
				text = "settings.general.actions.remove_from_autorun"
		)


func check():
	var exit_code = OS.execute("reg", ["query", AUTORUN_PATH, "/v", "DesktopNiko", "/t", "REG_SZ"])
	return exit_code == 0


func add_to_autorun():
	var path = OS.get_executable_path()
	path = '\"\"\"' + path.replace("/", "\\") + '\"\"\"'
	var succses = OS.execute("reg", ["add", AUTORUN_PATH, "/v", "DesktopNiko", "/t", "REG_SZ", "/d", path, "/f"])
	print(succses)


func remove_from_autorun():
	var succses = OS.execute("reg", ["delete", AUTORUN_PATH, "/v", "DesktopNiko", "/f"])
	print(succses)
