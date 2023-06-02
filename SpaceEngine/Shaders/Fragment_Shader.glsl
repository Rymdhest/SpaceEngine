#version 330

in vec3 fragColor;
in vec3 fragNormal;
in vec3 posWorld;

layout (location = 0) out vec4 gDiffuse;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gPosition;

uniform vec3 cameraPos;
const vec3 sunDirection = normalize(vec3(0.5f, 1, -1));
const vec3 lightColor = vec3(1f, 1f, 1f);

void main() {
	vec3 viewDir = normalize(cameraPos - posWorld);
	vec3 reflectDir = reflect(-sunDirection, fragNormal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
	vec3 specular = 0.5f * spec * lightColor;  
	float lighting = max(dot(sunDirection, fragNormal), 0.3f);

	gDiffuse = vec4(fragColor*lighting*lightColor+specular, 1.0);
	gNormal = vec4(fragNormal, 1.0);
	gPosition = vec4(posWorld, 1.0);
}