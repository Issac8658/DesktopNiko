shader_type canvas_item;

uniform float height : hint_range(0.0, 1.0, 0.001) = 0.0;
uniform sampler2D text;

void fragment() {
	vec4 color = texture(text, SCREEN_UV);
	if (clamp(color.r - (1.0 - height), 0.0, 1.0) > 0.0)
	{
		COLOR.w = 1.0 * COLOR.a;
	}
	else {
		COLOR.w = 0.0;
	}
}