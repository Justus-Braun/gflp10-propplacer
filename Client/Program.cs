﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CitizenFX.Core;
using Nexd.ESX.Client;
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

            RegisterCommand("del", new Action<int, List<object>, string>((source, args, raw) =>
            {
                foreach (var obj in _objects)
                {
                    int x = obj;
                    DeleteObject(ref x);
                }
            }), false);
        }

        private void ShowPropMenu(int source, List<object> args, string raw)
        {
            ESX.UI.Menu.CloseAll();

            var menuElements = new List<ESX.UI.MenuElement>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var index = 0; index < _objects.Count; index++)
            {
                var obj = _objects[index];
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
                        menu.close();
                        await PropHandler.MoveProp(prop);
                    }
                    else if (data.current.value.ToString() == "del")
                    {
                        menu.close();
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