# version 400


struct Light {
  bool enabled;

  int sourceType;
  vec3 position;
  vec3 normal;

  vec4 color;
  
  int diffuseFunction;
  int attenuationFunction;
  vec3 cosineAttenuation;
  vec3 distanceAttenuation;
};

uniform Light lights[8];
uniform vec4 ambientLightColor;
uniform vec3 cameraPosition;
uniform float shininess;
in vec3 vertexPosition;
in vec3 vertexNormal;
in vec4 vertexColor0;
in vec2 uv0;

out vec4 fragColor;


void getSurfaceToLightNormalAndAttenuation(Light light, vec3 position, vec3 normal, out vec3 surfaceToLightNormal, out float attenuation) {
  vec3 surfaceToLight = light.position - position;
  
  surfaceToLightNormal = (light.sourceType == 3)
    ? -light.normal : normalize(surfaceToLight);

  if (light.attenuationFunction == 0) {
    attenuation = 1;
    return;
  }
  

  // Attenuation is calculated as a fraction, (cosine attenuation) / (distance attenuation).

  // Numerator (Cosine attenuation)
  vec3 cosAttn = light.cosineAttenuation;
  
  vec3 attnDotLhs = (light.attenuationFunction == 1)
    ? normal : surfaceToLightNormal;
  float attn = dot(attnDotLhs, light.normal);
  vec3 attnPowers = vec3(1, attn, attn*attn);

  float attenuationNumerator = max(0, dot(cosAttn, attnPowers));

  // Denominator (Distance attenuation)
  float attenuationDenominator = 1;
  if (light.sourceType != 3) {
    vec3 distAttn = light.distanceAttenuation;
    
    if (light.attenuationFunction == 1) {
      float attn = max(0, -dot(normal, light.normal));
      if (light.diffuseFunction != 0) {
        distAttn = normalize(distAttn);
      }
      
      attenuationDenominator = dot(distAttn, attnPowers);
    } else {
      float dist2 = dot(surfaceToLight, surfaceToLight);
      float dist = sqrt(dist2);
      attenuationDenominator = dot(distAttn, vec3(1, dist, dist2));
    }
  }

  attenuation = attenuationNumerator / attenuationDenominator;
}

void getIndividualLightColors(Light light, vec3 position, vec3 normal, float shininess, out vec4 diffuseColor, out vec4 specularColor) {
  if (!light.enabled) {
     diffuseColor = specularColor = vec4(0);
     return;
  }

  vec3 surfaceToLightNormal;
  float attenuation;
  getSurfaceToLightNormalAndAttenuation(light, position, normal, surfaceToLightNormal, attenuation);

  float diffuseLightAmount = 1;
  if (light.diffuseFunction == 1 || light.diffuseFunction == 2) {
    diffuseLightAmount = max(0, dot(normal, surfaceToLightNormal));
  }
  diffuseColor = light.color * diffuseLightAmount * attenuation;
  
  if (dot(normal, surfaceToLightNormal) >= 0) {
    vec3 surfaceToCameraNormal = normalize(cameraPosition - position);
    float specularLightAmount = pow(max(0, dot(reflect(-surfaceToLightNormal, normal), surfaceToCameraNormal)), shininess);
    specularColor = light.color * specularLightAmount * attenuation;
  }
}

void main() {
  // Have to renormalize because the vertex normals can become distorted when interpolated.
  vec3 fragNormal = normalize(vertexNormal);

  vec4 individualLightDiffuseColors[8];
  vec4 individualLightSpecularColors[8];
  for (int i = 0; i < 8; ++i) {
    vec4 diffuseLightColor;
    vec4 specularLightColor;
    getIndividualLightColors(lights[i], vertexPosition, fragNormal, shininess, diffuseLightColor, specularLightColor);
    individualLightDiffuseColors[i] = diffuseLightColor;
    individualLightSpecularColors[i] = specularLightColor;
  }

  vec3 colorComponent = (vec3(0.9529411792755127,0.8509804010391235,0)*ambientLightColor.rgb + individualLightDiffuseColors[0].rgb + individualLightDiffuseColors[1].rgb + individualLightDiffuseColors[2].rgb + individualLightDiffuseColors[3].rgb + individualLightDiffuseColors[4].rgb + individualLightDiffuseColors[5].rgb + individualLightDiffuseColors[6].rgb + individualLightDiffuseColors[7].rgb)*vec3(2)*vec3(0.7019608020782471,0.501960813999176,0)*vertexColor0.rgb + vec3(1,1,0.5490196347236633)*(individualLightSpecularColors[0].rgb + individualLightSpecularColors[1].rgb + individualLightSpecularColors[2].rgb + individualLightSpecularColors[3].rgb + individualLightSpecularColors[4].rgb + individualLightSpecularColors[5].rgb + individualLightSpecularColors[6].rgb + individualLightSpecularColors[7].rgb);

  float alphaComponent = vertexColor0.a;

  fragColor = vec4(colorComponent, alphaComponent);

  if (!(fragColor.a >= 0.95)) {
    discard;
  }
}
