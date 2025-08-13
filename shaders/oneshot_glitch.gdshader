shader_type canvas_item;

uniform float scale = 1.0;
uniform float pixelate = 1.0;
uniform float glich_speed = 1.0;
uniform vec3 glich_color = vec3(0,0, .6);

void fragment() {
	float noise = texture(TEXTURE, (round(FRAGCOORD.xy / pixelate) + TIME * glich_speed) * pixelate * TEXTURE_PIXEL_SIZE / scale).r;
	float noise2 = texture(TEXTURE, (round(FRAGCOORD.yx / pixelate + 0.5) + TIME * glich_speed) * pixelate * TEXTURE_PIXEL_SIZE / scale).r;
	float glitch1 = round(sin(noise * 100.0) - 0.3);
	float glitch2 = round(sin(noise2 * 100.0) + 0.3);
	COLOR = vec4(vec3(glitch1) * glich_color, glitch1 + glitch2);
}