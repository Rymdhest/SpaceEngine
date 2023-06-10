#version 330

in vec2 textureCoords;
layout (location = 0) out vec4 out_Colour;

uniform sampler2D gDiffuse;
uniform sampler2D gPosition;

void main(void){

	vec3 diffuse = texture(gDiffuse, textureCoords).rgb;
	float bloom = texture(gPosition, textureCoords).a;
	out_Colour.rgb = diffuse*bloom;
	out_Colour.a = 1.0f;
}