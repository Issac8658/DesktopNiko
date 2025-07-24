shader_type canvas_item;

uniform float offset_power = 0.1;
void fragment() {
	vec4 randomcolor = texture(TEXTURE, UV * 10.0);
	vec2 offset = (COLOR.yx + randomcolor.xy + vec2(0.01)) * offset_power;
	COLOR = texture(TEXTURE, UV + offset);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
