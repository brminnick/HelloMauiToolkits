namespace HelloMauiToolkits;

sealed class AppShell : Shell
{
	public AppShell(TapGamePage tapGamePage)
	{
		Items.Add(tapGamePage);
	}
}