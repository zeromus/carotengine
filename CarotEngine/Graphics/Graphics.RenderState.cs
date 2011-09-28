
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using pr2;


namespace pr2.CarotEngine {
partial class GameEngine {

	/// <summary>
	/// Set device render states through this to cache states for nice performance speedups
	/// </summary>
	public CarotRenderState RenderState;

	/// <summary>
	/// Caches renderstate values to eliminate redundant state changes
	/// </summary>
	public class CarotRenderState {
		GameEngine game;
		public CarotRenderState(GameEngine game) {
			this.game = game;
		}

		// Summary:
		//     Gets or sets a value to enable alpha-blended transparency.  The default value
		//     is false.
		//
		// Returns:
		//     true if alpha-blended transparency is enabled; false otherwise.
		public bool AlphaBlendEnable { get { return _alphaBlendEnable; } set { if(_alphaBlendEnable != value) game.Device.RenderState.AlphaBlendEnable = _alphaBlendEnable = value; } }
		bool _alphaBlendEnable = false;
		//
		// Summary:
		//     Gets or sets the arithmetic operation applied to separate alpha blending.
		//      The default is BlendFunction.Add.
		//
		// Returns:
		//     A value from the BlendFunction enumeration.
		public BlendFunction AlphaBlendOperation { get { return _alphaBlendOperation; } set { if(_alphaBlendOperation != value) game.Device.RenderState.AlphaBlendOperation = _alphaBlendOperation = value; } }
		BlendFunction _alphaBlendOperation = BlendFunction.Add;
		//
		// Summary:
		//     Gets or sets the type of destination alpha blending.  The default is Blend.Zero.
		//
		// Returns:
		//     A value from the Blend enumeration.
		public Blend AlphaDestinationBlend { get { return _alphaDestinationBlend; } set { if(_alphaDestinationBlend != value) game.Device.RenderState.AlphaDestinationBlend = _alphaDestinationBlend = value; } }
		Blend _alphaDestinationBlend = Blend.Zero;
		//
		// Summary:
		//     Gets or sets the comparison function for the alpha test. The default is CompareFunction.Always.
		//
		// Returns:
		//     A member of the CompareFunction enumeration that represents the comparison
		//     function to set or get.
		public CompareFunction AlphaFunction { get { return _alphaFunction; } set { if(_alphaFunction != value) game.Device.Render.AlphaFunction = _alphaFunction = value; } }
		CompareFunction _alphaFunction = CompareFunction.Always;
		//
		// Summary:
		//     Gets or sets the type of source alpha blending.  The default is Blend.One.
		//
		// Returns:
		//     A value from the Blend enumeration.
		public Blend AlphaSourceBlend { get { return _alphaSourceBlend; } set { if(_alphaSourceBlend != value) game.Device.RenderState.AlphaSourceBlend = _alphaSourceBlend = value; } }
		Blend _alphaSourceBlend = Blend.One;
		//
		// Summary:
		//     Gets or sets a render state that enables a per-pixel alpha test. The default
		//     value is false.
		//
		// Returns:
		//     true if per-pixel alpha blending is enabled; false otherwise.
		public bool AlphaTestEnable { get { return _alphaTestEnable; } set { if(_alphaTestEnable != value) game.Device.RenderState.AlphaTestEnable = _alphaTestEnable = value; } }
		bool _alphaTestEnable = false;
		//
		// Summary:
		//     Gets or sets the color used for a constant-blend factor during alpha blending.
		//      The default is Color.White.
		//
		// Returns:
		//     The color used for a constant-blend factor during alpha blending.
		public Color BlendFactor { get { return _blendFactor; } set { if(_blendFactor != value) game.Device.RenderState.BlendFactor = _blendFactor = value; } }
		Color _blendFactor = Color.White;
		//
		// Summary:
		//     Gets or sets a value to select the arithmetic operation to apply when RenderState.AlphaBlendEnable
		//     is set to true.  The default is BlendFunction.Add.
		//
		// Returns:
		//     The blending operation to set or get.
		public BlendFunction BlendFunction { get { return _blendFunction; } set { if(_blendFunction != value) game.Device.RenderState.BlendFunction = _blendFunction = value; } }
		BlendFunction _blendFunction = BlendFunction.Add;
		//
		// Summary:
		//     Gets or sets a value that enables a per-channel write for the render target
		//     color buffer.  The default value is ColorWriteChannels.None.
		//
		// Returns:
		//     Value of the ColorWriteChannels enumeration that specifies the color channel
		//     to set or get.
		public ColorWriteChannels ColorWriteChannels { get { return _colorWriteChannels; } set { if(_colorWriteChannels != value) game.Device.RenderState.ColorWriteChannels = _colorWriteChannels = value; } }
		ColorWriteChannels _colorWriteChannels = ColorWriteChannels.None;
		//
		// Summary:
		//     Gets or sets a value that enables a per-channel write for the render target
		//     color buffer.  The default value is ColorWriteChannels.None.
		//
		// Returns:
		//     Value of the ColorWriteChannels enumeration that specifies the color channel
		//     to set or get.
		public ColorWriteChannels ColorWriteChannels1 { get { return _colorWriteChannels1; } set { if(_colorWriteChannels1 != value) game.Device.RenderState.ColorWriteChannels1 = _colorWriteChannels1 = value; } }
		ColorWriteChannels _colorWriteChannels1 = ColorWriteChannels.None;
		//
		// Summary:
		//     Gets or sets a value that enables a per-channel write for the render target
		//     color buffer.  The default value is ColorWriteChannels.None.
		//
		// Returns:
		//     Value of the ColorWriteChannels enumeration that specifies the color channel
		//     to set or get.
		public ColorWriteChannels ColorWriteChannels2 { get { return _colorWriteChannels2; } set { if(_colorWriteChannels2 != value) game.Device.RenderState.ColorWriteChannels2 = _colorWriteChannels2 = value; } }
		ColorWriteChannels _colorWriteChannels2 = ColorWriteChannels.None;
		//
		// Summary:
		//     Gets or sets a value that enables a per-channel write for the render target
		//     color buffer.  The default value is ColorWriteChannels.None.
		//
		// Returns:
		//     Value of the ColorWriteChannels enumeration that specifies the color channel
		//     to set or get.
		public ColorWriteChannels ColorWriteChannels3 { get { return _colorWriteChannels4; } set { if(_colorWriteChannels4 != value) game.Device.RenderState.ColorWriteChannels3 = _colorWriteChannels4 = value; } }
		ColorWriteChannels _colorWriteChannels4 = ColorWriteChannels.None;
		//
		// Summary:
		//     Gets or sets the stencil operation to perform if the stencil test passes
		//     and the depth-buffer test fails for a counterclockwise triangle.  The default
		//     is StencilOperation.Keep.
		//
		// Returns:
		//     The stencil operation to performCounterClockwiseStencilDepthBufferFail
		public StencilOperation CounterClockwiseStencilDepthBufferFail { get { return _counterClockwiseStencilDepthBufferFail; } set { if(_counterClockwiseStencilDepthBufferFail != value) game.Device.RenderState.CounterClockwiseStencilDepthBufferFail = _counterClockwiseStencilDepthBufferFail = value; } }
		StencilOperation _counterClockwiseStencilDepthBufferFail = StencilOperation.Keep;
		//
		// Summary:
		//     Gets or sets the stencil operation to perform if the stencil test fails for
		//     a counterclockwise triangle.  The default is StencilOperation.Keep.
		//
		// Returns:
		//     The stencil operation to perform.
		public StencilOperation CounterClockwiseStencilFail { get { return _counterClockwiseStencilFail; } set { if(_counterClockwiseStencilFail != value) game.Device.RenderState.CounterClockwiseStencilFail = _counterClockwiseStencilFail = value; } }
		StencilOperation _counterClockwiseStencilFail = StencilOperation.Keep;
		//
		// Summary:
		//     Gets or sets the comparison function to use for counterclockwise stencil
		//     tests.  The default is CompareFunction.Always.
		//
		// Returns:
		//     A CompareFunction value indicating which test to perform.
		public CompareFunction CounterClockwiseStencilFunction { get { return _counterClockwiseStencilFunction; } set { if(_counterClockwiseStencilFunction != value) game.Device.RenderState.CounterClockwiseStencilFunction = _counterClockwiseStencilFunction = value; } }
		public CompareFunction _counterClockwiseStencilFunction = CompareFunction.Always;
		//
		// Summary:
		//     Gets or sets the stencil operation to perform if the stencil and z-tests
		//     pass for a counterclockwise triangle.  The default is StencilOperation.Keep.
		//
		// Returns:
		//     The stencil operation to perform.
		public StencilOperation CounterClockwiseStencilPass { get { return _counterClockwiseStencilPass; } set { if(_counterClockwiseStencilPass != value) game.Device.RenderState.CounterClockwiseStencilPass = _counterClockwiseStencilPass = value; } }
		StencilOperation _counterClockwiseStencilPass = StencilOperation.Keep;
		//
		// Summary:
		//     Specifies how back-facing triangles are culled, if at all.  The default value
		//     is CullMode.CounterClockwise.
		//
		// Returns:
		//     The culling mode to set or get.
		public CullMode CullMode { get { return _cullMode; } set { if(_cullMode != value) game.Device.RenderState.CullMode = _cullMode = value; } }
		CullMode _cullMode = CullMode.CullCounterClockwiseFace;
		//
		// Summary:
		//     Sets or retrieves the depth bias for polygons.  The default value is 0. 
		//
		// Returns:
		//     Depth bias for polygons.
		public float DepthBias { get { return _depthBias; } set { if(_depthBias != value) game.Device.RenderState.DepthBias = _depthBias = value; } }
		float _depthBias = 0;
		//
		// Summary:
		//     Enables or disables depth buffering.  The default is true.
		//
		// Returns:
		//     true if depth buffering is enabled; false otherwise.
		public bool DepthBufferEnable { get { return _depthBufferEnable; } set { if(_depthBufferEnable != value) game.Device.RenderState.DepthBufferEnable = _depthBufferEnable = value; } }
		bool _depthBufferEnable = true;

