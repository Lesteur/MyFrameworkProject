#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Texture and sampler from SpriteBatch
texture SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

// Custom parameters
float4 SolidColor = float4(1, 0, 0, 1); // Default: Red
float AlphaThreshold = 0.1; // Pixels with alpha below this are considered transparent

// Vertex shader input structure
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// Vertex shader output structure
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// Vertex Shader - Simple passthrough (transformation handled by SpriteBatch)
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    // DO NOT apply MatrixTransform here - SpriteBatch already handles it
    output.Position = input.Position;
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

// Pixel Shader - Replace opaque pixels with solid color
float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Sample the original texture
    float4 texColor = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    
    // Calculate final alpha
    float finalAlpha = texColor.a * input.Color.a;
    
    // If pixel is transparent, discard it
    if (finalAlpha < AlphaThreshold)
    {
        return float4(0, 0, 0, 0);
    }
    
    // Return solid color with original alpha
    return float4(SolidColor.rgb, finalAlpha);
}

// Technique
technique SolidColorTechnique
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}