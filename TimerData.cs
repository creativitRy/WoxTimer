using System;

// ReSharper disable once CheckNamespace
namespace ctRy.WoxTimer
{
	public struct TimerData : IEquatable<TimerData>, IComparable<TimerData>, IComparable
	{
		private static int NextId;
		public readonly int Id;
		public readonly DateTime Time;

		public TimerData(DateTime time)
		{
			Id = NextId;
			NextId++;
			Time = time;
		}

		public bool Equals(TimerData other)
		{
			return Id == other.Id && Time.Equals(other.Time);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			return obj.GetType() == GetType() && Equals((TimerData) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Id * 397) ^ Time.GetHashCode();
			}
		}

		public static bool operator ==(TimerData left, TimerData right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TimerData left, TimerData right)
		{
			return !Equals(left, right);
		}

		public int CompareTo(TimerData other)
		{
			var timeComparison = Time.CompareTo(other.Time);
			if (timeComparison != 0)
				return timeComparison;
			return Id.CompareTo(other.Id);
		}

		public int CompareTo(object obj)
		{
			if (ReferenceEquals(null, obj))
				return 1;
			return obj is TimerData other
				? CompareTo(other)
				: throw new ArgumentException($"Object must be of type {nameof(TimerData)}");
		}

		public static bool operator <(TimerData left, TimerData right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator >(TimerData left, TimerData right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator <=(TimerData left, TimerData right)
		{
			return left.CompareTo(right) <= 0;
		}

		public static bool operator >=(TimerData left, TimerData right)
		{
			return left.CompareTo(right) >= 0;
		}
	}
}