		//
		// Summary:
		//     Gets or sets the comparison function for the depth-buffer test.  The default
		//     is CompareFunction.LessEqual
		//
		// Returns:
		//     Value of a CompareFunction that represents the comparison function to set
		//     or get.
		public CompareFunction DepthBufferFunction { get { return _depthBufferFunction; } set { if(_depthBufferFunction != value) game.Device.RenderState.DepthBufferFunction = _depthBufferFunction = value; } }
		CompareFunction _depthBufferFunction = CompareFunction.LessEqual;
		//
		// Summary:
		//     Enables or disables writing to the depth buffer.  The default is true.  
		//
		// Returns:
		//     true if writing to the depth buffer is enabled; false otherwise.
		public bool DepthBufferWriteEnable { get { return _depthBufferWriteEnable; } set { if(_depthBufferWriteEnable != value) game.Device.RenderState.DepthBufferWriteEnable = _depthBufferWriteEnable = value; } }
		bool _depthBufferWriteEnable = true;
		//
		// Summary:
		//     Contains a member of the Blend enumeration that represents the destination
		//     blend.  The default is Blend.Zero.
		//
		// Returns:
		//     Value of a Blend that represents the current blend mode or the blend mode
		//     to set.
		public Blend DestinationBlend { get { return _destinationBlend; } set { if(_destinationBlend != value) game.Device.RenderState.DestinationBlend = _destinationBlend = value; } }
		Blend _destinationBlend = Blend.Zero;
		//
		// Summary:
		//     Represents the fill mode.  The default is FillMode.Solid.
		//
		// Returns:
		//     Value of a FillMode that specifies the fill mode to set or get.
		public FillMode FillMode { get { return _fillMode; } set { if(_fillMode != value) game.Device.RenderState.FillMode = _fillMode = value; } }
		FillMode _fillMode = FillMode.Solid;
		#if !XBOX360
		//
		// Summary:
		//     [Windows Only] Gets or sets the fog color.  The default value is Color.TransparentBlack.
		//
		// Returns:
		//     A color that specifies the fog color to set or get_fogColor
		public Color FogColor { get { return _fogColor; } set { if(_fogColor != value) game.Device.RenderState.FogColor = _fogColor = value; } }
		Color _fogColor = Color.TransparentBlack;
		//
		// Summary:
		//     [Windows Only] Gets or sets the fog density for pixel or vertex fog used
		//     in exponential fog modes.  The default value is 1.0f.
		//
		// Returns:
		//     Value that represents the fog density to set or get.
		public float FogDensity { get { return _fogDensity; } set { if(_fogDensity != value) game.Device.RenderState.FogDensity = _fogDensity = value; } }
		float _fogDensity = 1;
		//
		// Summary:
		//     [Windows Only] Enables or disables fog blending.  The default is false. 
		//
		// Returns:
		//     true if fog blending is enabled; false otherwise.
		public bool FogEnable { get { return _fogEnable; } set { if(_fogEnable != value) game.Device.RenderState.FogEnable = _fogEnable = value; } }
		bool _fogEnable = false;
		//
		// Summary:
		//     [Windows Only] Gets or sets the depth at which pixel or vertex fog effects
		//     end for linear fog mode.  The default value is 1.0f.
		//
		// Returns:
		//     Value that represents the ending depth to set or get.
		public float FogEnd { get { return _fogEnd; } set { if(_fogEnd != value) game.Device.RenderState.FogEnd = _fogEnd = value; } }
		float _fogEnd = 1;
		//
		// Summary:
		//     [Windows Only] Gets or sets the depth at which pixel or vertex fog effects
		//     begin for linear fog mode.  The default value is 0.0f.
		//
		// Returns:
		//     Value that represents the beginning depth to set or get.
		public float FogStart { get { return _fogStart; } set { if(_fogStart != value) game.Device.RenderState.FogStart = _fogStart = value; } }
		float _fogStart = 0;
		//
		// Summary:
		//     [Windows Only] Gets or sets the fog formula to use for pixel fog.  The default
		//     is None.
		//
		// Returns:
		//     Value of a FogMode that specifies the fog mode to set or get.
		public FogMode FogTableMode { get { return _fogTableMode; } set { if(_fogTableMode != value) game.Device.RenderState.FogTableMode = _fogTableMode = value; } }
		FogMode _fogTableMode = FogMode.None;
		//
		// Summary:
		//     [Windows Only] Gets or sets the fog formula to use for vertex fog.  The default
		//     is FogMode.None.
		//
		// Returns:
		//     Value of a FogMode that specifies the fog mode to set or get.
		public FogMode FogVertexMode { get { return _fogVertexMode; } set { if(_fogVertexMode != value) game.Device.RenderState.FogVertexMode = _fogVertexMode = value; } }
		FogMode _fogVertexMode = FogMode.None;
		#endif
		//
		// Summary:
		//     Enables or disables multisample antialiasing.  The default is true.
		//
		// Returns:
		//     true to enable multisample antialiasing; false otherwise.
		public bool MultiSampleAntiAlias { get { return _multiSampleAntiAlias; } set { if(_multiSampleAntiAlias != value) game.Device.RenderState.MultiSampleAntiAlias = _multiSampleAntiAlias = value; } }
		bool _multiSampleAntiAlias = true;
		//
		// Summary:
		//     Gets or sets a bitmask controlling modification of the samples in a multisample
		//     render target.  The default is 0xffffffff.
		//
		// Returns:
		//     A bitmask value controlling write enables for the samples.  Each bit in this
		//     mask, starting at the least-significant bit, controls modification of one
		//     of the samples in a multisample render target. Thus, for an 8-sample render
		//     target, the low byte contains the eight write enables for each of the eight
		//     samples. This render state has no effect when rendering to a single sample
		//     buffer.
		public int MultiSampleMask { get { return _multiSampleMask; } set { if(_multiSampleMask != value) game.Device.RenderState.MultiSampleMask = _multiSampleMask = value; } }
		int _multiSampleMask = unchecked((int)0xffffffff);
		//
		// Summary:
		//     Gets or sets the size to use for point size computation in cases where point
		//     size is not specified for each vertex. The default value is the value a driver
		//     returns. If a driver returns 0 or 1, the default value is 64, which allows
		//     software point size emulation.
		//
		// Returns:
		//     This value is in world space units.
		public float PointSize { get { return game.Device.RenderState.PointSize; } set { game.Device.RenderState.PointSize = value; } }
		
