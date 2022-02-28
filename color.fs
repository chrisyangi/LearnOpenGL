#version 330 core
out vec4 FragColor;

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

struct Material{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

// 定向光源， 所以结构体中direction参数
struct DirLight{
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

//点光源
struct PointLight{
    vec3 position ; 
    float constant ;
    float linear;
    float quadratic;
    
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

//聚光
struct SpotLight{
    vec3 position;
    vec3 direction;
    float curOff;
    float outerCutOff;
    float constant;
    float linear;
    float quadratic;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

#define NR_POINT_LIGHTS 4

uniform DirLight dirLight;
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform SpotLight spotLight;
uniform Material material;



vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light , vec3 normal , vec3 fragPos , vec3 viewDir);
vec3 CalcSpotLight(SpotLight spotLight , vec3 normal , vec3 fragPos , vec3 viewDir);
void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(-FragPos);
    
    // 定向光照
    vec3 result = CalcDirLight(dirLight , norm , viewDir);

    // 点光源
    for(int i = 0 ; i < NR_POINT_LIGHTS ; i++){
        result += CalcPointLight(pointLights[i] , norm , FragPos , viewDir);
    }

    // 聚光
    result += CalcSpotLight(spotLight , norm , FragPos , viewDir);

    FragColor = vec4(result , 1.0f);
}
//定向光照
vec3 CalcDirLight(DirLight light , vec3 normal , vec3 viewDir){
    vec3 lightDir = normalize(-light.direction); //因为是定向光照，所以直接能有得到光源的方向，一般默认是光源向外的方向，所以这里取反
    float diff = max(dot(lightDir , normal),0.0f);
    vec3 reflectDir = reflect(-lightDir , normal);
    float spec = pow(max(dot(normal , reflectDir),0.0f),material.shininess);
    vec3 ambient = light.ambient * vec3(texture(material.diffuse , TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse , TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular , TexCoords));
    return (ambient + diffuse + specular);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos , vec3 viewDir){
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(lightDir , normal) , 0.0f);
    vec3 reflectDir = reflect(-lightDir , normal);
    float spec = pow(max(dot(viewDir , reflectDir ) , 0.0f) , material.shininess);
    float distance = length(light.position - fragPos);
    float attenuation = 1.0f / (light.constant + light.linear * distance + light.quadratic * distance * distance);

    vec3 ambient = light.ambient * vec3(texture(material.diffuse , TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse,TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular , TexCoords));

    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

vec3 CalcSpotLight(SpotLight light , vec3 normal , vec3 fragPos , vec3 viewDir){
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    // spotlight intensity
    float theta = dot(lightDir, normalize(-light.direction)); 
    float epsilon = light.curOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
    // combine results
    vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
    ambient *= attenuation * intensity;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
}