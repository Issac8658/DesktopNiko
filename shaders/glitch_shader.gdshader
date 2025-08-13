shader_type canvas_item;

uniform float glitch_offset = 0.0;
uniform float color_glitch_power = 0.0;
uniform float invert_color = 0.0;
uniform float sin_distortion = 0.0;
uniform float cos_distortion = 0.0;
uniform float color_distortion = 0.0;
uniform vec2 uv_offset = vec2(0);
uniform vec2 sin_offset = vec2(0);
uniform float sin_offset_power = 0.2;

void fragment() {
	vec2 pre_uv = UV + uv_offset + sin(sin_offset) * sin_offset_power - COLOR.xy * color_glitch_power;
	vec2 sin_uv = pow(sin(pre_uv * PI / 2.0), vec2(2.0));
	vec2 cos_uv = pow(cos(pre_uv * PI / 2.0), vec2(2.0));
	vec2 sin_uv_m = mix(pre_uv, sin_uv, sin_distortion);
	vec2 uv = mix(sin_uv_m, cos_uv, cos_distortion);
	COLOR = texture(TEXTURE, uv - vec2(uv.y * glitch_offset,0));
	COLOR.xyz = mix(COLOR.xyz, 1.0 - cos(COLOR.xyz * PI + COLOR.xyz * color_distortion + UV.xyx), clamp(color_distortion, -1.0, 1.0));
	COLOR.xyz = mix(COLOR.xyz, vec3(1) - COLOR.xyz, invert_color);
}
