# WPF_Engine
2D game on WPF
## Commands
|   Command   | Syntax  |     Description     |
|:------------|:--------|:--------------------|
|`clear`      |`<null>`|Clear chat            |
|`exit`       |`<null>`|Shutdown game         |
|`start`      |`<null>`|Start game            |
|`help`       |`<null>`|Displays help         |
|`location`   |`<null>`|Current location      |
|`fullscreen` |`<on/off>`|Fullscreen mode     |
|`spawn` |`<s_models/NPCs[weaponID, side(1/-1)], objID>`|Spawns NPC or material object|
|`remove`|`<s_models/NPCs, objID>`|Removes NPC or material object]
|`background`|`<image_name.imageformat>`|Game background|
|`tp`        |`<s_models/NPCs/Player, y, x>`|Teleports player/NPC/material object to the specified position|
|`slot_0`    |`<weaponID>`|Places primary weapon|
|`slot_1`    |`<weaponID>`|Places secondary  weapon|
|`hp`        |`<null>`|Displays player's health|
|`hud_draw`  |`<on/off>`|Draws a HUD|
|`cartridge` |`<pistol/rifle>`|Get cartridges for a rifle or pistol|
|`chat`      |`<set_color, Color>`|Sets chat color|
|`exec`      |`<file_name.cfg>`|Runs configuration files|
|`bind`      |`<funcID, keyID>`|Binds key to a specific action (for `keyID` see [here](https://docs.microsoft.com/ru-ru/dotnet/api/system.windows.input.key?view=netframework-4.8 "MS Docs: Key Enum"))|

##`System.ini`
```
[colt_45]
class             = WEAPON
name              = Colt .45
cartridge_name    = .45 ACP
sound             = weapons\sounds\colt1911_fire.ogg
skin              = weapons/colt_45.png,weapons/colt_45_fire.png
in_magazine       = 10
in_magazine_count = 0
cartridge_count   = 30
damage            = 25
[colt_45_2]
class             = WEAPON
name              = Colt .45
cartridge_name    = .45 ACP
sound             = weapons\sounds\colt1911_fire.ogg
skin              = weapons/colt_45.png,weapons/colt_45_fire.png
in_magazine       = 10
in_magazine_count = 0
cartridge_count   = 20
damage            = 25
[m40a1]
class             = WEAPON
name              = Remington M40A1
cartridge_name    = 7,62x51 NATO
sound             = weapons\sounds\r700_fire.ogg
skin              = weapons/m40a1.png
in_magazine       = 5
in_magazine_count = 0
cartridge_count   = 15
damage            = 100
[spas_12]
class             = WEAPON
name              = SPAS-12
cartridge_name    = 12x70мм
sound             = weapons\sounds\spas12_fire.ogg
skin              = weapons/spas_12.png,weapons/spas_12_fire.png
in_magazine       = 8
in_magazine_count = 2
cartridge_count   = 0
damage            = 50
[player]
class             = PLAYER
health            = 100
position          = 129,0
visual            = player_skin/player_coat.png
size              = 283,122
[bot]
class             = NPC
health            = 150
alive             = true
position          = 129,630
weapon            = 3
visual            = player_skin/player_armored.png
size              = 283,122
[bot_enemy]
class             = NPC
health            = 150
alive             = true
position          = 129,0
weapon            = 1
visual            = player_skin/player_armored.png
size              = 283,122
[weapon_obj] 
class             = NPC_WEAPON
size              = 73,78
owner             = 0
[weapon_obj_2] 
class             = NPC_WEAPON
size              = 73,78
owner             = 1
[weapon_table]
cr_position       = 5,4
#content           = 
font              = Consolas
foreground        = #FFFFFF
background        = #000000  
[chat]
cr_position       = 0,0
cr_span           = 2,6
content           = WPF__Engine v0.9
font              = Consolas
foreground        = White
[bullet_obj] 
class             = D_OBJECT  
position          = 194,82
size              = 73,78
visual            = objects/bullet.png
[woodenbox_obj]
class             = OBJECT  
cr_position       = 3,4
cr_span           = 1,4
visual            = objects/wooden_box.png
[play]
cr_position       = 1,4
content           = Начать игру
[authors]
cr_position       = 1,5
content           = Титры
```

