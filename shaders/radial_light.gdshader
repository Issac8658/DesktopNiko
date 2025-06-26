shader_type canvas_item;

void fragment() {
	float pos_x = (UV.x - 0.5) * 2.0;
	float pos_y = (UV.y - 0.5) * 2.0;
	float mult = pow(sin((atan(pos_y, pos_x) * 10.0) + 1.0 + TIME * 10.0), 0.15);
	COLOR.a *= mult;
}
