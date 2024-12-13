using System.Diagnostics;

namespace HelloMauiToolkits;

abstract partial class BasePage<TViewModel>(TViewModel viewModel) : BasePage(viewModel) where TViewModel : BaseViewModel
{
	protected new TViewModel BindingContext => (TViewModel)base.BindingContext;
}

abstract class BasePage : ContentPage
{
	protected BasePage(object? viewModel = null)
	{
		BindingContext = viewModel;
		Padding = 12;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		Trace.WriteLine($"OnAppearing: {Title}");
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();

		Trace.WriteLine($"OnDisappearing: {Title}");
	}
}