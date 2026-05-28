public readonly record struct PhoneNumber(string Value)
{
    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number is required");

        value = value.Trim();

        if (!value.StartsWith("+992"))
            throw new ArgumentException("Phone number must start with +992");

        if (value.Length != 13)
            throw new ArgumentException("Invalid phone number length");

        return new PhoneNumber(value);
    }

    public override string ToString() => Value;
}