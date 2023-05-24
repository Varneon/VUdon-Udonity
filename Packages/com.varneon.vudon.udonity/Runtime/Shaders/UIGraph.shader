// Made with Amplify Shader Editor v1.9.1.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UIGraph"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _LineThickness("LineThickness", Range( 0.001 , 0.1)) = 0.01
        _Line1Color("Line1Color", Color) = (1,0,0,1)
        _Line2Color("Line2Color", Color) = (0,1,0,1)
        _Line3Color("Line3Color", Color) = (0,0,1,1)
        _Line4Color("Line4Color", Color) = (1,1,0,1)
        _InputData("InputData", 2D) = "white" {}
        [Toggle]_UseImageColor("UseImageColor", Float) = 0

    }

    SubShader
    {
		LOD 0

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        Stencil
        {
        	Ref [_Stencil]
        	ReadMask [_StencilReadMask]
        	WriteMask [_StencilWriteMask]
        	Comp [_StencilComp]
        	Pass [_StencilOp]
        }


        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        
        Pass
        {
            Name "Default"
        CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4  mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
                
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            uniform float4 _Line1Color;
            uniform float _UseImageColor;
            uniform sampler2D _InputData;
            uniform float _LineThickness;
            uniform float4 _Line2Color;
            uniform float4 _Line3Color;
            uniform float4 _Line4Color;

            
            v2f vert(appdata_t v )
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                

                v.vertex.xyz +=  float3( 0, 0, 0 ) ;

                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.worldPosition = v.vertex;
                OUT.vertex = vPosition;

                float2 pixelSize = vPosition.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                OUT.texcoord = v.texcoord;
                OUT.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN ) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0/alphaPrecision);
                IN.color.a = round(IN.color.a * alphaPrecision)*invAlphaPrecision;

                float4 appendResult89 = (float4(IN.color.r , IN.color.g , IN.color.b , 1.0));
                float4 lerpResult83 = lerp( _Line1Color , appendResult89 , (( _UseImageColor )?( 1.0 ):( 0.0 )));
                float2 appendResult42 = (float2(1.0 , ( 1.0 / ( 256.0 * 7.0 ) )));
                float2 appendResult44 = (float2(0.0 , IN.color.a));
                float2 texCoord8 = IN.texcoord.xy * appendResult42 + appendResult44;
                float2 appendResult65 = (float2(texCoord8.x , ( ( floor( ( texCoord8.y * 256.0 ) ) / 256.0 ) + ( 1.0 / 512.0 ) )));
                float4 tex2DNode25 = tex2D( _InputData, appendResult65 );
                float temp_output_10_0_g7 = _LineThickness;
                float temp_output_3_0_g7 = (temp_output_10_0_g7 + (tex2DNode25.r - 0.0) * (( 1.0 - temp_output_10_0_g7 ) - temp_output_10_0_g7) / (1.0 - 0.0));
                float4 lerpResult9_g7 = lerp( float4( 0,0,0,0 ) , lerpResult83 , (( IN.texcoord.xy.y >= ( temp_output_3_0_g7 - temp_output_10_0_g7 ) && IN.texcoord.xy.y <= ( temp_output_3_0_g7 + temp_output_10_0_g7 ) ) ? 1.0 :  0.0 ));
                float4 lerpResult85 = lerp( _Line2Color , appendResult89 , (( _UseImageColor )?( 1.0 ):( 0.0 )));
                float temp_output_10_0_g4 = _LineThickness;
                float temp_output_3_0_g4 = (temp_output_10_0_g4 + (tex2DNode25.g - 0.0) * (( 1.0 - temp_output_10_0_g4 ) - temp_output_10_0_g4) / (1.0 - 0.0));
                float4 lerpResult9_g4 = lerp( float4( 0,0,0,0 ) , lerpResult85 , (( IN.texcoord.xy.y >= ( temp_output_3_0_g4 - temp_output_10_0_g4 ) && IN.texcoord.xy.y <= ( temp_output_3_0_g4 + temp_output_10_0_g4 ) ) ? 1.0 :  0.0 ));
                float4 blendOpSrc79 = lerpResult9_g7;
                float4 blendOpDest79 = lerpResult9_g4;
                float4 lerpResult86 = lerp( _Line3Color , appendResult89 , (( _UseImageColor )?( 1.0 ):( 0.0 )));
                float temp_output_10_0_g5 = _LineThickness;
                float temp_output_3_0_g5 = (temp_output_10_0_g5 + (tex2DNode25.b - 0.0) * (( 1.0 - temp_output_10_0_g5 ) - temp_output_10_0_g5) / (1.0 - 0.0));
                float4 lerpResult9_g5 = lerp( float4( 0,0,0,0 ) , lerpResult86 , (( IN.texcoord.xy.y >= ( temp_output_3_0_g5 - temp_output_10_0_g5 ) && IN.texcoord.xy.y <= ( temp_output_3_0_g5 + temp_output_10_0_g5 ) ) ? 1.0 :  0.0 ));
                float4 lerpResult87 = lerp( _Line4Color , appendResult89 , (( _UseImageColor )?( 1.0 ):( 0.0 )));
                float temp_output_10_0_g6 = _LineThickness;
                float temp_output_3_0_g6 = (temp_output_10_0_g6 + (tex2DNode25.a - 0.0) * (( 1.0 - temp_output_10_0_g6 ) - temp_output_10_0_g6) / (1.0 - 0.0));
                float4 lerpResult9_g6 = lerp( float4( 0,0,0,0 ) , lerpResult87 , (( IN.texcoord.xy.y >= ( temp_output_3_0_g6 - temp_output_10_0_g6 ) && IN.texcoord.xy.y <= ( temp_output_3_0_g6 + temp_output_10_0_g6 ) ) ? 1.0 :  0.0 ));
                float4 blendOpSrc80 = lerpResult9_g5;
                float4 blendOpDest80 = lerpResult9_g6;
                float4 blendOpSrc81 = ( saturate( max( blendOpSrc79, blendOpDest79 ) ));
                float4 blendOpDest81 = ( saturate( max( blendOpSrc80, blendOpDest80 ) ));
                

                half4 color = ( saturate( 	max( blendOpSrc81, blendOpDest81 ) ));

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                color.rgb *= color.a;

                return color;
            }
        ENDCG
        }
    }
    CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19102
