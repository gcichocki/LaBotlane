// Copyright (C) 2013, 2014 Alvarez Josué
//
// This code is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2.1 of the License, or (at
// your option) any later version.
//
// This code is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
// License (LICENSE.txt) for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation,
// Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// The developer's email is jUNDERSCOREalvareATetudDOOOTinsa-toulouseDOOOTfr (for valid email, replace 
// capital letters by the corresponding character)

float4x4 MatrixTransform;
sampler src : register(s0);
float4x4 World;

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
	output.position = mul(input.position, World);
	output.position = mul(input.position, MatrixTransform);
	output.coords = input.texCoord;
	return output;
}

float4 Mode7(float2 texCoord: TEXCOORD0) : COLOR0
{
	return tex2D(src, texCoord);
}

technique Mode7Technique
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 Mode7();
	}
}