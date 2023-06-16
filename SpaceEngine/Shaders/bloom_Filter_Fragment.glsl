#version 330

in vec2 textureCoords;
layout (location = 0) out vec4 out_Colour;

uniform sampler2D gDiffuse;
uniform sampler2D gPosition;
float bloomDampener = 25.5;
void main(void){

	vec3 diffuse = texture(gDiffuse, textureCoords).rgb;
	float bloom = texture(gPosition, textureCoords).a;

	float luminance = dot(diffuse, vec3(0.2126, 0.7152, 0.0722));
	out_Colour.rgb = clamp(1-1/pow(luminance, 1/bloomDampener), 0, 9999)*diffuse+diffuse*bloom;
	out_Colour.a = 1.0f;
}