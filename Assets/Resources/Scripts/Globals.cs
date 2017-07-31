using UnityEngine;

namespace LD39
{
	public enum SPRITE_ORDER : int
	{
		FLOOR = 0,
		WALL = 1,
		WALL_TOP_TOP = 2,
		MIN_OBJ_LEVEL = 10,		// 0 + 10
		MAX_OBJ_LEVEL = 394,	// 382 (floor height) + 10
		WALL_TOP_SIDE = 999,
	}

	public static class Constants
	{
		public static int TOTAL_POWER_GENERATORS = 5;
		public static float expTick = 0.25f;
		public static float alertTick = 2.0f;
		public static float powerGenExpWorth = 0.5f;

		public static int expBallExpAmount = 5;
		public static int megaExpBallExpAmount = 10;

		public static int maxExpBallPerPowerGen = 3;

		public static float maxAlertAlhpa = 80;
	}

	namespace Utility
	{
		public static class Math
		{
			public static Bounds getIntersectingBounds(Bounds first, Bounds second)
			{
				Bounds bb = new Bounds();

				float minX = (first.min.x > second.min.x ? first.min.x : second.min.x);
				float minY = (first.min.y > second.min.y ? first.min.y : second.min.y);

				bb.min = new Vector3(minX, minY, 0);

				float sizeX, sizeY;

				if (first.min.x + first.size.x < second.min.x + second.size.x)
				{
					sizeX = first.min.x + first.size.x - bb.min.x;
				}
				else
				{
					sizeX = second.min.x + second.size.x - bb.min.x;
				}

				if (first.min.y + first.size.y < second.min.y + second.size.y)
				{
					sizeY = first.min.y + first.size.y - bb.min.y;
				}
				else
				{
					sizeY = second.min.y + second.size.y - bb.min.y;
				}

				bb.max = new Vector3(minX + sizeX, minY + sizeY);

				return bb;
			}

			public static int yPosToSortingOrder(float y)
			{
				int yInt = (int)y * -1;	// convert to int and flip.
				yInt += 114;        // 114 = floor height over y 0 in screen

				return yInt;
			}
		}
	}
}