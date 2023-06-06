#version 330

layout(location=0) in vec3 position;
layout(location=1) in vec3 color;
layout(location=2) in vec3 normal;

out vec3 fragColor;
out vec3 positionViewSpace;
out vec3 fragNormal;
uniform mat4 TransformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main() {
	gl_Position =  vec4(position, 1.0)*TransformationMatrix*viewMatrix*projectionMatrix;
	positionViewSpace =  (vec4(position, 1.0)*TransformationMatrix*viewMatrix).xyz;
	fragColor = color;
	fragNormal = (inverse(viewMatrix)*vec4(normal, 1.0f)).xyz;
}