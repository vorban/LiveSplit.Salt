﻿using System;
using System.Reflection;
using LiveSplit.Model;
using LiveSplit.UI.Components;

namespace LiveSplit.Salt
{
    // ReSharper disable once UnusedMember.Global
    public class SaltFactory : IComponentFactory
    {
        public IComponent Create(LiveSplitState state)
        {
            return new SaltComponent(state, ComponentName);
        }

        public string UpdateName => ComponentName;
        public string XMLURL => UpdateURL + "Components/LiveSplit.Salt.Updates.xml";
        public string UpdateURL => "https://raw.githubusercontent.com/vorban/LiveSplit.Salt/master/Livesplit.Salt/";
        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public string ComponentName => "Salt and Sanctuary Autosplitter v" + Version;
        public string Description => "Salt and Sanctuary Autosplitter";
        public ComponentCategory Category => ComponentCategory.Control;
    }
}
