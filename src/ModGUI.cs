﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace ModGUI
{
    public class ModGUI : MonoBehaviour
    {
        static bool debugEnabled;
        static int currentScreen;
        static Vector2 scrollPosition;
        static Vector2 size;
        static Vector2 position;
        void Start()
        {
            currentScreen = 0;
            scrollPosition = Vector2.zero;
            debugEnabled = false;
            size = new Vector2(1000, 1000);
            position = new Vector2((Screen.width / 2) - (size.x / 2),
                                   (Screen.height / 2) - (size.x / 2));
        }

        void Update()
        {
            if (Input.GetKeyUp("insert"))
            {
                debugEnabled = !debugEnabled;
            }

        }

        void OnGUI()
        {
            if (debugEnabled)
            {
                GUI.ModalWindow(0, new Rect(position, size), DebugWindow, "[BWML]Debug Menu");
            }
        }

        void DebugWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            currentScreen = GUI.SelectionGrid(new Rect(25, 25, size.x-50, 75), currentScreen,
                                              new string[] { "Mods", "Logs"}, 2);
            if(currentScreen == 0)
            {
                ModWindow();
            }
            else
            {
                LogWindow();
            }
        }

        void LogWindow()
        {
            GUI.Label(new Rect(0, 100, 100, 25), "LogWindow");
            int logNum = 0;
            if (ModLoader.Utils.logs.Any())
            {
                scrollPosition = GUI.BeginScrollView(new Rect(0, 100, size.x, size.y - 100), scrollPosition, new Rect(0, 0, size.x, 50));
                foreach (string log in ModLoader.Utils.logs)
                {
                    logNum++;
                    GUI.Label(new Rect(0, 25 * logNum, 1000, 25), log);
                }
                GUI.EndScrollView();
            }
        }
        void ModWindow()
        {
            if (GUI.Button(new Rect(0, 100, 100, 25), "Reload all mods"))
            {
                ModLoader.Utils.RefreshModFiles();
            }
            scrollPosition = GUI.BeginScrollView(new Rect(0, 100, size.x, size.y-100), scrollPosition, new Rect(0, 0, size.x, 50));
            int modNum = 0;
            foreach (FileInfo file in ModLoader.Utils.allMods.Keys)
            {
                foreach (Type mod in ModLoader.Utils.allMods[file])
                {
                    modNum++;
                    GUI.Label(new Rect(0, modNum * 25, 100, 25), mod.Name);
                    if (!ModLoader.Utils.IsLoaded(mod))
                    {
                        if (GUI.Button(new Rect(100, modNum * 25, 100, 25), "Enable"))
                        {
                            ModLoader.Utils.Load(file);
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(100, modNum * 25, 100, 25), "Disable"))
                        {
                            ModLoader.Utils.Unload(file);
                        }
                }
                    if (GUI.Button(new Rect(200, modNum * 25, 100, 25), "Reload"))
                    {
                        ModLoader.Utils.Unload(file);
                        ModLoader.Utils.RefreshModFiles();
                    }
                }
            }
            GUI.EndScrollView();
        }
    }
}