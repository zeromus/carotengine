//http://blogs.msdn.com/b/davidebb/archive/2009/07/17/two-ways-to-use-t4-templates-support-code-vs-one-time-generation.aspx
//http://www.microsoft.com/downloads/en/details.aspx?FamilyID=47305cf4-2bea-43c0-91cd-1b853602dcc5&displaylang=en (not helpful)
//http://www.microsoft.com/downloads/en/details.aspx?FamilyID=b3deb194-ca86-4fb6-a716-b67c2604a139&displaylang=en


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace pr2.CarotEngine {
public class CarotRasterizerState
{

internal Microsoft.Xna.Framework.Graphics.CullMode cache_CullMode = Microsoft.Xna.Framework.Graphics.CullMode.CullCounterClockwiseFace;
public Microsoft.Xna.Framework.Graphics.CullMode CullMode { get { return cache_CullMode; } set { if(cache_CullMode != value) { change(); cache_CullMode = value; } } }
internal float cache_DepthBias = 0f;
public float DepthBias { get { return cache_DepthBias; } set { if(cache_DepthBias != value) { change(); cache_DepthBias = value; } } }
internal Microsoft.Xna.Framework.Graphics.FillMode cache_FillMode = Microsoft.Xna.Framework.Graphics.FillMode.Solid;
public Microsoft.Xna.Framework.Graphics.FillMode FillMode { get { return cache_FillMode; } set { if(cache_FillMode != value) { change(); cache_FillMode = value; } } }
internal bool cache_MultiSampleAntiAlias = true;
public bool MultiSampleAntiAlias { get { return cache_MultiSampleAntiAlias; } set { if(cache_MultiSampleAntiAlias != value) { change(); cache_MultiSampleAntiAlias = value; } } }
internal bool cache_ScissorTestEnable = false;
public bool ScissorTestEnable { get { return cache_ScissorTestEnable; } set { if(cache_ScissorTestEnable != value) { change(); cache_ScissorTestEnable = value; } } }
internal float cache_SlopeScaleDepthBias = 0f;
public float SlopeScaleDepthBias { get { return cache_SlopeScaleDepthBias; } set { if(cache_SlopeScaleDepthBias != value) { change(); cache_SlopeScaleDepthBias = value; } } }
 

bool dirty = false;
private void change()
{
	dirty = true;
}


 
public void Apply() {
	if(!dirty) return;
	dirty = false;
	RasterizerState s = new RasterizerState();

	s.CullMode = cache_CullMode;
	s.DepthBias = cache_DepthBias;
	s.FillMode = cache_FillMode;
	s.MultiSampleAntiAlias = cache_MultiSampleAntiAlias;
	s.ScissorTestEnable = cache_ScissorTestEnable;
	s.SlopeScaleDepthBias = cache_SlopeScaleDepthBias;
GameEngine.Game.Device.RasterizerState = s;

} //Apply()
}//class
}//namespace


