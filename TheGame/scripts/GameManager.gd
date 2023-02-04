extends Node

export (PackedScene) var mob_prefab

var DEBUG_SPAWN = false

var current_power:int = 10
var max_power:int = 10
var blue_tower = null
var red_tower = null
var blue_path = []
var red_path = []
var spawns_per_wave = 10

#Power Nodes
var test_nodes
var current_power_node:Node = null

func _ready():
	init_nodes()
	
	var towers = get_tree().get_nodes_in_group("tower")
	var tower_edge_map = {}
	
	for tower in towers:
		_map_edge(tower, tower.next_tower_1, tower_edge_map)
		_map_edge(tower, tower.next_tower_2, tower_edge_map)
		_map_edge(tower, tower.next_tower_3, tower_edge_map)
		if tower.team_spawn == "blue":
			blue_tower = tower
		if tower.team_spawn == "red":
			red_tower = tower
	if blue_tower:
		blue_path = _generate_path(blue_tower, blue_tower, tower_edge_map, {}, true)
	if red_tower:
		red_path = _generate_path(red_tower, red_tower, tower_edge_map, {}, true)
	if (DEBUG_SPAWN):
		do_spawn()

func get_current_power() -> int:
	return current_power

func get_max_power() -> int:
	return max_power

func increase_current_power():
	current_power += 1
	
func decrease_current_power():
	current_power -= 1

func init_nodes():
	test_nodes = get_parent().get_node("TestNodes")
	current_power_node = null

func get_current_power_node() -> Node:
	return current_power_node

func set_current_power_node(power_node:Node):
	#Its the StaticBody under PowerNode
	current_power_node = power_node

#For some reason, calling set_current_power_node with NULL from C# crashes the game
#So this is temporary solution
func clear_current_power_node():
	current_power_node = null
	
func _map_edge(t1, t2_path, tower_edge_map):
	if not t1 or not t2_path:
		return
	var t2 = t1.get_node(t2_path)
	if not t2:
		return
	if not tower_edge_map.has(t1):
		tower_edge_map[t1] = {}
	tower_edge_map[t1][t2] = true
	if not tower_edge_map.has(t2):
		tower_edge_map[t2] = {}
	tower_edge_map[t2][t1] = true

func _generate_path(root_tower, tower, tower_edge_map, visited_set, is_first = false):
	visited_set[tower] = true
	var tree_root = { "node": tower, "next_nodes": [] }
	var nodes = tower_edge_map.get(tower)
	if nodes and (not tower.team_spawn or is_first):
		for next_node in nodes:
			var force_visit = next_node != root_tower and next_node.team_spawn
			if visited_set.has(next_node) and not force_visit:
				continue
			tree_root["next_nodes"].append(_generate_path(root_tower, next_node, tower_edge_map, visited_set))
	return tree_root


func do_spawn():
	var teams = []
	if red_path:
		teams.append({ "team": "red", "enemy_team": "blue", "enemy_tower": blue_tower, "path": red_path })
	if blue_path:
		teams.append({ "team": "blue", "enemy_team": "red", "enemy_tower": red_tower, "path": blue_path })
	for team_data in teams:
		var team = team_data["team"]
		var path = team_data["path"]

		for i in range(spawns_per_wave):
			var mob = mob_prefab.instance()
			add_child(mob)
			mob.initialize(path, team_data["team"], team_data["enemy_team"])


func _on_Timer_timeout():
	if (DEBUG_SPAWN):
		do_spawn()
