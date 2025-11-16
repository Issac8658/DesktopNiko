shader_type canvas_item;

uniform sampler2D codebgTexture : repeat_enable;
const float time_scale = 6.0;
const float scale = 50.0;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec3 col1 = texture(codebgTexture, (UV.xy - vec2(TIME / time_scale, 0)) / scale).bbb;
	vec3 col2 = texture(codebgTexture, (UV.xy - vec2(0, TIME / time_scale)) / scale).bbb;
	vec3 col3 = texture(codebgTexture, (UV.xy + vec2(TIME / time_scale, 0)) / scale).bbb;
	vec3 col4 = texture(codebgTexture, (UV.xy + vec2(0, TIME / time_scale)) / scale).bbb;
	COLOR.rgb = COLOR.rgb - (col1 + col2 + col3 + col4) * 2.0;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
