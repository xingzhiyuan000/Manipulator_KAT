#version 330 core             
//in vec4 vColor;
in vec2 vUV;
in vec3 vNormal;
out vec4 pixelColor;
in vec3 worldPosition;

//uniform vec3 uColor;
uniform sampler2D sampler;
uniform sampler2D specularMaskSampler;
uniform vec3 lightDirection;
uniform vec3 lightColor;
uniform vec3 cameraPosition;
uniform float specularIntensity;
uniform vec3 ambientColor;
uniform float shiness;

uniform float opacity;

void main()
{
    
    vec3 objectColor  = texture(sampler, vUV).xyz;
    float alpha= texture(sampler, vUV).a;

    vec3 normalN=normalize(vNormal);
    vec3 lightDirN = normalize(lightDirection);
    vec3 viewDir = normalize(worldPosition - cameraPosition);
    
    float diffuse = clamp(dot(-lightDirN, normalN), 0.0,1.0);
    vec3 diffuseColor = lightColor * diffuse * objectColor;
    
    float dotResult = dot(-lightDirN, normalN);
    float flag = step(0.0, dotResult);
    
    vec3 lightReflect=normalize(reflect(lightDirN,normalN));
    float specular = max(dot(lightReflect,-viewDir), 0.0);
    specular = pow(specular, shiness);
    float specularMask= texture(specularMaskSampler, vUV).r;

    vec3 ambientColor = objectColor * ambientColor;
    
    vec3 specularColor = lightColor * specular * flag * specularIntensity * specularMask;
    vec3 finalColor = diffuseColor + specularColor + ambientColor;

    
    pixelColor = vec4(finalColor, alpha*opacity);
    //pixelColor=vColor;

}