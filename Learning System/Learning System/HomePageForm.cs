﻿using Learning_System.ExternalClass;
using Learning_System.Properties;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Learning_System
{
    public partial class HomePageForm : Form
    {
        public HomePageForm()
        {
            InitializeComponent();
            LoadContestList();
        }

        private void LoadContestList()
        {
            JArray? contestJson = JsonProcessing.ImportJsonContentInDefaultFolder("contest.json", null, null);

            try
            {
                if (contestJson != null)
                {
                    DataProcessing contestData = new();
                    List<string> contestColumns = new() { "Name", "TimeStart", "TimeEnd" };
                    List<Type> contestType = new() { typeof(string), typeof(DateTime), typeof(DateTime) };
                    List<string> contestKey = new() { "PRIMARY KEY", "", "" };

                    contestData.Import(contestColumns, contestType, contestKey);
                    contestData.Import(contestJson);

                    DataTable? _DT = contestData.Init().Sort("TimeStart asc").Get();

                    if (_DT != null)
                    {
                        for (int i = 0; i < _DT.Rows.Count; i++)
                        {
                            LinkLabel linklbl = new()
                            {
                                Name = "ExportForm_Linklbl" + i.ToString(),
                                Location = new Point(120, 150 + i * 45),
                                Font = new Font("Arvoregular", 12F, FontStyle.Regular, GraphicsUnit.Point),
                                ActiveLinkColor = Color.Black,
                                VisitedLinkColor = Color.Black,
                                LinkColor = Color.Black,
                                LinkBehavior = LinkBehavior.HoverUnderline,
                                FlatStyle = FlatStyle.Flat,
                                TabIndex = 10,
                                Text = _DT.Rows[i].Field<string>("Name"),
                                Size = new Size(919, 30),
                            };

                            linklbl.Click += new EventHandler((sender, args) =>
                            {
                                Settings.Default["ChoosingContest"] = linklbl.Text;
                                Settings.Default.Save();

                                MessageBox.Show(Settings.Default.ChoosingContest, "Success", MessageBoxButtons.OK);
                            });

                            System.ComponentModel.ComponentResourceManager resources = new(typeof(HomePageForm));

                            PictureBox pictureBox = new()
                            {
                                Name = "ExportForm_PictureBox" + i.ToString(),
                                Location = new Point(90, 150 + i * 45),
                                Size = new Size(20, 20),
                                SizeMode = PictureBoxSizeMode.AutoSize,
                                TabIndex = 0,
                                TabStop = false
                            };

                            DateTime endTime = _DT.Rows[i].Field<DateTime>("TimeEnd");
                            if (DateTime.Compare(endTime, DateTime.Now) < 0)
                                pictureBox.Image = resources.GetObject("checked_file_blur") as Image;
                            else
                                pictureBox.Image = resources.GetObject("checked_file") as Image;

                            panel2.Controls.Add(linklbl);
                            panel2.Controls.Add(pictureBox);
                        }
                    }
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't load your contest file!\n" + ex, "Error", MessageBoxButtons.OK);
            }
        }

        private void Picture_setting_Click(object sender, EventArgs e)
        {
            if (panel_popup.Visible == false)
            {
                panel_popup.Visible = true;
            }
            else
            {
                panel_popup.Visible = false;
            }
        }

        private void Button_Import_Click(object sender, EventArgs e)
        {
            if (panel_popup.Visible == false)
            {
                panel_popup.Visible = true;
            }
            else
            {
                panel_popup.Visible = false;
            }
            PopUpForm.Visible = true;
            PopUpForm.Show_Import();
        }

        private void Button_Categories_Click(object sender, EventArgs e)
        {
            if (panel_popup.Visible == false)
            {
                panel_popup.Visible = true;
            }
            else
            {
                panel_popup.Visible = false;
            }
            PopUpForm.Visible = true;
            PopUpForm.Show_Categories();
        }

        private void Button_Questions_Click(object sender, EventArgs e)
        {
            if (panel_popup.Visible == false)
            {
                panel_popup.Visible = true;
            }
            else
            {
                panel_popup.Visible = false;
            }
            PopUpForm.Visible = true;
            PopUpForm.Show_Questions();
        }

        private void Button_Export_Click(object sender, EventArgs e)
        {
            if (panel_popup.Visible == false)
            {
                panel_popup.Visible = true;
            }
            else
            {
                panel_popup.Visible = false;
            }
            PopUpForm.Visible = true;
            PopUpForm.Show_Export();
        }
    }
}