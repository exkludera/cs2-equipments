using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;

namespace Clientprefs.API;

/// <summary>
/// Cookie access types for client viewing
/// </summary>
public enum CookieAccess
{
    /// <summary>
    /// Visible and Changeable by users.
    /// </summary>
    CookieAccess_Public,
    /// <summary>
    /// Read only to users.
    /// </summary>
    CookieAccess_Protected,
    /// <summary>
    /// Completely hidden cookie.
    /// </summary>
    CookieAccess_Private
};

/// <summary>
/// Cookie Prefab menu types
/// </summary>
public enum CookieMenu
{
    /// <summary>
    /// Yes/No menu with "yes"/"no" results saved into the cookie
    /// </summary>
    CookieMenu_YesNo,
    /// <summary>
    /// Yes/No menu with 1/0 saved into the cookie
    /// </summary>
    CookieMenu_YesNo_Int,
    /// <summary>
    /// On/Off menu with "on"/"off" results saved into the cookie
    /// </summary>
    CookieMenu_OnOff,
    /// <summary>
    /// On/Off menu with 1/0 saved into the cookie
    /// </summary>
    CookieMenu_OnOff_Int
};

/// <summary>
/// Cookie Prefab Menu Actions
/// </summary>
public enum CookieMenuAction
{
    /// <summary>
    /// An option is being drawn for a menu.
    /// INPUT : PlayerController.
    /// </summary>
    CookieMenuAction_DisplayOption = 0,

    /// <summary>
    /// A menu option has been selected.
    /// INPUT : PlayerController.
    /// </summary>
    CookieMenuAction_SelectOption = 1
};

/// <summary>
/// API for Clienprefs.
/// </summary>
public interface IClientprefsApi
{
    /// <summary>
    /// Maximum length of a cookie name.
    /// </summary>
    public const int COOKIE_MAX_NAME_LENGTH = 64;
    /// <summary>
    /// Maximum length of a cookie description.
    /// </summary>
    public const int COOKIE_MAX_DESCRIPTION_LENGTH = 128;
    /// <summary>
    /// Maximum length of a cookie value.
    /// </summary>
    public const int COOKIE_MAX_VALUE_LENGTH = 256;

    event Action<CCSPlayerController>? OnPlayerCookiesCached;
    event Action? OnDatabaseLoaded;

    /// <summary>
    /// Creates a new player preference cookie.
    /// if a cookie with the same name or id exists,
    /// it will return the existing cookie id.
    /// -1 if unable to create.
    /// </summary>
    /// <param name="handler">ID of registered player cookie.</param>
    public int RegPlayerCookie(string name, string description, CookieAccess access = CookieAccess.CookieAccess_Public);

    /// <summary>
    /// Returns cookieId for the given cookie name.
    /// -1 if cookie not found.
    /// </summary>
    /// <param name="handler">Handler to find player cookie.</param>
    public int FindPlayerCookie(string name);

    /// <summary>
    /// Set the value of a player preference cookie.
    /// </summary>
    /// <param name="handler">Handler to set player cookie.</param>
    public void SetPlayerCookie(CCSPlayerController player, int cookieId, string name);

    /// <summary>
    /// Set the value of a steamId preference cookie.
    /// </summary>
    /// <param name="handler">Handler to set steamId cookie.</param>
    public void SetPlayerCookie(string steamId, int cookieId, string name);
    
    /// <summary>
    /// Get the value of a Player preference cookie.
    /// </summary>
    /// <param name="handler">Handler to set player cookie.</param>
    public string GetPlayerCookie(CCSPlayerController player, int cookieId);

    /// <summary>
    /// Checks if a players cookies have been loaded from the database.
    /// </summary>
    /// <param name="handler">Handler to check if player cookies are loaded.</param>
    public bool ArePlayerCookiesCached(CCSPlayerController player);

    /// <summary>
    /// Add a new prefab item to the client cookie settings menu.
    /// Note: This handles everything automatically and does not require a callback
    /// </summary>
    /// <param name="handler">Handler to set prefab menu.</param>
    public void SetCookiePrefabMenu(int cookieId, CookieMenu type, string display, Action<CCSPlayerController, CookieMenuAction, string> cookieMenuHandler);

    /// <summary>
    /// Adds a new item to the client cookie settings menu.
    /// Note: This only adds the top level menu item. You need to handle any submenus from the callback.
    /// </summary>
    /// <param name="handler">Handler to set cookie menu item.</param>
    public void SetCookieMenuItem(Action<CCSPlayerController, CookieMenuAction, string> cookieMenuHandler, string display);

    /// <summary>
    /// Displays the settings menu to a player.
    /// </summary>
    /// <param name="handler">Handler to display cookie menu to player.</param>
    public void ShowCookieMenu(CCSPlayerController player);
}