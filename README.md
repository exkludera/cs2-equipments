# cs2-equipments
**a plugin that allows players to equip, models, particles & weapons (like hats, backpacks, trails & custom weapon models)**

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

**AllowMultiple** - Default: `false` (false = only 1 selection per category, true = can equip all at the same time) <br>
**Permission** - Default: `""` (empty for no check, @css/reservation for vip) <br>
**Team** - Default: `""` (T for Terrorist, CT for CounterTerrorist or empty for both) <br>

**Name** - Default: `"Model Name"` (the title of the item in the menu) <br>
**Model** - Default: `""` (model file the player will equip) <br>
**Particle** - Default: `""` (particle file the player will equip) <br>
**Weapon** - Default: `""` (weapon model file, use weapon name split by `:` then file `weapon_awp:models/example.vmdl`) <br>

```json
{
  "Prefix": "{orange}[Equipments]{default}",
  "MenuCommands": "css_equipments,css_equipment",
  "MenuType": "html",
  "MenuBackButton": false,
  "Permission": "",
  "Team": "",
  "Categories": {
    "Hats": {
      "AllowMultiple": false,
      "Equipment": [
        {
          "Name": "Hat #1",
          "Model": "models/example_1.vmdl"
        },
        {
          "Name": "Hat #2",
          "Model": "models/example_2.vmdl"
        }
      ]
    },
    "Particles": {
      "AllowMultiple": true,
      "Permission": "@css/reservation",
      "Team": "CT",
      "Equipment": [
        {
          "Name": "Particle #1",
          "Particle": "particles/example_1.vpcf"
        },
        {
          "Name": "Particle #2",
          "Particle": "particles/example_2.vpcf"
        }
      ]
    },
    "Weapons": {
      "AllowMultiple": true,
      "Equipment": [
        {
          "Permission": "@css/root",
          "Team": "T",
          "Name": "Custom AK47",
          "Weapon": "weapon_ak47:models/example_ak47.vmdl"
        },
        {
          "Name": "Custom AWP",
          "Weapon": "weapon_awp:models/example_awp.vmdl"
        }
      ]
    }
  }
}
```

<br> <a href="https://ko-fi.com/exkludera" target="blank"><img src="https://cdn.ko-fi.com/cdn/kofi5.png" height="48px" alt="Buy Me a Coffee at ko-fi.com"></a>
