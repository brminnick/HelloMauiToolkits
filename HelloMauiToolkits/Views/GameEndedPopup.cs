using CommunityToolkit.Maui.Markup;
using CommunityToolkit.Maui.Views;

namespace HelloMauiToolkits;

class GameEndedPopup : Popup
{
	public GameEndedPopup(string title, string description)
	{
		CanBeDismissedByTappingOutsideOfPopup = false;
		
		Content  = new VerticalStackLayout
		{
			Spacing = 12,
			
			Children =
			{
				new Label()
					.Center()
					.TextCenter()
					.Font(size: 32, bold: true)
					.Text(title),
				
				new Label()
					.Center()
					.TextCenter()
					.Text(description)
			}
		};
	}
}