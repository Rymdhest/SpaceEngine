#version 330

in vec2 textureCoords;
layout (location = 0) out vec4 out_Colour;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedo;

uniform vec3 sunPositionViewSpace;
const vec3 lightColor = vec3(1f, 1f, 1f);
const vec3  fogColor  = vec3(0.5,0.6,0.7);
const float ambient = 0.35f;
uniform float fogDensity = 0.001f;
vec3 applyFog( in vec3  rgb,      // original color of the pixel
               in float distance, // camera to point distance
               in vec3  rayDir,   // camera to point vector
               in vec3  sunDir )  // sun light direction
{
	vec3 rayOri = vec3(0f);
    float fogAmount = 1.0 - exp( -distance*fogDensity );
    float sunAmount = max( dot( rayDir, sunDir ), 0.0 );
    vec3  sunScatteredFogColor  = mix( fogColor, 
                           vec3(1.0,0.9,0.7), 
                           pow(sunAmount,2.0) );
    return mix( rgb, sunScatteredFogColor, fogAmount );
}

void main(void){
	vec3 position = texture(gPosition, textureCoords).xyz;
	vec3 normal = texture(gNormal, textureCoords).xyz;
	vec3 albedo = texture(gAlbedo, textureCoords).rgb;
	float ambientOcclusion = texture(gAlbedo, textureCoords).a;
	float specularStrength = texture(gNormal, textureCoords).a;

	vec3 totalAmbient = vec3(ambient*ambientOcclusion*albedo);
	vec3 lighting = totalAmbient;

	vec3 viewDir = normalize(-position);
	vec3 lightDir = normalize(sunPositionViewSpace - position);

	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
	vec3 specular = specularStrength * spec * lightColor;  

	vec3 diffuse = max(dot(lightDir, normal), 0f)*albedo*lightColor;

	lighting += diffuse + specular;
	lighting = applyFog(lighting, -position.z, -viewDir, lightDir);

	out_Colour =  vec4(lighting, 1.0f);
}