namespace HelloMauiToolkits;

sealed class App : Application
{
	public App(AppShell appShell)
	{
		MainPage = appShell;
	}
}