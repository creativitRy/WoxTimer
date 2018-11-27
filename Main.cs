using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Wox.Plugin;

// ReSharper disable once CheckNamespace
namespace ctRy.WoxTimer
{
	public class Main : IPlugin
	{
		private PluginInitContext _context;
		private readonly SortedSet<TimerData> Entries = new SortedSet<TimerData>();

		public List<Result> Query(Query query)
		{
			var results = new List<Result>();
			if (query.Terms.Length < 2 || query.Terms[1].Equals("list", StringComparison.OrdinalIgnoreCase))
			{
				foreach (var timerData in Entries)
				{
					var time = timerData.Time - DateTime.Now;
					results.Add(new Result
					{
						Title = "Timer",
						SubTitle = time.ToString("g"),
						IcoPath = "Images\\icon.png",
						Action = e => true
					});
				}

				return results;
			}

			if (query.Terms[1].Equals("remove", StringComparison.OrdinalIgnoreCase))
			{
				foreach (var timerData in Entries)
				{
					var time = timerData.Time - DateTime.Now;
					results.Add(new Result
					{
						Title = "Timer",
						SubTitle = time.ToString("g"),
						IcoPath = "Images\\icon.png",
						Action = e =>
						{
							Entries.Remove(timerData);
							return true;
						}
					});
				}

				return results;
			}

			var timeRaw = query.Terms[1].Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries).Select(q => q.Trim())
				.ToArray();
			if (timeRaw.Length < 1 || timeRaw.Length > 3)
				return results;

			var times = new int[timeRaw.Length];
			for (var i = 0; i < timeRaw.Length; i++)
			{
				if (!int.TryParse(timeRaw[i], out var time))
					return results;
				times[i] = time;
			}

			if (times.Length == 3)
			{
				results.Add(GetResult(times[0], times[1], times[2]));
			}
			else if (times.Length == 2)
			{
				results.Add(GetResult(0, times[0], times[1]));
				results.Add(GetResult(times[0], times[1], 0));
			}
			else
			{
				results.Add(GetResult(0, 0, times[0]));
				results.Add(GetResult(0, times[0], 0));
				results.Add(GetResult(times[0], 0, 0));
			}

			return results;
		}

		private Result GetResult(int hours, int minutes, int seconds)
		{
			var sb = new StringBuilder();
			if (hours > 0)
				sb.Append(" " + hours + " hour" + (hours > 1 ? "s" : ""));
			if (minutes > 0)
				sb.Append(" " + minutes + " minute" + (minutes > 1 ? "s" : ""));
			if (seconds > 0)
				sb.Append(" " + seconds + " second" + (seconds > 1 ? "s" : ""));

			var formatted = sb.ToString().Substring(1);

			return new Result
			{
				Title = "Timer",
				SubTitle = "Rings in " + formatted,
				IcoPath = "Images\\icon.png",
				Action = e =>
				{
					var time = DateTime.Now + new TimeSpan(hours, minutes, seconds);
					var timerData = new TimerData(time);
					Entries.Add(timerData);
					var t = new Thread(x => Alarm(hours * 3600 + minutes * 60 + seconds, timerData));
					t.Start();
					_context.API.ShowMsg("Timer Started", $"Timer will ring at {time:G}", "Images\\icon.png");
					return true;
				}
			};
		}

		private void Alarm(int secs, TimerData timerData)
		{
			Thread.Sleep(secs * 1000);
			if (!Entries.Contains(timerData))
				return;
			_context.API.ShowMsg("Timer ended", "", "Images\\icon.png");
			System.Media.SystemSounds.Beep.Play();
			Thread.Sleep(1000);
			System.Media.SystemSounds.Beep.Play();
			Thread.Sleep(1000);
			System.Media.SystemSounds.Beep.Play();
			Entries.Remove(timerData);
		}

		public void Init(PluginInitContext context)
		{
			_context = context;
		}

		private struct TimerData : IEquatable<TimerData>, IComparable<TimerData>, IComparable
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
}