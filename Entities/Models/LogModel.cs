using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models;

public class LogModel : IEntitiy
{
    public string Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string MessageTemplate { get; set; }
    public string TraceId { get; set; }
    public string SpanId { get; set; }
}
