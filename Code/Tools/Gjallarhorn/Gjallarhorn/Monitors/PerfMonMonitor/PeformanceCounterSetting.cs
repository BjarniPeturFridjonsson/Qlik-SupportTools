using System;

namespace Gjallarhorn.Monitors.PerfMonMonitor
{
    public class PeformanceCounterSetting
    {
        private bool Equals(PeformanceCounterSetting other)
        {
            return Duration.Equals(other.Duration)
                   && string.Equals(ValidationExpression, other.ValidationExpression)
                   && string.Equals(Instance, other.Instance)
                   && string.Equals(Name, other.Name)
                   && string.Equals(Category, other.Category)
                   && string.Equals(MachineName, other.MachineName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PeformanceCounterSetting)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Duration.GetHashCode();
                hashCode = (hashCode * 397) ^ (ValidationExpression?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Instance?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Category?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (MachineName?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public string MachineName { get; }
        public string Category { get; }
        public string Name { get; }
        public string Instance { get; }
        public string ValidationExpression { get; }
        public TimeSpan Duration { get; }

        public PeformanceCounterSetting(string machineName, string category, string name, string instance, string validationExpression, TimeSpan duration)
        {
            MachineName = machineName;
            Category = category;
            Name = name;
            Instance = instance;

            ValidationExpression = validationExpression;
            Duration = duration;
        }
    }
}
