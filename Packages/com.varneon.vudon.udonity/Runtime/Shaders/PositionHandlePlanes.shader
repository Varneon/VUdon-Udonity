// Made with Amplify Shader Editor v1.9.1.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Varneon/VUdon/Udonity/PositionHandlePlanes"
{
	Properties
	{
		_Outline("Outline", Range( 0 , 0.5)) = 0.25
		[Toggle(UNITY_SINGLE_PASS_STEREO)] _UNITY_SINGLE_PASS_STEREO("UNITY_SINGLE_PASS_STEREO", Float) = 0
		_StereoSeparation("StereoSeparation", Float) = 0.064
		_CameraScale("CameraScale", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Overlay"  "Queue" = "Overlay+0" "IsEmissive" = "true"  }
		Cull Off
		ZTest Always
		Blend One One
		BlendOp Add
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float3 worldNormal;
			float3 viewDir;
			float3 worldPos;
		};

		uniform float _Outline;
		uniform float _StereoSeparation;
		uniform float _CameraScale;


		float EyeIndex100(  )
		{
			return unity_StereoEyeIndex;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float temp_output_88_0 = ( 1.0 - _Outline );
			float clampResult9 = clamp( max( (( i.uv_texcoord.x >= _Outline && i.uv_texcoord.x <= temp_output_88_0 ) ? 0.0 :  1.0 ) , (( i.uv_texcoord.y >= _Outline && i.uv_texcoord.y <= temp_output_88_0 ) ? 0.0 :  1.0 ) ) , 0.1 , 1.0 );
			float3 ase_worldNormal = i.worldNormal;
			float dotResult17 = dot( ase_worldNormal , i.viewDir );
			float clampResult23 = clamp( (-0.5 + (abs( dotResult17 ) - 0.0) * (1.5 - -0.5) / (1.0 - 0.0)) , 0.0 , 1.0 );
			float temp_output_105_0 = ( ( _StereoSeparation * _CameraScale ) / 2.0 );
			float localEyeIndex100 = EyeIndex100();
			float lerpResult102 = lerp( temp_output_105_0 , -temp_output_105_0 , localEyeIndex100);
			float4 appendResult99 = (float4(lerpResult102 , 0.0 , 0.0 , 0.0));
			float3 viewToWorld91 = mul( UNITY_MATRIX_I_V, float4( appendResult99.xyz, 1 ) ).xyz;
			#ifdef UNITY_SINGLE_PASS_STEREO
				float3 staticSwitch93 = viewToWorld91;
			#else
				float3 staticSwitch93 = _WorldSpaceCameraPos;
			#endif
			float3 worldToObj25 = mul( unity_WorldToObject, float4( staticSwitch93, 1 ) ).xyz;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_69_0 = max( ( sign( worldToObj25.y ) == sign( ase_vertex3Pos.y ) ? ( i.vertexColor.g < 0.9 ? 1.0 : 0.0 ) : 0.0 ) , i.vertexColor.g );
			o.Emission = ( i.vertexColor * clampResult9 * clampResult23 * min( min( max( ( sign( worldToObj25.x ) == sign( ase_vertex3Pos.x ) ? ( i.vertexColor.r < 0.9 ? 1.0 : 0.0 ) : 0.0 ) , i.vertexColor.r ) , temp_output_69_0 ) , min( temp_output_69_0 , max( ( sign( worldToObj25.z ) == sign( ase_vertex3Pos.z ) ? ( i.vertexColor.b < 0.9 ? 1.0 : 0.0 ) : 0.0 ) , i.vertexColor.b ) ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19102
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;194.4654,58.20619;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareWithRange;10;-1280.475,-88.65934;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareWithRange;6;-1275.799,133.6927;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;5;-1535.247,-113.1627;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMaxOpNode;12;-957.2481,-84.39948;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;9;-806.7784,-83.07885;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1;-9.758287,-93.68857;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;21;-865.0955,158.9265;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;16;-906.8735,300.8081;Inherit;True;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;17;-625.3708,216.9297;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;87;-497.503,216.7214;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;24;-358.7301,217.1754;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.5;False;4;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;23;-154.9388,217.1534;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SignOpNode;39;-1154.313,518.439;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SignOpNode;42;-1155.734,590.439;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SignOpNode;45;-1151.607,703.6501;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SignOpNode;46;-1150.607,778.6504;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SignOpNode;47;-1148.607,852.6492;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;56;-1442.75,1085.426;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;58;-1175.063,1101.434;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;48;-826.2336,515.7173;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;49;-830.6014,775.9954;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;50;-837.6011,1077.118;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;59;-1172.752,1249.958;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SignOpNode;37;-1152.313,447.4394;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;35;-1483.407,727.7494;Inherit;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;57;-1174.183,954.9253;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;68;-656.0225,612.7306;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;69;-654.784,873.6613;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;70;-654.7409,1175.994;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;85;-423.3417,975.8574;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;84;-420.5713,761.8824;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;86;-232.501,839.2753;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;368.1598,11.47874;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Varneon/VUdon/Udonity/PositionHandlePlanes;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Off;0;False;;7;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Overlay;;Overlay;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;4;1;False;;1;False;;0;5;False;;10;False;;1;False;;1;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1782.507,157.2068;Inherit;False;Property;_Outline;Outline;0;0;Create;True;0;0;0;False;0;False;0.25;0.02;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;88;-1485.438,72.57199;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;25;-1376.947,460.6413;Inherit;False;World;Object;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;26;-2099.795,465.8834;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;99;-2190.465,623.1431;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;102;-2355.465,623.1431;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;103;-2527.465,682.1431;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;100;-2505.465,752.1431;Inherit;False;return unity_StereoEyeIndex@;1;Create;0;EyeIndex;True;False;0;;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;91;-2058.886,617.3777;Inherit;False;View;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleDivideOpNode;105;-2700.358,622.1837;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;93;-1672.575,461.0055;Inherit;False;Property;_UNITY_SINGLE_PASS_STEREO;UNITY_SINGLE_PASS_STEREO;2;0;Create;True;0;0;0;False;0;False;0;0;0;True;UNITY_SINGLE_PASS_STEREO;Toggle;2;Key0;Key1;Fetch;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-2846.97,622.4205;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-3061.272,616.9002;Inherit;False;Property;_StereoSeparation;StereoSeparation;3;0;Create;True;0;0;0;False;0;False;0.064;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-3034.97,697.4205;Inherit;False;Property;_CameraScale;CameraScale;4;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
WireConnection;13;0;1;0
WireConnection;13;1;9;0
WireConnection;13;2;23;0
WireConnection;13;3;86;0
WireConnection;10;0;5;1
WireConnection;10;1;3;0
WireConnection;10;2;88;0
WireConnection;6;0;5;2
WireConnection;6;1;3;0
WireConnection;6;2;88;0
WireConnection;12;0;10;0
WireConnection;12;1;6;0
WireConnection;9;0;12;0
WireConnection;17;0;21;0
WireConnection;17;1;16;0
WireConnection;87;0;17;0
WireConnection;24;0;87;0
WireConnection;23;0;24;0
WireConnection;39;0;25;2
WireConnection;42;0;25;3
WireConnection;45;0;35;1
WireConnection;46;0;35;2
WireConnection;47;0;35;3
WireConnection;58;0;56;2
WireConnection;48;0;37;0
WireConnection;48;1;45;0
WireConnection;48;2;57;0
WireConnection;49;0;39;0
WireConnection;49;1;46;0
WireConnection;49;2;58;0
WireConnection;50;0;42;0
WireConnection;50;1;47;0
WireConnection;50;2;59;0
WireConnection;59;0;56;3
WireConnection;37;0;25;1
WireConnection;57;0;56;1
WireConnection;68;0;48;0
WireConnection;68;1;56;1
WireConnection;69;0;49;0
WireConnection;69;1;56;2
WireConnection;70;0;50;0
WireConnection;70;1;56;3
WireConnection;85;0;69;0
WireConnection;85;1;70;0
WireConnection;84;0;68;0
WireConnection;84;1;69;0
WireConnection;86;0;84;0
WireConnection;86;1;85;0
WireConnection;0;2;13;0
WireConnection;88;0;3;0
WireConnection;25;0;93;0
WireConnection;99;0;102;0
WireConnection;102;0;105;0
WireConnection;102;1;103;0
WireConnection;102;2;100;0
WireConnection;103;0;105;0
WireConnection;91;0;99;0
WireConnection;105;0;107;0
WireConnection;93;1;26;0
WireConnection;93;0;91;0
WireConnection;107;0;98;0
WireConnection;107;1;106;0
ASEEND*/
//CHKSM=34EECB2071C93C45D34C3913F00C3A5FC2BEB3A1