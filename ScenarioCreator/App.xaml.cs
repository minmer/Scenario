﻿using ManagementSystemLibrary.Pipeline;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ScenarioCreator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Pipeline Pipeline { get; set; } = new Pipeline(new ServerParametres());
    }
}
