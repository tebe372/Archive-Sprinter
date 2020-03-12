using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArchiveSprinterGUI.ViewModels.SettingsViewModels;

namespace AS.Test
{
    [TestClass]
    public class SettingsViewModelTest
    {
        [TestMethod]
        public void TestSelectMinuend()
        {
            SettingsViewModel viewModel = new SettingsViewModel();
            viewModel.SelectedStep = new PreProcessStepViewModel("Subtraction");

        }
    }
}
