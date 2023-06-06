#version 330

layout(location=0) in vec3 position;
layout(location=1) in vec3 color;

out vec3 geoColor;
out vec3 positionViewSpace;
uniform mat4 TransformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main() {
	gl_Position =  vec4(position, 1.0)*TransformationMatrix*viewMatrix*projectionMatrix;
	positionViewSpace =  (vec4(position, 1.0)*TransformationMatrix*viewMatrix).xyz;
	geoColor = color;
}