using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pr2.Common;

namespace pr2.CarotEngine {

	public class MatrixStack  {
		public MatrixStack() {
			LoadIdentity();
			IsDirty = false;
		}

		public static implicit operator Matrix(MatrixStack ms) { return ms.top; }

		public MatrixStack(Matrix matrix) { LoadMatrix(matrix); }

		internal bool IsDirty;

		Matrix top;
		Stack<Matrix> stack = new Stack<Matrix>();

		public Matrix Top { get { return top; } }

		/// <summary>
		/// Resets the matrix stack to an empty identity matrix stack
		/// </summary>
		public void Clear() {
			stack.Clear();
			LoadIdentity();
		}

		/// <summary>
		/// Clears the matrix stack and loads the specified value
		/// </summary>
		public void Clear(Matrix value) {
			stack.Clear();
			top = value;
		}

		public void LoadMatrix(Matrix value) { top = value; IsDirty = true; }
		public void LoadIdentity() { top = Matrix.Identity; IsDirty = true; }
		public void MultiplyMatrix(Matrix value) { top *= value; IsDirty = true; }
		public void MultiplyMatrixLocal(Matrix value) { top = value * top; IsDirty = true; }
		public void Pop() { top = stack.Pop(); IsDirty = true; }
		public void Push() { stack.Push(top); IsDirty = true; }
		public void RotateAxis(Vector3 axisRotation, float angle) { top *= Matrix.CreateFromAxisAngle(axisRotation, angle); IsDirty = true; }
		public void RotateAxisLocal(Vector3 axisRotation, float angle) { top = Matrix.CreateFromAxisAngle(axisRotation, angle) * top; IsDirty = true; }
		//public void RotateYawPitchRoll(float yaw, float pitch, float roll);
		//public void RotateYawPitchRollLocal(float yaw, float pitch, float roll);
		public void Scale(Vector3 scale) { top *= Matrix.CreateScale(scale); IsDirty = true; }
		public void Scale(Vector2 scale) { top *= Matrix.CreateScale(scale.X,scale.Y,1); IsDirty = true; }
		public void Scale(float x, float y, float z) { top *= Matrix.CreateScale(x, y, z); IsDirty = true; }
		public void ScaleLocal(Vector3 scale) { top = Matrix.CreateScale(scale) * top; IsDirty = true; }
		public void ScaleLocal(Vector2 scale) { top = Matrix.CreateScale(scale.X,scale.Y,1) * top; IsDirty = true; }
		//public void InvScaleLocal(Vector2 scale) { top = Matrix.CreateScale(1f/scale.X, 1f/scale.Y, 1) * top; IsDirty = true; }
		public void ScaleLocal(float x, float y, float z) { top = Matrix.CreateScale(x, y, z) * top; IsDirty = true; }
		public void Translate(Vector3 trans) { top *= Matrix.CreateTranslation(trans); IsDirty = true; }
		public void Translate(float x, float y, float z) { top *= Matrix.CreateTranslation(x, y, z); IsDirty = true; }
		public void Translate(int x, int y) { top *= Matrix.CreateTranslation(x, y, 0); IsDirty = true; }
		public void TranslateLocal(Vector3 trans) { top = Matrix.CreateTranslation(trans) * top; IsDirty = true; }
		public void TranslateLocal(float x, float y, float z) { top = Matrix.CreateTranslation(x, y, z) * top; IsDirty = true; }
		public void TranslateLocal(FxVector3 trans) { top = Matrix.CreateTranslation(trans.x.toFloat(), trans.y.toFloat(), trans.z.toFloat()) * top; IsDirty = true; }
		public void Translate(FxVector3 trans) { top *= Matrix.CreateTranslation(trans.x.toFloat(), trans.y.toFloat(), trans.z.toFloat()); IsDirty = true; }



