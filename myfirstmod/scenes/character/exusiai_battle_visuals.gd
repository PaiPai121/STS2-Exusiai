extends Node2D

const ATTACK_ANIMATION := &"attack"
const IDLE_ANIMATION := &"idle"
const DIE_ANIMATION := &"die"

@onready var visuals: AnimatedSprite2D = $Visuals

func _ready() -> void:
	print("[myfirstmod] exusiai visuals ready")
	if visuals.sprite_frames == null:
		print("[myfirstmod] exusiai visuals missing sprite_frames")
		return
	visuals.animation_finished.connect(_on_animation_finished)
	play_idle()

func play_idle() -> void:
	print("[myfirstmod] exusiai play_idle")
	if visuals.sprite_frames == null:
		return
	_set_frames(IDLE_ANIMATION)
	visuals.play(&"default")

func play_attack() -> void:
	print("[myfirstmod] exusiai play_attack")
	if visuals.sprite_frames == null:
		return
	_set_frames(ATTACK_ANIMATION)
	visuals.play(&"default")

func play_die() -> void:
	print("[myfirstmod] exusiai play_die")
	if visuals.sprite_frames == null:
		return
	_set_frames(DIE_ANIMATION)
	visuals.play(&"default")

func _set_frames(anim_name: StringName) -> void:
	var path := "res://myfirstmod/scenes/character/exusiai_%s_sprite_frames.tres" % String(anim_name)
	print("[myfirstmod] exusiai set_frames ", path)
	var frames := load(path) as SpriteFrames
	if frames != null:
		visuals.sprite_frames = frames
	else:
		print("[myfirstmod] exusiai failed to load frames ", path)

func _on_animation_finished() -> void:
	print("[myfirstmod] exusiai animation_finished")
	if visuals.sprite_frames == null:
		return
	if visuals.sprite_frames.get_animation_loop(&"default"):
		return
	if visuals.sprite_frames.get_frame_count(&"default") <= 24:
		play_idle()
