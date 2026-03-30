namespace Pokedle.Api.GraphQL;

public class TypeSlotHint
{
    public string TypeName { get; set; } = "none";
    public Hint Hint { get; set; }
}

public class TypeHintResult
{
    public TypeSlotHint Slot1 { get; set; } = default!;
    public TypeSlotHint Slot2 { get; set; } = default!;
}