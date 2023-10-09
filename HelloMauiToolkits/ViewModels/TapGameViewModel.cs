using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloMauiToolkits;

partial class TapGameViewModel(TapCountService tapCountService, IDispatcher dispatcher) : BaseViewModel
{
	[ObservableProperty]
	int tapCount, highScore = tapCountService.TapCountHighScore, timerSecondsRemaining = GameConstants.GameDuration.Seconds;

	[ObservableProperty]
	string gameButtonText = GameConstants.GameButtonText_Start;

	[ObservableProperty, NotifyCanExecuteChangedFor(nameof(GameButtonTappedCommand))]
	bool canGameButtonTappedCommandExecute = true;

	public event EventHandler<GameEndedEventArgs>? GameEnded;

	[RelayCommand(CanExecute = nameof(CanGameButtonTappedCommandExecute))]
	void GameButtonTapped(string buttonText)
	{
		if (buttonText is GameConstants.GameButtonText_Start)
		{
			GameButtonText = GameConstants.GameButtonText_Tap;
			StartGame();
		}
		else if (buttonText is GameConstants.GameButtonText_Tap)
		{
			TapCount++;
		}
		else
		{
			throw new NotSupportedException("Invalid Game State");
		}
	}

	[RelayCommand]
	void UpdateHighScore(int score)
	{
		tapCountService.TapCountHighScore = HighScore = score;
	}

	void StartGame()
	{
		var timer = dispatcher.CreateTimer();
		timer.Interval = TimeSpan.FromSeconds(1);

		timer.Tick += HandleTimerTicked;

		TapCount = 0;

		timer.Start();
	}

	async Task EndGame(int score)
	{
		try
		{
			CanGameButtonTappedCommandExecute = false;

			GameEnded?.Invoke(this, new GameEndedEventArgs(score));

			TimerSecondsRemaining = GameConstants.GameDuration.Seconds;
			GameButtonText = GameConstants.GameButtonText_Start;

			await Task.Delay(GameConstants.GameEndPopupDisplayTime.Seconds);
		}
		finally
		{
			CanGameButtonTappedCommandExecute = true;
		}
	}


	async void HandleTimerTicked(object? sender, EventArgs e)
	{
		TimerSecondsRemaining--;

		if (TimerSecondsRemaining == 0)
		{
			var timer = sender as IDispatcherTimer;

			timer?.Stop();
			timer = null;

			await EndGame(TapCount);
		}
	}
}