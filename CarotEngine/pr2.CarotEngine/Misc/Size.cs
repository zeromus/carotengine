using System;
using System.Globalization;

using Microsoft.Xna.Framework;

namespace pr2.CarotEngine {
		
	/// <summary>Stores an ordered pair of integers, typically the width and height of a rectangle.</summary>
		/// <filterpriority>1</filterpriority>
		public struct Size {
			/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Size"></see> class.</summary>
			/// <filterpriority>1</filterpriority>
			public static readonly Size Empty;
			private int width;
			private int height;
			/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Size"></see> class from the specified <see cref="T:System.Drawing.Point"></see>.</summary>
			/// <param name="pt">The <see cref="T:System.Drawing.Point"></see> from which to initialize this <see cref="T:System.Drawing.Size"></see>. </param>
			public Size(Point pt) {
				this.width = pt.X;
				this.height = pt.Y;
			}

			/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Size"></see> class from the specified dimensions.</summary>
			/// <param name="width">The width component of the new <see cref="T:System.Drawing.Size"></see>. </param>
			/// <param name="height">The height component of the new <see cref="T:System.Drawing.Size"></see>. </param>
			public Size(int width, int height) {
				this.width = width;
				this.height = height;
			}

			public static implicit operator SizeF(Size p) {
				return new SizeF((float)p.Width, (float)p.Height);
			}

			/// <summary>Adds the width and height of one <see cref="T:System.Drawing.Size"></see> structure to the width and height of another <see cref="T:System.Drawing.Size"></see> structure.</summary>
			/// <returns>A <see cref="T:System.Drawing.Size"></see> structure that is the result of the addition operation.</returns>
			/// <param name="sz2">The second <see cref="T:System.Drawing.Size"></see> to add. </param>
			/// <param name="sz1">The first <see cref="T:System.Drawing.Size"></see> to add. </param>
			/// <filterpriority>3</filterpriority>
			public static Size operator +(Size sz1, Size sz2) {
				return Size.Add(sz1, sz2);
			}

			/// <summary>Subtracts the width and height of one <see cref="T:System.Drawing.Size"></see> structure from the width and height of another <see cref="T:System.Drawing.Size"></see> structure.</summary>
			/// <returns>A <see cref="T:System.Drawing.Size"></see> structure that is the result of the subtraction operation.</returns>
			/// <param name="sz2">The <see cref="T:System.Drawing.Size"></see> structure on the right side of the subtraction operator. </param>
			/// <param name="sz1">The <see cref="T:System.Drawing.Size"></see> structure on the left side of the subtraction operator. </param>
			/// <filterpriority>3</filterpriority>
			public static Size operator -(Size sz1, Size sz2) {
				return Size.Subtract(sz1, sz2);
			}

			/// <summary>Tests whether two <see cref="T:System.Drawing.Size"></see> structures are equal.</summary>
			/// <returns>true if sz1 and sz2 have equal width and height; otherwise, false.</returns>
			/// <param name="sz2">The <see cref="T:System.Drawing.Size"></see> structure on the right of the equality operator. </param>
			/// <param name="sz1">The <see cref="T:System.Drawing.Size"></see> structure on the left side of the equality operator. </param>
			/// <filterpriority>3</filterpriority>
			public static bool operator ==(Size sz1, Size sz2) {
				if(sz1.Width == sz2.Width) {
					return (sz1.Height == sz2.Height);
				}
				return false;
			}

			/// <summary>Tests whether two <see cref="T:System.Drawing.Size"></see> structures are different.</summary>
			/// <returns>true if sz1 and sz2 differ either in width or height; false if sz1 and sz2 are equal.</returns>
			/// <param name="sz2">The <see cref="T:System.Drawing.Size"></see> structure on the right of the inequality operator. </param>
			/// <param name="sz1">The <see cref="T:System.Drawing.Size"></see> structure on the left of the inequality operator. </param>
			/// <filterpriority>3</filterpriority>
			public static bool operator !=(Size sz1, Size sz2) {
				return !(sz1 == sz2);
			}

			public static explicit operator Point(Size size) {
				return new Point(size.Width, size.Height);
			}

			/// <summary>Tests whether this <see cref="T:System.Drawing.Size"></see> has width and height of 0.</summary>
			/// <returns>This property returns true when this <see cref="T:System.Drawing.Size"></see> has both a width and height of 0; otherwise, false.</returns>
			/// <filterpriority>1</filterpriority>
			public bool IsEmpty {
				get {
					if(this.width == 0) {
						return (this.height == 0);
					}
					return false;
				}
			}
			/// <summary>Gets or sets the horizontal component of this <see cref="T:System.Drawing.Size"></see>.</summary>
			/// <returns>The horizontal component of this <see cref="T:System.Drawing.Size"></see>.</returns>
			/// <filterpriority>1</filterpriority>
			public int Width {
				get {
					return this.width;
				}
				set {
					this.width = value;
				}
			}
			/// <summary>Gets or sets the vertical component of this <see cref="T:System.Drawing.Size"></see>.</summary>
			/// <returns>The vertical component of this <see cref="T:System.Drawing.Size"></see>.</returns>
			/// <filterpriority>1</filterpriority>
			public int Height {
				get {
					return this.height;
				}
				set {
					this.height = value;
				}
			}
			/// <summary>Adds the width and height of one <see cref="T:System.Drawing.Size"></see> structure to the width and height of another <see cref="T:System.Drawing.Size"></see> structure.</summary>
			/// <returns>A <see cref="T:System.Drawing.Size"></see> structure that is the result of the addition operation.</returns>
			/// <param name="sz2">The second <see cref="T:System.Drawing.Size"></see> to add.</param>
			/// <param name="sz1">The first <see cref="T:System.Drawing.Size"></see> to add.</param>
			public static Size Add(Size sz1, Size sz2) {
				return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
			}

