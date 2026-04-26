extends Node2D

@export var bob_height: float = 5.0
@export var bob_speed: float = 1.6
@export var breathe_scale_amount: float = 0.018
@export var breathe_speed: float = 1.25

@onready var visuals: Node2D = $Visuals

var _base_position: Vector2
var _base_scale: Vector2
var _time := 0.0

func _ready() -> void:
	_base_position = visuals.position
	_base_scale = visuals.scale

func _process(delta: float) -> void:
	_time += delta
	var bob_offset := sin(_time * TAU * 0.25 * bob_speed) * bob_height
	var breathe := 1.0 + sin(_time * TAU * 0.25 * breathe_speed) * breathe_scale_amount
	visuals.position = _base_position + Vector2(0, bob_offset)
	visuals.scale = _base_scale * breathe