namespace pr2.CarotEngine {
public class CarotBlendState
{

internal BlendFunction cache_AlphaBlendFunction = BlendFunction.Add;
public BlendFunction AlphaBlendFunction { get { return cache_AlphaBlendFunction; } set { if(cache_AlphaBlendFunction != value) { change(); cache_AlphaBlendFunction = value; } } }
internal Blend cache_AlphaDestinationBlend = Blend.Zero;
public Blend AlphaDestinationBlend { get { return cache_AlphaDestinationBlend; } set { if(cache_AlphaDestinationBlend != value) { change(); cache_AlphaDestinationBlend = value; } } }
internal Blend cache_AlphaSourceBlend = Blend.One;
public Blend AlphaSourceBlend { get { return cache_AlphaSourceBlend; } set { if(cache_AlphaSourceBlend != value) { change(); cache_AlphaSourceBlend = value; } } }
internal Color cache_BlendFactor = Color.White;
public Color BlendFactor { get { return cache_BlendFactor; } set { if(cache_BlendFactor != value) { change(); cache_BlendFactor = value; } } }
internal BlendFunction cache_ColorBlendFunction = BlendFunction.Add;
public BlendFunction ColorBlendFunction { get { return cache_ColorBlendFunction; } set { if(cache_ColorBlendFunction != value) { change(); cache_ColorBlendFunction = value; } } }
internal Blend cache_ColorDestinationBlend = Blend.Zero;
public Blend ColorDestinationBlend { get { return cache_ColorDestinationBlend; } set { if(cache_ColorDestinationBlend != value) { change(); cache_ColorDestinationBlend = value; } } }
internal Blend cache_ColorSourceBlend = Blend.One;
public Blend ColorSourceBlend { get { return cache_ColorSourceBlend; } set { if(cache_ColorSourceBlend != value) { change(); cache_ColorSourceBlend = value; } } }
internal Microsoft.Xna.Framework.Graphics.ColorWriteChannels cache_ColorWriteChannels = Microsoft.Xna.Framework.Graphics.ColorWriteChannels.All;
public Microsoft.Xna.Framework.Graphics.ColorWriteChannels ColorWriteChannels { get { return cache_ColorWriteChannels; } set { if(cache_ColorWriteChannels != value) { change(); cache_ColorWriteChannels = value; } } }
internal Microsoft.Xna.Framework.Graphics.ColorWriteChannels cache_ColorWriteChannels1 = Microsoft.Xna.Framework.Graphics.ColorWriteChannels.All;
public Microsoft.Xna.Framework.Graphics.ColorWriteChannels ColorWriteChannels1 { get { return cache_ColorWriteChannels1; } set { if(cache_ColorWriteChannels1 != value) { change(); cache_ColorWriteChannels1 = value; } } }
internal Microsoft.Xna.Framework.Graphics.ColorWriteChannels cache_ColorWriteChannels2 = Microsoft.Xna.Framework.Graphics.ColorWriteChannels.All;
public Microsoft.Xna.Framework.Graphics.ColorWriteChannels ColorWriteChannels2 { get { return cache_ColorWriteChannels2; } set { if(cache_ColorWriteChannels2 != value) { change(); cache_ColorWriteChannels2 = value; } } }
internal Microsoft.Xna.Framework.Graphics.ColorWriteChannels cache_ColorWriteChannels3 = Microsoft.Xna.Framework.Graphics.ColorWriteChannels.All;
public Microsoft.Xna.Framework.Graphics.ColorWriteChannels ColorWriteChannels3 { get { return cache_ColorWriteChannels3; } set { if(cache_ColorWriteChannels3 != value) { change(); cache_ColorWriteChannels3 = value; } } }
internal int cache_MultiSampleMask = -1;
public int MultiSampleMask { get { return cache_MultiSampleMask; } set { if(cache_MultiSampleMask != value) { change(); cache_MultiSampleMask = value; } } }
 

bool dirty = false;
private void change()
{
	dirty = true;
}


 
public void Apply() {
	if(!dirty) return;
	dirty = false;
	BlendState s = new BlendState();

	s.AlphaBlendFunction = cache_AlphaBlendFunction;
	s.AlphaDestinationBlend = cache_AlphaDestinationBlend;
	s.AlphaSourceBlend = cache_AlphaSourceBlend;
	s.BlendFactor = cache_BlendFactor;
	s.ColorBlendFunction = cache_ColorBlendFunction;
	s.ColorDestinationBlend = cache_ColorDestinationBlend;
	s.ColorSourceBlend = cache_ColorSourceBlend;
	s.ColorWriteChannels = cache_ColorWriteChannels;
	s.ColorWriteChannels1 = cache_ColorWriteChannels1;
	s.ColorWriteChannels2 = cache_ColorWriteChannels2;
	s.ColorWriteChannels3 = cache_ColorWriteChannels3;
	s.MultiSampleMask = cache_MultiSampleMask;
GameEngine.Game.Device.BlendState = s;

} //Apply()
}//class
}//namespace


