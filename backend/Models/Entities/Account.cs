using System;
using Student_management.Models.Base;
using Student_management.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Account")]
public class Account : BaseEntity
{
    [Key]
    public int AccountID { get; set; }

    [Required]
    [StringLength(100)]
    public string? Email { get; set; } 

    [Required]
    [StringLength(255)]
    public string? Password { get; set; }

    [StringLength(255)]
    public string? Avatar { get; set; }

    public byte Status { get; set; } = 1; 

    [Required]
    public int RoleID { get; set; }

    [StringLength(100)]
    public string? FullName { get; set; }

    [StringLength(15)]
    public string? PhoneNumber { get; set; }
    public Role? Role { get; set; }
    public Teacher? Teacher { get; set; }
    public Student? Student { get; set; }
}