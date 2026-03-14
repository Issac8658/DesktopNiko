shader_type canvas_item;

#include "theme_include.gdshaderinc"

void fragment() {
	COLOR = getThemeColor(COLOR);
}
