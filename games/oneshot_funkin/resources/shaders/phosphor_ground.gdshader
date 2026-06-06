shader_type spatial;
render_mode unshaded, skip_vertex_transform;

uniform sampler2D phosphor_texture : filter_nearest;
uniform sampler2D noise_texture;
const vec3 color1 = vec3(.065,.065,.26);
const vec3 color2 = vec3(0.1,0.43,0.5);

varying vec3 globalVertex;


void vertex() {
	UV = VERTEX.xz + NODE_POSITION_WORLD.xz;
	globalVertex = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	VERTEX = (MODELVIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
    NORMAL = normalize((MODEL_MATRIX * vec4(NORMAL, 0.0)).xyz);
}

void fragment() {
	float height = (1.0-COLOR.r) * -8.0;
	if (dot(NORMAL, vec3(0,1,0)) <= 0.5) {
		ALBEDO = (color1 + color2 * 0.3 * pow(clamp(height + 1.0625, 0.0, 1.0), 1200.0)) * clamp(height / 5.0 + 1.0, 0.0, 1.0);
	}
	else {
		ALBEDO = color1 + texture(phosphor_texture, UV / 16.0).xyz * clamp(FRAGCOORD.z*50.0 * 1.0, 0.0, 1.0) * color2 * (texture(noise_texture, (UV + vec2(TIME / 2.0)) / 20.0 ).xyz + 1.0);
	}
}
