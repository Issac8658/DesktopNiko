shader_type spatial;
render_mode unshaded, skip_vertex_transform;

uniform sampler2D noise;
uniform float wave_speed = 1.0;
uniform float edge_size = 1.0;

uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, filter_linear_mipmap;

const vec3 color2 = vec3(0.1,0.2,0.5);

varying vec3 noise_texture;
varying vec3 water_texture_height;
varying vec3 globalVertex;

void vertex() {
	UV = (VERTEX.xz + NODE_POSITION_WORLD.xz) / 80.0;
	noise_texture = texture(noise, UV + vec2(TIME * 0.02 * wave_speed)).xyz;
	water_texture_height = texture(noise, UV - (noise_texture.xy * 0.1 + vec2(TIME * 0.02 * wave_speed))).xyz;
	globalVertex = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	VERTEX.y += water_texture_height.x * 0.5;
	VERTEX = (MODELVIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
}
float edge(float depth){
	depth = 2.0 * depth - 1.0;
	return 1.0 / (1.0 + depth);
}

void fragment() {
	float z_depth = edge(texture(DEPTH_TEXTURE, SCREEN_UV).r);
	float z_pos = edge(FRAGCOORD.z);
	float z_dif = z_depth - z_pos - (edge_size - 1.0);

	ALBEDO = mix(vec3(0.04, 0.04, 0.2), color2, pow(water_texture_height.x, 8.0)) + vec3(1.0 - step(1.0, z_dif)) * color2;
	ALPHA = water_texture_height.x + 0.1;
}