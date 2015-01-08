using Microsoft.VisualStudio.TextTemplating.VSHost;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BuildDyn
{
    [Guid("D9DB90FF-7F68-4fc4-B343-39863007FCC1")]
    [ComVisible(true)]
    public class BuildTool : BaseCodeGeneratorWithSite
    {
        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            try
            {
                string code = new ComponentAccessTool.Program().Process(inputFileName);
                return Encoding.ASCII.GetBytes(code);
            }
            catch (Exception ex)
            {
                return Encoding.ASCII.GetBytes(ex.ToString());
            }
        }

        public override string GetDefaultExtension()
        {
            return ".dyn.cs";
        }

        #region Registration

        // You have to make sure that the value of this field (CustomToolGuid) is exactly 
        // the same as the value of the Guid attribure (at the top of the class)
        private static Guid CustomToolGuid =
            new Guid("{D9DB90FF-7F68-4fc4-B343-39863007FCC1}");

        private static Guid CSharpCategory =
            new Guid("{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}");

        private static Guid VBCategory =
            new Guid("{164B10B9-B200-11D0-8C61-00A0C91E29D5}");


        private const string CustomToolName = "BuildDyn";

        private const string CustomToolDescription = "Generates the component provider and extension classes for a .dyn file";

        private const string KeyFormat
            = @"SOFTWARE\Microsoft\VisualStudio\{0}\Generators\{1}\{2}";

        protected static void Register(Version vsVersion, Guid categoryGuid)
        {
            string subKey = String.Format(KeyFormat,
                vsVersion, categoryGuid.ToString("B"), CustomToolName);

            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(subKey))
            {
                if (key == null)
                    return;
                key.SetValue("", CustomToolDescription);
                key.SetValue("CLSID", CustomToolGuid.ToString("B"));
                key.SetValue("GeneratesDesignTimeSource", 1);
            }
        }

        protected static void Unregister(Version vsVersion, Guid categoryGuid)
        {
            string subKey = String.Format(KeyFormat,
                vsVersion, categoryGuid.ToString("B"), CustomToolName);

            Registry.LocalMachine.DeleteSubKey(subKey, false);
        }

        public static int[] StudioVersions = {8, 9, 10};

        [ComRegisterFunction]
        public static void RegisterClass(Type t)
        {
            foreach (var version in StudioVersions)
            {
                Register(new Version(version, 0), CSharpCategory);
                Register(new Version(version, 0), VBCategory);
            }
        }

        [ComUnregisterFunction]
        public static void UnregisterClass(Type t)
        {
            foreach (var version in StudioVersions)
            {
                Unregister(new Version(version, 0), CSharpCategory);
                Unregister(new Version(version, 0), VBCategory);
            }
        }

        #endregion
    }
}
