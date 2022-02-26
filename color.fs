#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec3 LightPos;
in vec2 TexCoords;
//uniform vec3 objectColor;
//uniform vec3 lightColor;
// uniform vec3 viewPos;

struct Material{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

struct Light{
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform Light light;
uniform Material material;
uniform sampler2D emission;

void main()
{
    vec3 norm = normalize(Normal);

    vec3 ambient =light.ambient * vec3(texture(material.diffuse,TexCoords));

    vec3 lightDir = normalize(LightPos - FragPos);
    float diff = max(dot(lightDir , norm) , 0);
    vec3 diffuse =light.diffuse * vec3(texture(material.diffuse,TexCoords));

    vec3 viewDir = normalize(-FragPos);
    vec3 reflectDir = reflect(-lightDir , norm);
    float spec = pow(max(dot(viewDir , reflectDir) , 0.0f) , material.shininess);
    vec3 specular = light.specular * spec * vec3(texture(material.specular,TexCoords));

    vec3 emission = vec3(texture(emission , TexCoords));

    vec3 result = ambient + specular + diffuse + emission;
    FragColor = vec4(result , 1.0f);
}