Node;AmplifyShaderEditor.SamplerNode;25;-1485.944,84.4968;Inherit;True;Property;_DataTexture;DataTexture;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;-145.6,-50.69999;Float;False;True;-1;2;ASEMaterialInspector;0;3;UIGraph;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;True;True;True;True;True;0;True;_ColorMask;False;False;False;False;False;False;False;True;True;0;True;_Stencil;255;True;_StencilReadMask;255;True;_StencilWriteMask;0;True;_StencilComp;0;True;_StencilOp;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;0;True;unity_GUIZTestMode;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.DynamicAppendNode;65;-1635.77,117.6681;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;67;-1765.029,190.1197;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-2173.065,189.9834;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;256;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;44;-2580.479,132.2482;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-2412.556,93.83363;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;42;-2580.957,27.67682;Inherit;False;FLOAT2;4;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-2960.294,40.03559;Inherit;False;2;2;0;FLOAT;256;False;1;FLOAT;7;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;60;-2769.917,133.8744;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;41;-2764.934,27.89496;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;256;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;68;-1899.92,191.1039;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;256;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;63;-1897.865,292.8834;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;512;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;66;-2036.128,190.1197;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;35;-1317.4,774.4;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;32;-1124.23,587.7448;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.01;False;4;FLOAT;0.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;34;-788.0469,542.7626;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;46;-841.9958,417.4075;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-783.6923,636.3928;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareWithRange;29;-571.6727,483.2212;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;37;-295.5371,435.9787;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;39;-563.7162,311.634;Inherit;False;Property;_GraphColor;GraphColor;1;0;Create;True;0;0;0;False;0;False;0.3333333,1,0,1;0.3333332,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;73;-1005.712,26.34708;Inherit;False;GraphRenderComparison;-1;;4;fc33f69944449cb4393f3d7e4e829c23;0;3;12;FLOAT;0;False;10;FLOAT;0;False;11;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;74;-1010.042,141.8027;Inherit;False;GraphRenderComparison;-1;;5;fc33f69944449cb4393f3d7e4e829c23;0;3;12;FLOAT;0;False;10;FLOAT;0;False;11;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;75;-1007.155,265.9174;Inherit;False;GraphRenderComparison;-1;;6;fc33f69944449cb4393f3d7e4e829c23;0;3;12;FLOAT;0;False;10;FLOAT;0;False;11;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;80;-706.9747,186.0598;Inherit;False;Lighten;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;79;-704.5557,-48.54586;Inherit;False;Lighten;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;81;-467.5313,54.24527;Inherit;False;Lighten;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-1647.655,394.24;Inherit;False;Property;_LineThickness;LineThickness;0;0;Create;True;0;0;0;False;0;False;0.01;0.025;0.001;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;72;-1001.513,-99.68096;Inherit;False;GraphRenderComparison;-1;;7;fc33f69944449cb4393f3d7e4e829c23;0;3;12;FLOAT;0;False;10;FLOAT;0;False;11;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;52;-1868.312,-62.49854;Inherit;True;Property;_InputData;InputData;6;0;Create;True;0;0;0;False;0;False;f320730d06205644186fc12c708b644a;f320730d06205644186fc12c708b644a;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ColorNode;71;-1585.638,-640.6156;Inherit;False;Property;_Line1Color;Line1Color;2;0;Create;True;0;0;0;False;0;False;1,0,0,1;0.3333332,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;76;-1579.737,-446.1707;Inherit;False;Property;_Line2Color;Line2Color;3;0;Create;True;0;0;0;False;0;False;0,1,0,1;0.3333332,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;77;-1571.077,-264.328;Inherit;False;Property;_Line3Color;Line3Color;4;0;Create;True;0;0;0;False;0;False;0,0,1,1;0.3333332,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;78;-1565.347,-88.70975;Inherit;False;Property;_Line4Color;Line4Color;5;0;Create;True;0;0;0;False;0;False;1,1,0,1;0.3333332,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;83;-1295.609,-629.0928;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;85;-1296.938,-417.6025;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;86;-1290.288,-232.7152;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;87;-1288.957,-63.78923;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexColorNode;88;-2015.206,-589.189;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;89;-1767.803,-565.247;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ToggleSwitchNode;84;-1584.246,-743.4835;Inherit;False;Property;_UseImageColor;UseImageColor;7;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
WireConnection;25;0;52;0
WireConnection;25;1;65;0
WireConnection;0;0;81;0
WireConnection;65;0;8;1
WireConnection;65;1;67;0
WireConnection;67;0;68;0
WireConnection;67;1;63;0
WireConnection;64;0;8;2
WireConnection;44;1;60;4
WireConnection;8;0;42;0
WireConnection;8;1;44;0
WireConnection;42;1;41;0
WireConnection;41;1;49;0
WireConnection;68;0;66;0
WireConnection;66;0;64;0
WireConnection;35;0;36;0
WireConnection;32;0;25;1
WireConnection;32;3;36;0
WireConnection;32;4;35;0
WireConnection;34;0;32;0
WireConnection;34;1;36;0
WireConnection;33;0;32;0
WireConnection;33;1;36;0
WireConnection;29;0;46;2
WireConnection;29;1;34;0
WireConnection;29;2;33;0
WireConnection;37;1;39;0
WireConnection;37;2;29;0
WireConnection;73;12;25;2
WireConnection;73;10;36;0
WireConnection;73;11;85;0
WireConnection;74;12;25;3
WireConnection;74;10;36;0
WireConnection;74;11;86;0
WireConnection;75;12;25;4
WireConnection;75;10;36;0
WireConnection;75;11;87;0
WireConnection;80;0;74;0
WireConnection;80;1;75;0
WireConnection;79;0;72;0
WireConnection;79;1;73;0
WireConnection;81;0;79;0
WireConnection;81;1;80;0
WireConnection;72;12;25;1
WireConnection;72;10;36;0
WireConnection;72;11;83;0
WireConnection;83;0;71;0
WireConnection;83;1;89;0
WireConnection;83;2;84;0
WireConnection;85;0;76;0
WireConnection;85;1;89;0
WireConnection;85;2;84;0
WireConnection;86;0;77;0
WireConnection;86;1;89;0
WireConnection;86;2;84;0
WireConnection;87;0;78;0
WireConnection;87;1;89;0
WireConnection;87;2;84;0
WireConnection;89;0;88;1
WireConnection;89;1;88;2
WireConnection;89;2;88;3
ASEEND*/
//CHKSM=43DA75A7F661847F62DB63EB925E6DC3CE9CAB8C