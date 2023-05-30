#version 330

flat in vec3 fragColor;
in vec3 fragNormal;
out vec4 outColor;

const vec3 sunDirection = normalize(vec3(0.5f, 1, 1));

void main() {
	float lighting = max(dot(sunDirection, fragNormal), 0.3f);
	outColor = vec4(fragColor*lighting, 1.0);
	//outColor = vec4(fragNormal, 1.0);
}