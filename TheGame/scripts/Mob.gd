extends KinematicBody

# Emitted when the player jumped on the mob.
signal squashed

# Minimum speed of the mob in meters per second.
export var min_speed = 2
# Maximum speed of the mob in meters per second.
export var max_speed = 4
export var attack_distance = 3.0

var velocity = Vector3.ZERO
var direction = Vector3.ZERO
var current_target
var current_targets = []
var path
var current_subpath
var current_target_tower
var random_speed
var this_side
var target_side
var hp = 5
var random = RandomNumberGenerator.new()


func _ready():
	random.randomize()

func _physics_process(_delta):
	var target_is_enemy = true
	var target = current_target.get_ref()
	if not target:
		target_is_enemy = false
		target =  current_target_tower
	if not target:
		return
	var delta_to_target = target.transform.origin - transform.origin
	look_at(target.transform.origin, Vector3.UP)

	if delta_to_target.length() < attack_distance:
		if target_is_enemy:
			$AnimationPlayer.assigned_animation = "attack"
		else:
			$AnimationPlayer.assigned_animation = "move"
	else:
		$AnimationPlayer.assigned_animation = "move"
		velocity = Vector3.FORWARD * random_speed
		velocity = velocity.rotated(Vector3.UP, rotation.y)
		move_and_slide(velocity)


func initialize(path, side, target_side):
	self.path = path
	current_target_tower = path["node"]
	current_subpath = path
	var initial_position = current_target_tower.transform.origin
	initial_position.x += rand_range(-10.0, 10.0)
	initial_position.z += rand_range(-10.0, 10.0)
	translation = initial_position
	this_side = side
	self.target_side = target_side
	current_target = weakref(null)

	add_to_group(side)

	random_speed = rand_range(min_speed, max_speed)
	$AnimationPlayer.playback_speed = random_speed / min_speed
	$AnimationPlayer.assigned_animation = "move"
	$CollisionShape.disabled = false

func _update_current_target():
	var old_target = current_target
	var origin = transform.origin
	var best_target = null
	var best_distance = 100000000.0
	for weak_target in current_targets:
		var target = weak_target.get_ref()
		if not target:
			continue
		var distance = origin.distance_to(target.transform.origin)
		if distance < best_distance:
			best_distance = distance
			best_target = target
	current_target = weakref(best_target)
	if not current_target:
		$AnimationPlayer.assigned_animation = "move"

func _sweep_null_current_targets():
	for i in range(len(current_targets) - 1, -1, -1):
		var target = current_targets[i].get_ref()
		if not target or target.hp <= 0:
			current_targets.remove(i)

func squash():
	emit_signal("squashed")
	queue_free()

func get_hit():
	hp -= 1
	if hp <= 0:
		squash()

func _target_next_tower():
	if not current_subpath:
		return
	var next_nodes = current_subpath["next_nodes"]
	if not next_nodes:
		return
	var next_index = random.randi_range(0, len(next_nodes) - 1)
	current_subpath = next_nodes[next_index]
	current_target_tower = current_subpath["node"]
	_sweep_null_current_targets()
	_update_current_target()

func _on_LargeArea_body_shape_entered(body_rid, body, body_shape_index, local_shape_index):
	if current_target_tower.is_a_parent_of(body):
		_target_next_tower()
	if body.is_in_group(target_side):
		current_targets.append(weakref(body))
		_update_current_target()
	_sweep_null_current_targets()

func _on_LargeArea_body_shape_exited(body_rid, body, body_shape_index, local_shape_index):
	var find_index = current_targets.find(body)
	if find_index < 0:
		return
	current_targets.remove(find_index)
	_update_current_target()
	_sweep_null_current_targets()

func _on_Attack_hit():
	var target = current_target.get_ref()
	if not target:
		return
	current_target.get_ref().get_hit()
	_sweep_null_current_targets()
	if not current_target:
		_update_current_target()
