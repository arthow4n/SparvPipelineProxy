using System.Text;
using System.IO;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SparvPipelineProxy.Utils
{
    public static class CommandService
    {
        public static async Task<string> AnnotateWithSparvPipeline(string sparvLanguageCode, string input)
        {
            if (sparvLanguageCode != "swe")
            {
                throw new ArgumentOutOfRangeException(nameof(sparvLanguageCode), sparvLanguageCode, "Only Swedish (swe) is supported for now.");
            }

            using var tempDir = new TempDir();

            var sourceDirName = "source";
            var sourceDir = Path.Join(tempDir.FullPath, sourceDirName);
            Directory.CreateDirectory(sourceDir);

            // TODO: Consider batch processing instead of spawning process for each request.
            await File.WriteAllTextAsync(
                Path.Join(sourceDir, "input.txt"),
                input,
                Encoding.UTF8
            );

            var configYamlContent =
$@"metadata:
  id: tmp
  name:
    eng: tmp
    swe: tmp
  language: swe
  description:
    eng: tmp
    swe: tmp
import:
  source_dir: {sourceDirName}
  importer: text_import:parse
  document_annotation: text
classes:
  token:msd: <token>:stanza.msd
  token:pos: <token>:stanza.pos
export:
  default:
    - xml_export:pretty
  annotations:
    - segment.token
    - segment.sentence
    - segment.paragraph
    - <token>:misc.head
    - <token>:misc.tail
    - <token>:misc.upos
    - <token>:misc.number_rel_<sentence>
    - <sentence>:misc.number_rel_<text>
    - <text>:misc.number_position
    - <token>:stanza.msd
    - <token>:stanza.ufeats
    - <token>:stanza.baseform
    - <token>:saldo.compwf
    - <token>:saldo.baseform2
preload:
  - saldo:annotate
  - saldo:compound
";

            await File.WriteAllTextAsync(
                Path.Join(tempDir.FullPath, "config.yaml"),
                configYamlContent,
                Encoding.UTF8
            );

            await new RunContext(tempDir.FullPath).RunShell(
                "sparv", "run",
                "-j", $"{Environment.ProcessorCount}",
                "--socket", "/tmp/sparv_preload.sock"
            );

            var result = await File.ReadAllTextAsync(
                Path.Join(tempDir.FullPath, "export", "xml_pretty", "input_export.xml")
            );

            return result;
        }

        private class RunContext
        {
            private readonly string? _workingDirectory;

            public RunContext(string? workingDirectory = null)
            {
                _workingDirectory = workingDirectory;
            }

            public async Task RunShell(string command, params string[] args)
            {
                var p = new Process();
                p.StartInfo.FileName = command;

                if (!string.IsNullOrWhiteSpace(_workingDirectory))
                {
                    p.StartInfo.WorkingDirectory = _workingDirectory;
                }

                foreach (var arg in args)
                {
                    p.StartInfo.ArgumentList.Add(arg);
                }

                p.Start();
                await p.WaitForExitAsync();

                if (p.ExitCode != 0)
                {
                    throw new Exception($"Exited with {p.ExitCode}: {command} {string.Join(" ", args)}");
                }
            }
        }

        private class TempDir : IDisposable
        {
            private bool disposedValue;

            public string FullPath { get; }

            public TempDir(string directoryNamePrefix = "")
            {
                FullPath = Path.Join(Path.GetTempPath(), $"{directoryNamePrefix}{(string.IsNullOrWhiteSpace(directoryNamePrefix) ? "" : "_")}{Guid.NewGuid()}");
                Directory.CreateDirectory(FullPath);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                    }

                    Directory.Delete(FullPath, true);
                    disposedValue = true;
                }
            }
            ~TempDir()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: false);
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
