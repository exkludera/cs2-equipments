# cs2-equipments

**players can equip models like hats, wings, backpacks or similar models.**

<br>

<details>
	<summary>showcase</summary>
	<img src="https://github.com/exkludera/cs2-equipments/assets/51145038/37b60f6f-e1c3-4257-aee8-4bea23e8735a" width="200"> <br>
	<img src="https://github.com/exkludera/cs2-equipments/assets/51145038/a7eb7832-6c3a-4edb-81cd-a38b3763044d" width="178">
	<img src="https://github.com/exkludera/cs2-equipments/assets/51145038/e5ba25cf-4f31-4379-bbf5-139c00cb6f56" width="200">
</details>

item config example:
```
"Items": {
   "hat": {
      "1": {
         "name": "hat 1",
         "uniqueid": "models/example.vmdl",
         "type": "hat",
         "slot": "1",
         "enable": "true"
      }
   }
},
```

<br>

> [!WARNING]
> models must have a player bone or it will not attach to the player

> [!IMPORTANT]
> almost all code is from [schwarper/cs2-store](https://github.com/schwarper/cs2-store) (all credits should go to their efforts)
