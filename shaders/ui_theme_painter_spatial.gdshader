shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx, unshaded;

#include "res://shaders/theme_include.gdshaderinc"

void fragment() {
	ALBEDO = getThemeColor(COLOR).xyz;
}
