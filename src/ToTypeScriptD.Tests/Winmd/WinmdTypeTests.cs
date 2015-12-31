using ApprovalTests;
using System;
using System.IO;
using ToTypeScriptD.Core;
using ToTypeScriptD.Core.Extensions;
using ToTypeScriptD.Lexical.Extensions;
using ToTypeScriptD.Lexical.WinMD;
using Xunit;

namespace ToTypeScriptD.Tests.Winmd
{

    public class WinmdTypeTests : WinmdTestBase
    {
        private Type GetNativeType(string postfix)
        {
            return base.NativeAssembly.GetNativeType("ToTypeScriptD.Native." + postfix);
        }

        private Type GetWinNativeType(string type)
        {
            return base.WindowsAssembly.GetNativeType(type);
        }

        [Fact]
        public void EnumType()
        {
            var result = GetNativeType("SampleEnum").ToTypeScript();
            Approvals.Verify(result);
        }

        [Fact]
        public void EnumTypeWithExplicitValues()
        {
            var result = GetNativeType("SampleEnumNumbered").ToTypeScript();
            Approvals.Verify(result);
        }

        [Fact]
        public void ClassWithEventHandler()
        {
            var result = GetNativeType("ClassWithEventHandler").ToTypeScript();
            Approvals.Verify(result);
        }

        [Fact]
        public void FullSampleAssembly()
        {
            var file = base.NativeAssembly.ComponentPath;
            var config = new WinmdConfig();
            var w = new StringWriter();
            Render.FromAssembly(file, config, w);
            Approvals.Verify(w.GetStringBuilder().ToString());
        }

        [Fact]
        public void WindowsStorageClass()
        {
            var result = GetWinNativeType("Windows.Storage.StorageFile").ToTypeScript();
            Approvals.Verify(result);
        }
        [Fact]
        public void WindowsSystemUserProfileGlobalizationPreferencesClass()
        {
            var result = GetWinNativeType("Windows.System.UserProfile.GlobalizationPreferences").ToTypeScript();
            Approvals.Verify(result);
        }

        [Fact]
        public void IfEnumNamesAreAllCapsThenTheTranslateToAllLowerCase()
        {
            var result = GetWinNativeType("Windows.Foundation.Metadata.ThreadingModel")
                .ToTypeScript();
            Approvals.Verify(result);
        }

        [Fact]
        public void FullWindowsAssembly()
        {
            var file = @"C:\Windows\System32\WinMetadata\Windows.Foundation.winmd";
            var config = new WinmdConfig();
            var s = new StringWriter();
            Render.FromAssembly(file, config, s);
            Approvals.Verify(s.GetStringBuilder().ToString());
        }

        //[Fact]
        //public void RenderAllWinmdFilesInWindowsMetadataDirectory()
        //{
        //    var files = @"C:\Windows\System32\WinMetadata\Windows.ApplicationModel.winmd C:\Windows\System32\WinMetadata\Windows.Data.winmd C:\Windows\System32\WinMetadata\Windows.Devices.winmd C:\Windows\System32\WinMetadata\Windows.Foundation.winmd C:\Windows\System32\WinMetadata\Windows.Globalization.winmd C:\Windows\System32\WinMetadata\Windows.Graphics.winmd C:\Windows\System32\WinMetadata\Windows.Management.winmd C:\Windows\System32\WinMetadata\Windows.Media.winmd C:\Windows\System32\WinMetadata\Windows.Networking.winmd C:\Windows\System32\WinMetadata\Windows.Security.winmd C:\Windows\System32\WinMetadata\Windows.Storage.winmd C:\Windows\System32\WinMetadata\Windows.System.winmd C:\Windows\System32\WinMetadata\Windows.UI.winmd C:\Windows\System32\WinMetadata\Windows.UI.Xaml.winmd C:\Windows\System32\WinMetadata\Windows.Web.winmd".Split(' ');
        //    var sw = new System.IO.StringWriter();
        //    var error = new StringBuilderTypeNotFoundErrorHandler();
        //    ToTypeScriptD.Render.FromAssemblies(files, true, sw, error);
        //    var result = error.ToString() + Environment.NewLine + Environment.NewLine + sw.ToString();
        //    result.Verify();
        //}


        [Fact]
        public void AllWinmdFilesInWinMetadata()
        {
            var allFiles = System.IO.Directory.GetFiles(@"C:\Windows\System32\WinMetadata\", "*.winmd");
            var sw = new System.IO.StringWriter();
            var config = new WinmdConfig
            {
                IncludeSpecialTypes = false
            };
            Render.FromAssemblies(allFiles, config, sw);
            var result = Environment.NewLine + Environment.NewLine + sw;
            result.Verify();
        }

        // Some types in Windows.winmd are duplicated in Windows.Foundation.winmd (huh?)
        //[Fact]
        //public void DumpFullWindowsWinmd()
        //{
        //    var file = @"C:\Program Files (x86)\Windows Kits\8.0\References\CommonConfiguration\Neutral\Windows.winmd";
        //    var result = ToTypeScriptD.Render.FullAssembly(file);
        //    Approvals.Verify(result);
        //}

    }
}
