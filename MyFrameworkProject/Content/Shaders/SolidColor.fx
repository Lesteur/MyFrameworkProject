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

// Custom parameters
float4 SolidColor = float4(1, 0, 0, 1); // Default: Red
float AlphaThreshold = 0.01; // Pixels with alpha below this are considered transparent

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// Pixel Shader
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 texColor = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    
    // If pixel alpha is above threshold, replace with solid color
    if (texColor.a > AlphaThreshold)
        return SolidColor;
    
    return texColor;
}

technique BasicColorReplace
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}