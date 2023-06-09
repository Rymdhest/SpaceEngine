#version 330

layout(location=0) in vec3 position;
layout(location=1) in vec3 color;
layout(location=2) in vec3 normal;

out vec3 fragColor;
out vec3 positionViewSpace;
flat out vec3 fragNormal;
uniform mat4 modelViewMatrix;
uniform mat4 modelViewProjectionMatrix;
uniform mat4 normalModelViewMatrix;

void main() {
	
	gl_Position =  vec4(position, 1.0)*modelViewProjectionMatrix;
	positionViewSpace =  (vec4(position, 1.0)*modelViewMatrix).xyz;
	fragColor = color;
	fragNormal = (vec4(normal, 1.0f)*normalModelViewMatrix).xyz;
}