namespace pr2.CarotEngine {
public class CarotSamplerState
{

int samplerNum;
public CarotSamplerState(int samplerNum) { this.samplerNum = samplerNum; }
internal TextureAddressMode cache_AddressU = TextureAddressMode.Clamp;
public TextureAddressMode AddressU { get { return cache_AddressU; } set { if(cache_AddressU != value) { change(); cache_AddressU = value; } } }
internal TextureAddressMode cache_AddressV = TextureAddressMode.Clamp;
public TextureAddressMode AddressV { get { return cache_AddressV; } set { if(cache_AddressV != value) { change(); cache_AddressV = value; } } }
internal TextureAddressMode cache_AddressW = TextureAddressMode.Clamp;
public TextureAddressMode AddressW { get { return cache_AddressW; } set { if(cache_AddressW != value) { change(); cache_AddressW = value; } } }
internal TextureFilter cache_Filter = TextureFilter.Linear;
public TextureFilter Filter { get { return cache_Filter; } set { if(cache_Filter != value) { change(); cache_Filter = value; } } }
internal int cache_MaxAnisotropy = 4;
public int MaxAnisotropy { get { return cache_MaxAnisotropy; } set { if(cache_MaxAnisotropy != value) { change(); cache_MaxAnisotropy = value; } } }
internal int cache_MaxMipLevel = 0;
public int MaxMipLevel { get { return cache_MaxMipLevel; } set { if(cache_MaxMipLevel != value) { change(); cache_MaxMipLevel = value; } } }
internal float cache_MipMapLevelOfDetailBias = 0f;
public float MipMapLevelOfDetailBias { get { return cache_MipMapLevelOfDetailBias; } set { if(cache_MipMapLevelOfDetailBias != value) { change(); cache_MipMapLevelOfDetailBias = value; } } }
 

bool dirty = false;
private void change()
{
	dirty = true;
}


 
public void Apply() {
	if(!dirty) return;
	dirty = false;
	SamplerState s = new SamplerState();

	s.AddressU = cache_AddressU;
	s.AddressV = cache_AddressV;
	s.AddressW = cache_AddressW;
	s.Filter = cache_Filter;
	s.MaxAnisotropy = cache_MaxAnisotropy;
	s.MaxMipLevel = cache_MaxMipLevel;
	s.MipMapLevelOfDetailBias = cache_MipMapLevelOfDetailBias;
GameEngine.Game.Device.SamplerStates[samplerNum] = s;

} //Apply()
}//class
}//namespace


