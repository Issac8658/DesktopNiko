shader_type canvas_item;

uniform float shadow_distance = 0.5;

float alpha_clamp(vec2 uv) {
	if (uv.x < 0.0 || uv.y < 0.0 || uv.y > 1.0 || uv.x > 1.0) {
		return 0.0;
	}
	return 1.0;
}

void vertex() {
	vec2 offset = (UV - 0.5) * 2.0 * shadow_distance;
	VERTEX += offset;
	UV += offset * TEXTURE_PIXEL_SIZE;
}

void fragment() {
	vec2 moved_uv = UV - TEXTURE_PIXEL_SIZE * shadow_distance;
	vec4 original_color = COLOR * alpha_clamp(UV);
	vec4 shadow_color = texture(TEXTURE, moved_uv) * vec4(0,0,0, alpha_clamp(moved_uv));
	COLOR = mix(original_color, shadow_color, clamp(shadow_color.a - original_color.a, 0, 1));
}
