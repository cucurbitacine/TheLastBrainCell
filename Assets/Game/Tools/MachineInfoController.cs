using System;
using CucuTools;
using CucuTools.Attributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tools
{
    public class MachineInfoController : CucuBehaviour
    {
        public string machineInfoText;
        
        public MachineInfo machineInfo;

        [Space]
        public Button copyToClipboardButton;
        
        public MachineInfo GetMachineInfo()
        {
            return machineInfo;
        }

        [CucuButton()]
        private void UpdateMachineInfo()
        {
            machineInfo.deviceType = SystemInfo.deviceType.ToString();
            machineInfo.deviceModel = SystemInfo.deviceModel.ToString();
            machineInfo.operatingSystem = SystemInfo.operatingSystem.ToString();
            
            machineInfo.processorType = SystemInfo.processorType.ToString();
            machineInfo.systemMemorySize = SystemInfo.systemMemorySize.ToString();
            machineInfo.processorCount = SystemInfo.processorCount.ToString();
            
            machineInfo.graphicsDeviceName = SystemInfo.graphicsDeviceName.ToString();
            machineInfo.graphicsDeviceType = SystemInfo.graphicsDeviceType.ToString();
            machineInfo.graphicsMemorySize = SystemInfo.graphicsMemorySize.ToString();

            machineInfoText = JsonConvert.SerializeObject(machineInfo);
        }

        [CucuButton()]
        private void CopyMachineInfoToClipboard()
        {
            GUIUtility.systemCopyBuffer = machineInfoText;
        }

        private void CopyToClipboard()
        {
            UpdateMachineInfo();
            CopyMachineInfoToClipboard();
        }
        
        private void OnEnable()
        {
            copyToClipboardButton?.onClick.AddListener(CopyToClipboard);
        }
        
        private void OnDisable()
        {
            copyToClipboardButton?.onClick.RemoveListener(CopyToClipboard);
        }
    }
    
    [Serializable]
    public class MachineInfo
    {
        public string deviceType;
        public string deviceModel;
        public string operatingSystem;
        
        public string processorType;
        public string systemMemorySize;
        public string processorCount;
        
        public string graphicsDeviceName;
        public string graphicsDeviceType;
        public string graphicsMemorySize;
    }
}