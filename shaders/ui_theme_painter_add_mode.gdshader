shader_type canvas_item;
render_mode blend_add;

#include "theme_include.gdshaderinc"

void fragment() {
	COLOR = getThemeColor(COLOR);
}
