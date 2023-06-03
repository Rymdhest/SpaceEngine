#version 330

in vec3 fragColor;
in vec3 fragNormal;
in vec3 positionViewSpace_pass;

layout (location = 0) out vec4 gDiffuse;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;

//uniform vec3 cameraPos;
//const vec3 sunDirection = normalize(vec3(0.5f, 1, -1));
//const vec3 lightColor = vec3(1f, 1f, 1f);

void main() {
	gDiffuse = vec4(fragColor, 1.0);
	gNormal = vec4(fragNormal, 1.0);
	gPosition = vec4(positionViewSpace_pass, 1.0);
}