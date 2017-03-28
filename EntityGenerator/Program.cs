using System;
using System.IO;
using ThuCommix.EntityFramework.Metadata;

namespace EntityGenerator
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if(args.Length < 2)
				ExitWithMessage("Invalid arguments. Please specify 1.) Input folder, 2.) Output folder", 1);

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
					entityService.GenerateEntity(inputStream, destinationStream);
					Console.WriteLine($"Generated entity '{entityFile.Name}'.");
				}
				catch (Exception)
				{
					ExitWithMessage($"Failed to generate entity '{entityFile.Name}'.");
				}

				inputStream.Dispose();
				destinationStream.Dispose();
			}

			ExitWithMessage("Entity generation completed.");
		}

		private static void ExitWithMessage(string message, int exitCode = 0)
		{
			Console.WriteLine(message);
			Environment.Exit(exitCode);
		}
	}
}
