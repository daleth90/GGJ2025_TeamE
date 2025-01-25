using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Physalia
{
    public class ScreenShooter
    {
        public static void TakeScreenshot(bool openExplorer = false)
        {
            string directory = $"{FileUtility.ProjectRoot}/Screenshots";
            string fileName = $"Screenshot_{DateTime.Now:yyyyMMdd_hhmmss}.png";
            string filePath = $"{directory}/{fileName}";

            // Make sure the folder exists
            _ = Directory.CreateDirectory(directory);

            // Take a screenshot
            ScreenCapture.CaptureScreenshot(filePath);

            // Open the folder where the screenshot was saved
            if (openExplorer)
            {
                Process.Start("explorer.exe", $"/select \"{filePath.Replace("/", "\\")}\"");
            }
        }
    }
}
