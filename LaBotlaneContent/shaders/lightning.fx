float4x4 MatrixTransform;

sampler s : register(s0);
// Texture comportant uniquement les obstacles.
texture xObstacleMapTexture;
// Texture de la map.
texture xMapTexture;
// Texture contenant la luminosit� de chaque pixel de la map.
texture xLightMap;
// Texture de brouillard.
texture xFogMap;
// Position en texels de la lumi�re.
float2 xPosition;
// Scrolling en texels.
float2 xScrolling;
// Rayon des lumi�res en texels.
float xRadius;
float2 xScreenSize = { 800, 600 };
// Temps de jeu �coul� en secondes.
float xElapsedTime;
// Vitesse du brouillard en texels / seconde.
float xFogSpeed;
// Rapport entre la taille de la map en pixels et la taille de la lightmap.
float xLightMapRatio;
sampler2D obstacleMap = sampler_state
{
	texture = <xObstacleMapTexture>;
	MipFilter = None;
	MinFilter = Point;
	MagFilter = Point;
};
sampler2D fogMap = sampler_state
{
	texture = <xFogMap>;
	MipFilter = None;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Wrap;
	AddressV = Wrap;
};
sampler2D map = sampler_state
{
	texture = <xMapTexture>;
	MipFilter = None;
	MinFilter = Point;
	MagFilter = Point;
};

sampler2D lightMap = sampler_state
{
	texture = <xLightMap>;
	MipFilter = Point;
	MinFilter = Linear;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VSIn
{
	float4 color : COLOR0;
	float2 texCoord : TEXCOORD0;
	float4 position : POSITION0;
};
struct VSOut
{
	float2 coords : TEXCOORD0;
	float4 position : POSITION0;
};

VSOut SpriteVertexShader(VSIn input)
{
	VSOut output;
	output.position = mul(input.position, MatrixTransform);
	output.coords = input.texCoord;
	return output;
}

// A partir d'une obstacleMap, dessine une texture comportant
// l'�clairage de chaque pixel de la map par une source de lumi�re
// plac�e en xPosition.
float4 Lightning(float2 texCoord	: TEXCOORD0) : COLOR0
{
	// Nombre de samples pour l'ombre.
	// Plus de samples = plus lent mais meilleure qualit�.
	const float samples = 6;
	// Distance de calcul de l'ombre.
	const float shadowDist = 0.025;
	// Taille en % du rayon de la t�che centrale.
	const float centerSize = 0.5;
	// Taille de la zone en texels o� les sombres ne sont pas projet�es.
	const float noShadowRadius = 0.05;
	// Plus cette valeur est grande, plus les ombres sont prononc�es.
	const float shadowPower = 0.3f;
	// Distance entre la source lumineuse et le pixel.
	float dst = distance(texCoord*xScreenSize, xPosition*xScreenSize)/xScreenSize;

	float4 color = tex2D(map, texCoord);
	float obstacles = 0;
	float2 startPos = texCoord;
	float2 endPos = startPos + normalize(xPosition - startPos) * shadowDist;
	int maxObs = 0;

	for (int i = 0; i < samples; i++)
	{

		float4 colorValue = tex2D(obstacleMap, lerp(startPos, endPos, i / samples));

		// Incr�mente le nombre d'obscales si un pixel non transparent est trouv�
		// sur la map d'obstacles.
		if (colorValue.a >= 0.3)
		{
			obstacles += i;
		}

		obstacles *= 0.88;
		maxObs += i;
	}
	
	// Att�nuation de la luminosit� bas�e sur la distance.
	float light = 0.3 - saturate(dst/xRadius-centerSize)/(1-centerSize);
	float attenuation = 0;

	// Att�nuation de la luminosit� bas�e sur les ombres.
	if (tex2D(obstacleMap, startPos).a < 0.3)
	{
		attenuation = saturate(obstacles / maxObs * shadowPower) * saturate((dst - noShadowRadius) / (noShadowRadius));
	}
	
	return float4(light, 0, light-attenuation, 1); //float4(color.rgb*light, 1);
}

// Combine la lightmap � la map.
float4 Combine(float2 texCoord	: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(map, texCoord);


	// R�cup�re la luminosit� de base du pixel dans un level downsampl� de la mipmap.
	float light = tex2Dbias(lightMap, float4(texCoord.x, texCoord.y, 0, 1)).r;


	// R�cup�re l'att�nuation du pixel.
	float4 lightData = tex2Dbias(lightMap, float4(texCoord.x, texCoord.y, 0, 0));
	float attenuation = lightData.r - lightData.b;

	// R�cup�re la couleur du brouillard (mobile).
	float4 fogColor = tex2D(fogMap, texCoord + xScrolling + xElapsedTime*xFogSpeed);

	//return float4(color.rgb, 1);
	return float4(color.rgb*clamp(light - attenuation, 0.1, 1)*(0.5 + fogColor.b*2), 1);
}

technique LightCalc
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 Lightning();
	}
}


technique LightCombine
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 Combine();
	}
}