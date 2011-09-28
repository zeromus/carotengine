//Input variables
float4x4 worldViewProjection;
float4x4 matrixTexture;

//float2 textureTranslate;

float testval;

//texture Tex0;
sampler Sampler0;// = sampler_state { Texture = (Tex0); };
//texture Tex1;
sampler Sampler1;// = sampler_state { Texture = (Tex1); };

struct VS_OUTPUT 
{
   float4 pos:   POSITION;
   float4 col:	 COLOR;
   float2 tex:   TEXCOORD0;
};


VS_OUTPUT Transform(VS_OUTPUT src)
{
   VS_OUTPUT ret;
   ret = src;
   ret.pos = mul(src.pos,worldViewProjection);
   //ret.tex *= texScale; //this would work if we need it
   //ret.tex = mul(src.tex,textureTransform);
  // ret.tex += textureTranslate;
   return ret;
}

//pixel shader constants
float4 mapArgs;
float4 rasterArgs;
//---------

//the new master pixel shader
float4 fixedColor;
float master_texalpha;
float4 master_texmask;
float master_coloralpha;
float4 master_colormask;
float4 master_gradmask;
bool master_colormodulate, master_premultiply_alpha, master_greyscale;
bool master_texflip_h, master_texflip_v;
float master_brightness;

float4 PS_Master(
	float2 tex : TEXCOORD0,
	float4 gradcol : COLOR
) : COLOR
{
	if(master_texflip_h) tex.x = 1-tex.x;
	if(master_texflip_v) tex.y = 1-tex.y;
	float4 texcol = tex2D(Sampler0,tex);
	if(master_colormodulate) texcol *= fixedColor;

	float4 col = texcol*float4(1,1,1,master_texalpha)*master_texmask
	+ fixedColor*float4(1,1,1,master_coloralpha)*master_colormask
	+ gradcol*master_gradmask;
	
	//perform alpha premultiplication here
	col = col * float4(col.a,col.a,col.a,1);
	
	//not very useful...
	//if(master_greyscale) {
	//	float temp = max(max(col.r,col.g),col.b);
	//	col = float4(temp,temp,temp,col.a);
	//}

	if(master_brightness < 0)
	{
		//un-premultiply
		col.rgb = col.rgb / col.a;
		col.rgb = col.rgb * (1 + master_brightness);
		col.rgb *= col.a;
	}
	if(master_brightness > 0)
	{
		//un-premultiply
		col.rgb = col.rgb / col.a;
		col.rgb = col.rgb + (1 - col.rgb) * master_brightness;
		//re-premultiply
		col.rgb *= col.a;
	}

	return col;
}


//------------

//----experimental 3d renderer

//-----------------

float4 Color() : COLOR { return fixedColor; }
float4 Texture(float2 tex: TEXCOORD0) : COLOR { return tex2D(Sampler0,tex); }
float4 Modulate(float2 tex: TEXCOORD0) : COLOR { return fixedColor * tex2D(Sampler0,tex); }
float4 Add(float2 tex: TEXCOORD0) : COLOR { return fixedColor + tex2D(Sampler0,tex); }
float4 TextureAlpha(float2 tex: TEXCOORD0) : COLOR { return tex2D(Sampler0,tex) * float4(1,1,1,fixedColor.a); }
float4 Silhouette(float2 tex: TEXCOORD0) : COLOR { return float4(fixedColor.rgb,tex2D(Sampler0,tex).a); }
float4 Gradient(float4 col: COLOR) : COLOR { return col; }

technique Master {
   pass p0
   {
		PixelShader = compile ps_2_0 PS_Master();
		VertexShader = compile vs_1_1 Transform();
	}  
}




//--------------------------------------------------------------//
// Technique Section for Sun Effects
//--------------------------------------------------------------//
//technique Sun
//{
   //pass Color
   //{
      //CULLMODE = NONE;
      //ALPHABLENDENABLE = TRUE;
      //SRCBLEND = SRCALPHA;
      //DESTBLEND = INVSRCALPHA;
      //ZENABLE = FALSE; //Always want this on top

      //VertexShader = compile vs_1_1 PassThrough();
      //PixelShader = compile ps_1_1 Color();
   //}
//
//}

