using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace pr2.Common {

	//a 20.12 fixed point class. We could use T4 to make other precisions if we wanted to
	public struct Fx32
	{
		const int FRACTION_BITS = 12;
		const int MASK = (1<<FRACTION_BITS)-1;
		public static int fractionBits() { return FRACTION_BITS; }

		Fx32(int mVal) { this.mVal = mVal; }

		public override string ToString()
		{
			return toFloat().ToString();
		}

		public static readonly Fx32 ZERO = new Fx32(0);
		public static readonly Fx32 ONE = new Fx32(1 << FRACTION_BITS);
		public static readonly Fx32 HALF = new Fx32((1 << FRACTION_BITS-1));
		public static readonly Fx32 THIRD = new Fx32((1 << FRACTION_BITS)/3);
		public static readonly Fx32 MAX = new Fx32(int.MaxValue);
		public static readonly Fx32 MIN = new Fx32(int.MinValue);
		public static readonly Fx32 EPSILON = new Fx32(1);

		public override int GetHashCode() { return mVal.GetHashCode(); }
		public override bool Equals(object obj) { return ((Fx32)obj).mVal == mVal; }

		public static implicit operator Fx32(int val) { return new Fx32(val << FRACTION_BITS); }
		public static implicit operator Fx32(double val) { return new Fx32((int)(val * (double)(1 << FRACTION_BITS))); }
		public static implicit operator Fx32(float val) { return new Fx32((int)(val * (float)(1 << FRACTION_BITS))); }

		public static Fx32 operator<<(Fx32 lhs, int rhs) { return new Fx32(lhs.mVal << rhs); }
		public static Fx32 operator>>(Fx32 lhs, int rhs) { return new Fx32(lhs.mVal >> rhs); }

		public static bool operator==(Fx32 lhs, Fx32 rhs) { return lhs.mVal == rhs.mVal; }
		public static bool operator!=(Fx32 lhs, Fx32 rhs) { return lhs.mVal != rhs.mVal; }
		public static bool operator<(Fx32 lhs, Fx32 rhs) { return lhs.mVal < rhs.mVal; }
		public static bool operator<=(Fx32 lhs, Fx32 rhs) { return lhs.mVal <= rhs.mVal; }
		public static bool operator>(Fx32 lhs, Fx32 rhs) { return lhs.mVal > rhs.mVal; }
		public static bool operator>=(Fx32 lhs, Fx32 rhs) { return lhs.mVal >= rhs.mVal; }

		public static Fx32 operator~(Fx32 lhs) { return new Fx32(~lhs.mVal); }

		public static Fx32 operator^(Fx32 lhs, Fx32 rhs) { return new Fx32(lhs.mVal ^ rhs.mVal); }
		public static Fx32 operator&(Fx32 lhs, Fx32 rhs) { return new Fx32(lhs.mVal & rhs.mVal); }
		public static Fx32 operator|(Fx32 lhs, Fx32 rhs) { return new Fx32(lhs.mVal | rhs.mVal); }

		public static Fx32 operator+(Fx32 lhs, Fx32 rhs) { return new Fx32(lhs.mVal + rhs.mVal); }
		public static Fx32 operator-(Fx32 lhs) { return new Fx32(-lhs.mVal); }
		public static Fx32 operator-(Fx32 lhs, Fx32 rhs) { return new Fx32(lhs.mVal - rhs.mVal); }

		public static Fx32 operator ++(Fx32 lhs) { return lhs + 1; }
		public static Fx32 operator --(Fx32 lhs) { return lhs - 1; }

		public static Fx32 operator*(Fx32 lhs, Fx32 rhs) {
			return new Fx32((int)(((long)(lhs.mVal) * rhs.mVal + 0x800L) >> 12));
		}

		public static Fx32 operator/(Fx32 lhs, Fx32 rhs) {
			int dropPrecision = FRACTION_BITS / 2;
			int left = lhs.mVal << dropPrecision;
			int right = rhs.mVal >> dropPrecision;
			return new Fx32(left/right);
		}


		public Fx32 Fraction() { return new Fx32(mVal & MASK); }
		public int toInt() { return mVal >> FRACTION_BITS; }
		public float toFloat() { return (float)mVal / (1 << FRACTION_BITS); }
		public static Fx32 noConvert(int mVal) { return new Fx32(mVal); }
		public Fx32 Abs() { return this < 0 ? 0 - this : this; }
		public static Fx32 Max(Fx32 a, Fx32 b) { return a > b ? a : b; }
		public static Fx32 Min(Fx32 a, Fx32 b) { return a < b ? a : b; }

		public int mVal;

		public static Fx32 Read(BinaryReader br) { return new Fx32(br.ReadInt32()); }
		public void Write(BinaryWriter bw) { bw.Write(mVal); }

		public Fx32 Interpolate(Fx32 param, Fx32 target)
		{
			Fx32 diff = target - this;
			return this + diff * param;
		}
	}


	public struct FxVector3
	{
		public Fx32 x, y, z;
		public FxVector3(Fx32 x, Fx32 y) { this.x = x; this.y = y; this.z = 0; }
		public void Set(Fx32 x, Fx32 y) { this.x = x; this.y = y; this.z = 0; }
		public FxVector3(Fx32 x, Fx32 y, Fx32 z) { this.x = x; this.y = y; this.z = z; }
		public void Set(FxVector3 other) { x = other.x; y = other.y; z = other.z; }
		public FxVector3 ToInt() { return new FxVector3(x.toInt(), y.toInt(), z.toInt()); }
		public static FxVector3 operator -(FxVector3 lhs) { return new FxVector3(-lhs.x, -lhs.y, -lhs.z); }
		public static FxVector3 operator -(FxVector3 lhs, FxVector3 rhs) { return new FxVector3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z); }
		public static FxVector3 operator +(FxVector3 lhs, FxVector3 rhs) { return new FxVector3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z); }
		public static FxVector3 operator /(FxVector3 lhs, Fx32 rhs) { return new FxVector3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs); }
		public static FxVector3 operator *(FxVector3 lhs, Fx32 rhs) { return new FxVector3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs); }
		public Fx32 Length()
		{
			float fx = x.toFloat();
			float fy = y.toFloat();
			float fz = z.toFloat();
			return (Fx32)Math.Sqrt(fx * fx + fy * fy + fz * fz);
		}
		public Fx32 LengthSquared()
		{
			return x * x + y * y + z * z;
		}
		public FxVector3 Interpolate(Fx32 param, FxVector3 target)
		{
			FxVector3 diff = target - this;
			return this + diff * param;
		}
		public static bool TwoComponentPrint = true;
		public static bool operator ==(FxVector3 lhs, FxVector3 rhs) { return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z; }
		public static bool operator !=(FxVector3 lhs, FxVector3 rhs) { return !(lhs == rhs); }
		public override int GetHashCode() { return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode(); }
		public override bool Equals(object obj) { return ((FxVector3)obj) == this; }
		public bool IsZero() { return (x == 0 && y == 0 && z == 0); }
		public override string ToString()
		{
			if (TwoComponentPrint)
				return string.Format("({0},{1})", x, y);
			else
				return string.Format("TBD");
		}
	}

}
