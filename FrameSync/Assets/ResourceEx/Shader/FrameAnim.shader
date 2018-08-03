Shader "Custom/Unlit/FrameAnim" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}               //序列帧动画纹理  
		_Color("Color Tint", Color) = (1, 1, 1, 1)            //颜色
		_HorizontalAmount("Horizontal Amount", float) = 6        // 行数
		_VerticalAmount("Vertical Amount", float) = 1            // 列数 
		_Speed("Speed", Range(1, 100)) = 7                     // 播放速度 
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	sampler2D _MainTex;

	uniform half4 _MainTex_ST;
	uniform fixed4 _Color;
	uniform float _HorizontalAmount;
	uniform float _VerticalAmount;
	uniform float _Speed;

	struct appdata
	{
		float4 vertex : POSITION;   //从形似寄存器中读取顶点坐标
		float2 uv : TEXCOORD0;      //从形似寄存器中读取uv坐标信息
	};


	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;

		//mul(UNITY_MATRIX_MVP,*)' == 'UnityObjectToClipPos(*) 
		//（物体的模型到世界矩阵 * 从世界到摄像机的矩阵 * 投影的矩阵 ）
		o.vertex = UnityObjectToClipPos(v.vertex);

		//TRANSFORM_TEX,就是将模型顶点的uv和Tiling、offset两个变量进行计算得出实际显示用的定点uv
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		//所经过的时间,Unity内置变量_Timefloat4 _Time : Time (t/20, t, t*2, t*3)
		float time = floor(_Time.y * _Speed);
		//该时间渲染序列帧动画指定的行列
		float row = floor(time / _HorizontalAmount);
		float col = time - row * _HorizontalAmount;

		//将所显示的单张图片缩放到应有的大小区域
		half2 uv = float2(i.uv.x / _HorizontalAmount, i.uv.y / _VerticalAmount);
		//移动到对应缩放后的区域位置
		uv.x += col / _HorizontalAmount;
		uv.y -= row / _VerticalAmount;
		fixed4 color = tex2D(_MainTex, uv);
		color.rgb *= _Color.rgb;
		return color;
	}

	ENDCG

	SubShader {
	Tags{ "Queue" = "Transparent" "RenderType" = "Transparent"}
		Cull Back
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 

		ENDCG

		}

	}
	FallBack Off
}