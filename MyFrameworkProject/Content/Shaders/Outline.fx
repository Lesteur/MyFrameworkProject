// Paramètres automatiques fournis par SpriteBatch
float4x4 MatrixTransform : register(vs, c0);

// Paramètres personnalisés
float4 OutlineColor = float4(1, 0, 0, 1);
float Thickness = 1.0;
float2 TextureSize = float2(256, 256);

// Texture du sprite
texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VS(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, MatrixTransform);
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;
    return output;
}

float4 PS(VertexShaderOutput input) : COLOR0
{
    // Calculer la taille d'un pixel en coordonnées de texture
    float2 texelSize = 1.0 / TextureSize;
    float2 offset = texelSize * Thickness;
    
    // Échantillonner le pixel central
    float4 centerColor = tex2D(TextureSampler, input.TexCoord);
    
    // Si le pixel central est opaque, le dessiner normalement
    if (centerColor.a > 0.5)
    {
        return centerColor * input.Color;
    }
    
    // Sinon, vérifier les pixels voisins pour créer le contour
    float outlineAlpha = 0.0;
    
    // Échantillonner les 4 directions (gauche, droite, haut, bas)
    outlineAlpha += tex2D(TextureSampler, input.TexCoord + float2(offset.x, 0)).a;
    outlineAlpha += tex2D(TextureSampler, input.TexCoord + float2(-offset.x, 0)).a;
    outlineAlpha += tex2D(TextureSampler, input.TexCoord + float2(0, offset.y)).a;
    outlineAlpha += tex2D(TextureSampler, input.TexCoord + float2(0, -offset.y)).a;
    
    // Si au moins un voisin est opaque, dessiner le contour
    if (outlineAlpha > 0.5)
    {
        return OutlineColor;
    }
    
    // Sinon, pixel transparent
    return float4(0, 0, 0, 0);
}

technique Outline
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 VS();
        PixelShader = compile ps_3_0 PS();
    }
}