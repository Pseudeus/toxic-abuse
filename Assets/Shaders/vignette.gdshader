shader_type canvas_item;

uniform bool frame = false;
uniform sampler2D img;
uniform float alpha = 1.0;
uniform float inner_radius = 0.0;
uniform float outer_radius = 1.0;

void fragment() {
	float x = abs(UV.r - 0.5) * 2.0;
	float y = abs(UV.g - 0.5) * 2.0;
	float q = 1.0 - (1.0 - sqrt(x * x + y * y) / outer_radius) / (1.0 - inner_radius);
	if (frame)
	{
		vec4 tex = texture(img, UV);
		COLOR = vec4(tex.r, tex.g, tex.b, -q * alpha);
	}
	else
	{
		COLOR = vec4(0, 0, 0, q * alpha);
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
