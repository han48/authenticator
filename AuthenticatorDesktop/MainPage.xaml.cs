using System.Timers;

namespace AuthenticatorDesktop;

public partial class MainPage : ContentPage
{
	private readonly System.Timers.Timer _timer;
	private readonly string account;
	private readonly Core.Totp otp;
	const double PADDING = 30;
	const double SPACING = 10;

	public MainPage()
	{
		InitializeComponent();

		account = "zzz4everzzz@live.co.uk";
		otp = new Core.Totp();

		_timer = new System.Timers.Timer(1000);
		if (OnTimedEvent != null)
		{
			_timer.Elapsed += OnTimedEvent;
		}
		_timer.AutoReset = true;
		_timer.Enabled = true;

		this.Resources.TryGetValue("SubHeadline", out var textStyle);
		var doubleTapRecognizer = new TapGestureRecognizer
		{
			NumberOfTapsRequired = 2,
		};
		doubleTapRecognizer.Tapped += CopyCodeToClipboard;
		string[] accounts = ["zzz4everzzz@live.co.uk"];
		foreach (string item in accounts)
		{
			VerticalStackLayout itemContainer = new()
			{
				Padding = new Thickness(PADDING, PADDING, PADDING, PADDING),
				Spacing = SPACING
			};

			Label lblName = new()
			{
				Text = item
			};
			itemContainer.Add(lblName);

			FlexLayout row = new()
			{
				Direction = Microsoft.Maui.Layouts.FlexDirection.Row,
				JustifyContent = Microsoft.Maui.Layouts.FlexJustify.SpaceBetween,
				AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center,
				HeightRequest = PADDING * 2
			};
			row.GestureRecognizers.Add(doubleTapRecognizer);
			Label lblCode = new()
			{
				Text = "Loading...",
				Padding = new Thickness(PADDING, 0, 0, 0),
				HorizontalOptions = LayoutOptions.Start,
				Style = textStyle is Style ? (Style)textStyle : null,
			};
			lblCode.GestureRecognizers.Add(doubleTapRecognizer);
			SemanticProperties.SetHeadingLevel(lblCode, SemanticHeadingLevel.Level2);
			row.Add(lblCode);
			Label lblTimer = new()
			{
				Text = "",
				Padding = new Thickness(0, 0, PADDING, 0),
				HorizontalOptions = LayoutOptions.End,
				Style = textStyle is Style style ? style : null,
			};
			SemanticProperties.SetHeadingLevel(lblCode, SemanticHeadingLevel.Level2);
			row.Add(lblTimer);

			Frame frame = new()
			{
				BorderColor = Colors.Gray,
				Content = row
			};
			itemContainer.Add(frame);

			view.Add(itemContainer);
		}
	}

	private void OnTimedEvent(Object? source, ElapsedEventArgs e)
	{
		// Cập nhật UI từ main thread
		MainThread.BeginInvokeOnMainThread(() =>
		{
			string secret = otp.CreateBase32Secret(account);
			OtpNet.Totp objTotp = otp.GetTotp(secret);
			totp.Text = $"{objTotp.ComputeTotp()}";
			totpTimer.Text = $"{objTotp.RemainingSeconds()}";
			totpName.Text = $"{account}";
			LinearGradientBrush bgColor = new()
			{
				StartPoint = new Point(0, 0),
				EndPoint = new Point(1.0 * objTotp.RemainingSeconds() / objTotp.Step, 0),
			};
			bgColor.GradientStops.Add(new GradientStop(Colors.SeaGreen, 0.0f));
			bgColor.GradientStops.Add(new GradientStop(Colors.SeaGreen, 0.98f));
			bgColor.GradientStops.Add(new GradientStop(Colors.Transparent, 0.99f));
			bgColor.GradientStops.Add(new GradientStop(Colors.Transparent, 1.0f));
			totpContainer.Background = bgColor;
		});
	}

	private void CopyCodeToClipboard(object? sender, EventArgs e)
	{
		if (sender is Label)
		{
			Clipboard.Default.SetTextAsync(((Label)sender).Text);
		}
		if (sender is FlexLayout) 
		{
			IView? view = ((FlexLayout)sender).FirstOrDefault();
			if (view != null && view is Label)
			{
				Clipboard.Default.SetTextAsync(((Label)view).Text);
			}
		}
	}
}

