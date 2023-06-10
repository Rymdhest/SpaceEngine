#version 330

in vec2 textureCoords;
layout (location = 0) out vec4 out_Colour;

uniform vec3 viewPositionWorld;
uniform vec3 viewDirectionWorld;

uniform vec3 sunPositionWorld;
uniform vec3 sunDirectionWorld;
uniform vec2 screenResolution;
uniform mat4 viewMatrix;

void main(void){
	vec3 colorGround = vec3(0.78f,  0.86f,  0.95f);
	vec3 colorSpace = vec3(0.12f,  0.12f,  0.12f);
	vec3 sunColor = vec3(1.0,0.9,0.7);

	vec2 uv = ((textureCoords*2f)-1f);
	uv = (uv*0.5f*screenResolution)/screenResolution.y;

	float heightFactor = clamp(1f-viewPositionWorld.y*0.001f, 0f, 1f);
	vec3 skyColor = mix(colorSpace, colorGround, heightFactor);

	vec3 viewDir = normalize((viewMatrix*vec4(uv, -1f, 1.0f)).xyz);
	
	vec3 upNormalViewSpace = normalize((vec4(0, 1f, 0.0f, 1f)).xyz);

	skyColor *= 1f-pow(max( dot( viewDir, upNormalViewSpace), 0.0 ), 1)*0.6f;

	vec3 sunDir = normalize(sunPositionWorld - viewPositionWorld);
	float sunAmount = pow(max( dot( viewDir, sunDir ), 0.0 ), 128);

	skyColor = mix( skyColor, sunColor*1.5f, sunAmount );

	out_Colour.rgb = skyColor;
}