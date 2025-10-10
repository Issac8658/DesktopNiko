shader_type spatial;
render_mode unshaded;

uniform sampler2D shadow;
uniform sampler2D dither_texture : filter_nearest;
uniform vec2 dither_res;

void fragment() {
	ALBEDO = vec3(0);
	ALPHA = round((1.0 - texture(shadow, SCREEN_UV).r + texture(dither_texture, FRAGCOORD.xy / dither_res).r) / 2.0) + 0.2;
}