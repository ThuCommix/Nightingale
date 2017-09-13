using System;
using System.Collections.Generic;
using System.IO;
using Concordia.Framework.Metadata;

namespace ConcordiaFrameworkCli
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if(args.Length < 2)
				ExitWithMessage("Invalid arguments. Please specify 1.) Input folder, 2.) Output folder", 1);

            Console.WriteLine($"Concordia.Framework v{typeof(EntityMetadata).Assembly.GetName().Version}\r\n");
            Console.WriteLine($"InputPath: {args[0]}");
            Console.WriteLine($"OutputPath: {args[1]}");

            var inputFolder = new DirectoryInfo(args[0]);
			var outputFolder = new DirectoryInfo(args[1]);

			if (!inputFolder.Exists)
				ExitWithMessage("The input folder does not exist.", 1);

			var entityFiles = inputFolder.GetFiles("*.xml", SearchOption.TopDirectoryOnly);

			if (!outputFolder.Exists)
				outputFolder.Create();

			var entityService = new EntityMetadataService();

			foreach(var entityFile in entityFiles)
			{
				var inputStream = new FileStream(entityFile.FullName, FileMode.Open, FileAccess.Read);
				var destinationStream = new FileStream(Path.Combine(outputFolder.FullName, Path.GetFileNameWithoutExtension(entityFile.FullName) + ".cs"), FileMode.Create, FileAccess.Write);

				try
				{
                    PadMessage($"Validating '{entityFile.Name}' ...");

                    var validationIssues = new List<string>();
                    if (!entityService.ValidateXml(inputStream, validationIssues))
                    {
                        Console.WriteLine("\t[Failed]");
                        Console.WriteLine($"Validation failed for '{entityFile.Name}'.");
                        validationIssues.ForEach(x => Console.WriteLine($"\t-> {x}"));
                    }
                    Console.WriteLine("\t[Success]");

                    inputStream.Seek(0, SeekOrigin.Begin);

                    entityService.GenerateEntity(inputStream, destinationStream);
					PadMessage($"Generated entity '{entityFile.Name}'.");
				}
				catch (Exception e)
				{
                    ExitWithMessage($"Failed to generate entity '{entityFile.Name}'. \r\n\t-> {e.Message}{e.StackTrace}");
				}

                Console.WriteLine("\t[Success]");

                inputStream.Dispose();
				destinationStream.Dispose();
			}

			ExitWithMessage($"Entity generation completed ({entityFiles.Length}).");
		}

		private static void ExitWithMessage(string message, int exitCode = 0)
		{
			Console.WriteLine(message);
			Environment.Exit(exitCode);
		}

        private static void PadMessage(string message)
        {
            Console.Write(message.PadRight(50));
        }
	}
}
