using BookReader.Mobile.Pages;

namespace BookReader.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(nameof(LibraryPage), typeof(LibraryPage));
	}
}
