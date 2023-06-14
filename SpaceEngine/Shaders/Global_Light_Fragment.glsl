#version 330

in vec2 textureCoords;
layout (location = 0) out vec4 out_Colour;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedo;
uniform sampler2DShadow shadowMap;

uniform vec3 sunDirectionViewSpace;
uniform vec3 sunColor;
uniform vec3 sunScatterColor;
uniform vec3 fogColor;
uniform float ambient;
uniform float fogDensity;

uniform mat4 sunSpaceMatrix;
uniform vec2 shadowMapResolution;


float calcShadow(vec3 positionWorldSpace) {
	vec4 positionSunSpace = (vec4(positionWorldSpace, 1.0))*sunSpaceMatrix;
	float shadowDepth = textureProj(shadowMap, positionSunSpace);
	float sunFactor = 1f;
	if (shadowDepth < positionSunSpace.z) {
		sunFactor = 0.0f;
	}
	return sunFactor;
}

vec3 applyFog( in vec3  rgb,      // original color of the pixel
               in float distance, // camera to point distance
               in vec3  rayDir,   // camera to point vector
               in vec3  sunDir )  // sun light direction
{
    float fogAmount = 1.0 - exp( -distance*fogDensity );
    float sunAmount = max( dot( rayDir, sunDir ), 0.0 );
    vec3  sunScatteredFogColor  = mix( fogColor, 
                           sunScatterColor, 
                           pow(sunAmount,2.0) );
    return mix( rgb, sunScatteredFogColor, fogAmount );
}

void main(void){
	vec3 position = texture(gPosition, textureCoords).xyz;
	vec3 normal = texture(gNormal, textureCoords).xyz;
	vec3 albedo = texture(gAlbedo, textureCoords).rgb;


	float sunFactor = calcShadow(position);

	float ambientOcclusion = texture(gAlbedo, textureCoords).a;
	float specularStrength = texture(gNormal, textureCoords).a;

	vec3 totalAmbient = vec3(ambient*ambientOcclusion*albedo);

	vec3 viewDir = normalize(-position);

	vec3 reflectDir = reflect(-sunDirectionViewSpace, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
	vec3 specular = specularStrength * spec * sunColor;  

	vec3 diffuse = max(dot(sunDirectionViewSpace, normal), 0f)*albedo*sunColor;

	vec3 lighting = max((diffuse + specular)*ambientOcclusion*sunFactor, totalAmbient);
	lighting = applyFog(lighting, -position.z, -viewDir, sunDirectionViewSpace);

	out_Colour =  vec4(lighting, 1.0f);
	//out_Colour =  vec4(positionSunSpace.xyz, 1.0f);
	//out_Colour =  vec4(vec3(shadowDepth), 1.0f);
}