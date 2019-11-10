namespace ApplicationCore.Model
{
    public class FormField
    {
        public object? Value { get; }
        public string Error { get; } = string.Empty;
        public bool HasError => !string.IsNullOrEmpty(Error);

        public FormField(object? value)
        {
            Value = value;
        }

        public FormField(string error) : this(value: null)
        {
            Error = error;
        }
    }
}
