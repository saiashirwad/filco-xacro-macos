using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using FilcoSetupApp.Properties;
using WixSharp.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: AssemblyTitle("FILCO Assist")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Diatec Corp.")]
[assembly: AssemblyProduct("FILCO Assist")]
[assembly: AssemblyCopyright("Copyright © 2024 I3D Technology Inc.")]
[assembly: AssemblyTrademark("")]
[assembly: ComVisible(false)]
[assembly: ThemeInfo(/*Could not decode attribute arguments.*/)]
[assembly: AssemblyFileVersion("2.0.97.0")]
[assembly: TargetFramework(".NETFramework,Version=v4.6.1", FrameworkDisplayName = ".NET Framework 4.6.1")]
[assembly: AssemblyVersion("2.0.97.0")]
namespace FilcoSetupApp
{
	public class App : Application
	{
		private bool _contentLoaded;

		public static string MsiFile { get; set; }

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			DoEvents();
			byte[] fILCO_Assist = Resources.FILCO_Assist;
			MsiFile = Path.Combine(Path.GetTempPath(), "FILCO Assist.msi");
			if (!File.Exists(MsiFile) || new FileInfo(MsiFile).Length != fILCO_Assist.Length)
			{
				File.WriteAllBytes(MsiFile, fILCO_Assist);
			}
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return Assembly.Load(Resources.WixSharp_Msi);
		}

		public static void DoEvents()
		{
			((DispatcherObject)Application.Current).Dispatcher.Invoke((DispatcherPriority)4, (Delegate)(Action)delegate
			{
			});
		}

