#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec3 LightPos;
in vec2 TexCoords;

struct Material{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

struct Light{
    vec3 position;
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float cutOff;
    float outerCutOff;
    float constant;
    float linear;
    float quadratic;
};
uniform Light light;
uniform Material material;

void main()
{
    
    vec3 lightDir = normalize(light.position - FragPos);
    float distance = length(light.position - FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    float theta     = dot(lightDir, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);   
        vec3 norm = normalize(Normal);

        vec3 ambient =light.ambient * vec3(texture(material.diffuse,TexCoords)) * attenuation * intensity;

        float diff = max(dot(lightDir , norm) , 0);
        vec3 diffuse =light.diffuse * vec3(texture(material.diffuse,TexCoords)) * attenuation;

        vec3 viewDir = normalize(-FragPos);
        vec3 reflectDir = reflect(-lightDir , norm);
        float spec = pow(max(dot(viewDir , reflectDir) , 0.0f) , material.shininess);
        vec3 specular = light.specular * spec * vec3(texture(material.specular,TexCoords)) * attenuation * intensity;

        //vec3 emission = vec3(texture(emission , TexCoords));

        vec3 result = ambient + specular + diffuse;
        FragColor = vec4(result , 1.0f);
}