#version 410

layout(location=0) in vec3 vertices;
layout(location=1) in vec3 colors;

out vec3 fragColor;

void main() {
	gl_Position = vec4(vertices, 1.0);	
	fragColor = colors;
}