using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloMauiToolkits;

partial class TapGameViewModel(TapCountService tapCountService, IDispatcher dispatcher) : BaseViewModel
{
	const int maximumTimerSeconds = 5;
	const string gameButtonText_Start = "Start", gameButtonText_Tap = "Tap!";

	[ObservableProperty]
	int tapCount, highScore = tapCountService.TapCountHighScore, timerSecondsRemaining = maximumTimerSeconds;

	[ObservableProperty]
	string gameButtonText = gameButtonText_Start;

	public event EventHandler<GameEndedEventArgs>? GameEnded;

	[RelayCommand]
	void GameButtonTapped(string buttonText)
	{
		if (buttonText is gameButtonText_Start)
		{
			GameButtonText = gameButtonText_Tap;
			StartGame();
		}
		else if (buttonText is gameButtonText_Tap)
		{
			TapCount++;
		}
		else
		{
			throw new NotSupportedException("Invalid Game State");
		}
	}

	void StartGame()
	{
		var timer = dispatcher.CreateTimer();
		timer.Interval = TimeSpan.FromSeconds(1);

		timer.Tick += HandleTimerTicked;

		TapCount = 0;

		timer.Start();
	}

	void EndGame(int score)	
	{
		GameEnded?.Invoke(this, new GameEndedEventArgs(score));

		if (score > tapCountService.TapCountHighScore)
		{
			HighScore = tapCountService.TapCountHighScore = score;
		}
		
		TimerSecondsRemaining = maximumTimerSeconds;
		GameButtonText = gameButtonText_Start;
	}


	void HandleTimerTicked(object? sender, EventArgs e)
	{
		TimerSecondsRemaining--;

		if (TimerSecondsRemaining == 0)
		{
			var timer = sender as IDispatcherTimer;

			timer?.Stop();
			timer = null;

			EndGame(TapCount);
		}
	}
}