using System;
using BepInEx;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using System.Collections;
using BepInEx.Configuration;
using UnityEngine.Networking;

namespace DropInMultiplayer
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.SushiDev.DropInMultiplayer", "DropInMultiplayer", "3.0.1")]
    //makes sure that this mod loads after roguewisp to prevent issues
    //unused until rein changes body naem
    //[BepInDependency("com.ReinThings.RogueWisp", BepInDependency.DependencyFlags.SoftDependency)]
    public class DropInMultiplayer : BaseUnityPlugin
    {
        //configx
        private static ConfigEntry<bool> ImmediateSpawn { get; set; }
        private static ConfigEntry<bool> NormalSurvivorsOnly { get; set; }
        private static ConfigEntry<bool> AllowSpawnAsWhileAlive { get; set; }
        private static ConfigEntry<bool> StartWithItems { get; set; }
        public static ConfigEntry<bool> SpawnAsEnabled { get; set; }
        public static ConfigEntry<bool> HostOnlySpawnAs { get; set; }
        public static ConfigEntry<bool> GiveLunarItems { get; set; }
        public static ConfigEntry<bool> GiveRedItems { get; set; }
        public static ConfigEntry<bool> GiveBossItems { get; set; }
        public static ConfigEntry<bool> GiveExactItems { get; set; }

        //unused bool for roguewisp
        //private bool RogueWisp = false;
        private static DropInMultiplayer instance { get; set; }
        //survivor list for NormalSurvivorsOnly 
        //should probably just use the in-game enum
        public static List<string> survivorList = new List<string>{
            "CommandoBody",
            "HuntressBody",
            "EngiBody",
            "ToolbotBody",
            "MercBody",
            "MageBody",
            "BanditBody",
            "TreebotBody",
            "LoaderBody",
            "CrocoBody",
        };
        //body list, it also contains aliases for bodies
        public static List<List<string>> bodyList = new List<List<string>> {
            new List<string> { "AssassinBody", "Assassin"},
            new List<string> { "CommandoBody", "Commando"},
            new List<string> { "HuntressBody", "Huntress"},
            new List<string> { "EngiBody", "Engi", "Engineer"},
            new List<string> { "ToolbotBody", "Toolbot", "MULT", "MUL-T"},
            new List<string> { "MercBody", "Merc", "Mercenary"},
            new List<string> { "MageBody", "Mage", "Artificer", "Arti"},
            new List<string> { "BanditBody", "Bandit"},
            new List<string> { "SniperBody", "Sniper"},
            new List<string> { "HANDBody", "HAND", "HAN-D"},
            new List<string> { "TreebotBody", "Support", "Rex"},
            new List<string> { "LoaderBody", "Loader", "Loadie"},
            new List<string> { "CrocoBody", "Croco", "Acrid"},
            //drones
            new List<string> { "BackupDroneBody", "BackupDrone"},
            new List<string> { "BackupDroneOldBody", "BackupDroneOld"},
            new List<string> { "Drone1Body", "GunnerDrone", "Gunner"},
            new List<string> { "Drone2Body", "HealingDrone", "Healer"},
            new List<string> { "FlameDroneBody", "FlameDrone"},
            new List<string> { "MegaDroneBody", "TC280Prototype"},
            new List<string> { "MissileDroneBody", "MissileDrone"},
            new List<string> { "EquipmentDroneBody", "EquipmentDrone", "EquipDrone"},
            new List<string> { "EmergencyDroneBody", "EmergencyDrone", "EmDrone"},
            //everything else
            new List<string> { "AltarSkeletonBody", "AltarSkeleton"},
            new List<string> { "AncientWispBody", "AncientWisp"},
            new List<string> { "ArchWispBody", "ArchWisp"},
            new List<string> { "BeetleBody", "Beetle"},
            new List<string> { "BeetleGuardAllyBody", "BeetleGuardAlly"},
            new List<string> { "BeetleGuardBody", "BeetleGuard"},
            new List<string> { "BeetleQueen2Body", "BeetleQueen"},
            new List<string> { "BellBody", "Bell"},
            new List<string> { "BirdsharkBody", "Birdshark"},
            new List<string> { "BisonBody", "Bison"},
            new List<string> { "BomberBody", "Bomber"},
            new List<string> { "ClayBody", "Clay"},
            new List<string> { "ClayBossBody", "ClayBoss"},
            new List<string> { "ClayBruiserBody", "ClayBruiser"},
            new List<string> { "CommandoPerformanceTestBody", "CommandoPerformanceTest"},
            new List<string> { "ElectricWormBody", "ElectricWorm"},
            new List<string> { "EnforcerBody", "Enforcer"},
            new List<string> { "EngiBeamTurretBody", "EngiBeamTurret"},
            new List<string> { "EngiTurretBody", "EngiTurret"},
            new List<string> { "ExplosivePotDestructibleBody", "ExplosivePotDestructible"},
            new List<string> { "FusionCellDestructibleBody", "FusionCellDestructible"},
            new List<string> { "GolemBody", "Golem"},
            new List<string> { "GolemBodyInvincible", "GolemInvincible"},
            new List<string> { "GravekeeperBody", "Gravekeeper"},
            new List<string> { "GreaterWispBody", "GreaterWisp"},
            new List<string> { "HaulerBody", "Hauler"},
            new List<string> { "HermitCrabBody", "HermitCrab"},
            new List<string> { "ImpBody", "Imp"},
            new List<string> { "ImpBossBody", "ImpBoss"},
            new List<string> { "JellyfishBody", "Jellyfish"},
            new List<string> { "LemurianBody", "Lemurian"},
            new List<string> { "LemurianBruiserBody", "LemurianBruiser"},
            new List<string> { "MagmaWormBody", "MagmaWorm"},
            new List<string> { "PaladinBody", "Paladin"},
            new List<string> { "Pot2Body", "Pot2"},
            new List<string> { "PotMobile2Body", "PotMobile2"},
            new List<string> { "PotMobileBody", "PotMobile"},
            new List<string> { "ShopkeeperBody", "Shopkeeper"},
            new List<string> { "SpectatorBody", "Spectator"},
            new List<string> { "SpectatorSlowBody", "SpectatorSlow"},
            new List<string> { "SquidTurretBody", "SquidTurret"},
            new List<string> { "TimeCrystalBody", "TimeCrystal"},
            new List<string> { "TitanBody", "Titan"},
            new List<string> { "TitanGoldBody", "TitanGold"},
            new List<string> { "Turret1Body", "GunnerTurret"},
            new List<string> { "UrchinTurretBody", "UrchinTurret"},
            new List<string> { "VagrantBody", "Vagrant"},
            new List<string> { "VultureBody", "Vulture" },
            new List<string> { "SuperRoboBallBossBody", "AWU" },
            new List<string> { "RoboBallMiniBody", "Solus", "Probe"},
            new List<string> { "ScavBody", "Scav", "Scavenger"},
            new List<string> { "NullifierBody", "VoidReaver", "Reaver", "Void"},
        };
        //thanks hopoo
        public static List<ItemIndex> bossitemList = new List<ItemIndex>{
            ItemIndex.NovaOnLowHealth,
            ItemIndex.Knurl,
            ItemIndex.BeetleGland,
            ItemIndex.TitanGoldDuringTP,
            ItemIndex.SprintWisp,
            //Excluding pearls because those aren't boss items, they come from the Cleansing Pool 
        };
        /*
        public static List<ItemIndex> WorldUnique = new List<ItemIndex>{
            ItemIndex.Pearl,
            ItemIndex.ShinyPearl,
            //Yes, I do know TitanGoldDuringTP is in the WorldUnique catagory, but you have to fight a boss for it. Hence why it's not in this list
            //Suddenly, I realize that this list is unneeded.
        };
        */
        public static string GetBodyNameFromString(string name)
        {

            foreach (var bodyLists in bodyList)
            {
                foreach (var tempName in bodyLists)
                {
                    if (tempName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return bodyLists[0];
                    }
                }
            }
            return name;
        }
        //fetches the body name from string
        //should work but i'm not too sure, i need to learn more about enums
        /*public static string GetBodyNameFromString(string name)
        {

            foreach (var bodyLists in (SurvivorIndex[])Enum.GetValues(typeof(SurvivorIndex)))
            {
                foreach (var tempName in (SurvivorIndex[])Enum.GetValues(typeof(SurvivorIndex)))
                {
                    if (SurvivorIndex.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return bodyLists[1];
                    }
                }   
            }

            return name;
        }*/

        public void Awake()
        {
            Debug.Log("Created by SushiDev! Derived from DropInMultiplayer by Morris1927 with permission.");
            if (instance == null)
            {
                instance = this;
            }
            else
                Destroy(this);

            ImmediateSpawn = Config.Bind("Enable/Disable", "ImmediateSpawn", false, "Enables or disables immediate spawning as you join");
            NormalSurvivorsOnly = Config.Bind("Enable/Disable", "NormalSurvivorsOnly", true, "Changes whether or not spawn_as can only be used to turn into survivors");
            StartWithItems = Config.Bind("Enable/Disable", "StartWithItems", true, "Enables or disables giving players items if they join mid-game");
            AllowSpawnAsWhileAlive = Config.Bind("Enable/Disable", "AllowSpawnAsWhileAlive", true, "Enables or disables players using spawn_as while alive");
            SpawnAsEnabled = Config.Bind("Enable/Disable", "SpawnAs", true, "Enables or disables the spawn_as command");
            HostOnlySpawnAs = Config.Bind("Enable/Disable", "HostOnlySpawnAs", false, "Changes the dim_spawn_as command to be host only");
            GiveLunarItems = Config.Bind("Enable/Disable", "GiveLunarItems", false, "Allows lunar items to be given to players, needs StartWithItems to be enabled!");
            GiveRedItems = Config.Bind("Enable/Disable", "GiveRedItems", true, "Allows red items to be given to players, needs StartWithItems to be enabled!");
            GiveBossItems = Config.Bind("Enable/Disable", "GiveRedItems", true, "Allows boss items to be given to players, needs StartWithItems to be enabled!");
            //GiveExactItems = Config.Bind("Enable/Disable", "GiveExactItems", false, "Chooses a random member in the game and gives the new player their items, should be used with ShareSuite, needs StartWithItems to be enabled!");

            //support for wispy, coming when rein changes the body name
            /*if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ReinThings.RogueWisp"))
            {
                RogueWisp = true;
                Logger.LogWarning("RogueWisp detected: Attempting to add wispy to the survivor list!");
                bodyList.Add(new List<string> { "RogueWispBody"});
                bodyList.Add(new List<string> { "RogueWispBody", "RogueWisp", "Wispy" });
            }*/
            //registers commands
            On.RoR2.Console.Awake += (orig, self) =>
            {
                Utilities.Generic.CommandHelper.RegisterCommands(self);
                orig(self);
            };
            //config stuff
            if (ImmediateSpawn.Value)
            {
                On.RoR2.Run.Start += (orig, self) =>
                {
                    orig(self);
                    self.SetFieldValue("allowNewParticipants", true);
                };
            }
            //this hooks when someone joins so it can send the welcome message
            On.RoR2.NetworkUser.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && Stage.instance != null)
                    AddChatMessage("Hello " + self.userName + "! Join the game by typing 'dim_spawn_as [name]' (without the apostrophes of course) into the chat. Names are Commando, Huntress, Engi, Artificer, Merc, MUlT, Bandit, Rex, Loader, and Acrid!", 10f);
            };

            On.RoR2.Run.SetupUserCharacterMaster += SetupUserCharacterMaster;
            On.RoR2.Chat.UserChatMessage.ConstructChatString += UserChatMessage_ConstructChatString;
        }
        //adds a chat message, this is how the hello message functions
        private static void AddChatMessage(string message, float time = 0.1f)
        {
            instance.StartCoroutine(AddHelperMessage(message, time));
        }

        private static IEnumerator AddHelperMessage(string message, float time)
        {
            yield return new WaitForSeconds(time);
            var chatMessage = new Chat.SimpleChatMessage { baseToken = message };
            Chat.SendBroadcastChat(chatMessage);

        }
        //hooks the chat so sending stuff like "dim_spawn_as Loadie" works
        private string UserChatMessage_ConstructChatString(On.RoR2.Chat.UserChatMessage.orig_ConstructChatString orig, Chat.UserChatMessage self)
        {

            if (!NetworkServer.active)
            {
                return orig(self);
            }

            List<string> split = new List<string>(self.text.Split(Char.Parse(" ")));
            string commandName = ArgsHelper.GetValue(split, 0);

            if (commandName.Equals("dim_spawn_as", StringComparison.OrdinalIgnoreCase))
            {


                string bodyString = ArgsHelper.GetValue(split, 1);
                string userString = ArgsHelper.GetValue(split, 2);


                SpawnAs(self.sender.GetComponent<NetworkUser>(), bodyString, userString);
            }
            return orig(self);
        }

        [Server]
        //this is spawnas, this is pretty much the foundation of the mod
        private static void SpawnAs(NetworkUser user, string bodyString, string userString)
        {

            if (!SpawnAsEnabled.Value)
            {
                return;
            }

            if (HostOnlySpawnAs.Value)
            {
                if (NetworkUser.readOnlyInstancesList[0].netId != user.netId)
                {
                    return;
                }
            }

            bodyString = GetBodyNameFromString(bodyString);
            
            GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(bodyString);
            //bodyPrefab.GetComponent<CharacterBody>().AddTimedBuff(BuffIndex.HiddenInvincibility, 10);
            //This line bricks SpawnAs, I have no idea why, but it does!
            //warning message
            if (bodyPrefab == null)
            {
                AddChatMessage("Could not find " + bodyString + ", options for dim_spawn_as are Commando, Huntress, Engi, Mage, Merc, Toolbot, Bandit, Rex, Loader, and Acrid.");
                return;
            }
            
            //normalsurvivorsonly 
            if (NormalSurvivorsOnly.Value)
            {
                if (!survivorList.Contains(bodyString))
                {
                    AddChatMessage("You can only spawn as normal survivors");
                    return;
                }
            }

            NetworkUser player = GetNetUserFromString(userString);
            player = player ?? user;
            CharacterMaster master = player.master;
            //
            if (master)
            {
                if (AllowSpawnAsWhileAlive.Value && master.alive)
                {
                    master.bodyPrefab = bodyPrefab;
                    string bodystringname;
                    bodystringname = Resources.Load<GameObject>($"prefabs/characterbodies/" + bodyString + "").GetComponent<CharacterBody>().baseNameToken;
                    master.Respawn(master.GetBody().transform.position, master.GetBody().transform.rotation);
                    AddChatMessage(player.userName + " is spawning as " + bodystringname + "!");
                }
                else if (!master.alive)
                {
                    AddChatMessage("You can't use dim_spawn_as while dead.");
                }
                else if (!AllowSpawnAsWhileAlive.Value && master.alive)
                {
                    AddChatMessage("You can't use dim_spawn_as while alive.");
                }
            }
            else
            {
                //this handles spawns
                Run.instance.SetFieldValue("allowNewParticipants", true);
                Run.instance.OnUserAdded(user);

                user.master.bodyPrefab = bodyPrefab;

                Transform spawnTransform = Stage.instance.GetPlayerSpawnTransform();
                CharacterBody body = user.master.SpawnBody(bodyPrefab, spawnTransform.position, spawnTransform.rotation);

                Run.instance.HandlePlayerFirstEntryAnimation(body, spawnTransform.position, spawnTransform.rotation);
                AddChatMessage(player.userName + " spawning as " + bodyString);
                if (!ImmediateSpawn.Value)
                    Run.instance.SetFieldValue("allowNewParticipants", false);
            }

        }
        //this makes putting a username aftewr dim_spawn_as [name] work. 
        private static NetworkUser GetNetUserFromString(string playerString)
        {
            int result = 0;
            if (playerString != "")
            {
                if (int.TryParse(playerString, out result))
                {
                    if (result < NetworkUser.readOnlyInstancesList.Count && result >= 0)
                    {

                        return NetworkUser.readOnlyInstancesList[result];
                    }
                    Debug.Log("Specified player index does not exist");
                    return null;
                }
                else
                {
                    foreach (NetworkUser n in NetworkUser.readOnlyInstancesList)
                    {
                        if (n.userName.Equals(playerString, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return n;
                        }
                    }
                    return null;
                }
            }

            return null;
        }


        private void SetupUserCharacterMaster(On.RoR2.Run.orig_SetupUserCharacterMaster orig, Run self, NetworkUser user)
        {
            orig(self, user);
            //this setups up the chactermaster, as you can see
            //it also gives out items, i still fetch t3 and tl regardless of the config option, the config just dicates if i give the items
            if (!StartWithItems.Value || Run.instance.fixedTime < 5f)
            {
                return;
            }

            //the way i did this is confusing so i'm just gonna explain these int names
            /*
            averageItemCountT1 = average item count tier 1 
            averageItemCountT2 = average item count tier 2 
            averageItemCountT3 = average item count tier 3
            averageItemCountTL = average item count tier lunar
            averageItemCountTB = average item count tier boss
            yes i do know lunar isn't really a tier but shhhhhhhhhhh
            */
            int averageItemCountT1 = 0;
            int averageItemCountT2 = 0;
            int averageItemCountT3 = 0;
            int averageItemCountTL = 0;
            int averageItemCountTB = 0;

            ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;

            int playerCount = PlayerCharacterMasterController.instances.Count;
            if (playerCount <= 1)
                return;
            else
                playerCount--;

            for (int i = 0; i < readOnlyInstancesList.Count; i++)
            {
                if (readOnlyInstancesList[i].id.Equals(user.id))
                    continue;
                CharacterMaster cm = readOnlyInstancesList[i].master;
                averageItemCountT1 += cm.inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
                averageItemCountT2 += cm.inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
                averageItemCountT3 += cm.inventory.GetTotalItemCountOfTier(ItemTier.Tier3);
                averageItemCountTL += cm.inventory.GetTotalItemCountOfTier(ItemTier.Lunar);
                averageItemCountTB += cm.inventory.GetTotalItemCountOfTier(ItemTier.Boss);
            }

            averageItemCountT1 /= playerCount;
            averageItemCountT2 /= playerCount;
            averageItemCountT3 /= playerCount;
            averageItemCountTL /= playerCount;
            averageItemCountTB /= playerCount;

            CharacterMaster characterMaster = user.master;
            int itemCountT1 = averageItemCountT1 - characterMaster.inventory.GetTotalItemCountOfTier(ItemTier.Tier1);
            int itemCountT2 = averageItemCountT2 - characterMaster.inventory.GetTotalItemCountOfTier(ItemTier.Tier2);
            int itemCountT3 = averageItemCountT3 - characterMaster.inventory.GetTotalItemCountOfTier(ItemTier.Tier3);
            int itemCountTL = averageItemCountTL - characterMaster.inventory.GetTotalItemCountOfTier(ItemTier.Lunar);
            int itemCountTB = averageItemCountTL - characterMaster.inventory.GetTotalItemCountOfTier(ItemTier.Boss);

            itemCountT1 = itemCountT1 < 0 ? 0 : itemCountT1;
            itemCountT2 = itemCountT2 < 0 ? 0 : itemCountT2;
            itemCountT3 = itemCountT3 < 0 ? 0 : itemCountT3;
            itemCountTL = itemCountTL < 0 ? 0 : itemCountTL;
            itemCountTB = itemCountTB < 0 ? 0 : itemCountTB;

            /*
            if (GiveExactItems.Value)
            {
                characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.tier1ItemList), itemCountT1);
                characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.tier2ItemList), itemCountT2);
                if (GiveRedItems.Value)
                {
                    characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.tier3ItemList), itemCountT3);
                }
                if (GiveLunarItems.Value)
                {
                    characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.lunarItemList), itemCountTL);
                }
            }*/
            Debug.Log(itemCountT1 + " " + itemCountT2 + " " + itemCountT3 + " itemcount to add");
            Debug.Log(averageItemCountT1 + " " + averageItemCountT2 + " " + averageItemCountT3 + " average");
            for (int i = 0; i < itemCountT1; i++)
            {
                characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.tier1ItemList), 1);
            }
            for (int i = 0; i < itemCountT2; i++)
            {
                characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.tier2ItemList), 1);
            }
            if (GiveRedItems.Value)
            {
                for (int i = 0; i < itemCountT3; i++)
                {
                    characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.tier3ItemList), 1);
                }
            }
            if (GiveLunarItems.Value)
            {
                for (int i = 0; i < itemCountTL; i++)
                {
                    characterMaster.inventory.GiveItem(GetRandomItem(ItemCatalog.lunarItemList), 1);
                }
            }
            if (GiveBossItems.Value)
            {
                for (int i = 0; i < itemCountTB; i++)
                {
                    characterMaster.inventory.GiveItem(GetRandomItem(bossitemList), 1);
                }
            }
        }
        //this is how i get the random item, it just cycles through the itemindex for something to give
        private ItemIndex GetRandomItem(List<ItemIndex> items)
        {
            int itemID = UnityEngine.Random.Range(0, items.Count);

            return items[itemID];
        }

        /*
        private ItemTier GetRandomItemButBadly(List<ItemTier> itemT)
        {
            int itemTID = UnityEngine.Random.Range(0, itemT.Count);

            return itemT[itemTID];
        }
        */

        //this is the command
        [ConCommand(commandName = "dim_spawn_as", flags = ConVarFlags.ExecuteOnServer, helpText = "Spawn as a new character. Type body_list for a full list of characters")]
        private static void CCSpawnAs(ConCommandArgs args)
        {
            if (args.Count == 0)
            {
                return;
            }

            string bodyString = ArgsHelper.GetValue(args.userArgs, 0);
            string playerString = ArgsHelper.GetValue(args.userArgs, 1);

            SpawnAs(args.sender, bodyString, playerString);

        }
        //gets the player list + ids
        [ConCommand(commandName = "player_list", flags = ConVarFlags.ExecuteOnServer, helpText = "Shows list of players with their ID")]
        private static void CCPlayerList(ConCommandArgs args)
        {
            NetworkUser n;
            for (int i = 0; i < NetworkUser.readOnlyInstancesList.Count; i++)
            {
                n = NetworkUser.readOnlyInstancesList[i];
                Debug.Log(i + ": " + n.userName);
            }
        }
    }
    //args helper
    public class ArgsHelper
    {

        public static string GetValue(List<string> args, int index)
        {
            if (index < args.Count && index >= 0)
            {
                return args[index];
            }

            return "";
        }
    }
}