			/// <summary>Converts the specified <see cref="T:System.Drawing.SizeF"></see> structure to a <see cref="T:System.Drawing.Size"></see> structure by rounding the values of the <see cref="T:System.Drawing.Size"></see> structure to the next higher integer values.</summary>
			/// <returns>The <see cref="T:System.Drawing.Size"></see> structure this method converts to.</returns>
			/// <param name="value">The <see cref="T:System.Drawing.SizeF"></see> structure to convert. </param>
			/// <filterpriority>1</filterpriority>
			public static Size Ceiling(SizeF value) {
				return new Size((int)Math.Ceiling((double)value.Width), (int)Math.Ceiling((double)value.Height));
			}

			/// <summary>Subtracts the width and height of one <see cref="T:System.Drawing.Size"></see> structure from the width and height of another <see cref="T:System.Drawing.Size"></see> structure.</summary>
			/// <returns>The <see cref="T:System.Drawing.Size"></see> that is a result of the subtraction operation.</returns>
			/// <param name="sz2">The <see cref="T:System.Drawing.Size"></see> structure on the right side of the subtraction operator. </param>
			/// <param name="sz1">The <see cref="T:System.Drawing.Size"></see> structure on the left side of the subtraction operator. </param>
			public static Size Subtract(Size sz1, Size sz2) {
				return new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
			}

			/// <summary>Converts the specified <see cref="T:System.Drawing.SizeF"></see> structure to a <see cref="T:System.Drawing.Size"></see> structure by truncating the values of the <see cref="T:System.Drawing.SizeF"></see> structure to the next lower integer values.</summary>
			/// <returns>The <see cref="T:System.Drawing.Size"></see> structure this method converts to.</returns>
			/// <param name="value">The <see cref="T:System.Drawing.SizeF"></see> structure to convert. </param>
			/// <filterpriority>1</filterpriority>
			public static Size Truncate(SizeF value) {
				return new Size((int)value.Width, (int)value.Height);
			}

			/// <summary>Converts the specified <see cref="T:System.Drawing.SizeF"></see> structure to a <see cref="T:System.Drawing.Size"></see> structure by rounding the values of the <see cref="T:System.Drawing.SizeF"></see> structure to the nearest integer values.</summary>
			/// <returns>The <see cref="T:System.Drawing.Size"></see> structure this method converts to.</returns>
			/// <param name="value">The <see cref="T:System.Drawing.SizeF"></see> structure to convert. </param>
			/// <filterpriority>1</filterpriority>
			public static Size Round(SizeF value) {
				return new Size((int)Math.Round((double)value.Width), (int)Math.Round((double)value.Height));
			}

			/// <summary>Tests to see whether the specified object is a <see cref="T:System.Drawing.Size"></see> with the same dimensions as this <see cref="T:System.Drawing.Size"></see>.</summary>
			/// <returns>true if obj is a <see cref="T:System.Drawing.Size"></see> and has the same width and height as this <see cref="T:System.Drawing.Size"></see>; otherwise, false.</returns>
			/// <param name="obj">The <see cref="T:System.Object"></see> to test. </param>
			/// <filterpriority>1</filterpriority>
			public override bool Equals(object obj) {
				if(obj is Size) {
					Size size1 = (Size)obj;
					if(size1.width == this.width) {
						return (size1.height == this.height);
					}
				}
				return false;
			}

			/// <summary>Returns a hash code for this <see cref="T:System.Drawing.Size"></see> structure.</summary>
			/// <returns>An integer value that specifies a hash value for this <see cref="T:System.Drawing.Size"></see> structure.</returns>
			/// <filterpriority>1</filterpriority>
			public override int GetHashCode() {
				return (this.width ^ this.height);
			}

			/// <summary>Creates a human-readable string that represents this <see cref="T:System.Drawing.Size"></see>.</summary>
			/// <returns>A string that represents this <see cref="T:System.Drawing.Size"></see>.</returns>
			/// <filterpriority>1</filterpriority>
			/// <PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode" /></PermissionSet>
			public override string ToString() {
				return ("{Width=" + this.width.ToString(CultureInfo.CurrentCulture) + ", Height=" + this.height.ToString(CultureInfo.CurrentCulture) + "}");
			}

			static Size() {
				Size.Empty = new Size();
			}

		}
	}
