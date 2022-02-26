#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
uniform mat4 model;
uniform mat4 view ;
uniform mat4 projection;
uniform vec3 lightPos;

out vec3 Normal;
out vec3 FragPos;
out vec3 LightPos;
void main()
{
    Normal = mat3(transpose(inverse(view * model))) * aNormal;  // 法线变换到观察空间
    FragPos = vec3(view * model * vec4(aPos , 1.0f));           // 着色点（顶点）变换到观察空间
    LightPos = vec3(view * vec4(lightPos , 1.0f));              // 光源变换到观察空间
    gl_Position = projection * view * model * vec4(aPos, 1.0f) ;
}