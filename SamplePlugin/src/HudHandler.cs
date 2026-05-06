using Dalamud.IoC;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using HolyScreenSaver;
using HolyScreenSaver.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace HolyScreenSaver.HudHandler
{
    public class HudHandler
    {
        [PluginService] internal static IPlayerState PlayerState { get; private set; } = null!;

        private readonly IGameGui gameGui;
        public HudHandler(IGameGui gameGui)
        {
            this.gameGui = gameGui;
        }
        public unsafe Vector2? GetHotbarPosition(string hudElementName)
        {
            var addonPtrWrapper = this.gameGui.GetAddonByName(hudElementName, 1);
            nint address = (nint)addonPtrWrapper;
            
            if (address == nint.Zero)
                return null;

            var addon = (FFXIVClientStructs.FFXIV.Component.GUI.AtkUnitBase*)address;
            return new Vector2(addon->X, addon->Y);
        }
        public unsafe void SetHotbarPosition(string hudElementName, Vector2 newPos)
        {
            var addonPtrWrapper = this.gameGui.GetAddonByName(hudElementName, 1);
            nint address = (nint)addonPtrWrapper;

            if (address == nint.Zero) return;

            var addon = (FFXIVClientStructs.FFXIV.Component.GUI.AtkUnitBase*)address;

            addon->X = (short)newPos.X;
            addon->Y = (short)newPos.Y;

            if (addon->RootNode != null)
            {
                addon->RootNode->X = newPos.X;
                addon->RootNode->Y = newPos.Y;

                addon->RootNode->IsDirty = true;
            }
        }
        
        public unsafe void UpdateElementPosition(HudElement element)
        {
            Vector2 nextPos = new Vector2(
                element.CurrentPosition.X + element.Velocity.X,
                element.CurrentPosition.Y + element.Velocity.Y
            );

            element.CurrentPosition = nextPos;

            SetHotbarPosition(element.ElementId, nextPos);
        }
        public unsafe void ResetElementPosition(HudElement element)
        {
            element.CurrentPosition = element.FirstPosition;

            element.Velocity = Vector2.Zero;

            SetHotbarPosition(element.ElementId, element.FirstPosition);
        }
        public void HandleBouncing(HudElement element, float screenWidth, float screenHeight)
        {
            float vx = element.Velocity.X;
            float vy = element.Velocity.Y;
            float px = element.CurrentPosition.X;
            float py = element.CurrentPosition.Y;

            if (px <= 0 || px >= screenWidth)
            {
                vx = -vx;

                px = Math.Clamp(px, 1, screenWidth - 1);
            }

            if (py <= 0 || py >= screenHeight)
            {
                vy = -vy;

                py = Math.Clamp(py, 1, screenHeight - 1);
            }
            element.Velocity = new Vector2(vx, vy);
            element.CurrentPosition = new Vector2(px, py);
        }
    }
}
