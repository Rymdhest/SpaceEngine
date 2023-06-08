#version 330

in vec3 fragColor;
in vec3 fragNormal;
in vec3 positionViewSpace;

layout (location = 0) out vec4 gDiffuse;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;

void main() {
	gDiffuse = vec4(fragColor, 1.0);
	gNormal = vec4(normalize(fragNormal), 1.0);
	gPosition = vec4(positionViewSpace, 1.0f);
}