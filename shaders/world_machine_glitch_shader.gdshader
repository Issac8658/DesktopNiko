shader_type canvas_item;

const vec4 overlay_color = vec4(1.0,0.6,1.0,1.0);
uniform sampler2D mask;

void fragment() {
	float middle = (COLOR.r + COLOR.g + COLOR.b)/3.0;
	float light = sin((round(TIME * 20.0) + UV.y / TEXTURE_PIXEL_SIZE.y / 2.0) * PI) * 0.1;
	vec4 overlay = mix(vec4(middle, middle, middle, 1.0), overlay_color, 0.2) + vec4(light, light, light, 0.0);
	COLOR = vec4(overlay.xyz, texture(mask, UV).r * COLOR.a);
}