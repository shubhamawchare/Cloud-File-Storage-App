using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudFileStorageApp.Models
{
    public class StoredFile
    {
        [Key] // Optional but good practice
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        public DateTime UploadedOn { get; set; }

        // For multi-user support
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }


        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }

        public string? FolderName { get; set; } // Nullable to support root-level files

        public string? ShareToken { get; set; } // Unique token for public access
        public bool IsPublic { get; set; } = false;



    }
}
