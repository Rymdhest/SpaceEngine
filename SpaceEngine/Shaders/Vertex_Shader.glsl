#version 330

layout(location=0) in vec3 vertices;
layout(location=1) in vec3 colors;

out vec3 geoColor;
out vec3 geoPos;
uniform mat4 TransformationMatrix;
uniform mat4 projectionMatrix;

void main() {
	gl_Position = vec4(vertices, 1.0) *TransformationMatrix* projectionMatrix;
	geoPos =  (vec4(vertices, 1.0) *TransformationMatrix).xyz;
	geoColor = colors;
}