		//SPECIAL ----- pass this one through to device

		//
		// Summary:
		//     Gets or sets the maximum size of point primitives.  The default is 64.0f.
		//
		// Returns:
		//     The maximum size of point primitives. Must be less than or equal to Capabilities.MaxPointSize
		//     and greater than or equal to RenderState.PointSizeMin.
		public float PointSizeMax { get { return _pointSizeMax; } set { if(_pointSizeMax != value) game.Device.RenderState.PointSizeMax = _pointSizeMax = value; } }
		float _pointSizeMax = 64;
		//
		// Summary:
		//     Gets or sets the minimum size of point primitives.  The default is 1.0f.
		//
		// Returns:
		//     The minimum size of point primitives.
		public float PointSizeMin { get { return _pointSizeMin; } set { if(_pointSizeMin != value) game.Device.RenderState.PointSizeMin = _pointSizeMin = value; } }
		float _pointSizeMin = 1;
		//
		// Summary:
		//     Enables or disables full texture mapping on each point.  The default is false.
		//
		// Returns:
		//     true to set texture coordinates of point primitives so that full textures
		//     are mapped on each point; false otherwise.  When false, the vertex texture
		//     coordinates are used for the entire point.
		public bool PointSpriteEnable { get { return _pointSpriteEnable; } set { if(_pointSpriteEnable != value) game.Device.RenderState.PointSpriteEnable = _pointSpriteEnable = value; } }
		bool _pointSpriteEnable = false;
		#if !XBOX360
		//
		// Summary:
		//     [Windows Only] Gets or sets enabling of range-based vertex fog. The default
		//     value is false.
		//
		// Returns:
		//     true if range-based vertex fog is enabled; false otherwise.  If false, depth-based
		//     fog is used.
		public bool RangeFogEnable { get { return _rangeFogEnable; } set { if(_rangeFogEnable != value) game.Device.RenderState.RangeFogEnable = _rangeFogEnable = value; } }
		bool _rangeFogEnable = false;
		#endif
		//
		// Summary:
		//     Specifies a reference alpha value against which pixels are tested when alpha
		//     testing is enabled. The default value is 0.
		//
		// Returns:
		//     Integer that specifies the reference alpha value to set or get.  This is
		//     an 8-bit value placed in the low 8 bits of the DWORD render-state value.
		//     Values can range from 0x00000000 through 0x000000FF.
		public int ReferenceAlpha { get { return _referenceAlpha; } set { if(_referenceAlpha != value) game.Device.RenderState.ReferenceAlpha = _referenceAlpha = value; } }
		int _referenceAlpha = 0;
		//
		// Summary:
		//     Specifies a reference value to use for the stencil test.  The default is
		//     0.
		//
		// Returns:
		//     Integer that specifies the stencil test value to set or get.
		public int ReferenceStencil { get { return _referenceStencil; } set { if(_referenceStencil != value) game.Device.RenderState.ReferenceStencil = _referenceStencil = value; } }
		int _referenceStencil = 0;
		//
		// Summary:
		//     Enables or disables scissor testing.  The default is false.
		//
		// Returns:
		//     true to enable scissor testing; false otherwise.
		public bool ScissorTestEnable { get { return _scissorTestEnable; } set { if(_scissorTestEnable != value) game.Device.RenderState.ScissorTestEnable = _scissorTestEnable = value; } }
		bool _scissorTestEnable = false;
		//
		// Summary:
		//     Enables or disables the separate blend mode for the alpha channel.  The default
		//     is false.
		//
		// Returns:
		//     true to enable the separate blend mode for the alpha channel; false otherwise.
		public bool SeparateAlphaBlendEnabled { get { return _separateAlphaBlendEnabled; } set { if(_separateAlphaBlendEnabled != value) game.Device.RenderState.SeparateAlphaBlendEnabled = _separateAlphaBlendEnabled = value; } }
		bool _separateAlphaBlendEnabled = false;
		//
		// Summary:
		//     Gets or sets a value used to determine how much bias can be applied to coplanar
		//     primitives to reduce flimmering z-fighting.  The default is 0.
		//
		// Returns:
		//     Value that specifies the slope scale bias to _slopeScaleDepthBias.
		public float SlopeScaleDepthBias { get { return _slopeScaleDepthBias; } set { if(_slopeScaleDepthBias != value) game.Device.RenderState.SlopeScaleDepthBias = _slopeScaleDepthBias = value; } }
		float _slopeScaleDepthBias = 0;
		//
		// Summary:
		//     Gets or sets the color blending mode.  The default is Blend.One.
		//
		// Returns:
		//     A Blend mode to set or get.
		public Blend SourceBlend { get { return _sourceBlend; } set { if(_sourceBlend != value) game.Device.RenderState.SourceBlend = _sourceBlend = value; } }
		Blend _sourceBlend = Blend.One;
		//
		// Summary:
		//     Gets or sets the stencil operation to perform if the stencil test passes
		//     and the depth-test fails.  The default is StencilOperation.Keep.
		//
		// Returns:
		//     The stencil operation to perform.
		public StencilOperation StencilDepthBufferFail { get { return _stencilDepthBufferFail; } set { if(_stencilDepthBufferFail != value) game.Device.RenderState.StencilDepthBufferFail = _stencilDepthBufferFail = value; } }
		StencilOperation _stencilDepthBufferFail = StencilOperation.Keep;
		//
		// Summary:
		//     Gets or sets stencil enabling.  The default is false.
		//
		// Returns:
		//     true if stenciling is enabled; false otherwise.
		public bool StencilEnable { get { return _stencilEnable; } set { if(_stencilEnable != value) game.Device.RenderState.StencilEnable = _stencilEnable = value; } }
		bool _stencilEnable = false;
		//
		// Summary:
		//     Gets or sets the stencil operation to perform if the stencil test fails.
		//      The default is StencilOperation.Keep.
		//
		// Returns:
		//     The stencil operation to perform.
		public StencilOperation StencilFail { get { return _stencilFail; } set { if(_stencilFail != value) game.Device.RenderState.StencilFail = _stencilFail = value; } }
		StencilOperation _stencilFail = StencilOperation.Keep;
		//
		// Summary:
		//     Gets or sets the comparison function for the stencil test.  The default is
		//     CompareFunction.Always.
		//
		// Returns:
		//     Value of a CompareFunction that represents the comparison function to set
		//     or get.
		public CompareFunction StencilFunction { get { return _stencilFunction; } set { if(_stencilFunction != value) game.Device.RenderState.StencilFunction = _stencilFunction = value; } }
		CompareFunction _stencilFunction = CompareFunction.Always;
		//
		// Summary:
		//     Gets or sets the mask applied to the reference value and each stencil buffer
		//     entry to determine the significant bits for the stencil test.  The default
		//     mask is Int32.MaxValue.
		//
		// Returns:
		//     Value that represents the mask to set or get.
		public int StencilMask { get { return _stencilMask; } set { if(_stencilMask != value) game.Device.RenderState.StencilMask = _stencilMask = value; } }
		int _stencilMask = Int32.MaxValue;
		//
		// Summary:
		//     Gets or sets the stencil operation to perform if the stencil test passes.
		//      The default is StencilOperation.Keep.
		//
		// Returns:
		//     The stencil operation to perform.
		public StencilOperation StencilPass { get { return _stencilPass; } set { if(_stencilPass != value) game.Device.RenderState.StencilPass = _stencilPass = value; } }
		StencilOperation _stencilPass = StencilOperation.Keep;
		//
		// Summary:
		//     Gets or sets the write mask applied to values written into the stencil buffer.
		//      The default mask is Int32.MaxValue.
		//
		// Returns:
		//     Value that represents the write mask to set or get.
		public int StencilWriteMask { get { return _stencilWriteMask; } set { if(_stencilWriteMask != value) game.Device.RenderState.StencilWriteMask = _stencilWriteMask = value; } }
		int _stencilWriteMask = Int32.MaxValue;
		//
		// Summary:
		//     Enables or disables two-sided stenciling.  The default is false.
		//
		// Returns:
		//     true to enable two-sided stenciling; false otherwise.
		public bool TwoSidedStencilMode { get { return _twoSidedStencilMode; } set { if(_twoSidedStencilMode != value) game.Device.RenderState.TwoSidedStencilMode = _twoSidedStencilMode = value; } }
		bool _twoSidedStencilMode = false;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap0 { get { return _wrap0; } set { if(_wrap0 != value) game.Device.RenderState.Wrap0 = _wrap0 = value; } }
		TextureWrapCoordinates _wrap0 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap1 { get { return _wrap1; } set { if(_wrap1 != value) game.Device.RenderState.Wrap1 = _wrap1 = value; } }
		TextureWrapCoordinates _wrap1 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap10 { get { return _wrap10; } set { if(_wrap10 != value) game.Device.RenderState.Wrap10 = _wrap10 = value; } }
		TextureWrapCoordinates _wrap10 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap11 { get { return _wrap11; } set { if(_wrap11 != value) game.Device.RenderState.Wrap11 = _wrap11 = value; } }
		TextureWrapCoordinates _wrap11 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap12 { get { return _wrap12; } set { if(_wrap12 != value) game.Device.RenderState.Wrap12 = _wrap12 = value; } }
		TextureWrapCoordinates _wrap12 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap13 { get { return _wrap13; } set { if(_wrap13 != value) game.Device.RenderState.Wrap13 = _wrap13 = value; } }
		TextureWrapCoordinates _wrap13 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap14 { get { return _wrap14; } set { if(_wrap14 != value) game.Device.RenderState.Wrap14 = _wrap14 = value; } }
		TextureWrapCoordinates _wrap14 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap15 { get { return _wrap15; } set { if(_wrap15 != value) game.Device.RenderState.Wrap15 = _wrap15 = value; } }
		TextureWrapCoordinates _wrap15 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap2 { get { return _wrap2; } set { if(_wrap2 != value) game.Device.RenderState.Wrap2 = _wrap2 = value; } }
		TextureWrapCoordinates _wrap2 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap3 { get { return _wrap3; } set { if(_wrap3 != value) game.Device.RenderState.Wrap3 = _wrap3 = value; } }
		TextureWrapCoordinates _wrap3 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap4 { get { return _wrap4; } set { if(_wrap4 != value) game.Device.RenderState.Wrap4 = _wrap4 = value; } }
		TextureWrapCoordinates _wrap4 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap5 { get { return _wrap5; } set { if(_wrap5 != value) game.Device.RenderState.Wrap5 = _wrap5 = value; } }
		TextureWrapCoordinates _wrap5 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap6 { get { return _wrap6; } set { if(_wrap6 != value) game.Device.RenderState.Wrap6 = _wrap6 = value; } }
		TextureWrapCoordinates _wrap6 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap7 { get { return _wrap7; } set { if(_wrap7 != value) game.Device.RenderState.Wrap7 = _wrap7 = value; } }
		TextureWrapCoordinates _wrap7 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from _wrap8 to set or get.
		public TextureWrapCoordinates Wrap8 { get { return _wrap8; } set { if(_wrap8 != value) game.Device.RenderState.Wrap8 = _wrap8 = value; } }
		TextureWrapCoordinates _wrap8 = TextureWrapCoordinates.Zero;
		//
		// Summary:
		//     Gets or sets the texture-wrapping behavior for multiple sets of texture coordinates.
		//      The default value for this render state is TextureWrapCoordinates.Zero (wrapping
		//     disabled in all directions).
		//
		// Returns:
		//     Combination of values from TextureWrapCoordinates to set or get.
		public TextureWrapCoordinates Wrap9 { get { return _wrap9; } set { if(_wrap9 != value) game.Device.RenderState.Wrap9 = _wrap9 = value; } }
		TextureWrapCoordinates _wrap9 = TextureWrapCoordinates.Zero;
	}
}
}
