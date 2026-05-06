namespace Pokedle.Api.GraphQL;

/// <summary>Hint for a single type slot.</summary>
public class TypeSlotHint
{
    /// <summary>Type name or "none".</summary>
    public string TypeName { get; set; } = "none";
    /// <summary>Correct / Partial / Wrong.</summary>
    public Hint Hint { get; set; }
}

/// <summary>Type hint covering both slots.</summary>
public class TypeHintResult
{
    /// <summary>First type slot.</summary>
    public TypeSlotHint Slot1 { get; set; } = default!;
    /// <summary>Second type slot.</summary>
    public TypeSlotHint Slot2 { get; set; } = default!;
}
