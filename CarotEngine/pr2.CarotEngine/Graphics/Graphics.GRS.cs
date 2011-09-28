using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pr2;

namespace pr2.CarotEngine {
partial class GameEngine {

/// <summary>
/// this class consolidates a minimal set of useful 2d rendering state
/// </summary>
public class GRS :GameEngineComponent
{
	public GRS() {}

	public Color _color;
	public float _alpha = 1.0f;
	public Technique _renderMode;

	public Color color { set { isColorDirty |= (_color != value); _color = value; } get { return _color; } }
	public float alpha { set { isColorDirty |= (_alpha != value); _alpha = value; } get { return _alpha; } }
	public Technique renderMode { set { isRenderModeDirty = (_renderMode != value); _renderMode = value; } get { return _renderMode; } }

	public void setColor(float alpha, Color c) {
		isColorDirty |= (_alpha != alpha || _color != c);
		_color = c;
		_alpha = alpha;
	}

	internal bool IsTransformDirty { get { return isTransformDirty || _userTransform.IsDirty; } }

	private bool isTransformDirty = true;
	internal Matrix _transform = Matrix.Identity;
	public Matrix transform {
		get { return _transform; }
		set { _transform = value; isTransformDirty = true; }
	}

	private MatrixStack _userTransform = new MatrixStack();
	public MatrixStack UserTransform {
		get { return _userTransform; }
	}

	internal Matrix GetTransform() {
		return _userTransform.Top * _transform;
	}

	public bool isDestDirty = true;
	internal Image _dest;
	public Image dest {
		get { return _dest; }
		set { isDestDirty |= (_dest != value); _dest = value; }
	}

	public bool isSrcDirty = true;
	internal Image _src;
	public Image src {
		get { return _src; }
		set { isSrcDirty |= (_src != value); _src = value; }
	}

	public bool isClipDirty = true;
	internal Rectangle _clipRectangle;
	internal bool _clipEnabled;
	public bool ClipEnabled {
		get { return _clipEnabled; }
		set { isClipDirty |= (_clipEnabled != value); _clipEnabled = value; }
	}
	public Rectangle clipRectangle {
		get { return _clipRectangle; }
		set { isClipDirty |= (clipRectangle != value); _clipRectangle = value; }
	}

	public bool isColorDirty = true;
	public bool isRenderModeDirty = true;

	//---picking--
	internal int pick_x;
	internal int pick_y;
	internal bool pick;
	internal int pick_val;
	public void enablePick(int x, int y) {
		isColorDirty = true;
		isDestDirty = true;
		isRenderModeDirty = true;
		pick = true;
		pick_x = x;
		pick_y = y;
	}

	public void disablePick() {
		isColorDirty = true;
		isDestDirty = true;
		isRenderModeDirty = true;
		pick = false;
	}

	public void setPickValue(int val) {
		if(pick) isColorDirty = true;
		pick_val = val;
	}

	//---picking--

	/// <summary>
	/// activates the renderstate
	/// </summary>
	public void Activate() {
		game.setActiveRenderState(this);
	}

	/// <summary>
	/// flags all states as undirty
	/// </summary>
	public void undirty() {
		isColorDirty = isRenderModeDirty = isSrcDirty = isTransformDirty = isDestDirty = isClipDirty = false;
		_userTransform.IsDirty = false;
	}
}
}
}