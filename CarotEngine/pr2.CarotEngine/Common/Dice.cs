using System;


namespace pr2.Common {

public static class Dice {
	private static Random _die = new Random();

	public static int Roll(int maxExclusive) {
		return _die.Next(maxExclusive);
	}

	public static int Roll(int minInclusive, int maxExclusive) {
		return _die.Next(minInclusive, maxExclusive);
	}

	public static float RollF(float maxInclusive) {
		double lResults = _die.NextDouble() * maxInclusive;
		return (float)lResults;
	}

	public static float RollF() {
		return (float)_die.NextDouble();
	}

	public static float RollF(float minInclusive, float maxInclusive) {
		float lDifference = minInclusive - maxInclusive;
		double lResults = _die.NextDouble() * lDifference;

		return (float)(minInclusive + lResults);
	}

	public static bool RollTrueFalse() {
		return (Roll(0, 1) == 0);
	}
}

}
