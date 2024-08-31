using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Text;

public class WasdMenuPlayer
{
    public CCSPlayerController player = null!;
    public WasdMenu? MainMenu = null;
    public LinkedListNode<IWasdMenuOption>? CurrentChoice = null;
    public LinkedListNode<IWasdMenuOption>? MenuStart = null;
    public string CenterHtml = "";
    public int VisibleOptions = 5;
    public PlayerButtons Buttons { get; set; }

    public void OpenMainMenu(WasdMenu? menu)
    {
        if (menu == null)
        {
            MainMenu = null;
            CurrentChoice = null;
            CenterHtml = "";
            return;
        }
        MainMenu = menu;
        VisibleOptions = menu.Title != "" ? 4 : 5;
        CurrentChoice = MainMenu.Options?.First;
        MenuStart = CurrentChoice;
        UpdateCenterHtml();
    }

    public void OpenSubMenu(IWasdMenu? menu)
    {
        if (menu == null)
        {
            CurrentChoice = MainMenu?.Options?.First;
            MenuStart = CurrentChoice;
            UpdateCenterHtml();
            return;
        }

        VisibleOptions = menu.Title != "" ? 4 : 5;
        CurrentChoice = menu.Options?.First;
        MenuStart = CurrentChoice;
        UpdateCenterHtml();
    }
    public void GoBackToPrev(LinkedListNode<IWasdMenuOption>? menu)
    {
        if (menu == null)
        {
            CurrentChoice = MainMenu?.Options?.First;
            MenuStart = CurrentChoice;
            UpdateCenterHtml();
            return;
        }

        VisibleOptions = menu.Value.Parent?.Title != "" ? 4 : 5;
        CurrentChoice = menu;
        if (CurrentChoice.Value.Index >= 5)
        {
            MenuStart = CurrentChoice;
            for (int i = 0; i < 4; i++)
            {
                MenuStart = MenuStart?.Previous;
            }
        }
        else
            MenuStart = CurrentChoice.List?.First;
        UpdateCenterHtml();
    }

    public void CloseSubMenu()
    {
        if (CurrentChoice?.Value.Parent?.Prev == null)
            return;
        GoBackToPrev(CurrentChoice?.Value.Parent.Prev);
    }

    public void CloseAllSubMenus()
    {
        OpenSubMenu(null);
    }

    public void Choose()
    {
        CurrentChoice?.Value.OnChoose?.Invoke(player, CurrentChoice.Value);
    }

    public void ScrollDown()
    {
        if (CurrentChoice == null || MainMenu == null)
            return;
        CurrentChoice = CurrentChoice.Next ?? CurrentChoice.List?.First;
        MenuStart = CurrentChoice!.Value.Index >= VisibleOptions ? MenuStart!.Next : CurrentChoice.List?.First;
        UpdateCenterHtml();
    }

    public void ScrollUp()
    {
        if (CurrentChoice == null || MainMenu == null)
            return;
        CurrentChoice = CurrentChoice.Previous ?? CurrentChoice.List?.Last;
        if (CurrentChoice == CurrentChoice?.List?.Last && CurrentChoice?.Value.Index >= VisibleOptions)
        {
            MenuStart = CurrentChoice;
            for (int i = 0; i < VisibleOptions - 1; i++)
                MenuStart = MenuStart?.Previous;
        }
        else
            MenuStart = CurrentChoice!.Value.Index >= VisibleOptions ? MenuStart!.Previous : CurrentChoice.List?.First;
        UpdateCenterHtml();
    }

    private void UpdateCenterHtml()
    {
        if (CurrentChoice == null || MainMenu == null)
            return;

        StringBuilder builder = new();
        int i = 0;
        LinkedListNode<IWasdMenuOption>? option = MenuStart!;
        if (option.Value.Parent?.Title != "")
        {
            builder.AppendLine($"{Plugin.Instance.Localizer["WASD Menu Prefix"]}{option.Value.Parent?.Title}</u><font color='white'><br>");
        }

        while (i < VisibleOptions && option != null)
        {
            if (option == CurrentChoice)
                builder.AppendLine($"{Plugin.Instance.Localizer["WASD Menu Selection Left"]} {option.Value.OptionDisplay} {Plugin.Instance.Localizer["WASD Menu Selection Right"]} <br>");
            else
                builder.AppendLine($"{option.Value.OptionDisplay} <br>");
            option = option.Next;
            i++;
        }

        if (option != null)
        {
            builder.AppendLine(
                $"{Plugin.Instance.Localizer["WASD Menu Selection Options Below"]}");
        }

        builder.AppendLine("<br>" +
                           $"{Plugin.Instance.Localizer["WASD Menu Bottom Text"]}<br>");
        builder.AppendLine("</div>");
        CenterHtml = builder.ToString();
    }
}