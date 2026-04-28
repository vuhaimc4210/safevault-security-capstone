using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SafeVault.Models;

/// <summary>
/// Extended ApplicationUser with additional security-related properties.
/// Inherits from IdentityUser for ASP.NET Identity integration.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required]
        [StringLength(100, MinimumLength = 2)]
            public string FirstName { get; set; } = string.Empty;

                [Required]
                    [StringLength(100, MinimumLength = 2)]
                        public string LastName { get; set; } = string.Empty;

                            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

                                public DateTime? LastLoginAt { get; set; }

                                    public bool IsActive { get; set; } = true;

                                        /// <summary>
                                            /// Tracks failed login attempts for security monitoring
                                                /// </summary>
                                                    public int FailedLoginCount { get; set; } = 0;

                                                        /// <summary>
                                                            /// Department or role classification for RBAC
                                                                /// </summary>
                                                                    [StringLength(50)]
                                                                        public string? Department { get; set; }

                                                                            public string FullName => $"{FirstName} {LastName}";
                                                                            }
