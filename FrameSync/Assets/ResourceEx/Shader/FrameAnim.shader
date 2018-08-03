Shader "Custom/Unlit/FrameAnim" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}               //序列帧动画纹理  
		_Color("Color Tint", Color) = (1, 1, 1, 1)            //颜色
		_Row("行",Int) = 1
		_Column("列",Int) = 1
		_Speed("speed",Range(0,10)) = 1

	}

	CGINCLUDE

	#include "UnityCG.cginc"

	sampler2D _MainTex;

	uniform half4 _MainTex_ST;
	uniform fixed4 _Color;
	uniform int _Row;
	uniform int _Column;
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

	fixed4 frag(v2f IN) : SV_Target
	{
		float2 uv = IN.uv;

		float cellX = uv.x / _Column;
		float cellY = uv.y / _Row;

		//Sprite总数
		int count = _Row * _Column;

		//在0到count-1 范围内循环
		int SpriteIndex = fmod(_Time.w*_Speed,count);

		//当前Sprite所在行的下标
		int SpriteRowIndx = (SpriteIndex / _Column);

		//当前Sprite所在列的下标
		int SpriteColumnIndex = fmod(SpriteIndex,_Column);

		//因uv坐标左下角为（0,0），第一行为最底下一行，为了合乎我们常理，我们转换到最上面一行为第一行,eg:0,1,2-->2,1,0
		SpriteRowIndx = (_Row - 1) - fmod(SpriteRowIndx,_Row);

		//乘以1.0转为浮点数,不然加号右边，整数除以整数，还是整数（有误）
		uv.x = cellX + SpriteColumnIndex*1.0 / _Column;
		uv.y = cellY + SpriteRowIndx*1.0 / _Row;

		half4 c = tex2D(_MainTex,uv);

		c.rgb *= _Color.rgb;
		return c;
	}

	ENDCG

	SubShader {
	Tags{ "Queue" = "Transparent" "RenderType" = "Transparent"}
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass{

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		//#pragma fragmentoption ARB_precision_hint_fastest 

		ENDCG

		}

	}
	FallBack Off
}