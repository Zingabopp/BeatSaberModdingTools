using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeatSaberModTemplates.ViewModels;
using System.Linq;
using BeatSaberModTemplates.Models;
using System.Collections.Generic;

namespace BSMT_Tests.ViewModels.SettingsViewModel_Tests
{
    [TestClass]
    public class NotifyPropertyChanged_Tests
    {
        [TestMethod]
        public void NotifyPropertyChanged()
        {
            var vm = new SettingsViewModel();
            var changesNotified = new Dictionary<string, object>();
            var setProperties = new Dictionary<string, object>();
            int eventCount = 0;
            var thing = typeof(SettingsViewModel).GetProperty("TestTest");
            vm.PropertyChanged += (s, e) =>
            {
                Assert.IsTrue(setProperties.ContainsKey(e.PropertyName), $"{e.PropertyName} changed, but not in Dictionary.");
                changesNotified.Add(e.PropertyName, null);
                eventCount++;
            };
            int propertyCount = 0;
            foreach (var prop in typeof(SettingsViewModel).GetProperties().Where(i => i.CanWrite))
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
        }

        [TestMethod]
        public void PropertyNotChanged()
        {
            var vm = new SettingsViewModel();
            vm.PropertyChanged += (s, e) =>
            {
                Assert.Fail($"{e.PropertyName} wasn't changed but triggered a notify event.");
            };
            foreach (var prop in typeof(SettingsViewModel).GetProperties().Where(i => i.CanWrite))
            {
                prop.SetValue(vm, prop.GetValue(vm));
            }
        }

    }
}
