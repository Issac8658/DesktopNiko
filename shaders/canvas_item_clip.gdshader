shader_type canvas_item;

uniform float clipTop = 0;
uniform float clipRight = 0;
uniform float clipBottom = 0;
uniform float clipLeft = 0;

const vec4 transparent = vec4(0, 0, 0, 0);

void fragment() {
	COLOR = UV.y > clipTop ? COLOR : transparent; 
	COLOR = 1.0 - UV.x > clipRight ? COLOR : transparent; 
	COLOR = UV.x > clipLeft ? COLOR : transparent; 
	COLOR = 1.0 - UV.y > clipBottom ? COLOR : transparent; 
}
