using System.Linq;
using System.Numerics;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.IoC;
using Dalamud.Plugin;
using GameObjectStruct = FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject;
using Dalamud.Game.Gui;
using System;
using System.Collections.Generic;
using Lumina.Excel.GeneratedSheets;
using System.Globalization;

namespace BetterTargetPlugin
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "BetterTarget";
        private const string CommandName = "/btarget";
        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        private ChatGui ChatGui { get; set; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager,
            //Debug ChatGui
            [RequiredVersion("1.0")] ChatGui chatGui
        ) {
            PluginInterface = pluginInterface;
            CommandManager = commandManager;
            //Debug ChatGui
            ChatGui = chatGui;

            pluginInterface.Create<Service>();
            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Allows targetting closest entity that matches any term of a space-delimited list"
            });
        }

        public void Dispose()
        {
            CommandManager.RemoveHandler(CommandName);
        }

        private unsafe void OnCommand(string command, string args)
        {
            //create gameObjectList and items
            List<GameObjectList> gameObjectList = new List<GameObjectList>();
            GameObjectList gameObjectListItem;

            //Original code
            GameObject? closestMatch = null;
            var closestDistance = float.MaxValue;
            var player = Service.ClientState.LocalPlayer;
            var searchTerms = args.Split(" ");


            //parse String args
            int numericValue;
            var tryNumber = searchTerms.Last();
            bool isNumber = int.TryParse(tryNumber, out numericValue);
            if (isNumber)
            {
                searchTerms = searchTerms.SkipLast(1).ToArray();
            }
            else
            {
                numericValue = 1;
            }

            //Original code
            foreach (var actor in Service.Objects)
            {
                if (actor == null) continue;
                var valueFound = searchTerms.Any(searchName => actor.Name.TextValue.ToLowerInvariant().Contains(searchName.ToLowerInvariant()));
                if (valueFound && ((GameObjectStruct*)actor.Address)->GetIsTargetable())
                {
                    var distance = Vector3.Distance(player.Position, actor.Position);

                    //gameObjectListItem add matched item
                    gameObjectListItem = new GameObjectList();
                    gameObjectListItem.gameObject = actor;
                    gameObjectListItem.distance = distance;
                    gameObjectListItem.objectId = actor.ObjectId;
                    gameObjectList=gameObjectList.Append(gameObjectListItem).ToList();

                    //Original code
                    if (closestMatch == null)
                    {
                        closestMatch = actor;
                        closestDistance = distance;
                        continue;
                    }
                    if (closestDistance > distance)
                    {
                        closestMatch = actor;
                        closestDistance = distance;
                    }
                }
            }

            //debug result
            //foreach (var i in gameObjectList)
            //{
                //ChatGui.Print(i.objectId.ToString() + " " + i.distance.ToString());
            //}

            if (closestMatch != null)
            {
                if (numericValue == 1)
                {
                    //Original code
                    Service.Targets.SetTarget(closestMatch);
                }
                else
                {
                    var sorted = gameObjectList.OrderBy(p => p.distance).ThenBy(p => p.objectId).ToList();
                    Service.Targets.SetTarget(sorted[numericValue-1].gameObject);
                }
            }
        }
    }
}
