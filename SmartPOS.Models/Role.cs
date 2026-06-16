namespace SmartPOS.Models;

/// <summary>
/// Role entity for role-based access control
/// </summary>
public class Role : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    
    // Navigation property
    public ICollection<User> Users { get; set; } = new List<User>();
}
