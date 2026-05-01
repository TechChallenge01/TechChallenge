using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public record EmailPayloadDTO
    {
        public ICollection<string> To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = false;
    }
}
