extends Node2D

const ATTACK_ANIMATION := &"attack"
const IDLE_ANIMATION := &"idle"
const DIE_ANIMATION := &"die"
const LOG_PREFIX := "[exusiai]"

@onready var visuals: AnimatedSprite2D = $Visuals

func _ready() -> void:
	print(LOG_PREFIX, " exusiai visuals ready")
	if visuals.sprite_frames == null:
		print(LOG_PREFIX, " exusiai visuals missing sprite_frames")
		return
	visuals.animation_finished.connect(_on_animation_finished)
	play_idle()

func play_idle() -> void:
	print(LOG_PREFIX, " exusiai play_idle")
	if visuals.sprite_frames == null:
		return
	_set_frames(IDLE_ANIMATION)
	visuals.play(&"default")

func play_attack() -> void:
	print(LOG_PREFIX, " exusiai play_attack")
	if visuals.sprite_frames == null:
		return
	_set_frames(ATTACK_ANIMATION)
	visuals.play(&"default")

func play_die() -> void:
	print(LOG_PREFIX, " exusiai play_die")
	if visuals.sprite_frames == null:
		return
	_set_frames(DIE_ANIMATION)
	visuals.play(&"default")

func _set_frames(anim_name: StringName) -> void:
	var path := "res://myfirstmod/scenes/character/exusiai_%s_sprite_frames.tres" % String(anim_name)
	print(LOG_PREFIX, " exusiai set_frames ", path)
	var frames := load(path) as SpriteFrames
	if frames != null:
		visuals.sprite_frames = frames
	else:
		print(LOG_PREFIX, " exusiai failed to load frames ", path)

func _on_animation_finished() -> void:
	print(LOG_PREFIX, " exusiai animation_finished")
	if visuals.sprite_frames == null:
		return
	if visuals.sprite_frames.get_animation_loop(&"default"):
		return
	if visuals.sprite_frames.get_frame_count(&"default") <= 24:
		play_idle()
