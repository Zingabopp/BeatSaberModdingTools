using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeatSaberModTemplates.ViewModels;
using System.Linq;
using BeatSaberModTemplates.Models;

namespace BSMT_Tests.ViewModels.SettingsViewModel_Tests
{
    [TestClass]
    public class NotifyPropertyChanged_Tests
    {
        [TestMethod]
        public void NotEqual_SettingsModel()
        {
            var vm = new SettingsViewModel();
            vm.PropertyChanged += Vm_PropertyChanged;
            foreach (var prop in typeof(SettingsViewModel).GetProperties().Where(i => i.CanWrite))
            {
                if (prop.PropertyType == typeof(string))
                    prop.SetValue(vm, prop.Name);
                else if (prop.PropertyType == typeof(bool))
                    prop.SetValue(vm, !(bool)prop.GetValue(vm));
                else if (prop.PropertyType == typeof(BuildReferenceType))
                    prop.SetValue(vm, BuildReferenceType.DirectoryJunctions);
                else
                    Assert.Fail($"Type {prop.PropertyType} is unhandled for {prop.Name}");
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
