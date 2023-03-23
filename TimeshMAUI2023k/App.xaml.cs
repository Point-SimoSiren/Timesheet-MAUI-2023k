namespace TimeshMAUI2023k;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        MainPage = new NavigationPage(new EmployeePage());
    }
}