namespace pr2.CarotEngine {
public class CarotDepthStencilState
{

internal StencilOperation cache_CounterClockwiseStencilDepthBufferFail = StencilOperation.Keep;
public StencilOperation CounterClockwiseStencilDepthBufferFail { get { return cache_CounterClockwiseStencilDepthBufferFail; } set { if(cache_CounterClockwiseStencilDepthBufferFail != value) { change(); cache_CounterClockwiseStencilDepthBufferFail = value; } } }
internal StencilOperation cache_CounterClockwiseStencilFail = StencilOperation.Keep;
public StencilOperation CounterClockwiseStencilFail { get { return cache_CounterClockwiseStencilFail; } set { if(cache_CounterClockwiseStencilFail != value) { change(); cache_CounterClockwiseStencilFail = value; } } }
internal CompareFunction cache_CounterClockwiseStencilFunction = CompareFunction.Always;
public CompareFunction CounterClockwiseStencilFunction { get { return cache_CounterClockwiseStencilFunction; } set { if(cache_CounterClockwiseStencilFunction != value) { change(); cache_CounterClockwiseStencilFunction = value; } } }
internal StencilOperation cache_CounterClockwiseStencilPass = StencilOperation.Keep;
public StencilOperation CounterClockwiseStencilPass { get { return cache_CounterClockwiseStencilPass; } set { if(cache_CounterClockwiseStencilPass != value) { change(); cache_CounterClockwiseStencilPass = value; } } }
internal bool cache_DepthBufferEnable = true;
public bool DepthBufferEnable { get { return cache_DepthBufferEnable; } set { if(cache_DepthBufferEnable != value) { change(); cache_DepthBufferEnable = value; } } }
internal CompareFunction cache_DepthBufferFunction = CompareFunction.LessEqual;
public CompareFunction DepthBufferFunction { get { return cache_DepthBufferFunction; } set { if(cache_DepthBufferFunction != value) { change(); cache_DepthBufferFunction = value; } } }
internal bool cache_DepthBufferWriteEnable = true;
public bool DepthBufferWriteEnable { get { return cache_DepthBufferWriteEnable; } set { if(cache_DepthBufferWriteEnable != value) { change(); cache_DepthBufferWriteEnable = value; } } }
internal int cache_ReferenceStencil = 0;
public int ReferenceStencil { get { return cache_ReferenceStencil; } set { if(cache_ReferenceStencil != value) { change(); cache_ReferenceStencil = value; } } }
internal StencilOperation cache_StencilDepthBufferFail = StencilOperation.Keep;
public StencilOperation StencilDepthBufferFail { get { return cache_StencilDepthBufferFail; } set { if(cache_StencilDepthBufferFail != value) { change(); cache_StencilDepthBufferFail = value; } } }
internal bool cache_StencilEnable = false;
public bool StencilEnable { get { return cache_StencilEnable; } set { if(cache_StencilEnable != value) { change(); cache_StencilEnable = value; } } }
internal StencilOperation cache_StencilFail = StencilOperation.Keep;
public StencilOperation StencilFail { get { return cache_StencilFail; } set { if(cache_StencilFail != value) { change(); cache_StencilFail = value; } } }
internal CompareFunction cache_StencilFunction = CompareFunction.Always;
public CompareFunction StencilFunction { get { return cache_StencilFunction; } set { if(cache_StencilFunction != value) { change(); cache_StencilFunction = value; } } }
internal int cache_StencilMask = -1;
public int StencilMask { get { return cache_StencilMask; } set { if(cache_StencilMask != value) { change(); cache_StencilMask = value; } } }
internal StencilOperation cache_StencilPass = StencilOperation.Keep;
public StencilOperation StencilPass { get { return cache_StencilPass; } set { if(cache_StencilPass != value) { change(); cache_StencilPass = value; } } }
internal int cache_StencilWriteMask = -1;
public int StencilWriteMask { get { return cache_StencilWriteMask; } set { if(cache_StencilWriteMask != value) { change(); cache_StencilWriteMask = value; } } }
internal bool cache_TwoSidedStencilMode = false;
public bool TwoSidedStencilMode { get { return cache_TwoSidedStencilMode; } set { if(cache_TwoSidedStencilMode != value) { change(); cache_TwoSidedStencilMode = value; } } }
 

bool dirty = false;
private void change()
{
	dirty = true;
}


 
public void Apply() {
	if(!dirty) return;
	dirty = false;
	DepthStencilState s = new DepthStencilState();

	s.CounterClockwiseStencilDepthBufferFail = cache_CounterClockwiseStencilDepthBufferFail;
	s.CounterClockwiseStencilFail = cache_CounterClockwiseStencilFail;
	s.CounterClockwiseStencilFunction = cache_CounterClockwiseStencilFunction;
	s.CounterClockwiseStencilPass = cache_CounterClockwiseStencilPass;
	s.DepthBufferEnable = cache_DepthBufferEnable;
	s.DepthBufferFunction = cache_DepthBufferFunction;
	s.DepthBufferWriteEnable = cache_DepthBufferWriteEnable;
	s.ReferenceStencil = cache_ReferenceStencil;
	s.StencilDepthBufferFail = cache_StencilDepthBufferFail;
	s.StencilEnable = cache_StencilEnable;
	s.StencilFail = cache_StencilFail;
	s.StencilFunction = cache_StencilFunction;
	s.StencilMask = cache_StencilMask;
	s.StencilPass = cache_StencilPass;
	s.StencilWriteMask = cache_StencilWriteMask;
	s.TwoSidedStencilMode = cache_TwoSidedStencilMode;
GameEngine.Game.Device.DepthStencilState = s;

} //Apply()
}//class
}//namespace



