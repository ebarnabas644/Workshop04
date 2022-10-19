using System;
using System.Collections.Generic;
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
        public string BasePath { get; set; }
        public string File { get; set; }
        public DateTime Time { get; set; }
        public string Email { get; set; }

        public OperationType OperationType { get; set; }
    }
}
