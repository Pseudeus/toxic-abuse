shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

uniform float radial_blur_lod : hint_range(0.0, 0.1) = 0.01;
uniform float blur_lod : hint_range(0.0, 5.0) = 0.0;
uniform int radial_iterations : hint_range(0, 10, 1);


// --- Functions --- //
void fragment() {
	vec3 rad_blur = texture(SCREEN_TEXTURE, SCREEN_UV, blur_lod).rgb;
	
	for (int i = 1; i <= radial_iterations; i++)
	{
		rad_blur += texture(SCREEN_TEXTURE, SCREEN_UV * (1.0 - (radial_blur_lod * float(i))) + 0.5 * radial_blur_lod * float(i), blur_lod).rgb;
	}
	
	rad_blur /= 10.0;
	
	COLOR.rgb = rad_blur;
}