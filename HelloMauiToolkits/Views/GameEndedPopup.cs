using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using LayoutAlignment = Microsoft.Maui.Primitives.LayoutAlignment;

namespace HelloMauiToolkits;

class GameEndedPopup : Popup
{
	public GameEndedPopup(string title, string description, string scoreEmoji)
	{
		const int titleFontSize = 32;
		const int descriptionFontSize = 24;
		const int scoreEmojiSize = 64;
		
		VerticalOptions = HorizontalOptions = LayoutAlignment.Center;
		
		Opened += HandleOpened;
		Color = Colors.Transparent;
		CanBeDismissedByTappingOutsideOfPopup = false;

		Content = new Border
		{
			BackgroundColor = ColorConstants.PopupBackgroundColor,
			StrokeThickness = 16,
			Stroke = ColorConstants.PopupBorderColor,
#if Android
			StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(40) },
#endif
			
			Content = new VerticalStackLayout
			{
				Spacing = 12,
				Children =
				{
					new Label()
						.Center()
						.TextCenter()
						.Font(size: titleFontSize, bold: true)
						.Text(title, Colors.Black),

					new Label()
						.Center()
						.TextCenter()
						.Font(size: descriptionFontSize)
						.Text(description, Colors.Black)
						.Assign(out Label descriptionLabel),
					
					new Label()
						.Text(scoreEmoji, Colors.Black)
						.Center()
						.TextCenter()
						.Font(size: scoreEmojiSize)
						.Bind(Label.HeightRequestProperty, 
								static (Label descriptionLabel) => descriptionLabel.Height,
								convert: (double descriptionLabelHeight) => 250 - descriptionLabelHeight,
								source: descriptionLabel)
				}
			}.Size(250, -1)
			 .Padding(24)
		};
	}

	async void HandleOpened(object? sender, PopupOpenedEventArgs e)
	{
		await Task.Delay(TimeSpan.FromSeconds(3));
		await CloseAsync();
	}
}