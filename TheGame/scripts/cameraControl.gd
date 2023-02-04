extends Spatial

export var SCROLL_SPEED:float = 10
export var ZOOM_SPEED:float = 2

export var ZOOM_MIN:float = 5
export var ZOOM_MAX:float = 18

export var MAP_BOUNDARY:int = 40

var UP:bool = false
var DOWN:bool = false
var LEFT:bool = false
var RIGHT:bool = false

func outsideMapBoundary(position, modifier:int = 0):
	return position < (MAP_BOUNDARY - modifier)

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _input(event):
	if event is InputEventMouseMotion:
		# rotation
		if event.button_mask&(BUTTON_MASK_MIDDLE+BUTTON_MASK_RIGHT):
			self.rotate(Vector3(0, 1, 0), event.relative.x * -0.002)

		# scroll x
		if event.position.x<=5:
			LEFT=true
		elif event.position.x>OS.window_size.x-5:
			RIGHT=true
		else:
			LEFT=false
			RIGHT=false

		#scroll y
		if event.position.y<=5:
			UP=true
		elif event.position.y>OS.window_size.y-5:
			DOWN=true
		else:
			UP=false
			DOWN=false

	# zoom
	if event is InputEventMouseButton:
		if event.is_pressed():
			# zoom out
			if event.button_index == BUTTON_WHEEL_DOWN:
				if self.transform.origin.y < ZOOM_MAX:
					self.translate(Vector3(0, ZOOM_SPEED, 0))
			# zoom in
			elif event.button_index == BUTTON_WHEEL_UP:
				if self.transform.origin.y > ZOOM_MIN:
					self.translate(Vector3(0, -ZOOM_SPEED, 0))

func _process(delta):
	if Input.is_key_pressed(KEY_W) or UP:
		if outsideMapBoundary(-self.transform.origin.z, 20):
			self.translate(Vector3(0,0,-SCROLL_SPEED*delta))

	if Input.is_key_pressed(KEY_S) or DOWN:
		if outsideMapBoundary(self.transform.origin.z, -35):
			self.translate(Vector3(0,0,SCROLL_SPEED*delta))

	if Input.is_key_pressed(KEY_A) or LEFT:
		if outsideMapBoundary(-self.transform.origin.x):
			self.translate(Vector3(-SCROLL_SPEED*delta,0,0))

	if Input.is_key_pressed(KEY_D) or RIGHT:
		if outsideMapBoundary(self.transform.origin.x, 5):
			self.translate(Vector3(SCROLL_SPEED*delta,0,0))

	if Input.is_key_pressed(KEY_Q):
		self.rotate(Vector3(0,1,0),1*delta)

	if Input.is_key_pressed(KEY_E):
		self.rotate(Vector3(0,1,0),-1*delta)

