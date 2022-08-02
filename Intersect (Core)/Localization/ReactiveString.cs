namespace Intersect.Localization;

public sealed class ReactiveString
{
    public delegate string Consumer(LocalizedString localizedString);

    private readonly Consumer _consumer;
    private readonly LocalizedString _localizedString;

    private string? _value;

    public ReactiveString(LocalizedString localizedString, Consumer consumer)
    {
        _consumer = consumer;
        _localizedString = localizedString;
        _localizedString.Repopulated += (_, _) => _value = default;
    }

    public override string ToString()
    {
        return _value ??= _consumer(_localizedString);
    }

    public static implicit operator string(ReactiveString str) => str?.ToString();
}
