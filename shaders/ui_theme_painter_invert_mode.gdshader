shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

#include "theme_include.gdshaderinc"

void fragment() {
	COLOR = getThemeColor(COLOR);
	vec4 screen_color = texture(screen_texture, SCREEN_UV);
	vec3 inverted = vec3(1.0) - screen_color.xyz;
	inverted = vec3(mix(screen_color.x, inverted.x, COLOR.x), mix(screen_color.y, inverted.y, COLOR.y), mix(screen_color.z, inverted.z, COLOR.z));
	COLOR.xyz = inverted;
}
