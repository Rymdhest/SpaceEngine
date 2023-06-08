#version 330

in vec2 textureCoords;
layout (location = 0) out vec4 out_Colour;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedo;

uniform vec3 sunPositionViewSpace;
const vec3 lightColor = vec3(1f, 1f, 1f);
const vec3  fogColor  = vec3(0.5,0.6,0.7);
const float specularStrength = 0.0f;
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
    vec3  sunScatteredFogColor  = mix( fogColor, // bluish
                           vec3(1.0,0.9,0.7), // yellowish
                           pow(sunAmount,2.0) );
    return mix( rgb, sunScatteredFogColor, fogAmount );
}

void main(void){
	vec3 position = texture(gPosition, textureCoords).xyz;
	vec3 normal = texture(gNormal, textureCoords).xyz;
	vec3 albedo = texture(gAlbedo, textureCoords).rgb;

	vec3 viewDir = normalize(-position);
	vec3 lightDir = normalize(sunPositionViewSpace - position);

	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
	vec3 specular = specularStrength * spec * lightColor;  
	float lighting = max(dot(lightDir, normal), ambient);

	
	vec3 color = albedo*lighting+specular;
	color = applyFog(color, -position.z, -viewDir, lightDir);

	out_Colour =  vec4(color, 1.0f);
}