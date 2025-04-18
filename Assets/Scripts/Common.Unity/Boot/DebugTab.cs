﻿using System;
using Common.Lang.Observable;

namespace Common.Unity.Boot
{
    public class DebugTab
    {
        public UnicomDebug Manager;
        
        public object Model;
        
        public Type ViewType;
        
        public string Label;

        public readonly BoolHolder Selected = new();

        public void Select()
        {
            Manager.Select(this);
        }
    }
}