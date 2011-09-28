//Input variables
float4x4 worldViewProjection;


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
   return ret;
}


//pixel shader constants
float4 mapArgs;
float4 rasterArgs;
float4 fixedColor;
//---------

//the new master pixel shader
float master_texalpha;
float4 master_texmask;
float master_coloralpha;
float4 master_colormask;
float4 master_gradmask;
float4 Master(float2 tex: TEXCOORD0, float4 gradcol: COLOR) : COLOR {
	float4 texcol = tex2D(Sampler0,tex);
	return texcol*float4(1,1,1,master_texalpha)*master_texmask
	+ fixedColor*float4(1,1,1,master_coloralpha)*master_colormask
	+ gradcol*master_gradmask;
	//return texcol*master_texmask;
}

//------------

float4 Color() : COLOR { return fixedColor; }
float4 Texture(float2 tex: TEXCOORD0) : COLOR { return tex2D(Sampler0,tex); }
float4 Modulate(float2 tex: TEXCOORD0) : COLOR { return fixedColor * tex2D(Sampler0,tex); }
float4 TextureAlpha(float2 tex: TEXCOORD0) : COLOR { return tex2D(Sampler0,tex) * float4(1,1,1,fixedColor.a); }
float4 Silhouette(float2 tex: TEXCOORD0) : COLOR { return float4(fixedColor.rgb,tex2D(Sampler0,tex).a); }
float4 Gradient(float4 col: COLOR) : COLOR { return col; }

texture2D tex_mapLayer, tex_mapTileset;
sampler2D s_mapLayer = sampler_state{Texture=(tex_mapLayer);};
sampler2D s_mapTileset= sampler_state{Texture=(tex_mapTileset);};
float4 Map(float4 tex: TEXCOORD0) : COLOR {

	float map_vspTilesW = mapArgs.x;
	float map_vspTilesH = mapArgs.y;
	float2 map_layerSize = mapArgs.zw;

	//enable rasterfx:
	float rasterPhase = rasterArgs.x;
	float rasterOmega = rasterArgs.y;
	float rasterCoeff = rasterArgs.z;
	tex.x += rasterCoeff * sin(rasterPhase + tex.y * rasterOmega);  //raster offset effects
	
	float2 pix = frac(tex); //pixel offset
	float2 tile = floor(tex); //tile coord
	tile += 0.5; //adjust to hit the center of the texcoord for the tile
	int tilenum = tex2D(s_mapLayer,tile/map_layerSize).x; //lookup tile number
	//if(tilenum==0) clip(1);
	float tilenumdiv = tilenum/map_vspTilesW;
	float tilenummod = frac(tilenumdiv); //x loc of tile in tileset
	float tilenumint = floor(tilenumdiv); //y loc of tile in tileset
	
	//enable this adjustment if we want linear filtering (probably with raster fx):
	//keep from overrunning neighboring tiles
	//if(pix.x < 1.0/32) pix.x = 1.0/32;
	//if(pix.y < 1.0/32) pix.y = 1.0/32;
	//if(pix.x > 31.0/32) pix.x = 31.0/32;
	//if(pix.y > 31.0/32) pix.y = 31.0/32;
	
	float u = tilenummod + pix.x/map_vspTilesW; //tilemap.u
	float v = (tilenumint+pix.y)/map_vspTilesH; //tilemap.v

	//debug: bail if we are outside of the tilemap
	float4 color;
	if(tex.x<0) color = float4(1,0,0,1);
	else if(tex.y<0) color = float4(0,1,0,1);
	else if(tex.x>=map_layerSize.x) color = float4(0,0,1,1);
	else if(tex.y>=map_layerSize.y) color = float4(1,1,0,1);
	else color = tex2D(s_mapTileset,float2(u,v)); //get pixel from tilemap
	return color;
}

technique {
   pass Master { PixelShader = compile ps_2_0 Master(); }
   pass Map { PixelShader = compile ps_2_0 Map(); }
   pass Color { PixelShader = compile ps_1_1 Color(); }
   pass Texture { PixelShader = compile ps_1_1 Texture(); }
   pass Modulate { PixelShader = compile ps_1_1 Modulate(); }
   pass TextureAlpha { PixelShader = compile ps_1_1 TextureAlpha(); }
   pass Silhouette { PixelShader = compile ps_1_1 Silhouette(); }
   pass Gradient { PixelShader = compile ps_1_1 Gradient(); }
   
   pass Transform { VertexShader = compile vs_1_1 Transform(); }

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

