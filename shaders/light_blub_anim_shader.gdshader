shader_type canvas_item;

void vertex() {
	float a = floor((TIME - floor(TIME)) * 24.0)/25.0;
	float x = floor((a*5.0 - floor(a*5.0)) * 5.0) / 5.0;
	float y = floor((a - floor(a)) * 5.0) / 5.0;
	UV /= 5.0;
	UV.x += x;
	UV.y += y;
}
