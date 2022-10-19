using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop04Models
{
    public enum OperationType
    {
        Create, Delete
    }
    public class LoggingModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string BasePath { get; set; }
        [Required]
        public string File { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public OperationType OperationType { get; set; }
    }
}
