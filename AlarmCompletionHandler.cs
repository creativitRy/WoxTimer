using System;
using System.IO;
using System.Media;
using System.Threading;
using Wox.Plugin;

// ReSharper disable once CheckNamespace
namespace ctRy.WoxTimer
{
	public class AlarmCompletionHandler
	{
		private readonly IPublicAPI _api;
		private readonly Settings _settings;

		private readonly SoundPlayer _player;

		public AlarmCompletionHandler(IPublicAPI api, Settings settings)
		{
			_api = api;
			_settings = settings;

			_player = new SoundPlayer();
		}

		public void Complete()
		{
			_api.ShowMsg("Timer ended", "", "Images\\icon.png");

			if (_settings.UseCustomAlarmTone && !string.IsNullOrWhiteSpace(_settings.AlarmTonePath) &&
			    File.Exists(_settings.AlarmTonePath))
			{
				PlaySound();
			}
			else
			{
				DefaultBeeps();
			}
		}

		private void PlaySound()
		{
			// credits to LesterCovax for help and ayZagen for suggestion

			_player.SoundLocation = _settings.AlarmTonePath;
			try
			{
				_player.Play();
			}
			catch (Exception e)
			{
				_api.ShowMsg($"An exception has occured while attempting to play {_settings.AlarmTonePath}", e.Message,
					"Images\\icon.png");

				Console.Error.WriteLine(e);
			}
		}

		private static void DefaultBeeps()
		{
			// lol
			System.Media.SystemSounds.Beep.Play();
			Thread.Sleep(1000);
			System.Media.SystemSounds.Beep.Play();
			Thread.Sleep(1000);
			System.Media.SystemSounds.Beep.Play();
		}
	}
}