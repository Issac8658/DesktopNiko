shader_type sky;

const vec3 sky_color = vec3(0.5, 0.0, 0.25);
const vec3 sky_color2 = vec3(0.0,0.2,0.15);
uniform sampler2D noise;

void sky() {
	vec3 blue_color = vec3(0.1, 0.0, 0.1);
	vec3 blue_color2 = vec3(0.05, 0.0, 0.1);
	vec3 blue_sky = mix(mix(blue_color, blue_color2, clamp(EYEDIR.y-0.1, 0.0, 1.0)), vec3(0.0), clamp(-EYEDIR.y + 0.6, 0.0, 1.0));

	float green_color = clamp(1.0 - (EYEDIR.y+0.2)/2.0 - 0.5, 0.0, 1.0);
	float green_color2 = pow(1.0 - abs(EYEDIR.y - 0.2), 2.0);
	vec3 green_sky = vec3(green_color) * sky_color + vec3(green_color2) * sky_color2 + texture(noise, (EYEDIR.xz + 1.0) / 2.0).xyz / 2.0 * sky_color2;

	COLOR = blue_sky;
}
