<div align="center">
  <img width="50" height="50" alt="cssharp" src="https://github.com/user-attachments/assets/3393573f-29be-46e1-bc30-fafaec573456" />
	<h3><strong>Equipments</strong></h3>
	<h4>a plugin that allows players to equip models, particles & weapons</h4>
	<h2>
		<img src="https://img.shields.io/github/downloads/exkludera-cssharp/equipments/total" alt="Downloads">
		<img src="https://img.shields.io/github/stars/exkludera-cssharp/equipments?style=flat&logo=github" alt="Stars">
		<img src="https://img.shields.io/github/forks/exkludera-cssharp/equipments?style=flat&logo=github" alt="Forks">
		<img src="https://img.shields.io/github/license/exkludera-cssharp/equipments" alt="License">
	</h2>
	<!--<a href="https://discord.gg" target="_blank"><img src="https://img.shields.io/badge/Discord%20Server-7289da?style=for-the-badge&logo=discord&logoColor=white" /></a> <br>-->
	<a href="https://ko-fi.com/exkludera" target="_blank"><img src="https://img.shields.io/badge/KoFi-af00bf?style=for-the-badge&logo=kofi&logoColor=white" alt="Buy Me a Coffee at ko-fi.com" /></a>
	<a href="https://paypal.com/donate/?hosted_button_id=6AWPNVF5TLUC8" target="_blank"><img src="https://img.shields.io/badge/PayPal-0095ff?style=for-the-badge&logo=paypal&logoColor=white" alt="PayPal"  /></a>
	<a href="https://github.com/sponsors/exkludera" target="_blank"><img src="https://img.shields.io/badge/Sponsor-696969?style=for-the-badge&logo=github&logoColor=white" alt="GitHub Sponsor" /></a>
</div>

> [!WARNING]
> models must have a player bone or it will not attach to the player

## Requirements
- [MetaMod](https://github.com/alliedmodders/metamod-source)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [Clientprefs](https://github.com/Cruze03/Clientprefs)
- [CS2MenuManager](https://github.com/schwarper/CS2MenuManager)

## Showcase
<details>
	<summary>content</summary>
	<img src="https://github.com/exkludera/cs2-equipments/assets/51145038/37b60f6f-e1c3-4257-aee8-4bea23e8735a" width="200"> <br>
	<img src="https://github.com/exkludera/cs2-equipments/assets/51145038/a7eb7832-6c3a-4edb-81cd-a38b3763044d" width="178">
	<img src="https://github.com/exkludera/cs2-equipments/assets/51145038/e5ba25cf-4f31-4379-bbf5-139c00cb6f56" width="200">
</details>

## Config

<details>
<summary>Equipments.json</summary>
	
**Menu** - Default: `"CenterHtmlMenu"` (ChatMenu/WasdMenu/PlayerMenu) <br>

**Command** - Default: `[""]` (command per category, example: `css_hats`) <br>
**AllowMultiple** - Default: `false` (false = only 1 selection per category, true = can equip all at the same time) <br>
**Permission** - Default: `[""]` (empty for no check, @css/reservation for vip) <br>
**Team** - Default: `""` (T for Terrorist, CT for CounterTerrorist or empty for both) <br>

**Name** - Default: `"Equipment Name"` (the title of the item in the menu) <br>
**Model** - Default: `""` (model file) <br>
**Particle** - Default: `""` (particle file) <br>
**Weapon** - Default: `""` (`weapon:subclass` example: `weapon_awp:weapon_awp_subclass`, subclasses are defined in `scripts/weapons.vdata`) <br>

```json
{
  "Prefix": "{orange}[Equipments]{default}",
  "Menu": {
    "Command": ["css_equipments", "css_equipment"],
    "MenuType": "CenterHtmlMenu",
    "Permission": ["@css/reservation"],
    "Team": "",
  },
  "Categories": {
    "Hats": {
      "Command": ["css_hats"],
      "Permission": ["@css/reservation"],
      "Team": "CT",
      "Equipment":
      [
        { "Name": "Hat #1", "Model": "models/hat_1.vmdl" },
        { "Name": "Hat #2", "Model": "models/hat_2.vmdl" }
      ]
    },
    "Particles": {
      "AllowMultiple": true,
      "Permission": ["@css/generic"],
      "Team": "T",
      "Equipment":
      [
        { "Name": "Particle #1", "Particle": "particles/particle_1.vpcf" },
        { "Name": "Particle #2", "Particle": "particles/particle_2.vpcf" }
      ]
    },
    "Weapons": {
      "AllowMultiple": true,
      "Permission": ["@css/root"],
      "Equipment":
      [
        { "Name": "Knife 1", "Weapon": "weapon_knife:weapon_knife_subclass1" },
        { "Name": "Knife 2", "Weapon": "weapon_knife:weapon_knife_subclass2" }
      ],
      "Category": {
        "CT Examples": {
          "Team": "CT",
          "Equipment":
          [
            { "Name": "AWP CT", "Weapon": "weapon_awp:weapon_awp_subclass_ct" }
          ]
        },
        "T Examples": {
          "Team": "T",
          "Equipment":
          [
            { "Name": "AWP T", "Weapon": "weapon_awp:weapon_awp_subclass_t" }
          ]
        }
      }
    }
  }
}
```
</details>
