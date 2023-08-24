# UpgradePlus
UpgradePlus is an upgrade mod based on [Oempa's Equipment Upgrader mod](https://github.com/Levi2229/UpgradeEquipment)  

The Blacksmith is a TownNPC available after beating Eoc, EoW, or BoC. Fight bosses and strong mobs for tokens you can trade to upgrade your gear, take your favorite weapons deeper into the game or grind past difficult bosses.

## New Features
* Upgrade your armor, wings, and accessories
* Mana reduction added to mage weapons
* (Config) Autofire if your weapon is at or past a configurable level


## Improvements
* Items remember favorites  
Itemslot.Context swapped from `BankItem` to `InventoryItem`
* Right click the reforge button to buy max upgrades
* Preview the cost to reach the nearest level cap
* Serverside configs & more configs in general

## Known Bugs & Issues
* Calamity wipes the levels from gear when reforging from the goblin tinkerer  
Goblin tinkerer will refund items at standard rates when calamity is enabled to compensate
* Player projectiles from non-hotbar sources (accessories, armor bonuses, mounts, dual wielding mod, etc) inherit crit damage and rollover from the held item instead of their source
* Projectiles don't crit more often from the crit chance given by upgrading, crit rollover still works when they do crit
* Upgrades don't increase speed for wings in social slots (Antisocial mod) or WingSlot
* The upgrade slot shows a ¹ in the corner because it's an inventory slot internally

## Credits
https://github.com/Levi2229/UpgradeEquipment  
https://github.com/qwerty3-14/QwertysRandomContent

## Note: Building from Github
Due to namespace changes, tModloader will expect the mod folder to be named "UpgraderPlus", not "UpgradePlus".
