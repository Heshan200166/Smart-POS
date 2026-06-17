using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartPOS.Data;
using SmartPOS.Models;
using SmartPOS.Business;

namespace SmartPOS.UI;

/// <summary>
/// User Management window for admin users to manage system users
/// </summary>
public partial class UserManagementWindow : Window
{
    private readonly ApplicationDbContext _context;
    private readonly AuthenticationService _authService;
    private readonly ILogger<UserManagementWindow> _logger;
    
    private ObservableCollection<User> _users = new();
    private List<Role> _roles = new();
    private bool _isEditing = false;
    private int _editingUserId = 0;

    public UserManagementWindow(ApplicationDbContext context, AuthenticationService authService, ILogger<UserManagementWindow> logger)
    {
        InitializeComponent();
        _context = context;
        _authService = authService;
        _logger = logger;

        UsersItemsControl.ItemsSource = _users;
        
        Loaded += UserManagementWindow_Loaded;
    }

    private async void UserManagementWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadRolesAsync();
        await LoadUsersAsync();
    }

    private async Task LoadRolesAsync()
    {
        try
        {
            _roles = await _context.Roles.ToListAsync();
            RoleFormComboBox.ItemsSource = _roles;
            
            if (_roles.Any())
                RoleFormComboBox.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load roles");
            MessageBox.Show("Failed to load roles. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task LoadUsersAsync()
    {
        try
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .OrderBy(u => u.Username)
                .ToListAsync();

            _users.Clear();
            foreach (var user in users)
            {
                _users.Add(user);
            }
            
            _logger.LogInformation("Loaded {UserCount} users", users.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load users");
            MessageBox.Show("Failed to load users. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // For now, implement a simple search by reloading filtered data
        var searchTerm = SearchTextBox.Text.ToLower();
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            // Show all users - reload all data
            _ = Task.Run(async () =>
            {
                await Task.Delay(200); // Small debounce
                Dispatcher.Invoke(async () => await LoadUsersAsync());
            });
        }
        else
        {
            // Filter users with search term
            _ = Task.Run(async () =>
            {
                await Task.Delay(400); // Longer debounce for typing
                
                Dispatcher.Invoke(async () =>
                {
                    try
                    {
                        var filteredUsers = await _context.Users
                            .Include(u => u.Role)
                            .Where(u => u.Username.ToLower().Contains(searchTerm) ||
                                       u.FullName.ToLower().Contains(searchTerm) ||
                                       (u.Email != null && u.Email.ToLower().Contains(searchTerm)))
                            .OrderBy(u => u.Username)
                            .ToListAsync();

                        _users.Clear();
                        foreach (var user in filteredUsers)
                        {
                            _users.Add(user);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to search users with term: {SearchTerm}", searchTerm);
                    }
                });
            });
        }
    }

    private void AddUser_Click(object sender, RoutedEventArgs e)
    {
        ResetForm();
        _isEditing = false;
        FormTitle.Text = "➕ Add New User";
        PasswordPanel.Visibility = Visibility.Visible;
        SaveFormButton.Content = "💾 Save User";
    }

    private async void EditUser_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not int userId)
            return;

        try
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Populate form
            _isEditing = true;
            _editingUserId = userId;
            FormTitle.Text = "✏️ Edit User";
            PasswordPanel.Visibility = Visibility.Collapsed;
            SaveFormButton.Content = "💾 Update User";

            UsernameFormTextBox.Text = user.Username;
            FullNameFormTextBox.Text = user.FullName;
            EmailFormTextBox.Text = user.Email ?? "";
            PhoneFormTextBox.Text = user.Phone ?? "";
            RoleFormComboBox.SelectedValue = user.RoleId;
            IsActiveFormCheckBox.IsChecked = user.IsActive;
            UserIdFormTextBox.Text = userId.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load user for editing: {UserId}", userId);
            MessageBox.Show("Failed to load user details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void ToggleStatus_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not int userId)
            return;

        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return;

            var action = user.IsActive ? "deactivate" : "activate";
            var result = MessageBox.Show(
                $"Are you sure you want to {action} user '{user.Username}'?",
                "Confirm Action",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("User {Username} status changed to {Status}", user.Username, user.IsActive ? "Active" : "Inactive");
                
                await LoadUsersAsync(); // Refresh the list
                
                MessageBox.Show($"User '{user.Username}' has been {action}d successfully.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle user status: {UserId}", userId);
            MessageBox.Show("Failed to update user status.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void ResetPassword_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not int userId)
            return;

        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return;

            var result = MessageBox.Show(
                $"Reset password for user '{user.Username}' to default 'admin123'?",
                "Reset Password",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                user.PasswordHash = _authService.HashPassword("admin123");
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Password reset for user: {Username}", user.Username);
                
                MessageBox.Show($"Password for '{user.Username}' has been reset to 'admin123'.", 
                    "Password Reset", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reset password for user: {UserId}", userId);
            MessageBox.Show("Failed to reset password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void SaveUser_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateForm())
            return;

        try
        {
            if (_isEditing)
            {
                await UpdateUserAsync();
            }
            else
            {
                await CreateUserAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save user");
            MessageBox.Show("Failed to save user. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task CreateUserAsync()
    {
        // Check if username already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == UsernameFormTextBox.Text);

        if (existingUser != null)
        {
            MessageBox.Show("Username already exists. Please choose a different username.", 
                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var user = new User
        {
            Username = UsernameFormTextBox.Text.Trim(),
            FullName = FullNameFormTextBox.Text.Trim(),
            Email = string.IsNullOrWhiteSpace(EmailFormTextBox.Text) ? null : EmailFormTextBox.Text.Trim(),
            Phone = string.IsNullOrWhiteSpace(PhoneFormTextBox.Text) ? null : PhoneFormTextBox.Text.Trim(),
            RoleId = (int)RoleFormComboBox.SelectedValue,
            IsActive = IsActiveFormCheckBox.IsChecked ?? true,
            PasswordHash = _authService.HashPassword(PasswordFormBox.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New user created: {Username}", user.Username);

        MessageBox.Show($"User '{user.Username}' created successfully!", 
            "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        ResetForm();
        await LoadUsersAsync();
    }

    private async Task UpdateUserAsync()
    {
        var user = await _context.Users.FindAsync(_editingUserId);
        if (user == null)
            return;

        // Check if username already exists for other users
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == UsernameFormTextBox.Text && u.Id != _editingUserId);

        if (existingUser != null)
        {
            MessageBox.Show("Username already exists. Please choose a different username.", 
                "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        user.Username = UsernameFormTextBox.Text.Trim();
        user.FullName = FullNameFormTextBox.Text.Trim();
        user.Email = string.IsNullOrWhiteSpace(EmailFormTextBox.Text) ? null : EmailFormTextBox.Text.Trim();
        user.Phone = string.IsNullOrWhiteSpace(PhoneFormTextBox.Text) ? null : PhoneFormTextBox.Text.Trim();
        user.RoleId = (int)RoleFormComboBox.SelectedValue;
        user.IsActive = IsActiveFormCheckBox.IsChecked ?? true;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User updated: {Username}", user.Username);

        MessageBox.Show($"User '{user.Username}' updated successfully!", 
            "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        ResetForm();
        await LoadUsersAsync();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(UsernameFormTextBox.Text))
        {
            MessageBox.Show("Username is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            UsernameFormTextBox.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(FullNameFormTextBox.Text))
        {
            MessageBox.Show("Full Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            FullNameFormTextBox.Focus();
            return false;
        }

        if (RoleFormComboBox.SelectedValue == null)
        {
            MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            RoleFormComboBox.Focus();
            return false;
        }

        if (!_isEditing)
        {
            if (string.IsNullOrWhiteSpace(PasswordFormBox.Password))
            {
                MessageBox.Show("Password is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordFormBox.Focus();
                return false;
            }

            if (PasswordFormBox.Password != ConfirmPasswordFormBox.Password)
            {
                MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ConfirmPasswordFormBox.Focus();
                return false;
            }

            if (PasswordFormBox.Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordFormBox.Focus();
                return false;
            }
        }

        return true;
    }

    private void CancelForm_Click(object sender, RoutedEventArgs e)
    {
        ResetForm();
    }

    private void ResetForm()
    {
        _isEditing = false;
        _editingUserId = 0;
        FormTitle.Text = "➕ Add New User";
        PasswordPanel.Visibility = Visibility.Visible;
        SaveFormButton.Content = "💾 Save User";

        UsernameFormTextBox.Clear();
        FullNameFormTextBox.Clear();
        EmailFormTextBox.Clear();
        PhoneFormTextBox.Clear();
        PasswordFormBox.Clear();
        ConfirmPasswordFormBox.Clear();
        IsActiveFormCheckBox.IsChecked = true;
        UserIdFormTextBox.Clear();

        if (RoleFormComboBox.Items.Count > 0)
            RoleFormComboBox.SelectedIndex = 0;
    }
}