using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using SmartPOS.Data;
using SmartPOS.Models;
using SmartPOS.Business;

namespace SmartPOS.UI;

/// <summary>
/// Change Password window for users to update their password
/// </summary>
public partial class ChangePasswordWindow : Window
{
    private readonly ApplicationDbContext _context;
    private readonly User _currentUser;
    private readonly AuthenticationService _authService;

    public ChangePasswordWindow(ApplicationDbContext context, User currentUser)
    {
        InitializeComponent();
        _context = context;
        _currentUser = currentUser;
        _authService = new AuthenticationService(_context);

        UserInfoText.Text = $"User: {_currentUser.FullName} ({_currentUser.Username})";

        // Set focus to current password field
        Loaded += (s, e) => CurrentPasswordBox.Focus();
        
        // Handle Enter key navigation
        CurrentPasswordBox.KeyDown += (s, e) => {
            if (e.Key == Key.Enter)
                NewPasswordBox.Focus();
        };
        
        NewPasswordBox.KeyDown += (s, e) => {
            if (e.Key == Key.Enter)
                ConfirmPasswordBox.Focus();
        };
        
        ConfirmPasswordBox.KeyDown += (s, e) => {
            if (e.Key == Key.Enter)
                ChangePassword_Click(this, new RoutedEventArgs());
        };
    }

    private async void ChangePassword_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidatePasswords())
            return;

        try
        {
            // Update UI to show processing
            var button = sender as Button;
            var originalContent = button?.Content;
            if (button != null)
            {
                button.Content = "Changing...";
                button.IsEnabled = false;
            }

            StatusMessage.Visibility = Visibility.Collapsed;

            // Attempt to change password
            var success = await _authService.ChangePasswordAsync(
                _currentUser.Id, 
                CurrentPasswordBox.Password, 
                NewPasswordBox.Password);

            if (success)
            {
                ShowStatusMessage("✅ Password changed successfully!", true);
                
                await Task.Delay(1500); // Show success message briefly
                
                MessageBox.Show(
                    "Password changed successfully!\n\nYou will need to use your new password next time you login.",
                    "Password Changed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            else
            {
                ShowStatusMessage("❌ Current password is incorrect.", false);
                CurrentPasswordBox.Focus();
                CurrentPasswordBox.SelectAll();
            }
        }
        catch (Exception ex)
        {
            ShowStatusMessage("❌ Error changing password. Please try again.", false);
            // Log error (in a real app, you'd inject a logger)
            System.Diagnostics.Debug.WriteLine($"Password change error: {ex.Message}");
        }
        finally
        {
            // Reset UI
            if (sender is Button button)
            {
                button.Content = "💾 Change Password";
                button.IsEnabled = true;
            }
        }
    }

    private bool ValidatePasswords()
    {
        // Check if current password is provided
        if (string.IsNullOrWhiteSpace(CurrentPasswordBox.Password))
        {
            ShowStatusMessage("Please enter your current password.", false);
            CurrentPasswordBox.Focus();
            return false;
        }

        // Check if new password is provided
        if (string.IsNullOrWhiteSpace(NewPasswordBox.Password))
        {
            ShowStatusMessage("Please enter a new password.", false);
            NewPasswordBox.Focus();
            return false;
        }

        // Check minimum length
        if (NewPasswordBox.Password.Length < 6)
        {
            ShowStatusMessage("New password must be at least 6 characters long.", false);
            NewPasswordBox.Focus();
            return false;
        }

        // Check if passwords match
        if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
        {
            ShowStatusMessage("New passwords do not match.", false);
            ConfirmPasswordBox.Focus();
            return false;
        }

        // Check if new password is different from current
        if (CurrentPasswordBox.Password == NewPasswordBox.Password)
        {
            ShowStatusMessage("New password must be different from current password.", false);
            NewPasswordBox.Focus();
            return false;
        }

        return true;
    }

    private void ShowStatusMessage(string message, bool isSuccess)
    {
        StatusMessage.Text = message;
        StatusMessage.Foreground = isSuccess ? 
            System.Windows.Media.Brushes.Green : 
            System.Windows.Media.Brushes.Red;
        StatusMessage.Visibility = Visibility.Visible;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        
        // Remove minimize and maximize buttons
        var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
        if (hwnd != IntPtr.Zero)
        {
            var style = GetWindowLong(hwnd, -16); // GWL_STYLE
            style &= ~0x10000; // Remove WS_MAXIMIZEBOX
            style &= ~0x20000; // Remove WS_MINIMIZEBOX
            SetWindowLong(hwnd, -16, style);
        }
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}