using System;
using System.Collections.Generic;
using System.Text;

namespace Conductor.Domain.Models
{
    public class DynamicData
    {
        private Dictionary<string, object> _storage { get; set; } = new Dictionary<string, object>();

        public object this[string propertyName]
        {
            get => _storage.TryGetValue(propertyName, out var value) ? value : null;
            set => _storage[propertyName] = value;
        }
    }
}
