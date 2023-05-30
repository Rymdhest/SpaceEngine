#version 330

in vec3 fragColor;
out vec4 outColor;

uniform float saturation;

void main() {
	outColor = vec4(fragColor*saturation, 1.0);
}