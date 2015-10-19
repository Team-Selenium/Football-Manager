﻿namespace TestApp
{
    using System.IO;
    using System.Linq;
    using MySql.Data;
    using Newtonsoft.Json;
    using Telerik.OpenAccess;

    internal class Program
    {
        private const string ZipPlayersPath = "../../../Data Sources/ZIP/Players.zip";
        private const string XmlMatchesPath = "../../../Data Sources/XML/Matches.xml";
        private const string XmlPlayersPath = "../../../Data Sources/XML/Players.xml";
        private const string PdfReportPath = "../../../Data Sources/PDF/Report.pdf";

        private static void Main(string[] args)
        {
            UpdateDatabase();

            ImportDbDataFromJson();
        }

        private static void ImportDbDataFromJson()
        {
            var ctx = new FluentModel();


            using (ctx)
            {
                var files = Directory.GetFiles("../../../Data Sources/JSON");

                var teams = ctx.GetAll<TeamReportDto>().ToList();

                foreach (var file in files)
                {
                    var fileText = File.ReadAllText(file);
                    var teamReport = JsonConvert.DeserializeObject<TeamReportDto>(fileText);

                    var teamReportDb = ctx.TeamReports.FirstOrDefault(t => t.Name == teamReport.Name);
                    if (teamReportDb != null)
                    {
                        teamReportDb.Name = teamReport.Name;
                        teamReportDb.NumberOfPlayers = teamReport.NumberOfPlayers;
                        teamReportDb.NumbersOfMatches = teamReport.NumbersOfMatches;
                        teamReportDb.Coach = teamReport.Coach;
                        teamReportDb.Owner = teamReport.Owner;
                    }
                    else
                    {
                        ctx.Add(teamReport);
                        ctx.SaveChanges();
                    }
                }
            }
        }

        private static void UpdateDatabase()
        {
            using (var context = new FluentModel())
            {
                var schemaHandler = context.GetSchemaHandler();
                EnsureDB(schemaHandler);
            }
        }

        private static void EnsureDB(ISchemaHandler schemaHandler)
        {
            string script = null;
            if (schemaHandler.DatabaseExists())
            {
                script = schemaHandler.CreateUpdateDDLScript(null);
            }
            else
            {
                schemaHandler.CreateDatabase();
                script = schemaHandler.CreateDDLScript();
            }

            if (!string.IsNullOrEmpty(script))
            {
                schemaHandler.ExecuteDDLScript(script);
            }
        }

        //var repo = new MSSqlRepository();



        //var repo = new JsonUtils();
        //repo.JsonCreateReports();

        // var zip = System.IO.Compression.ZipFile.Open(ZipPlayersPath, ZipArchiveMode.Read);

        // using (zip)
        // {
        //    var teams = ExcelUtils.GetAllPlayers(zip);
        //   // var repo = new MSSqlRepository();

        //    //repo.FillPlayersFromZip(teams);
        //}

        //var xmlOperator = new XmlToDtoMatchConverter(XmlMatchesPath);
        //var matches = xmlOperator.GetAllDtoMatches();
        ////var players = xmlOperator.GetAllPlayers(XmlPlayersPath);

        //foreach (var match in matches)
        //{
        //    Console.WriteLine("Match Id: " + match.Id);
        //    Console.WriteLine("HomeTeam Id: " + match.HomeTeamId);
        //    Console.WriteLine("Attendance: " + match.Attendance);
        //}

        //foreach (var player in players)
        //{
        //    Console.WriteLine("Player Id: " + player.Id);
        //    Console.WriteLine("HomeTeam Id: " + player.Position);
        //}


        //PdfUtils.GeneratePdfReport();

    }
}
