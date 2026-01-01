#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

texture SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

// Wave parameters
float Time = 0.0;
float WaveAmplitude = 3.00; // Displacement strength (0.05 = 5% displacement)
float WaveFrequency = 6.28; // Wave frequency (2*PI = 1 complete wave)
float WaveSpeed = 2.0; // Animation speed

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// Pixel Shader - Applies wave distortion to texture coordinates
float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Calculate wave offset based on vertical position
    float wave = sin((input.TextureCoordinates.y * WaveFrequency) + (Time * WaveSpeed)) * WaveAmplitude;
    
    // Apply horizontal displacement to texture coordinates
    float2 distortedCoords = input.TextureCoordinates;
    distortedCoords.x += wave;
    
    // Sample texture with distorted coordinates
    float4 color = tex2D(SpriteTextureSampler, distortedCoords);
    
    // Apply vertex color tint
    return color * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}