using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloMauiToolkits;

class TapGameViewModel : BaseViewModel
{
	readonly TapCountService tapCountService;
	readonly IDispatcher dispatcher;

	int tapCount, highScore, timerSecondsRemaining = GameConstants.GameDuration.Seconds;

	string gameButtonText = GameConstants.GameButtonText_Start;

	bool canGameButtonTappedCommandExecute = true;

	public TapGameViewModel(TapCountService tapCountService, IDispatcher dispatcher)
	{
		this.tapCountService = tapCountService;
		this.dispatcher = dispatcher;

		GameButtonTappedCommand = new Command<string>(GameButtonTapped, _ => CanGameButtonTappedCommandExecute);
		HighScore = tapCountService.TapCountHighScore;
	}

	public event EventHandler<GameEndedEventArgs>? GameEnded;

	public Command GameButtonTappedCommand { get; }

	public string GameButtonText
	{
		get => gameButtonText;
		set => SetProperty(ref gameButtonText, value);
	}

	public bool CanGameButtonTappedCommandExecute
	{
		get => canGameButtonTappedCommandExecute;
		set
		{
			if (SetProperty(ref canGameButtonTappedCommandExecute, value))
				GameButtonTappedCommand.ChangeCanExecute();
		}
	}

	public int TapCount
	{
		get => tapCount;
		set => SetProperty(ref tapCount, value);
	}

	public int HighScore
	{
		get => highScore;
		set => SetProperty(ref highScore, value);
	}

	public int TimerSecondsRemaining
	{
		get => timerSecondsRemaining;
		set => SetProperty(ref timerSecondsRemaining, value);
	}

	void GameButtonTapped(string? buttonText)
	{
		ArgumentNullException.ThrowIfNull(buttonText);

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