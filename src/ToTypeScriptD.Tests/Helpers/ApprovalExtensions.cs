namespace ToTypeScriptD.Tests
{
    public static class ApprovalsExtensions
    {
        public static void DiffWith(this string expected, string actual)
        {
            if (expected != actual)
            {
                var expectedFile = System.IO.Path.GetTempPath() + "Expected.Approvals.Temp.txt";
                var actualFile = System.IO.Path.GetTempPath() + "Actual.Approvals.Temp.txt";

                System.IO.File.WriteAllText(expectedFile, expected);
                System.IO.File.WriteAllText(actualFile, actual);

                var reporter = ApprovalTests.Approvals.GetReporter();
                reporter.Report(expectedFile, actualFile);
                Xunit.Assert.Equal(expected, actual);
            }
        }

        public static void Verify(this string item)
        {
            item = item.StripHeaderGarbageromOutput();
            ApprovalTests.Approvals.Verify(item);
        }

        public static void Verify(this ToTypeScriptD.Tests.ExeTests.ExeProcessResult item)
        {
            item.ToString().Verify();
        }

        public static void Verify<T>(this T item)
        {
            ApprovalTests.Approvals.Verify(item);
        }

        public static void DumpWinMDAndVerify(this string path, System.Action<ToTypeScriptD.Core.ConfigBase> configOverrideHook = null)
        {
            var config = new ToTypeScriptD.Core.WinMD.WinmdConfig();

            if (configOverrideHook != null)
            {
                configOverrideHook(config);
            }

            var typeCollection = new ToTypeScriptD.Core.TypeWriters.TypeCollection();
            var result = ToTypeScriptD.Render.FullAssembly(path, typeCollection, config).StripHeaderGarbageromOutput();
            ApprovalTests.Approvals.Verify(result);
        }

        public static void DumpDotNetAndVerify(this string path, System.Action<ToTypeScriptD.Core.ConfigBase> configOverrideHook = null)
        {
            var config = new ToTypeScriptD.Core.DotNet.DotNetConfig();

            if (configOverrideHook != null)
            {
                configOverrideHook(config);
            }

            var typeCollection = new ToTypeScriptD.Core.TypeWriters.TypeCollection();
            var result = ToTypeScriptD.Render.FullAssembly(path, typeCollection, config).StripHeaderGarbageromOutput();
            ApprovalTests.Approvals.Verify(result);
        }
    }
}
