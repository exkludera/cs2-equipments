using CounterStrikeSharp.API.Core;

public interface IWasdMenu
{
    public string? Title { get; set; }
    public LinkedList<IWasdMenuOption>? Options { get; set; }
    // previous option node
    public LinkedListNode<IWasdMenuOption>? Prev { get; set; }
    public LinkedListNode<IWasdMenuOption> Add(string display, Action<CCSPlayerController, IWasdMenuOption> onChoice);
}