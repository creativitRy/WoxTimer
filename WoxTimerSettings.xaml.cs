using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Forms;
using UserControl = System.Windows.Controls.UserControl;

// ReSharper disable once CheckNamespace
namespace ctRy.WoxTimer
{
	public partial class WoxTimerSettings : UserControl
	{
		private readonly Settings _settings;

		public WoxTimerSettings(Settings settings)
		{
			InitializeComponent();

			_settings = settings;

			UseCustomAlarmTone.IsChecked = _settings.UseCustomAlarmTone;
			AlarmTonePath.Text = _settings.AlarmTonePath ?? "";
			OnEdit(_settings.UseCustomAlarmTone);
		}

		private void WoxTimerSettings_OnLoaded(object sender, RoutedEventArgs re)
		{
			UseCustomAlarmTone.IsChecked = _settings.UseCustomAlarmTone;
			AlarmTonePath.Text = _settings.AlarmTonePath;

			UseCustomAlarmTone.Checked += (o, e) =>
			{
				_settings.UseCustomAlarmTone = true;
				OnEdit(true);
				_settings.Save();
			};
			UseCustomAlarmTone.Unchecked += (o, e) =>
			{
				_settings.UseCustomAlarmTone = false;
				OnEdit(false);
				_settings.Save();
			};
			AlarmTonePath.TextChanged += (o, e) =>
			{
				_settings.AlarmTonePath = AlarmTonePath.Text;
				_settings.Save();
			};
			PickPath.Click += (o, e) =>
			{
				var fileChooser = new OpenFileDialog
				{
					Title = "Select alarm tone sound file",
					Multiselect = false,
					Filter = "Sound files (*.wav)|*.wav|All files (*.*)|*.*"
				};

				if (!string.IsNullOrWhiteSpace(AlarmTonePath.Text))
				{
					try
					{
						var dir = new FileInfo(AlarmTonePath.Text).Directory;
						if (dir != null && dir.Exists)
						{
							fileChooser.InitialDirectory = dir.FullName;
						}
					}
					catch (DirectoryNotFoundException)
					{
					}
					catch (PathTooLongException)
					{
					}
					catch (SecurityException)
					{
					}
				}

				if (fileChooser.ShowDialog() == DialogResult.OK)
				{
					_settings.AlarmTonePath = fileChooser.FileName;
					AlarmTonePath.Text = _settings.AlarmTonePath;
					_settings.Save();
				}
			};
		}

		private void OnEdit(bool value)
		{
			AlarmTonePath.IsEnabled = value;
			PickPath.IsEnabled = value;
		}
	}
}