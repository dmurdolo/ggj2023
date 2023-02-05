extends Node

export var surrounding_nodes:Array = []

# Called when the node enters the scene tree for the first time.
func _ready():
	var nodes = get_node("/root/Level/Nodes")
	print(nodes)


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
