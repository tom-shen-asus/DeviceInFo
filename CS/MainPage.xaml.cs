// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DeviceInfo.Utils;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DeviceInfo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            uiThreadToUpdateUIInformation();
        }

        private void uiThreadToUpdateUIInformation()
        {
            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            Task task = Task.Factory.StartNew(async () =>
            {
                int i = 0;
                while (true)
                {
                    Task.Factory.StartNew(async () =>
                    {
                        deviceInfo();
                        if ((i % 3) == 0) PE100A.Source = new BitmapImage(new Uri(base.BaseUri, @"/Assets/MBMBoard.png"));
                        //if ((i % 3) == 1) PE100A.Source = new BitmapImage(new Uri(base.BaseUri, @"/Assets/GenericBoard.png"));
                        //if ((i % 3) == 2) PE100A.Source = new BitmapImage(new Uri(base.BaseUri, @"/Assets/RaspberryPiBoard.png"));
                    }, CancellationToken.None, TaskCreationOptions.None, uiContext);
                    Thread.Sleep(1000);
                    i++;
                }
            });
        }

        private async void CPUInfoDialogAsync()
        {
            string outputText = await DeviceUtil.getCpuInfo();
            var messageDialog = new MessageDialog(outputText, "CPU Info");
            await messageDialog.ShowAsync();
        }

        private async void PhysicalDiskInfoDialogAsync()
        {
            string outputText = await DeviceUtil.getPhysicalDiskInfo();
            var messageDialog = new MessageDialog(outputText, "Stroage Info");
            await messageDialog.ShowAsync();
        }

        private async void DiskDriveInfoDialogAsync()
        {
            string outputText = await DeviceUtil.getDiskDriveInfo();
            var messageDialog = new MessageDialog(outputText, "Drive Info");
            await messageDialog.ShowAsync();
        }

        private async void MemoryInfoDialogAsync()
        {
            string outputText = await DeviceUtil.getMemoryInfo();
            var messageDialog = new MessageDialog(outputText, "Memory Info");
            await messageDialog.ShowAsync();
        }

        private async void deviceInfo()
        {
            string left = "";
            left += "[OS information]\n";
            left += await DeviceUtil.getOSInfo();
            left += "  OS Version: " + DeviceUtil.GetOSVersionString() + "\n";
            left += "  App Version: " + DeviceUtil.GetAppVersion() + "\n";
            left += await DeviceUtil.getDeviceType();
            left += "\n[systemperf]\n";
            left += await DeviceUtil.getSystemPerformance();
            LeftText.Text = left;

            string center = "";
            center += "[power]\n";
            center += await DeviceUtil.getPowerBattery();
            center += await DeviceUtil.getPowerState();
            center += "[Networking]\n";
            center += await DeviceUtil.getNetworkIpConfig();
            center += await DeviceUtil.getNetworkStatus();
            center += "\n[Connected USB Devices]\n";
            center += await DeviceUtil.getUSBconnectedDevice();
            CenterText.Text = center;
        }

        bool restart = false;
        private async void shutdownDialogAsync()
        {
            var messageDialog = new MessageDialog("Are you sure to shutdown?", "shutdown dialog"); ;
            if (restart == true) messageDialog = new MessageDialog("Are you sure to Reboot?", "Reboot dialog");
            messageDialog.Commands.Add(new UICommand(
                "OK",  new UICommandInvokedHandler(this.CommandInvokedHandler)));
            await messageDialog.ShowAsync();
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            if (restart == true) DeviceUtil.Shutdown(true);
            else DeviceUtil.Shutdown(false);
        }

        void powerButton(object sender, RoutedEventArgs e)
        {
            RadioButton li = (sender as RadioButton);
            if (li.Content.ToString() == "Shutdown")
            {
                restart = false;
                shutdownDialogAsync();
            }
            else if (li.Content.ToString() == "Reboot")
            {
                restart = true;
                shutdownDialogAsync();
            }
        }

        private void CPU_Info_Click(object sender, RoutedEventArgs e)
        {
            CPUInfoDialogAsync();
        }

        private void Storage_Info_Click(object sender, RoutedEventArgs e)
        {
            PhysicalDiskInfoDialogAsync();
        }

        private void Drive_Info_Click(object sender, RoutedEventArgs e)
        {
            DiskDriveInfoDialogAsync();
        }

        private void Memory_Info_Click(object sender, RoutedEventArgs e)
        {
            MemoryInfoDialogAsync();
        }
    }
}
