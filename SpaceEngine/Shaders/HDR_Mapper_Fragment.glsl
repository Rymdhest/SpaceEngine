#version 330

in vec2 textureCoords;
layout (location = 0) out vec4 out_Colour;

uniform sampler2D HDRcolorTexture;
uniform float radius;
float gamma = .35;
float exposure =.25f;
void main(void){

	vec3 color =  texture(HDRcolorTexture, textureCoords).rgb;
	color = vec3(1.0) - exp(-color * exposure);
	//color = color / (color + vec3(1.0));
	color = pow(color, vec3(1.0 / gamma));
	out_Colour =  vec4(color, 1.0f);
}