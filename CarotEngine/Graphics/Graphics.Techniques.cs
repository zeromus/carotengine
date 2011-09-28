using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using pr2.Common;

namespace pr2.CarotEngine {
	partial class GameEngine {

		public abstract class Technique : GameEngineComponent {

			//you may find these helpful. indeed, even necessary
			public Effect effect;
			public EffectTechnique technique;

			/// <summary>
			/// makes the technique current
			/// </summary>
			public virtual void Apply() {
				game.setTechnique(this);
			}

			/// <summary>
			/// passes changes to the current render color to the technique
			/// </summary>
			public virtual void SetColor(float alpha, Color color) { }

			public virtual void SetColor(int color) { }

			public virtual void Refresh()
			{
				if(!dirty) return;
				technique.Passes[0].Apply();
			}

			public void MakeDirty()
			{
				dirty = true;
			}

			protected bool dirty = true;
		}

		public CarotTechniques Techniques = new CarotTechniques();
		public class CarotTechniques : GameEngineComponent
		{
			public Effect effect;
			public EffectTechnique masterTechnique;
			public MasterTechnique color, texture, textureAlpha, gradient, silhouette, modulate, add;
			public void init(Effect effect)
			{
				this.effect = effect;
				masterTechnique = effect.Techniques["Master"];
				color = new MasterTechnique(MasterTechnique.Mode.Color, effect, masterTechnique);
				texture = new MasterTechnique(MasterTechnique.Mode.Texture, effect, masterTechnique);
				textureAlpha = new MasterTechnique(MasterTechnique.Mode.TextureAlpha, effect, masterTechnique);
				gradient = new MasterTechnique(MasterTechnique.Mode.Gradient, effect, masterTechnique);
				silhouette = new MasterTechnique(MasterTechnique.Mode.Silhouette, effect, masterTechnique);
				modulate = new MasterTechnique(MasterTechnique.Mode.Modulate, effect, masterTechnique);
				add = new MasterTechnique(MasterTechnique.Mode.Add, effect, masterTechnique);
			}

			public void EngageGreyscale()
			{
				effect.Parameters["master_greyscale"].SetValue(true);
				game.currTechnique.MakeDirty();
			}

			public void DisengageGreyscale()
			{
				effect.Parameters["master_greyscale"].SetValue(false);
				game.currTechnique.MakeDirty();
			}

			public void SetTexFlip(bool hflip, bool vflip)
			{
				effect.Parameters["master_texflip_h"].SetValue(hflip);
				effect.Parameters["master_texflip_v"].SetValue(vflip);
				game.currTechnique.MakeDirty();
			}

			public void SetMasterBrightness(Fx32 level)
			{
				effect.Parameters["master_brightness"].SetValue(level.toFloat());
				game.currTechnique.MakeDirty();
			}

			public class MasterTechnique : Technique
			{
				public MasterTechnique(Mode mode, Effect effect, EffectTechnique technique)
				{
					this.mode = mode;
					this.effect = effect;
					this.technique = technique;
				}
				Mode mode;
				public override void Apply()
				{
					switch (mode)
					{
						case Mode.Color:
							effect.Parameters["master_texmask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_colormask"].SetValue(new float[] { 1, 1, 1, 1 });
							effect.Parameters["master_gradmask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_colormodulate"].SetValue(false);
							break;
						case Mode.Texture:
							effect.Parameters["master_texmask"].SetValue(new float[] { 1, 1, 1, 1 });
							effect.Parameters["master_colormask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_gradmask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_texalpha"].SetValue(1.0f);
							effect.Parameters["master_colormodulate"].SetValue(false);
							break;
						case Mode.TextureAlpha:
							effect.Parameters["master_texmask"].SetValue(new float[] { 1, 1, 1, 1 });
							effect.Parameters["master_colormask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_gradmask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_texalpha"].SetValue(1.0f);
							effect.Parameters["master_colormodulate"].SetValue(false);
							break;
						case Mode.Gradient:
							effect.Parameters["master_texmask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_colormask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_gradmask"].SetValue(new float[] { 1, 1, 1, 1 });
							effect.Parameters["master_colormodulate"].SetValue(false);
							break;
						case Mode.Silhouette:
							effect.Parameters["master_texmask"].SetValue(new float[] { 0, 0, 0, 1 });
							effect.Parameters["master_colormask"].SetValue(new float[] { 1, 1, 1, 0 });
							effect.Parameters["master_gradmask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_colormodulate"].SetValue(false);
							break;
						case Mode.Modulate:
							effect.Parameters["master_texmask"].SetValue(new float[] { 1, 1, 1, 1 });
							effect.Parameters["master_colormask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_gradmask"].SetValue(new float[] { 0, 0, 0, 0 });
							effect.Parameters["master_colormodulate"].SetValue(true);
							break;
					}
					game.setTechnique(this);
				}

				public enum Mode
				{
					Color, Texture, TextureAlpha, Gradient, Silhouette, Modulate, Add
				}

				public override void SetColor(float alpha, Color color)
				{
					//be sure to combine alpha factor with the alpha in the color!
					//be sure to premultiply!
					//float color_a_float = color.A / 255.0f;
					//float realalpha = color_a_float * alpha;
					//float[] constant4 = new float[4];
					//constant4[0] = color.R / 255.0f * realalpha;
					//constant4[1] = color.G / 255.0f * realalpha;
					//constant4[2] = color.B / 255.0f * realalpha;
					//constant4[3] = realalpha;
					effect.Parameters["fixedColor"].SetValue(color.ToVector4());
					effect.Parameters["master_texalpha"].SetValue(alpha);
					effect.Parameters["master_coloralpha"].SetValue(alpha);
					//if(IsActive) effect.CommitChanges();
					dirty = true;
				}
			} //MasterTechnique


			public void clear() { none.Apply(); }

			public NullTechnique none = new NullTechnique();

			public class NullTechnique : Technique
			{
				public override void Apply()
				{
					game.setTechnique(this);
				}

				public override void Refresh()
				{
				}
			}
		} //CarotTechniques

		internal Technique currTechnique;
//		Effect currEffect;

		void setTechnique(Technique technique)
		{
			if (currTechnique == technique) return;
			currTechnique = technique;
			currTechnique.MakeDirty();
		}

	}
}