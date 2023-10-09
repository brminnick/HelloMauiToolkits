using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace HelloMauiToolkits;

class TapGamePage : BasePage<TapGameViewModel>
{
	readonly TapCountService tapCountService;
	
	public TapGamePage(TapGameViewModel viewModel, TapCountService tapCountService) : base(viewModel)
	{
		this.tapCountService = tapCountService;
		
		Title = "Tap Game";

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
				new Label()
					.Row(Row.HighScore)
					.Font(size: 36)
					.TextCenter()
					.Center()
					.Bind(Label.TextProperty, 
							static (TapGameViewModel vm) => vm.HighScore, 
							mode: BindingMode.OneWay, 
							convert: number => $"High Score: {number}"),
				
				new Label { LineBreakMode =  LineBreakMode.WordWrap }
					.Row(Row.Description)
					.Text("Start the game, then tap the button as many times as you can in 5 seconds!")
					.Font(size: 24, italic: true)
					.TextCenter()
					.Center(),
				
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
				
				new Label()
					.Row(Row.TapCounter)
					.Center()
					.TextCenter()
					.Font(size: 24, bold: true)
					.Bind(Label.TextProperty, 
							static (TapGameViewModel vm) => vm.TapCount),
				
				new Label()
					.Row(Row.Timer)
					.Center()
					.TextCenter()
					.Bind(Label.TextProperty, 
							static (TapGameViewModel vm) => vm.TimerSecondsRemaining,
							mode: BindingMode.OneWay,
							convert: seconds => $"Seconds Remaining: {seconds}")
			}
		};
	}
	
	async void HandleGameEnded(object? sender, GameEndedEventArgs e)
	{
		Popup popup = e.FinalScore switch
		{
			var score when (score > tapCountService.TapCountHighScore) => new GameEndedPopup("New High Score!", 
																									score.ToString(),
																									GameConstants.GetScoreEmoji(e.FinalScore, tapCountService.TapCountHighScore)),
			_ => new GameEndedPopup("Game Over",  
										$"You scored {e.FinalScore} points!",
										GameConstants.GetScoreEmoji(e.FinalScore, tapCountService.TapCountHighScore))
		};
		
		await this.ShowPopupAsync(popup);
	}

	enum Row { HighScore, Description, TapButton, TapCounter, Timer }

	class ShadowButton : Button
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