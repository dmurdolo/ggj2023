extends Spatial

export(String) var team_spawn
# export(NodePath) var next_tower_1
# export(NodePath) var next_tower_2
# export(NodePath) var next_tower_3
export(Array) var next_towers = []
export var adjacent_to_core:bool = false

func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
