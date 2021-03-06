﻿namespace Football.Client
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Windows.Forms;
    using MongoDb.Data;
    using MsSql.Data;
    using MySql.Data;
    using Sqlite.Data;
    using Utilities;
    using XML.Data;

    public partial class FootballManagerClient : Form
    {
        private const string XmlMatchesPath = "../../../Data Sources/XML/Matches.xml";

        public FootballManagerClient()
        {
            this.InitializeComponent();
        }

        private async void CreateSqlServerDb_Click(object sender, EventArgs e)
        {
            var repo = new MSSqlRepository();
            try
            {
                await repo.CreateDb();
                MessageBox.Show(
                    "The db is created",
                    "Db creation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Find appropriate exception
                var exmes = ex.Message;
            }
        }

        private async void GetMongoData_Click(object sender, EventArgs e)
        {
            try
            {
                var repo = new MongoDbRepository();

                var teams = (await repo.GetTeamsData()).ToList();
                var stadiums = (await repo.GetStadiumsData()).ToList();

                var ctx = new FootballContext();
                using (ctx)
                {
                    foreach (var team in teams)
                    {
                        if (!ctx.Teams.Any(pl => pl.Id == team.Id))
                        {
                            ctx.Teams.Add(team);
                        }
                    }

                    foreach (var stadium in stadiums)
                    {
                        if (!ctx.Stadiums.Any(pl => pl.Id == stadium.Id))
                        {
                            ctx.Stadiums.Add(stadium);
                        }
                    }

                    ctx.SaveChanges();
                }

                MessageBox.Show(
                    "The teams, couches, stadiums and towns are inserted",
                    "Teams insert",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (DataException)
            {
                MessageBox.Show(
                    "No connection to MongoDb!!!",
                    "MongoDb",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FillDatFromZip_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            var repo = new MSSqlRepository();
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var filePath = openFileDialog.FileName;
                    var zip = ZipFile.Open(filePath, ZipArchiveMode.Read);
                    using (zip)
                    {
                        var teams = Utilities.ExcelUtils.GetAllPlayers(zip);
                        repo.FillPlayersFromZip(teams);
                    }
                }

                MessageBox.Show(
                    "The players are inserted",
                    "Players insert",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (IOException)
            {
                MessageBox.Show(
                    "Error reading file!",
                    "Players insert",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FillFromXml_btn_Click(object sender, EventArgs e)
        {
            var repo = new MSSqlRepository();
            var ctx = new FootballContext();
            var xmlToDtoConverter = new XmlToDtoMatchConverter(XmlMatchesPath);
            var dtoToMatchModelConverter = new DtoMatchToDbMatchConverter(xmlToDtoConverter, ctx);
            var matches = dtoToMatchModelConverter.GetAllMatches();

            try
            {
                repo.FillMatchesFromXml(matches);

                MessageBox.Show(
                    "The matches are inserted",
                    "Matches insert",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                // Find appropriate exception
                MessageBox.Show(
                    "Something bad happened",
                    "Fatal Error",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
            }
        }

        private void GeneratePdfReport_btn_Click(object sender, EventArgs e)
        {
            try
            {
                PdfUtils.GeneratePdfReport();
                MessageBox.Show(
                    "The Pdf report is ready",
                    "Pdf report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                // Find appropriate exception
                MessageBox.Show(
                    "Something bad happened",
                    "Fatal Error",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
            }
        }

        private void GenerateXmlReport_btn_Click(object sender, EventArgs e)
        {
            try
            {
                var repo = new MSSqlRepository();
                XmlUtils.XmlCreateReports(repo.GetStadiumReport());
                // Write logic here
                MessageBox.Show(
                   "The XML report for stadiums",
                   "Stadiums report generated!",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                // Find appropriate exception
                MessageBox.Show(
                    "Something bad happened",
                    "Fatal Error",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
            }
        }

        private void GenerateJsonReport_btn_Click(object sender, EventArgs e)
        {
            try
            {
                JsonUtils.JsonCreateReports();
                MessageBox.Show(
                    "The JSON report is generated",
                    "JSON report",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                // Find appropriate exception
                MessageBox.Show(
                    "Something bad happened",
                    "Fatal Error",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
            }
        }

        private void CreateMySqlDb_Click(object sender, EventArgs e)
        {
            var repo = new MySqlRepository();

            repo.UpdateDatabase();

            var directoryOpen = new FolderBrowserDialog();

            if (directoryOpen.ShowDialog() == DialogResult.OK)
            {
                var filePath = directoryOpen.SelectedPath;
                repo.ImportDbDataFromJson(filePath);
            }
        }

        private void ExcelReportForTeams_Click(object sender, EventArgs e)
        {
            var repoMySql = new MySqlRepository();
            var repoSqlite = new SqliteRepository();
            ExcelUtils.GenerateExcelReportForTeams(repoSqlite.GetProductTaxData(), repoMySql.GetTeamReports());
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Team-Selenium/Football-Manager");
        }
    }
}