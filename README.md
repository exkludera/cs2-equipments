# cs2-equipments
**this plugin makes players able to equip models (like hats and trails)**

<br>

<details>
	<summary>showcase</summary>
	<img src="https://github.com/exkludera/cs2-equipments/assets/51145038/37b60f6f-e1c3-4257-aee8-4bea23e8735a" width="200"> <br>
	<img src="https://github.com/exkludera/cs2-equipments/assets/51145038/a7eb7832-6c3a-4edb-81cd-a38b3763044d" width="178">
	<img src="https://github.com/exkludera/cs2-equipments/assets/51145038/e5ba25cf-4f31-4379-bbf5-139c00cb6f56" width="200">
</details>

<br>

## information:


### requirements
- [MetaMod](https://github.com/alliedmodders/metamod-source)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [Cruze03/Clientprefs](https://github.com/Cruze03/Clientprefs)

<br>

> [!WARNING]
> models must have a player bone or it will not attach to the player

<br>

## example config

**MenuType** - Default: `"html"` (options: chat/html/wasd) <br>

**Permission** - Default: `""` (empty for no check, @css/reservation for vip) <br>
**Team** - Default: `""` (T for Terrorist, CT for CounterTerrorist or empty for both) <br>

**Name** - Default: `"Model Name"` (the title of the item in the menu) <br>
**File** - Default: `"models/example.vmdl"` (the file of the model the player will equip) <br>

```json
{
  "Prefix": "{orange}[Equipments]{default}",
  "MenuCommands": "css_equipments,css_equipment",
  "MenuType": "html",
  "MenuBackButton": false,
  "Permission": "",
  "Team": "",
  "Menu": {
    "1": {
      "Title": "Hats",
      "Models": [
        {
          "Name": "hat",
          "File": "models/hat.vmdl"
        },
		{
          "Name": "hat 2",
          "File": "models/hat_2.vmdl"
        }
      ]
    },
    "2": {
      "Title": "Backpacks",
      "Permission": "@css/reservation",
      "Team": "CT",
      "Models": [
        {
          "Name": "backpack",
          "File": "models/backpack.vmdl"
        }
      ]
    }
  }
}
```

<br> <a href="https://ko-fi.com/exkludera" target="blank"><img src="https://cdn.ko-fi.com/cdn/kofi5.png" height="48px" alt="Buy Me a Coffee at ko-fi.com"></a>