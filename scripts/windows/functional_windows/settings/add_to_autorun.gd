extends Button

const AUTORUN_PATH = "HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run"


func _ready() -> void:
	if not OS.get_name() == "Windows":
		disabled = true
		text = "AUTORUN_WINDOWS_ONLY"
	else:
		if check():
			text = "REMOVE_FROM_AUTO_RUN"
		pressed.connect(func ():
			if check():
				remove_from_autorun()
				text = "ADD_TO_AUTO_RUN"
			else:
				add_to_autorun()
				text = "REMOVE_FROM_AUTO_RUN"
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
