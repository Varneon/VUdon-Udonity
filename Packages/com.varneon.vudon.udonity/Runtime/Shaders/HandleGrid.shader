// Made with Amplify Shader Editor v1.9.1.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Varneon/VUdon/Udonity/HandleGrid"
{
	Properties
	{
		_GridTiling("GridTiling", Float) = 10
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Overlay"  "Queue" = "Overlay+0" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		ZTest Less
		Offset  -1 , 0.025
		Blend One One
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _GridTiling;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float clampResult18 = clamp( ( 1.0 - distance( ase_vertex3Pos , float3( 0,0,0 ) ) ) , 0.0 , 1.0 );
			float2 temp_cast_0 = (_GridTiling).xx;
			float temp_output_2_0_g3 = 0.95;
			float2 appendResult10_g4 = (float2(temp_output_2_0_g3 , temp_output_2_0_g3));
			float2 temp_output_11_0_g4 = ( abs( (frac( (i.uv_texcoord*temp_cast_0 + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult10_g4 );
			float2 break16_g4 = ( 1.0 - ( temp_output_11_0_g4 / fwidth( temp_output_11_0_g4 ) ) );
			float temp_output_12_0 = ( 1.0 - saturate( min( break16_g4.x , break16_g4.y ) ) );
			float temp_output_2_0_g5 = 0.99;
			float2 appendResult10_g6 = (float2(temp_output_2_0_g5 , temp_output_2_0_g5));
			float2 temp_output_11_0_g6 = ( abs( (frac( (i.uv_texcoord*float2( 2,2 ) + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult10_g6 );
			float2 break16_g6 = ( 1.0 - ( temp_output_11_0_g6 / fwidth( temp_output_11_0_g6 ) ) );
			float4 clampResult62 = clamp( ( i.vertexColor + ( 1.0 - saturate( min( break16_g6.x , break16_g6.y ) ) ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			o.Emission = ( clampResult18 * temp_output_12_0 * clampResult62 ).rgb;
			o.Alpha = ( clampResult18 * temp_output_12_0 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19102
Node;AmplifyShaderEditor.OneMinusNode;12;-463.4858,208.9057;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;16;-961.287,-80.29428;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;61;-978.0796,144.3162;Inherit;False;Property;_GridTiling;GridTiling;1;0;Create;True;0;0;0;False;0;False;10;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-248.7712,444.7655;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;58;-475.0796,357.3162;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;13;-713.4858,-24.09428;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;10;-779.4858,131.9057;Inherit;True;Grid;-1;;3;a9240ca2be7e49e4f9fa3de380c0dbe9;0;3;5;FLOAT2;10,10;False;6;FLOAT2;0,0;False;2;FLOAT;0.95;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;18;-266.4858,-37.09428;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;44;-519.725,450.3588;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;14;-496.4858,-7.094276;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;57;-722.0796,373.3162;Inherit;False;Grid;-1;;5;a9240ca2be7e49e4f9fa3de380c0dbe9;0;3;5;FLOAT2;2,2;False;6;FLOAT2;0,0;False;2;FLOAT;0.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;62;-85.07961,428.3162;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-103.4858,208.9057;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-0.4857941,83.90572;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;577,13;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Varneon/VUdon/Udonity/HandleGrid;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Off;2;False;;1;False;;True;-1;False;;0.025;False;;False;6;Custom;0.5;True;False;0;True;Overlay;;Overlay;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;4;1;False;;1;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;10;0
WireConnection;51;0;44;0
WireConnection;51;1;58;0
WireConnection;58;0;57;0
WireConnection;13;0;16;0
WireConnection;10;5;61;0
WireConnection;18;0;14;0
WireConnection;14;0;13;0
WireConnection;62;0;51;0
WireConnection;17;0;18;0
WireConnection;17;1;12;0
WireConnection;17;2;62;0
WireConnection;15;0;18;0
WireConnection;15;1;12;0
WireConnection;0;2;17;0
WireConnection;0;9;15;0
ASEEND*/
//CHKSM=FDA85B254F29AD355180B13391F3B3AABE931C9B