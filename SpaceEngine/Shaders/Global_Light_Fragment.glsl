#version 330

in vec2 textureCoords;
layout (location = 0) out vec4 out_Colour;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedo;

uniform vec3 sunPositionViewSpace;
const vec3 lightColor = vec3(1f, 1f, 1f);
const float specularStrength = 0.35f;
const float ambient = 0.35f;

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

	out_Colour =  vec4(color, 1.0f);
}