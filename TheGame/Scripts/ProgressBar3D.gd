extends Sprite3D

onready var bar:TextureProgress = $Viewport/ProgressBar2D

func _ready():
	texture = $Viewport.get_texture()
