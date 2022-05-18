using CitizenFX.Core;
using Nexd.ESX.Client;
using System;
using System.Collections.Generic;
using static CitizenFX.Core.Native.API;

namespace gflp10_propplacer
{
    public class Program : BaseScript
    {
        public Program()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private readonly List<int> _objects = new List<int>();

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            RegisterCommand("showPropMenu", new Action<int, List<object>, string>(ShowPropMenu), false);

            RegisterKeyMapping("showPropMenu", "Show Prop Menu", "keyboard", "F6");
        }

        private async void ShowPropMenu(int source, List<object> args, string raw)
        {
            ESX.UI.Menu.CloseAll();

            var menuElements = new List<ESX.UI.MenuElement>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < _objects.Count; index++)
            {
                menuElements.Add(new ESX.UI.MenuElement
                {
                    label = $"[{index}]",
                    value = index
                });
            }

            if (menuElements.Count < 10)
            {
                menuElements.Add(new ESX.UI.MenuElement
                {
                    label = "Create New",
                    value = "new"
                });
            }
            
            ESX.UI.Menu.Open("default", GetCurrentResourceName(), "props_menu", new ESX.UI.MenuData
            {
                title = "Default Menu Title",
                align = "top-left",
                elements = menuElements
            }, (data, menu) =>
            {
                if (data != null)
                {
                    menu.close();
                    if (data.current.value.ToString() == "new")
                    {
                        OpenCreatePropMenu();
                    }
                    else
                    {
                        OpenPropMenu(data.current.value);
                    }
                }
            }, (data, menu) =>
            {
                menu.close();
            });

            while (ESX.UI.Menu.IsOpen("default", GetCurrentResourceName(), "props_menu"))
            {
                ShowIds();

                await Delay(1);
            }
        }

        private void ShowIds()
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                int obj = _objects[i];

                string text = $"[{i}]";

                Vector3 entityCoords = GetEntityCoords(obj, false);

                Vector3 playerCoords = GetGameplayCamCoord();

                float dist = GetDistanceBetweenCoords(entityCoords.X, entityCoords.Y, entityCoords.Z, playerCoords.X,
                    playerCoords.Y, playerCoords.Z, true);

                float scale = 200 / (GetGameplayCamFov() * dist);

                SetTextColour(255, 255, 255, 255);
                SetTextScale(0.0f, scale);
                SetTextDropshadow(0, 0, 0, 0, 55);
                SetTextDropShadow();
                SetTextCentre(true);

                BeginTextCommandDisplayText("STRING");
                AddTextComponentSubstringPlayerName(text);
                SetDrawOrigin(entityCoords.X, entityCoords.Y, entityCoords.Z + 1, 0);
                EndTextCommandDisplayText(0.0f, 0.0f);
                ClearDrawOrigin();
            }
        }

        private void OpenPropMenu(int index)
        {
            var menuElements = new List<ESX.UI.MenuElement>
            {
                new ESX.UI.MenuElement
                {
                    label = "Move",
                    value = "move"
                },
                new ESX.UI.MenuElement
                {
                    label = "Delete",
                    value = "del"
                }
            };

            ESX.UI.Menu.Open("default", GetCurrentResourceName(), "prop_menu", new ESX.UI.MenuData
            {
                title = "Default Menu Title",
                align = "top-left",
                elements = menuElements
            }, async (data, menu) =>
            {
                int prop = _objects[index];
                if (data != null)
                {
                    menu.close();
                    if (data.current.value.ToString() == "move")
                    {
                        await Delay(500);
                        await PropHandler.MoveProp(prop);
                    }
                    else if (data.current.value.ToString() == "del")
                    {
                        DeleteObject(ref prop);
                        _objects.RemoveAt(index);
                    }
                }
            }, (data, menu) =>
            {
                menu.close();
            });
        }


        private void OpenCreatePropMenu()
        {
            ESX.UI.Menu.Open("dialog", GetCurrentResourceName(), "create_prop", new ESX.UI.MenuData
            {
                title = "Prop Name"
            }, async (data, menu) =>
            {
                menu.close();
                int obj = await PropHandler.CreateProp(data.value);
                _objects.Add(obj);
            }, (data, menu) =>
            {
                menu.close();
            });
        }
    }
}
