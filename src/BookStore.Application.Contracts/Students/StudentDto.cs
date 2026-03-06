using System;
using System.Collections.Generic;

namespace BookStore.Students;

public class StudentDto
{
    public Guid Id { get; set; }
    public string UserCode { get; set; }
    public string FullName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool? Gender { get; set; }
    public string? Address { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public DateTime? LastLogoutTime { get; set; }
    public string? LastLoginLocation { get; set; }
    public string? LastLoginUserAgent { get; set; }

    public string UserName { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool LockoutEnabled { get; set; }
    public List<string> Roles { get; set; } = new() { "User" };

    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public int Version { get; set; }
}

