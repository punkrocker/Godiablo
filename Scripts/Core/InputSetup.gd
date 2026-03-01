# Autoload singleton to register input actions at runtime
# Place this file in Scripts/Core and add it as an autoload in project.godot
extends Node

func _ready():
	# Define WoW-style third-person actions and their key/mouse mappings
	var actions_map = {
		# Movement (camera-relative)
		"move_forward": [KEY_W, KEY_UP],
		"move_backward": [KEY_S, KEY_DOWN],
		"move_left": [KEY_A, KEY_LEFT],
		"move_right": [KEY_D, KEY_RIGHT],

		# Movement modifiers
		"jump": [KEY_SPACE],
		"sprint": [KEY_SHIFT],
		"sneak": [KEY_CTRL, KEY_C],
		"auto_run": [KEY_R],

		# Camera controls
		"camera_rotate": [], # bound to mouse right button as an action, motion handled in code
		"camera_pan": [],    # bound to middle mouse button
		"camera_zoom_in": [KEY_PAGEUP],
		"camera_zoom_out": [KEY_PAGEDOWN],
		"toggle_camera_lock": [KEY_V],

		# Targeting / interaction
		"target_nearest": [KEY_TAB],
		"interact": [KEY_E],
		"attack": [],

		# Combat / abilities (number keys)
		"ability_1": [KEY_1],
		"ability_2": [KEY_2],
		"ability_3": [KEY_3],
		"ability_4": [KEY_4],
		"ability_5": [KEY_5],

		# UI / misc
		"open_inventory": [KEY_I],
		"open_quest_journal": [KEY_J],
		"open_map": [KEY_M],
		"pause": [KEY_ESCAPE]
	}

	# Ensure mouse buttons are available as actions where appropriate
	# (we'll add the actual mouse-button events explicitly below)
	if not InputMap.has_action("camera_rotate"):
		InputMap.add_action("camera_rotate")
	if not InputMap.has_action("camera_pan"):
		InputMap.add_action("camera_pan")

	# Iterate and ensure keyboard/button actions exist; create events and persist only if not already persisted
	for action_name in actions_map.keys():
		# ensure action exists in InputMap
		if not InputMap.has_action(action_name):
			InputMap.add_action(action_name)

		# If project already has persisted events for this action, skip creating duplicates
		var setting_key = "input/actions/" + action_name
		if ProjectSettings.has_setting(setting_key):
			var existing = ProjectSettings.get_setting(setting_key)
			if existing and existing.size() > 0:
				continue

		# Create events based on keys
		var keys = actions_map[action_name]
		var created_events := []
		for k in keys:
			var ev = InputEventKey.new()
			ev.keycode = k
			InputMap.action_add_event(action_name, ev)
			created_events.append(ev)

		# Special cases: add mouse buttons for camera and attack
		if action_name == "attack":
			var mb_a = InputEventMouseButton.new()
			mb_a.button_index = MOUSE_BUTTON_LEFT
			mb_a.pressed = true
			InputMap.action_add_event(action_name, mb_a)
			created_events.append(mb_a)

		if action_name == "camera_rotate":
			var mb_r = InputEventMouseButton.new()
			mb_r.button_index = MOUSE_BUTTON_RIGHT
			mb_r.pressed = true
			InputMap.action_add_event(action_name, mb_r)
			created_events.append(mb_r)

		if action_name == "camera_pan":
			var mb_m = InputEventMouseButton.new()
			mb_m.button_index = MOUSE_BUTTON_MIDDLE
			mb_m.pressed = true
			InputMap.action_add_event(action_name, mb_m)
			created_events.append(mb_m)

		# Persist the created_events to ProjectSettings so the editor shows them
		ProjectSettings.set_setting(setting_key, created_events)

	# Save project settings (writes project.godot)
	ProjectSettings.save()
