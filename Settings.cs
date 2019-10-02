// ReSharper disable once CheckNamespace

using System.IO;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace ctRy.WoxTimer
{
	public class Settings
	{
		public const string DefaultFileName = "settings.json";

		private readonly string _savePath;

		[JsonProperty]
		public bool UseCustomAlarmTone { get; set; }
		[JsonProperty]
		public string AlarmTonePath { get; set; }

		public Settings(string savePath)
		{
			_savePath = savePath;
		}

		public void Load()
		{
			if (!File.Exists(_savePath))
				return;

			var loaded = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_savePath));

			UseCustomAlarmTone = loaded.UseCustomAlarmTone;
			AlarmTonePath = loaded.AlarmTonePath;
		}

		public void Save()
		{
			File.WriteAllText(_savePath, JsonConvert.SerializeObject(this));
		}
	}
}