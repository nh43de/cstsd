﻿using System.IO;
using cstsd.Tests.ExeTests;
using cstsd.TypeScript;

namespace cstsd.Tests.Helpers
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

        public static void Verify(this ExeProcessResult item)
        {
            item.ToString().Verify();
        }

        public static void Verify<T>(this T item)
        {
            ApprovalTests.Approvals.Verify(item);
        }

        public static void DumpDotNetAndVerify(this string path, System.Action<WriterConfig> configOverrideHook = null)
        {
            var config = new WriterConfig();

            if (configOverrideHook != null)
            {
                configOverrideHook(config);
            }
            var w = new StringWriter();
            RenderTypescript.FromAssembly(path, config, w);
            ApprovalTests.Approvals.Verify(w.ToString().StripHeaderGarbageromOutput());
        }
    }
}
