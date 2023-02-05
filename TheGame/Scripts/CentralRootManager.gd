extends Node

export var surrounding_nodes:Array = []

# Called when the node enters the scene tree for the first time.
func _ready():
	var nodes = get_node("/root/Level/Nodes")
	for node in nodes.get_children():
		if (node.adjacent_to_core):
			surrounding_nodes.append(node)

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