		public static void DeleteTempWhenClose()
		{
			MsiFile = Path.Combine(Path.GetTempPath(), "FILCO Assist.msi");
			if (File.Exists(MsiFile))
			{
				File.Delete(MsiFile);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				((Application)this).Startup += new StartupEventHandler(Application_Startup);
				((Application)this).StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
				Uri uri = new Uri("/FILCOAssistSetup;component/app.xaml", UriKind.Relative);
				Application.LoadComponent((object)this, uri);
			}
		}

		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			((Application)app).Run();
		}
	}
	public class MainWindow : Window, IComponentConnector
	{
		public enum SetupPageID
		{
			kSPIDWelcome = 1,
			kSPIDInstalling,
			kSPIDFinish,
			kSPIDAlreadyInstall
		}

		private static string kWalcomeTitle = "FILCO Assist Installer";

		private static string kWalcomeContent1 = "Thank you for your purchase! \nFILCO Assist is FILCO keyboard support software that have many features.\nPlease read ";

		private static string kWalcomeContent2 = ", You must accept the terms of this agreement before continuing with the installation.";

		private static string kFinishContent1 = "FILCO Assist installation is complete!\n\n";

		private static string kFinishContent2 = "FILCO Assist is now available.";

		private static string kUpgradeContent1 = "This version already installed.";

		private static string kInstallButtonString = "Install";

		private static string kEndButtonString = "Finish";

		private static string kCancelButtonString = "Cancel";

		private static string kWalcomeTitle_jp = "FILCOアシストインストーラー";

		private static string kWalcomeContent1_jp = "この度はFILCO製品をお買い上げいただきありがとうございます。\nこのプログラムはお使いのシステムに「FILCOアシスト」をインストールします。\n「FILCOアシスト」はFILCOキーボードで多くの機能を使える様にする便利なソフトウェアです。\n";

		private static string kWalcomeContent2_jp = "をお読みいただき、ご使用条件に同意される場合のみ「インストール」ボタンを押して\nソフトウェアのインストールを開始してください。同意されない場合は「キャンセル」ボタンを押して退出してください。";

		private static string kFinishContent1_jp = "インストールが完了しました。\n";

		private static string kFinishContent2_jp = "FILCOアシストは使用可能な状態となりました。";

		private static string kUpgradeContent1_jp = "このバージョンは既にインストール済みです。";

		private static string kInstallButtonString_jp = "インストール";

		private static string kEndButtonString_jp = "終了";

		private static string kCancelButtonString_jp = "キャンセル";

		private SetupPageID setup_page_id;

		internal Button Minimize;

		internal TextBlock WelcomeTitle;

		internal Span WelcomeContent;

		internal Image LoadingGIF;

		internal StackPanel WelcomePage;

		internal Button CancelButton;

		internal Button NextButton;

		private bool _contentLoaded;

		public MyProductSetup Setup { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			if (CultureInfo.CurrentUICulture.Name == "ja-JP")
			{
				WelcomeTitle.Text = kWalcomeTitle_jp;
			}
			else
			{
				WelcomeTitle.Text = kWalcomeTitle;
			}
			SetToWelcomePage();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Setup = new MyProductSetup(App.MsiFile);
			((MsiSession)Setup).InUiThread = InUiThread;
			((MsiSession)Setup).SetupComplete += SetToFinishPage;
			if (Setup.IsAlreadyInstalled)
			{
				Setup.InitialCanInstall = true;
				SetToAlreadyInstallPage();
			}
			((FrameworkElement)this).DataContext = Setup;
		}

		public void InUiThread(Action action)
		{
			if (((DispatcherObject)this).Dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				((DispatcherObject)this).Dispatcher.BeginInvoke((DispatcherPriority)9, (Delegate)action);
			}
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			((Window)this).DragMove();
		}

		private void Minimize_Click(object sender, RoutedEventArgs e)
		{
			((Window)this).WindowState = (WindowState)1;
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			DoClose();
		}

		private void Install_Click(object sender, RoutedEventArgs e)
		{
			switch (setup_page_id)
			{
			case SetupPageID.kSPIDWelcome:
				SetToInstallingPage();
				Setup.StartInstall();
				break;
			case SetupPageID.kSPIDFinish:
				DoClose();
				break;
			case SetupPageID.kSPIDAlreadyInstall:
				DoClose();
				break;
			case SetupPageID.kSPIDInstalling:
				break;
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			((MsiSession)Setup).CancelRequested = true;
			DoClose();
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = ((GenericSetup)Setup).IsRunning;
		}

		private void ShowLog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start(((GenericSetup)Setup).LogFile);
		}

		private void SetToWelcomePage()
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected O, but got Unknown
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected O, but got Unknown
			setup_page_id = SetupPageID.kSPIDWelcome;
			((UIElement)LoadingGIF).Visibility = (Visibility)1;
			((ContentElement)WelcomeContent).IsEnabled = true;
			((TextElementCollection<Inline>)(object)WelcomeContent.Inlines).Clear();
			if (CultureInfo.CurrentUICulture.Name == "ja-JP")
			{
				WelcomeContent.Inlines.Add(kWalcomeContent1_jp);
			}
			else
			{
				WelcomeContent.Inlines.Add(kWalcomeContent1);
			}
			Hyperlink val = new Hyperlink();
			val.NavigateUri = new Uri("https://www.diatec.co.jp/filcoassist/filco_assist_eula_pp.php");
			val.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
			if (CultureInfo.CurrentUICulture.Name == "ja-JP")
			{
				((Span)val).Inlines.Add("ソフトウェア使用許諾契約書");
			}
			else
			{
				((Span)val).Inlines.Add("EULA");
			}
			((TextElementCollection<Inline>)(object)WelcomeContent.Inlines).Add((Inline)(object)val);
			if (CultureInfo.CurrentUICulture.Name == "ja-JP")
			{
				WelcomeContent.Inlines.Add(kWalcomeContent2_jp);
			}
			else
			{
				WelcomeContent.Inlines.Add(kWalcomeContent2);
			}
			((UIElement)CancelButton).IsEnabled = true;
			if (CultureInfo.CurrentUICulture.Name == "ja-JP")
			{
				((ContentControl)CancelButton).Content = kCancelButtonString_jp;
			}
			else
			{
				((ContentControl)CancelButton).Content = kCancelButtonString;
			}
			((UIElement)NextButton).IsEnabled = true;
			if (CultureInfo.CurrentUICulture.Name == "ja-JP")
			{
				((ContentControl)NextButton).Content = kInstallButtonString_jp;
			}
			else
			{
				((ContentControl)NextButton).Content = kInstallButtonString;
			}
		}

		private void SetToInstallingPage()
		{
			setup_page_id = SetupPageID.kSPIDInstalling;
			((UIElement)LoadingGIF).Visibility = (Visibility)0;
			new Task(delegate
			{
				for (int i = 1; i <= 12; i++)
				{
					string image_source = "Resources/Image/Progress/progress-";
					image_source = image_source + i + ".png";
					((DispatcherObject)this).Dispatcher.BeginInvoke((Delegate)(Action)delegate
					{
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0012: Expected O, but got Unknown
						BitmapImage source = new BitmapImage(new Uri(image_source, UriKind.Relative));
						LoadingGIF.Source = (ImageSource)(object)source;
					}, Array.Empty<object>());
					Thread.Sleep(100);
					if (i == 12)
					{
						i = 1;
					}
				}
			}).Start();
			((TextElementCollection<Inline>)(object)WelcomeContent.Inlines).Clear();
			((UIElement)CancelButton).IsEnabled = false;
			((ContentControl)CancelButton).Content = "";
			((UIElement)NextButton).IsEnabled = false;
			((ContentControl)NextButton).Content = "";
		}

		private void SetToFinishPage()
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			setup_page_id = SetupPageID.kSPIDFinish;
			((UIElement)LoadingGIF).Visibility = (Visibility)1;
			((TextElementCollection<Inline>)(object)WelcomeContent.Inlines).Clear();
			string text = ((!(CultureInfo.CurrentUICulture.Name == "ja-JP")) ? kFinishContent1 : kFinishContent1_jp);
			WelcomeContent.Inlines.Add(text);
			string text2 = ((!(CultureInfo.CurrentUICulture.Name == "ja-JP")) ? kFinishContent2 : kFinishContent2_jp);
			Run val = new Run(text2);
			((TextElementCollection<Inline>)(object)WelcomeContent.Inlines).Add((Inline)(object)val);
			((UIElement)NextButton).IsEnabled = true;
			if (CultureInfo.CurrentUICulture.Name == "ja-JP")
			{
				((ContentControl)NextButton).Content = kEndButtonString_jp;
			}
			else
			{
				((ContentControl)NextButton).Content = kEndButtonString;
			}
		}

		private void SetToAlreadyInstallPage()
		{
			setup_page_id = SetupPageID.kSPIDAlreadyInstall;
			((UIElement)LoadingGIF).Visibility = (Visibility)1;
			((TextElementCollection<Inline>)(object)WelcomeContent.Inlines).Clear();
			string text = ((!(CultureInfo.CurrentUICulture.Name == "ja-JP")) ? kUpgradeContent1 : kUpgradeContent1_jp);
			WelcomeContent.Inlines.Add(text);
			((UIElement)CancelButton).IsEnabled = false;
			((ContentControl)CancelButton).Content = "";
			if (CultureInfo.CurrentUICulture.Name == "ja-JP")
			{
				((ContentControl)NextButton).Content = kEndButtonString_jp;
			}
			else
			{
				((ContentControl)NextButton).Content = kEndButtonString;
			}
		}

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			((RoutedEventArgs)e).Handled = true;
		}

		private void DoClose()
		{
			App.DeleteTempWhenClose();
			((Window)this).Close();
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (!_contentLoaded)
			{
				_contentLoaded = true;
				Uri uri = new Uri("/FILCOAssistSetup;component/mainwindow.xaml", UriKind.Relative);
				Application.LoadComponent((object)this, uri);
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Expected O, but got Unknown
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Expected O, but got Unknown
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Expected O, but got Unknown
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Expected O, but got Unknown
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Expected O, but got Unknown
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Expected O, but got Unknown
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Expected O, but got Unknown
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Expected O, but got Unknown
			switch (connectionId)
			{
			case 1:
				((Window)(MainWindow)target).Closing += Window_Closing;
				((FrameworkElement)(MainWindow)target).Loaded += new RoutedEventHandler(Window_Loaded);
				break;
			case 2:
				((UIElement)(TextBlock)target).MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseLeftButtonDown);
				break;
			case 3:
				Minimize = (Button)target;
				((ButtonBase)Minimize).Click += new RoutedEventHandler(Minimize_Click);
				break;
			case 4:
				((ButtonBase)(Button)target).Click += new RoutedEventHandler(Close_Click);
				break;
			case 5:
				WelcomeTitle = (TextBlock)target;
				break;
			case 6:
				WelcomeContent = (Span)target;
				break;
			case 7:
				LoadingGIF = (Image)target;
				break;
			case 8:
				WelcomePage = (StackPanel)target;
				break;
			case 9:
				CancelButton = (Button)target;
				((ButtonBase)CancelButton).Click += new RoutedEventHandler(Cancel_Click);
				break;
			case 10:
				NextButton = (Button)target;
				((ButtonBase)NextButton).Click += new RoutedEventHandler(Install_Click);
				break;
			default:
				_contentLoaded = true;
				break;
			}
		}
	}
	public class MyProductSetup : GenericSetup
	{
		private bool installDocumentation;

		public bool InstallDocumentation
		{
			get
			{
				return installDocumentation;
			}
			set
			{
				installDocumentation = value;
				((MsiSession)this).OnPropertyChanged("InstallDocumentation");
			}
		}

		public bool InitialCanInstall { get; set; }

		public bool InitialCanUnInstall { get; set; }

		public bool InitialCanRepair { get; set; }

		public bool IsAlreadyInstalled { get; set; }

		public void StartRepair()
		{
			((GenericSetup)this).StartRepair("CUSTOM_UI=true");
		}

		public void StartInstall()
		{
			((GenericSetup)this).StartInstall("CUSTOM_UI=true ADDLOCAL=Binaries");
		}

		public MyProductSetup(string msiFile, bool enableLoging = true)
			: base(msiFile, enableLoging)
		{
			InitialCanInstall = ((GenericSetup)this).CanInstall;
			InitialCanUnInstall = ((GenericSetup)this).CanUnInstall;
			InitialCanRepair = ((GenericSetup)this).CanRepair;
			IsAlreadyInstalled = ((GenericSetup)this).IsCurrentlyInstalled;
		}
	}
}
namespace FilcoSetupApp.Properties
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					resourceMan = new ResourceManager("FilcoSetupApp.Properties.Resources", typeof(Resources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static byte[] FILCO_Assist => (byte[])ResourceManager.GetObject("FILCO_Assist", resourceCulture);

		internal static byte[] WixSharp_Msi => (byte[])ResourceManager.GetObject("WixSharp_Msi", resourceCulture);

		internal Resources()
		{
		}
	}
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.4.0.0")]
	internal sealed class Settings : ApplicationSettingsBase
	{
		private static Settings defaultInstance = (Settings)(object)SettingsBase.Synchronized((SettingsBase)(object)new Settings());

		public static Settings Default => defaultInstance;
	}
}
