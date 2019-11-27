using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeatSaberModdingTools.ViewModels;
using System.Linq;
using BeatSaberModdingTools.Models;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;

namespace BSMT_Tests.ViewModels.SettingsViewModel_Tests
{
    [TestClass]
    public class NotifyPropertyChanged_Tests
    {
        private static readonly object SyncRoot = new object();
        [TestInitialize]
        public void Initialize()
        {
            Monitor.Enter(SyncRoot);
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            Monitor.Exit(SyncRoot);
        }


        [TestMethod]
        public void NotifyPropertyChanged()
        {
            BeatSaberModdingTools.BSMTSettingsManager.UseDefaultManager();
            var vm = new SettingsViewModel();
            var changesNotified = new Dictionary<string, object>();
            var setProperties = new Dictionary<string, object>();
            int eventCount = 0;
            var thing = typeof(SettingsViewModel).GetProperty("TestTest");
            void handler(object s, PropertyChangedEventArgs e)
            {
                Assert.IsTrue(setProperties.ContainsKey(e.PropertyName), $"{e.PropertyName} changed, but not in Dictionary.");
                changesNotified.Add(e.PropertyName, null);
                eventCount++;
            }
            vm.PropertyChanged += handler;
            int propertyCount = 0;
            foreach (var prop in typeof(SettingsViewModel).GetProperties().Where(i => i.CanWrite).ToList())
            {
                if (!typeof(ISettingsModel).IsAssignableFrom(prop.PropertyType))
                {
                    setProperties.Add(prop.Name, null);
                    setProperties.Add(prop.Name + "Changed", null);
                }
                propertyCount += 2;
                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(vm, prop.Name);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(vm, !(bool)prop.GetValue(vm));
                }
                else if (prop.PropertyType == typeof(BuildReferenceType))
                {
                    prop.SetValue(vm, BuildReferenceType.DirectoryJunctions);
                }
                else if (typeof(ISettingsModel).IsAssignableFrom(prop.PropertyType))
                    continue;
                else
                    Assert.Fail($"Type {prop.PropertyType} is unhandled for {prop.Name}");
            }
            foreach (var key in setProperties.Keys)
            {
                Assert.IsTrue(changesNotified.ContainsKey(key), $"{key} not notified");
            }

            vm.PropertyChanged -= handler;
        }

        [TestMethod]
        public void PropertyNotChanged()
        {
            BeatSaberModdingTools.BSMTSettingsManager.UseDefaultManager();
            var vm = new SettingsViewModel();
            void handler(object s, PropertyChangedEventArgs e)
            {
                Assert.Fail($"{e.PropertyName} wasn't changed but triggered a notify event.");
            }
            vm.PropertyChanged += handler;
            foreach (var prop in typeof(SettingsViewModel).GetProperties().Where(i => i.CanWrite).ToList())
            {
                prop.SetValue(vm, prop.GetValue(vm));
            }
            vm.PropertyChanged -= handler;
        }

    }
}
