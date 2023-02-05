extends Node

var game_manager

# Called when the node enters the scene tree for the first time.
func _ready():
	game_manager = get_parent().get_node("GameManager")
	update_energy()

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass

func update_energy():
	$PowerLabel.set_text(String(game_manager.current_energy))
