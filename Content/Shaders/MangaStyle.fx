#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

float EdgeThreshold;
float EdgeThickness;
float HalftoneSize;
float HalftoneDensity;
int PosterizeLevels;
bool BlackAndWhite;
float2 TextureSize;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

// Simple edge detection
float GetEdge(float2 texCoord)
{
    float2 px = 1.0 / TextureSize;
    
    float3 c = tex2D(SpriteTextureSampler, texCoord).rgb;
    float3 n = tex2D(SpriteTextureSampler, texCoord + float2(0, -px.y)).rgb;
    float3 s = tex2D(SpriteTextureSampler, texCoord + float2(0, px.y)).rgb;
    float3 e = tex2D(SpriteTextureSampler, texCoord + float2(px.x, 0)).rgb;
    float3 w = tex2D(SpriteTextureSampler, texCoord + float2(-px.x, 0)).rgb;
    
    float3 diff = abs(n - s) + abs(e - w);
    float edge = (diff.r + diff.g + diff.b) / 3.0;
    
    return edge > EdgeThreshold;
}

// Screen tone dots
float GetScreenTone(float2 texCoord, float lum)
{
    // Only mid-tones get screen tone
    float inRange = (lum >= 0.1 && lum <= 0.9) ? 1.0 : 0.0;
    
    float2 dotPos = frac(texCoord * TextureSize / HalftoneSize);
    float dist = length(dotPos - 0.5);
    
    float dotSize = (0.7 - lum) * 0.5 * HalftoneDensity;
    float dotValue = (dist < dotSize) ? 0.0 : 1.0;
    
    return lerp(1.0, dotValue, inRange);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    
    // Skip transparent
    if (color.a < 0.01)
        return color;
    
    float lum = dot(color.rgb, float3(0.299, 0.587, 0.114));
    
    float3 result;
    
    // Manga B&W mode
    float3 bwColor = float3(lum, lum, lum);
    
    // Apply screen tone
    float tone = GetScreenTone(input.TextureCoordinates, lum);
    bwColor = bwColor * tone;
    
    // Keep pure white/black extremes
    float isPureWhite = (lum > 0.85) ? 1.0 : 0.0;
    float isPureBlack = (lum < 0.15) ? 1.0 : 0.0;
    bwColor = lerp(bwColor, float3(1,1,1), isPureWhite);
    bwColor = lerp(bwColor, float3(0,0,0), isPureBlack);
    
    // Choose between B&W and color
    result = BlackAndWhite ? bwColor : color.rgb;
    
    // Apply black edges
    float edge = GetEdge(input.TextureCoordinates);
    result = lerp(result, float3(0,0,0), edge);
    
    return float4(result, color.a) * input.Color;
}

technique MangaTechnique
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
