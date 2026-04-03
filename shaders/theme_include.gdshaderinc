global uniform vec4 theme_color_main;
global uniform vec4 theme_color_hover;
global uniform vec4 theme_color_button_outline_pressed;
global uniform vec4 theme_color_button_outline_hover;
global uniform vec4 theme_color_button_base_pressed;
global uniform vec4 theme_color_button_base_hover;
global uniform vec4 theme_color_background;
global uniform bool theme_rainbow;
global uniform bool theme_enabled;
const float mid1 = 0.649019608;
const float mid2 = 0.494117647;
const float mid3 = 0.323529412;
const float mid4 = 0.249019608;
const float mid5 = 0.162745098;
const float colorComparisonAccuracy = 1.0/256.0;

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p-K.xxx,0.0,1.0), c.y);
}

vec4 rainbow(vec4 c){
	float gray = (c.x + c.y + c.z) / 3.0;
	c.xyz = hsv2rgb(vec3(TIME / 10.0, 0.7, 1)) * gray;
	return c;
}

vec4 getThemeColor(vec4 originalColor) {
	if (!theme_enabled){
		return originalColor;
	}
	
	float gray = (originalColor.x + originalColor.y + originalColor.z) / 3.0;
	
	if (abs(originalColor.x - originalColor.y) < colorComparisonAccuracy
	 && abs(originalColor.y - originalColor.z) < colorComparisonAccuracy
	 && abs(originalColor.x - originalColor.z) < colorComparisonAccuracy) {
		vec4 color = vec4(1,1,1,1);
		if (theme_rainbow){
			color.xyz = rainbow(originalColor).xyz;
		}
		else
			if (gray >= mid1){
				float value = (gray - mid1) / (1.0 - mid1);
				color = mix(theme_color_hover, theme_color_main, value);
			}
			else if (gray >= mid2){
				float value = (gray - mid2) / (mid1 - mid2);
				color = mix(theme_color_button_outline_pressed, theme_color_hover, value);
			}
			else if (gray >= mid3){
				float value = (gray - mid3) / (mid2 - mid3);
				color = mix(theme_color_button_outline_hover, theme_color_button_outline_pressed, value);
			}
			else if (gray >= mid4){
				float value = (gray - mid4) / (mid3 - mid4);
				color = mix(theme_color_button_base_pressed, theme_color_button_outline_hover, value);
			}
			else if (gray >= mid5){
				float value = (gray - mid5) / (mid4 - mid5);
				color = mix(theme_color_button_base_hover, theme_color_button_base_pressed, value);
			}
			else {
				float value = gray / mid5;
				color = mix(theme_color_background, theme_color_button_base_hover, value);
			}
		originalColor.xyz = color.xyz;
		originalColor.a *= color.a;
	}
	return originalColor;
}