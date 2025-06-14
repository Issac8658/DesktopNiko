extends Node

const APPLICATION_ID = 1383570527100338206



func _ready() -> void:
	DiscordRPC.app_id = APPLICATION_ID
	DiscordRPC.details = "Don't click for:"
	DiscordRPC.start_timestamp = int(Time.get_unix_time_from_system())
	
	DiscordRPC.refresh()
	#$/root/TheWorldMachine/Timer.timeout.connect(func ():
		
	#)
