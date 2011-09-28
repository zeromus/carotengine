using System;
using System.Globalization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pr2.CarotEngine {

	/// <summary>Stores an ordered pair of floating-point numbers, typically the width and height of a rectangle.</summary>
	/// <filterpriority>1</filterpriority>
	public struct SizeF {
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.SizeF"></see> class.</summary>
		/// <filterpriority>1</filterpriority>
		public static readonly SizeF Empty;
		private float width;
		private float height;
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.SizeF"></see> class from the specified existing <see cref="T:System.Drawing.SizeF"></see>.</summary>
		/// <param name="size">The <see cref="T:System.Drawing.SizeF"></see> from which to create the new <see cref="T:System.Drawing.SizeF"></see>. </param>
		public SizeF(SizeF size) {
			this.width = size.width;
			this.height = size.height;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.SizeF"></see> class from the specified Vector2.</summary>
		/// <param name="pt">The Vector2 from which to initialize this <see cref="T:System.Drawing.SizeF"></see>. </param>
		public SizeF(Vector2 pt) {
			this.width = pt.X;
			this.height = pt.Y;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.SizeF"></see> class from the specified dimensions.</summary>
		/// <param name="width">The width component of the new <see cref="T:System.Drawing.SizeF"></see>. </param>
		/// <param name="height">The height component of the new <see cref="T:System.Drawing.SizeF"></see>. </param>
		public SizeF(float width, float height) {
			this.width = width;
			this.height = height;
		}

		/// <summary>Adds the width and height of one <see cref="T:System.Drawing.SizeF"></see> structure to the width and height of another <see cref="T:System.Drawing.SizeF"></see> structure.</summary>
		/// <returns>A <see cref="T:System.Drawing.Size"></see> structure that is the result of the addition operation.</returns>
		/// <param name="sz2">The second <see cref="T:System.Drawing.SizeF"></see> to add. </param>
		/// <param name="sz1">The first <see cref="T:System.Drawing.SizeF"></see> to add. </param>
		/// <filterpriority>3</filterpriority>
		public static SizeF operator +(SizeF sz1, SizeF sz2) {
			return SizeF.Add(sz1, sz2);
		}

		/// <summary>Subtracts the width and height of one <see cref="T:System.Drawing.SizeF"></see> structure from the width and height of another <see cref="T:System.Drawing.SizeF"></see> structure.</summary>
		/// <returns>A <see cref="T:System.Drawing.SizeF"></see> that is the result of the subtraction operation.</returns>
		/// <param name="sz2">The <see cref="T:System.Drawing.SizeF"></see> on the right side of the subtraction operator. </param>
		/// <param name="sz1">The <see cref="T:System.Drawing.SizeF"></see> on the left side of the subtraction operator. </param>
		/// <filterpriority>3</filterpriority>
		public static SizeF operator -(SizeF sz1, SizeF sz2) {
			return SizeF.Subtract(sz1, sz2);
		}

		/// <summary>Tests whether two <see cref="T:System.Drawing.SizeF"></see> structures are equal.</summary>
		/// <returns>This operator returns true if sz1 and sz2 have equal width and height; otherwise, false.</returns>
		/// <param name="sz2">The <see cref="T:System.Drawing.SizeF"></see> structure on the right of the equality operator. </param>
		/// <param name="sz1">The <see cref="T:System.Drawing.SizeF"></see> structure on the left side of the equality operator. </param>
		/// <filterpriority>3</filterpriority>
		public static bool operator ==(SizeF sz1, SizeF sz2) {
			if(sz1.Width == sz2.Width) {
				return (sz1.Height == sz2.Height);
			}
			return false;
		}

		/// <summary>Tests whether two <see cref="T:System.Drawing.SizeF"></see> structures are different.</summary>
		/// <returns>This operator returns true if sz1 and sz2 differ either in width or height; false if sz1 and sz2 are equal.</returns>
		/// <param name="sz2">The <see cref="T:System.Drawing.SizeF"></see> structure on the right of the inequality operator. </param>
		/// <param name="sz1">The <see cref="T:System.Drawing.SizeF"></see> structure on the left of the inequality operator. </param>
		/// <filterpriority>3</filterpriority>
		public static bool operator !=(SizeF sz1, SizeF sz2) {
			return !(sz1 == sz2);
		}

		public static explicit operator Vector2(SizeF size) {
			return new Vector2(size.Width, size.Height);
		}

		/// <summary>Gets a value indicating whether this <see cref="T:System.Drawing.SizeF"></see> has zero width and height.</summary>
		/// <returns>This property returns true when this <see cref="T:System.Drawing.SizeF"></see> has both a width and height of zero; otherwise, false.</returns>
		/// <filterpriority>1</filterpriority>
		public bool IsEmpty {
			get {
				if(this.width == 0f) {
					return (this.height == 0f);
				}
				return false;
			}
		}
		/// <summary>Gets or sets the horizontal component of this <see cref="T:System.Drawing.SizeF"></see>.</summary>
		/// <returns>The horizontal component of this <see cref="T:System.Drawing.SizeF"></see>.</returns>
		/// <filterpriority>1</filterpriority>
		public float Width {
			get {
				return this.width;
			}
			set {
				this.width = value;
			}
		}
		/// <summary>Gets or sets the vertical component of this <see cref="T:System.Drawing.SizeF"></see>.</summary>
		/// <returns>The vertical component of this <see cref="T:System.Drawing.SizeF"></see>.</returns>
		/// <filterpriority>1</filterpriority>
		public float Height {
			get {
				return this.height;
			}
			set {
				this.height = value;
			}
		}
		/// <summary>Adds the width and height of one <see cref="T:System.Drawing.SizeF"></see> structure to the width and height of another <see cref="T:System.Drawing.SizeF"></see> structure.</summary>
		/// <returns>A <see cref="T:System.Drawing.SizeF"></see> structure that is the result of the addition operation.</returns>
		/// <param name="sz2">The second <see cref="T:System.Drawing.SizeF"></see> to add.</param>
		/// <param name="sz1">The first <see cref="T:System.Drawing.SizeF"></see> to add.</param>
		public static SizeF Add(SizeF sz1, SizeF sz2) {
			return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		/// <summary>Subtracts the width and height of one <see cref="T:System.Drawing.SizeF"></see> structure from the width and height of another <see cref="T:System.Drawing.SizeF"></see> structure.</summary>
		/// <returns>The <see cref="T:System.Drawing.SizeF"></see> that is a result of the subtraction operation.</returns>
		/// <param name="sz2">The <see cref="T:System.Drawing.SizeF"></see> structure on the right side of the subtraction operator. </param>
		/// <param name="sz1">The <see cref="T:System.Drawing.SizeF"></see> structure on the left side of the subtraction operator. </param>
		public static SizeF Subtract(SizeF sz1, SizeF sz2) {
			return new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		/// <summary>Tests to see whether the specified object is a <see cref="T:System.Drawing.SizeF"></see> with the same dimensions as this <see cref="T:System.Drawing.SizeF"></see>.</summary>
		/// <returns>This method returns true if obj is a <see cref="T:System.Drawing.SizeF"></see> and has the same width and height as this <see cref="T:System.Drawing.SizeF"></see>; otherwise, false.</returns>
		/// <param name="obj">The <see cref="T:System.Object"></see> to test. </param>
		/// <filterpriority>1</filterpriority>
		public override bool Equals(object obj) {
			if(obj is SizeF) {
				SizeF ef1 = (SizeF)obj;
				if((ef1.Width == this.Width) && (ef1.Height == this.Height)) {
					return ef1.GetType().Equals(base.GetType());
				}
			}
			return false;
		}

		/// <summary>Returns a hash code for this <see cref="T:System.Drawing.Size"></see> structure.</summary>
		/// <returns>An integer value that specifies a hash value for this <see cref="T:System.Drawing.Size"></see> structure.</returns>
		/// <filterpriority>1</filterpriority>
		public override int GetHashCode() {
			return base.GetHashCode();
		}

		/// <summary>Converts a <see cref="T:System.Drawing.SizeF"></see> to a Vector2 .</summary>
		/// <returns>Returns a Vector2 structure.</returns>
		/// <filterpriority>1</filterpriority>
		public Vector2 ToVector2() {
			return (Vector2)this;
		}

		/// <summary>Converts a <see cref="T:System.Drawing.SizeF"></see> to a <see cref="T:System.Drawing.Size"></see>.</summary>
		/// <returns>Returns a <see cref="T:System.Drawing.Size"></see> structure.</returns>
		/// <filterpriority>1</filterpriority>
		public Size ToSize() {
			return Size.Truncate(this);
		}

		/// <summary>Creates a human-readable string that represents this <see cref="T:System.Drawing.SizeF"></see>.</summary>
		/// <returns>A string that represents this <see cref="T:System.Drawing.SizeF"></see>.</returns>
		/// <filterpriority>1</filterpriority>
		/// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" /></PermissionSet>
		public override string ToString() {
			return ("{Width=" + this.width.ToString(CultureInfo.CurrentCulture) + ", Height=" + this.height.ToString(CultureInfo.CurrentCulture) + "}");
		}

		static SizeF() {
			SizeF.Empty = new SizeF();
		}

	}


}