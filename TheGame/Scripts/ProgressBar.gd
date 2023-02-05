extends Sprite3D

onready var bar = $Viewport/ProgressBar2D

func _ready():
	bar.value(50);
	texture = $Viewport.get_texture()

func update(value, full):
	pass
