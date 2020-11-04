using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;

namespace LeeraJenkins.Migrations
{
    public class Runner
    {
        private static MigrationRunner _migrationRunner;
        public static string ConnectionString;
        public class MigrationOptions : IMigrationProcessorOptions
        {
            public bool PreviewOnly { get; set; }

            public string ProviderSwitches { get; set; }

            public int Timeout { get; set; }
        }

        public static void Initialize(string connectionString)
        {
            ConnectionString = connectionString;
            var announcer = new TextWriterAnnouncer(Console.Out);
            var assembly = Assembly.GetExecutingAssembly();

            var migrationContext = new RunnerContext(announcer)
            {
                Namespace = "LeeraJenkins.Migrations.Migrations",
                NestedNamespaces = true
            };
            var options = new MigrationOptions { PreviewOnly = false, Timeout = 60 };
            var factory = new FluentMigrator.Runner.Processors.SqlServer.SqlServer2012ProcessorFactory();
            var processor = factory.Create(connectionString, announcer, options);
            _migrationRunner = new MigrationRunner(assembly, migrationContext, processor);
        }

        public static void MigrateToLatest()
        {
            _migrationRunner.MigrateUp(true);
        }

        public static void MigrateDownToVersion(long migrationVesrion)
        {
            _migrationRunner.MigrateDown(migrationVesrion, true);
        }
    }
}
