namespace ApplicationCore.Model
{
    public class FormField
    {
        public string Name { get; }
        public object? Value { get; }
        public bool IsRequired { get; }
        public string Error { get; } = string.Empty;
        public bool HasError => !string.IsNullOrEmpty(Error);

        public FormField(string name, object? value, bool isRequired)
        {
            Name = name;
            Value = value;
            IsRequired = isRequired;
        }

        public FormField(string name, string error) : this(name, value: null, isRequired: true)
        {
            Error = error;
        }
    }
}
