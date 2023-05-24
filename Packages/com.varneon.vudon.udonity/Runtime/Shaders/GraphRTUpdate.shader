// Made with Amplify Shader Editor v1.9.1.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GraphRTUpdate"
{
    Properties
    {
		_CustomRenderTexture("CustomRenderTexture", 2D) = "white" {}
		[Toggle]_VectorMode("VectorMode", Float) = 0
		[Toggle]_Signed("Signed", Float) = 0
		_Multiplier("Multiplier", Float) = 1

    }

	SubShader
	{
		LOD 0

		
		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
        Pass
        {
			Name "Custom RT Update"
            CGPROGRAM
            
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex ASECustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0
			int _InputCount = 0;
			float _Inputs[256];
			float4 _Vectors[256];


			struct ase_appdata_customrendertexture
			{
				uint vertexID : SV_VertexID;
				
			};

			struct ase_v2f_customrendertexture
			{
				float4 vertex           : SV_POSITION;
				float3 localTexcoord    : TEXCOORD0;    // Texcoord local to the update zone (== globalTexcoord if no partial update zone is specified)
				float3 globalTexcoord   : TEXCOORD1;    // Texcoord relative to the complete custom texture
				uint primitiveID        : TEXCOORD2;    // Index of the update zone (correspond to the index in the updateZones of the Custom Texture)
				float3 direction        : TEXCOORD3;    // For cube textures, direction of the pixel being rendered in the cubemap
				
			};

			uniform sampler2D _CustomRenderTexture;
			uniform float _VectorMode;
			uniform float _Multiplier;
			uniform float _Signed;
			float GetInput44( int Index )
			{
				return _Inputs[Index];
			}
			
			float4 GetVector50( float Index )
			{
				return _Vectors[Index];
			}
			


			ase_v2f_customrendertexture ASECustomRenderTextureVertexShader(ase_appdata_customrendertexture IN  )
			{
				ase_v2f_customrendertexture OUT;
				
			#if UNITY_UV_STARTS_AT_TOP
				const float2 vertexPositions[6] =
				{
					{ -1.0f,  1.0f },
					{ -1.0f, -1.0f },
					{  1.0f, -1.0f },
					{  1.0f,  1.0f },
					{ -1.0f,  1.0f },
					{  1.0f, -1.0f }
				};

				const float2 texCoords[6] =
				{
					{ 0.0f, 0.0f },
					{ 0.0f, 1.0f },
					{ 1.0f, 1.0f },
					{ 1.0f, 0.0f },
					{ 0.0f, 0.0f },
					{ 1.0f, 1.0f }
				};
			#else
				const float2 vertexPositions[6] =
				{
					{  1.0f,  1.0f },
					{ -1.0f, -1.0f },
					{ -1.0f,  1.0f },
					{ -1.0f, -1.0f },
					{  1.0f,  1.0f },
					{  1.0f, -1.0f }
				};

				const float2 texCoords[6] =
				{
					{ 1.0f, 1.0f },
					{ 0.0f, 0.0f },
					{ 0.0f, 1.0f },
					{ 0.0f, 0.0f },
					{ 1.0f, 1.0f },
					{ 1.0f, 0.0f }
				};
			#endif

				uint primitiveID = IN.vertexID / 6;
				uint vertexID = IN.vertexID % 6;
				float3 updateZoneCenter = CustomRenderTextureCenters[primitiveID].xyz;
				float3 updateZoneSize = CustomRenderTextureSizesAndRotations[primitiveID].xyz;
				float rotation = CustomRenderTextureSizesAndRotations[primitiveID].w * UNITY_PI / 180.0f;

			#if !UNITY_UV_STARTS_AT_TOP
				rotation = -rotation;
			#endif

				// Normalize rect if needed
				if (CustomRenderTextureUpdateSpace > 0.0) // Pixel space
				{
					// Normalize xy because we need it in clip space.
					updateZoneCenter.xy /= _CustomRenderTextureInfo.xy;
					updateZoneSize.xy /= _CustomRenderTextureInfo.xy;
				}
				else // normalized space
				{
					// Un-normalize depth because we need actual slice index for culling
					updateZoneCenter.z *= _CustomRenderTextureInfo.z;
					updateZoneSize.z *= _CustomRenderTextureInfo.z;
				}

				// Compute rotation

				// Compute quad vertex position
				float2 clipSpaceCenter = updateZoneCenter.xy * 2.0 - 1.0;
				float2 pos = vertexPositions[vertexID] * updateZoneSize.xy;
				pos = CustomRenderTextureRotate2D(pos, rotation);
				pos.x += clipSpaceCenter.x;
			#if UNITY_UV_STARTS_AT_TOP
				pos.y += clipSpaceCenter.y;
			#else
				pos.y -= clipSpaceCenter.y;
			#endif

				// For 3D texture, cull quads outside of the update zone
				// This is neeeded in additional to the preliminary minSlice/maxSlice done on the CPU because update zones can be disjointed.
				// ie: slices [1..5] and [10..15] for two differents zones so we need to cull out slices 0 and [6..9]
				if (CustomRenderTextureIs3D > 0.0)
				{
					int minSlice = (int)(updateZoneCenter.z - updateZoneSize.z * 0.5);
					int maxSlice = minSlice + (int)updateZoneSize.z;
					if (_CustomRenderTexture3DSlice < minSlice || _CustomRenderTexture3DSlice >= maxSlice)
					{
						pos.xy = float2(1000.0, 1000.0); // Vertex outside of ncs
					}
				}

				OUT.vertex = float4(pos, 0.0, 1.0);
				OUT.primitiveID = asuint(CustomRenderTexturePrimitiveIDs[primitiveID]);
				OUT.localTexcoord = float3(texCoords[vertexID], CustomRenderTexture3DTexcoordW);
				OUT.globalTexcoord = float3(pos.xy * 0.5 + 0.5, CustomRenderTexture3DTexcoordW);
			#if UNITY_UV_STARTS_AT_TOP
				OUT.globalTexcoord.y = 1.0 - OUT.globalTexcoord.y;
			#endif
				OUT.direction = CustomRenderTextureComputeCubeDirection(OUT.globalTexcoord.xy);

				return OUT;
			}

            float4 frag(ase_v2f_customrendertexture IN ) : COLOR
            {
				float4 finalColor;
				float2 appendResult29 = (float2(( IN.localTexcoord.xy.x - ( 1.0 / 256.0 ) ) , IN.localTexcoord.xy.y));
				float temp_output_45_0 = floor( ( IN.localTexcoord.xy.y * 256.0 ) );
				int Index44 = (int)temp_output_45_0;
				float localGetInput44 = GetInput44( Index44 );
				float temp_output_62_0 = ( _Multiplier * 0.5 );
				float temp_output_49_0 = ( localGetInput44 * temp_output_62_0 );
				float4 appendResult55 = (float4(temp_output_49_0 , temp_output_49_0 , temp_output_49_0 , temp_output_49_0));
				float temp_output_48_0 = ( temp_output_49_0 + 0.5 );
				float4 appendResult22 = (float4(temp_output_48_0 , temp_output_48_0 , temp_output_48_0 , temp_output_48_0));
				float4 lerpResult58 = lerp( appendResult55 , appendResult22 , (( _Signed )?( 1.0 ):( 0.0 )));
				float Index50 = temp_output_45_0;
				float4 localGetVector50 = GetVector50( Index50 );
				float4 appendResult61 = (float4(temp_output_62_0 , temp_output_62_0 , temp_output_62_0 , temp_output_62_0));
				float4 temp_output_52_0 = ( localGetVector50 * appendResult61 );
				float4 lerpResult59 = lerp( temp_output_52_0 , ( temp_output_52_0 + float4( 0.5,0.5,0.5,0.5 ) ) , (( _Signed )?( 1.0 ):( 0.0 )));
				float4 lerpResult14 = lerp( tex2D( _CustomRenderTexture, appendResult29 ) , (( _VectorMode )?( lerpResult59 ):( lerpResult58 )) , (( ( IN.localTexcoord.xy.x * 256.0 ) >= 0.0 && ( IN.localTexcoord.xy.x * 256.0 ) <= 1.0 ) ? 1.0 :  0.0 ));
				
                finalColor = lerpResult14;
				return finalColor;
            }
            ENDCG
		}
    }
	
	CustomEditor "ASEMaterialInspector"
	Fallback Off
}
/*ASEBEGIN
Version=19102
Node;AmplifyShaderEditor.LerpOp;14;29.35681,-112.1329;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;-667.8434,-351.433;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;37;-1109.057,-353.6994;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;256;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;17;-1177.341,-257.2332;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;34;-944.1888,-352.8463;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareWithRange;23;-414.3431,-114.8329;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-432.9001,-380.3999;Inherit;True;Property;_CustomRenderTexture;CustomRenderTexture;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-621.6432,-124.1329;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;256;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;205.9,-109.6;Float;False;True;-1;2;ASEMaterialInspector;0;2;GraphRTUpdate;32120270d1b3a8746af2aca8bc749736;True;Custom RT Update;0;0;Custom RT Update;1;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;4;Include;;False;;Native;Custom;int _InputCount = 0@;False;;Custom;Custom;float _Inputs[256]@;False;;Custom;Custom;float4 _Vectors[256]@;False;;Custom;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1233.922,137.4166;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;256;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;51;96.24296,258.7549;Inherit;False;Property;_VectorMode;VectorMode;1;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-556.5955,267.028;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;44;-921.2377,263.9454;Inherit;False;return _Inputs[Index]@;1;Create;1;True;Index;INT;0;In;;Inherit;False;GetInput;True;False;0;;False;1;0;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-379.6432,269.4673;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FloorOpNode;45;-1093.621,137.1164;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;50;-918.4719,427.404;Inherit;False;return _Vectors[Index]@;4;Create;1;True;Index;FLOAT;0;In;;Inherit;False;GetVector;True;False;0;;False;1;0;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-559.528,504.8013;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.5,0.5,0.5,0.5;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;58;-178.6047,122.172;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;59;-182.9587,428.0581;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ToggleSwitchNode;54;-460.7946,613.7794;Inherit;False;Property;_Signed;Signed;2;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-1239.185,515.2797;Inherit;False;Property;_Multiplier;Multiplier;3;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;61;-895.1848,541.2797;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1076.323,519.7905;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;55;-556.6158,119.6817;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-710.4279,430.0014;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0.5,0.5,0.5,0.5;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-723.7953,264.028;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
WireConnection;14;0;9;0
WireConnection;14;1;51;0
WireConnection;14;2;23;0
WireConnection;29;0;34;0
WireConnection;29;1;17;2
WireConnection;34;0;17;1
WireConnection;34;1;37;0
WireConnection;23;0;26;0
WireConnection;9;1;29;0
WireConnection;26;0;17;1
WireConnection;0;0;14;0
WireConnection;47;0;17;2
WireConnection;51;0;58;0
WireConnection;51;1;59;0
WireConnection;48;0;49;0
WireConnection;44;0;45;0
WireConnection;22;0;48;0
WireConnection;22;1;48;0
WireConnection;22;2;48;0
WireConnection;22;3;48;0
WireConnection;45;0;47;0
WireConnection;50;0;45;0
WireConnection;53;0;52;0
WireConnection;58;0;55;0
WireConnection;58;1;22;0
WireConnection;58;2;54;0
WireConnection;59;0;52;0
WireConnection;59;1;53;0
WireConnection;59;2;54;0
WireConnection;61;0;62;0
WireConnection;61;1;62;0
WireConnection;61;2;62;0
WireConnection;61;3;62;0
WireConnection;62;0;60;0
WireConnection;55;0;49;0
WireConnection;55;1;49;0
WireConnection;55;2;49;0
WireConnection;55;3;49;0
WireConnection;52;0;50;0
WireConnection;52;1;61;0
WireConnection;49;0;44;0
WireConnection;49;1;62;0
ASEEND*/
//CHKSM=DC19FAD6CA0330B45FDA6A64712BD663D1F09184