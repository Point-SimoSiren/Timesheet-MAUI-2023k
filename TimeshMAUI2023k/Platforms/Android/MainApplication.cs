using Android.App;
using Android.Runtime;

namespace TimeshMAUI2023k;

#if DEBUG
[Application(UsesCleartextTraffic = true)]
//Yllä oleva (UsesCleartextTraffic = true) liittyy debuggaukseen local hostissa android emulaattorilla
#else
[Application]
#endif

public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