		public void RotateAxis(float x, float y, float z, float angle) { MultiplyMatrix(Matrix.CreateFromAxisAngle(new Vector3(x, y, z), angle)); IsDirty = true; }
		public void RotateAxisLocal(float x, float y, float z, float angle) { MultiplyMatrixLocal(Matrix.CreateFromAxisAngle(new Vector3(x, y, z), angle)); IsDirty = true; }
		public void RotateZLocal(float angle) { MultiplyMatrixLocal(Matrix.CreateRotationZ(angle)); IsDirty = true; }
		public void RotateZLocalDeg(float degangle) { MultiplyMatrixLocal(Matrix.CreateRotationZ(Lib.Rads(degangle))); IsDirty = true; }
		public void RotateZ(float angle) { MultiplyMatrix(Matrix.CreateRotationZ(angle)); IsDirty = true; }
		public void RotateZDeg(float degangle) { MultiplyMatrix(Matrix.CreateRotationZ(Lib.Rads(degangle))); IsDirty = true; }
		public void RotateYLocal(float angle) { MultiplyMatrixLocal(Matrix.CreateRotationY(angle)); IsDirty = true; }
		public void RotateYLocalDeg(float degangle) { MultiplyMatrixLocal(Matrix.CreateRotationY(Lib.Rads(degangle))); IsDirty = true; }
		public void RotateY(float angle) { MultiplyMatrix(Matrix.CreateRotationY(angle)); IsDirty = true; }
		public void RotateXLocal(float angle) { MultiplyMatrixLocal(Matrix.CreateRotationX(angle)); IsDirty = true; }
		public void RotateXLocalDeg(float degangle) { MultiplyMatrixLocal(Matrix.CreateRotationX(Lib.Rads(degangle))); IsDirty = true; }
		public void RotateX(float angle) { MultiplyMatrix(Matrix.CreateRotationX(angle)); IsDirty = true; }
		public void ScaleLocal(float ratio) { ScaleLocal(ratio, ratio, ratio); IsDirty = true; }
		public void ScaleLocal(Fx32 ratio) { float temp = ratio.toFloat(); ScaleLocal(temp, temp, temp); IsDirty = true; }
		public void ScaleLocal(float x, float y) { ScaleLocal(x, y, 1); IsDirty = true; }
		public void Scale(float ratio) { Scale(ratio, ratio, ratio); IsDirty = true; }
		public void Scale(Fx32 ratio) { float temp = ratio.toFloat(); Scale(temp, temp, temp); IsDirty = true; }
		public void Scale(float x, float y) { Scale(x, y, 1); IsDirty = true; }
		public void TranslateLocal(Vector2 v) { TranslateLocal(v.X, v.Y, 0); IsDirty = true; }
		public void TranslateLocal(float x, float y) { TranslateLocal(x, y, 0); IsDirty = true; }
		public void TranslateLocal(Fx32 x, Fx32 y) { TranslateLocal(x, y, 0); IsDirty = true; }
		public void TranslateLocal(Fx32 x, Fx32 y, Fx32 z) { TranslateLocal(x.toFloat(), y.toFloat(), z.toFloat()); IsDirty = true; }
		public void TranslateLocal(Point pt) { TranslateLocal(pt.X, pt.Y, 0); IsDirty = true; }
		public void Translate(Vector2 v) { Translate(v.X, v.Y, 0); IsDirty = true; }
		public void Translate(float x, float y) { Translate(x, y, 0); IsDirty = true; }
		public void Translate(double x, double y) { Translate((float)x, (float)y, 0); IsDirty = true; }
		public void Translate(Point pt) { top *= Matrix.CreateTranslation(pt.X, pt.Y, 0); IsDirty = true; }
		public void MultiplyMatrix(MatrixStack ms) { MultiplyMatrix(ms.Top); IsDirty = true; }
		public void MultiplyMatrixLocal(MatrixStack ms) { MultiplyMatrixLocal(ms.Top); IsDirty = true; }
	}
}