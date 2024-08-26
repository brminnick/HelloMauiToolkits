using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace HelloMauiToolkits;

class TapGamePage : BasePage<TapGameViewModel>
{
	readonly Label highScoreLabel;
	readonly TapCountService tapCountService;
	
	public TapGamePage(TapGameViewModel viewModel, TapCountService tapCountService) : base(viewModel)
	{
		this.tapCountService = tapCountService;

		BackgroundColor = Colors.White;

		viewModel.GameEnded += HandleGameEnded;

		Content = new Grid
		{
			RowSpacing = 12,
			
			RowDefinitions = Rows.Define(
				(Row.HighScore, 52),
				(Row.Description, 108),
				(Row.TapButton, Star),
				(Row.TapCounter, 32),
				(Row.Timer, 32)),
			
			Children =
			{
				new TapGameLabel(36)
					.Row(Row.HighScore)
					.Assign(out highScoreLabel)
					.Bind(Label.TextProperty, 
							static (TapGameViewModel vm) => vm.HighScore, 
							mode: BindingMode.OneWay, 
							convert: static number => $"High Score: {number}"),
				
				new TapGameLabel(24) { LineBreakMode =  LineBreakMode.WordWrap }
					.Row(Row.Description)
					.Text("How many taps can you get in 5 seconds???")
					.Font(italic: true),
				
				new ShadowButton()
					.Row(Row.TapButton)
					.BackgroundColor(ColorConstants.ButtonBackgroundColor)
					.TextColor(Colors.White)
					.Center()
					.Size(250,250)
					.Bind(Button.TextProperty, 
							static (TapGameViewModel vm) => vm.GameButtonText, 
							mode: BindingMode.OneWay)
					.BindCommand(
							static (TapGameViewModel vm) => vm.GameButtonTappedCommand, 
							commandBindingMode: BindingMode.OneTime, 
							parameterGetter: static (TapGameViewModel vm) => vm.GameButtonText, 
							parameterBindingMode: BindingMode.OneWay),
				
				new TapGameLabel(24)
					.Row(Row.TapCounter)
					.Bind(Label.TextProperty, 
							static (TapGameViewModel vm) => vm.TapCount),
				
				new TapGameLabel()
					.Row(Row.Timer)
					.Bind(Label.TextProperty, 
							static (TapGameViewModel vm) => vm.TimerSecondsRemaining,
							mode: BindingMode.OneWay,
							convert: static seconds => $"Seconds Remaining: {seconds}")
			}
		};
	}

	async void HandleGameEnded(object? gameViewModel, GameEndedEventArgs gameEndedEventArgs)
	{
		var isHighScore = gameEndedEventArgs.FinalScore > tapCountService.TapCountHighScore;
		var gameScoreEmoji = GameConstants.GetScoreEmoji(gameEndedEventArgs.FinalScore, tapCountService.TapCountHighScore);

		Popup popup = isHighScore switch
		{
			true => new GameEndedPopup("New High Score",
										gameEndedEventArgs.FinalScore,
										gameScoreEmoji),
			false => new GameEndedPopup("Game Over",
										gameEndedEventArgs.FinalScore,
										gameScoreEmoji)
		};
		
		popup.Closed += OnGameEndedPopupPopupClosed;

		await this.ShowPopupAsync(popup);

		async void OnGameEndedPopupPopupClosed(object? sender, PopupClosedEventArgs popupClosedEventArgs)
		{
			ArgumentNullException.ThrowIfNull(sender);
			
			var popup = (Popup)sender;
			popup.Closed -= OnGameEndedPopupPopupClosed;

			if (!isHighScore)
				return;
			
			await AnimateHighScoreColor(gameEndedEventArgs.FinalScore);
		}
	}

	async Task AnimateHighScoreColor(int highScore)
	{		
		var highScoreLabelOriginalTextColor = highScoreLabel.TextColor;

		var changeHighScoreLabelTextColorTask = highScoreLabel.TextColorTo(Colors.DarkGreen, length: 50);
		var scaleHighScoreLabelTask = highScoreLabel.ScaleTo(1.15, 110);
		var minimumAnimationTimeTask = Task.Delay(GameConstants.GameEndPopupDisplayTime);
		
		BindingContext.UpdateHighScoreCommand.Execute(highScore);
		
		await Task.WhenAll(changeHighScoreLabelTextColorTask, scaleHighScoreLabelTask);
		
		scaleHighScoreLabelTask = highScoreLabel.ScaleTo(1.0, 100);

		await Task.WhenAll(scaleHighScoreLabelTask, minimumAnimationTimeTask);
		
		changeHighScoreLabelTextColorTask = highScoreLabel.TextColorTo(highScoreLabelOriginalTextColor, length: 500);

		await changeHighScoreLabelTextColorTask;
	}

	enum Row { HighScore, Description, TapButton, TapCounter, Timer }

	sealed class TapGameLabel : Label
	{
		public TapGameLabel(double? fontSize = null)
		{
			this.Center()
				.TextCenter()
				.TextColor(ColorConstants.TapGameLabelTextColor)
				.Font(size: fontSize, bold: true);
		}
	}

	sealed class ShadowButton : Button
	{
		public ShadowButton()
		{
			Shadow = new Shadow
			{
				Brush = Colors.Black,
				Radius = 8,
				Opacity = 0.8f
			};
			
			BorderWidth = 8;
			BorderColor = ColorConstants.ButtonBorderColor;

			Clicked += HandleClicked;
		}
		
		async void HandleClicked(object? sender, EventArgs e)
		{
			await this.ScaleTo(1.1, 100);
			await this.ScaleTo(1.0, 100);
		}
	}
}