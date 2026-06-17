using System.Windows;

namespace SmartPOS.UI;

public partial class DatabaseProgressWindow : Window
{
    public DatabaseProgressWindow()
    {
        InitializeComponent();
    }

    public void UpdateProgress(string message, int percentage)
    {
        Dispatcher.Invoke(() =>
        {
            ProgressText.Text = message;
            ProgressBar.Value = percentage;
            PercentageText.Text = $"{percentage}%";
        });
    }
}