shader_type spatial;
render_mode unshaded;

uniform sampler2D phosphor_texture : filter_nearest;
uniform sampler2D noise_texture;
uniform float effect_scale = 16.0;
uniform float effect_depth = 1.0;
varying vec2 UV_l1;
varying vec2 UV_l2;
varying vec2 UV_l3;
varying vec2 UV_l4;
varying vec2 UV_l5;
varying vec2 UV_l6;
varying vec2 UV_l7;
varying vec2 UV_l8;
varying vec2 UV_l9;
varying vec2 UV_l10;
varying vec2 UV_l11;
varying vec2 UV_l12;
const vec3 color1 = vec3(0.1,0.1,0.5);
const vec3 color2 = vec3(0.1,0.2,0.5);

vec3 scale_vector(vec3 vec, vec3 cent, float scale) {
	return ((vec - cent) * scale + cent);
}

float parallax_scale(float height,float scale) {
	return 1.0 - (1.0 / height) * scale;
}

void vertex() {
	VERTEX += NODE_POSITION_WORLD;
	float height = -CAMERA_POSITION_WORLD.y + VERTEX.y;
	UV_l1 = VERTEX.xz;
	UV_l2 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth))).xz;
	UV_l3 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth * 2.))).xz;
	UV_l4 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth * 3.))).xz;
	UV_l5 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth * 4.))).xz;
	UV_l6 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth * 5.))).xz;
	UV_l7 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth * 20.))).xz + vec2(1.0, 1.0);
	UV_l8 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth * 21.))).xz + vec2(1.0, 1.0);
	UV_l9 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth * 22.))).xz + vec2(1.0, 1.0);
	UV_l10 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth * 23.))).xz + vec2(1.0, 1.0);
	UV_l11 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth * 24.))).xz + vec2(1.0, 1.0);
	UV_l12 = (scale_vector(VERTEX, CAMERA_POSITION_WORLD, parallax_scale(height, effect_depth * 25.))).xz + vec2(1.0, 1.0);
	VERTEX -= NODE_POSITION_WORLD;
}

void fragment() {
	vec3 layer1 = clamp(texture(phosphor_texture, UV_l1 / effect_scale).xyz, vec3(0.075), vec3(1.0)) * color1 * texture(noise_texture, UV_l1 + vec2(TIME/4.5)).xyz;
	vec3 layer2 = clamp(texture(phosphor_texture, UV_l2 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l2 + vec2(TIME/4.0)).xyz;
	vec3 layer3 = clamp(texture(phosphor_texture, UV_l3 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l3 + vec2(TIME/3.5)).xyz;
	vec3 layer4 = clamp(texture(phosphor_texture, UV_l4 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l4 + vec2(TIME/3.0)).xyz;
	vec3 layer5 = clamp(texture(phosphor_texture, UV_l5 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l5 + vec2(TIME/2.5)).xyz;
	vec3 layer6 = clamp(texture(phosphor_texture, UV_l6 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l6 + vec2(TIME/2.0)).xyz;
	vec3 layer7 = clamp(texture(phosphor_texture, UV_l7 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l7 + vec2(TIME/4.0)).xyz;
	vec3 layer8 = clamp(texture(phosphor_texture, UV_l8 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l8 + vec2(TIME/4.0)).xyz;
	vec3 layer9 = clamp(texture(phosphor_texture, UV_l9 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l9 + vec2(TIME/3.5)).xyz;
	vec3 layer10 = clamp(texture(phosphor_texture, UV_l10 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l10 + vec2(TIME/3.0)).xyz;
	vec3 layer11 = clamp(texture(phosphor_texture, UV_l11 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l11 + vec2(TIME/2.5)).xyz;
	vec3 layer12 = clamp(texture(phosphor_texture, UV_l12 / effect_scale).xyz, vec3(0.01), vec3(1.0)) * color2 * texture(noise_texture, UV_l12 + vec2(TIME/2.0)).xyz;
	ALBEDO = layer1 + layer2 + layer3 + layer4 + layer5 + layer6 + layer7 + layer8 + layer9 + layer10 + layer11 + layer12;
	ALPHA = clamp(ALBEDO.b, 0.5, 1.0);
}