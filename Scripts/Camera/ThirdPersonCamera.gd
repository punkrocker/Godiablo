# Deprecated: ThirdPersonCamera.gd replaced by C# implementation ThirdPersonCamera.cs
# Kept as a stub to avoid accidental editor load errors if referenced elsewhere.
# No runtime behavior here.

extends Camera3D

@export var target_path: NodePath
@export var auto_find_target: bool = true
@export var target_group: String = "player"

@export var distance: float = 5.0
@export var min_distance: float = 2.0
@export var max_distance: float = 12.0
@export var zoom_speed: float = 1.0

@export var yaw: float = 0.0 # radians
@export var pitch: float = -0.2 # radians (~ -11 degrees)
@export var min_pitch: float = deg_to_rad(-60.0)
@export var max_pitch: float = deg_to_rad(60.0)

@export var rotation_sensitivity: float = 0.003
@export var pan_sensitivity: float = 0.002
@export var smooth_speed: float = 10.0

var _target: Node3D = null
var _desired_distance: float
var _desired_yaw: float
var _desired_pitch: float
var _pivot_offset: Vector3 = Vector3.ZERO
var _desired_pivot_offset: Vector3 = Vector3.ZERO

func _ready():
	_desired_distance = distance
	_desired_yaw = yaw
	_desired_pitch = pitch

	if target_path and target_path != NodePath(""):
		if has_node(target_path):
			_target = get_node(target_path)
		elif auto_find_target:
			# try find by group
			var candidates = get_tree().get_nodes_in_group(target_group)
			if candidates.size() > 0:
				_target = candidates[0]
		# leave _target null if not found

func set_target(node: Node3D) -> void:
	_target = node

func _input(event):
	if event is InputEventMouseMotion:
		# Rotate when RMB (camera_rotate) pressed
		if Input.is_action_pressed("camera_rotate"):
			_desired_yaw -= event.relative.x * rotation_sensitivity
			_desired_pitch -= event.relative.y * rotation_sensitivity
			_desired_pitch = clamp(_desired_pitch, min_pitch, max_pitch)
		# Pan when MMB (camera_pan) pressed
		elif Input.is_action_pressed("camera_pan"):
			var right = global_transform.basis.x
			var up = global_transform.basis.y
			_desired_pivot_offset += (-right * event.relative.x + up * event.relative.y) * pan_sensitivity * _desired_distance

	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_WHEEL_UP and event.pressed:
			_desired_distance = max(min_distance, _desired_distance - zoom_speed)
		elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN and event.pressed:
			_desired_distance = min(max_distance, _desired_distance + zoom_speed)

func _physics_process(delta: float) -> void:
	# Keyboard zoom controls
	if Input.is_action_pressed("camera_zoom_in"):
		_desired_distance = max(min_distance, _desired_distance - zoom_speed * delta * 60.0)
	if Input.is_action_pressed("camera_zoom_out"):
		_desired_distance = min(max_distance, _desired_distance + zoom_speed * delta * 60.0)

	# Smoothly interpolate
	yaw = lerp(yaw, _desired_yaw, clamp(delta * smooth_speed, 0.0, 1.0))
	pitch = lerp(pitch, _desired_pitch, clamp(delta * smooth_speed, 0.0, 1.0))
	distance = lerp(distance, _desired_distance, clamp(delta * smooth_speed, 0.0, 1.0))
	_pivot_offset = _pivot_offset.lerp(_desired_pivot_offset, clamp(delta * smooth_speed, 0.0, 1.0))

	if _target:
		var target_pos = _target.global_transform.origin + _pivot_offset
		# Construct rotation basis from yaw and pitch
		var rot = Basis(Vector3.UP, yaw).rotated(Vector3.RIGHT, pitch)
		# Camera offset in local space (forward is -Z in Godot), so use Vector3(0,0,distance)
		var offset = rot.xform(Vector3(0.0, 0.0, distance))
		global_transform.origin = target_pos - offset
		look_at(target_pos, Vector3.UP)
