using Learning_System.ExternalClass;
using Microsoft.Office.Interop.Word;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Learning_System
{
    public partial class QuestionsForm : UserControl
    {
        //bien dung de gui id cua category dang duoc chon sang form edit question
        private int SendParentIdToEditForm;
        //bien luu cac category duoc hien tren datagridview
        private List<int> selectedCategoriesIdList = new();
        // bien co hien cau hoi tu category con hay khong
        private bool showQuestionsFromSubcategories = false;
        private void GetSubCategories(int _parentCategories, ref List<int> _subCategories, ref List<Categories> categories)
        {
            foreach (int x in categories[_parentCategories].SubArray)
            {
                _subCategories.Add(x);
                GetSubCategories(x, ref _subCategories, ref categories);
            }
        }

        public void ShowQuestions(List<int> showQuestionsFromCategoriesID, bool _showQuestionsFromSubcategories)
        {
            QuestionForm_ShowQuestionsDtg.Rows.Clear();
            DataProcessing questionsData = new();
            List<string> _showQuestionsColumns = new() { "ID", "Name", "CategoryID", "Content", "DefaultMark", "Choice" };
            List<Type> _showQuestionsType = new() { typeof(int), typeof(string), typeof(int), typeof(string), typeof(double), typeof(JArray) };
            List<string> _showQuestionsKey = new() { "PRIMARY KEY", "", "", "", "", "" };
            try
            {
                JArray? jArray = JsonProcessing.ImportJsonContentInDefaultFolder("Question.json", null, null);
                if (jArray == null) {

                    DialogResult dialogResult = MessageBox.Show("Can't get questions data:\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (dialogResult == DialogResult.OK)
                    System.Windows.Forms.Application.Exit();
                    return;
                }
                JArray _questionsData = jArray;
                questionsData.Import(_showQuestionsColumns, _showQuestionsType, _showQuestionsKey);
                questionsData.Import(_questionsData);

                if (questionsData.Length() == 0) MessageBox.Show("Không có câu hỏi nào trong Categories này!");
                else
                {
                    //var index = QuestionForm_ShowQuestionsDtg.Rows.Add();
                    //QuestionForm_ShowQuestionsDtg.Rows[index].Cells[1].Value = "Question name / ID number";
                    for (int i = 0; i < questionsData.Length(); i++)
                    {
                        DataRow? Question = questionsData.Init().Offset(i).Limit(1).Sort("ID desc").GetFirstRow();
                        if (Question == null)
                        {
                            DialogResult dialogResult = MessageBox.Show("Can't get questions data:\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            if (dialogResult == DialogResult.OK)
                                System.Windows.Forms.Application.Exit();
                            return;
                        }
                        int inCategories = Question.Field<int>("CategoryID");
                        int QuestionID = Question.Field<int>("ID");
                        if ((showQuestionsFromCategoriesID.Contains(inCategories) && _showQuestionsFromSubcategories) || (showQuestionsFromCategoriesID[0] == inCategories && !showQuestionsFromSubcategories))
                        {
                            string? _QuestionName = Question.Field<string>("Content");

                            //                            DataGridViewRow row = (DataGridViewRow)QuestionForm_ShowQuestionsDtg.Rows[0].Clone();
                            var index = QuestionForm_ShowQuestionsDtg.Rows.Add();
                            DataGridViewRow row = QuestionForm_ShowQuestionsDtg.Rows[index];
                            //
                            RichTextBox tmp = new()
                            {
                                Visible = false
                            };
                            try
                            {
                                tmp.Rtf = _QuestionName;
                                row.Cells[1].Value = tmp.Text;
                            }
                            catch
                            {
                                row.Cells[1].Value = _QuestionName;
                            }
                            row.Cells[2].Value = "Edit";
                            row.Cells[3].Value = QuestionID;
                            if (i % 2 == 0) row.DefaultCellStyle.BackColor = Color.White;
                            else row.DefaultCellStyle.BackColor = Color.AliceBlue;

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                DialogResult dialogResult = MessageBox.Show("Can't get questions data:\n" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (dialogResult == DialogResult.OK)
                    System.Windows.Forms.Application.Exit();
            }
        }
        public void LoadCategoriesData()
        {
            List<Categories>? listCategories = new();
            List<Categories> newListCategories = new();
            try
            {
                JArray? _categoriesData = JsonProcessing.ImportJsonContentInDefaultFolder("Category.json", null, null);
                if (_categoriesData == null) return;
                listCategories = _categoriesData.ToObject<List<Categories>>();
                if (listCategories == null) return;
                AddSpace(ref newListCategories, ref listCategories, 0, "  ");
                newListCategories.Reverse();
            }
            catch (Exception ex)
            {
                DialogResult dialog = MessageBox.Show("Can't get categories data:\n" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (dialog == DialogResult.OK)
                    System.Windows.Forms.Application.Exit();
            }


            QuestionsForm_SelectCategoryCbo.ValueMember = "Id";
            QuestionsForm_SelectCategoryCbo.DisplayMember = "Name";
            QuestionsForm_SelectCategoryCbo.DataSource = newListCategories;
        }

        //Them Space cho cac lua chon Combobox
        public void AddSpace(ref List<Categories> List, ref List<Categories> addList, int begin, string space)
        {
            foreach (int x in addList[begin].SubArray)
            {
                addList[x].Name = space + addList[x].Name;
                AddSpace(ref List, ref addList, x, space + "  ");
            }
            addList[begin].Name = "  " + addList[begin].Name;

            if (addList[begin].QuestionArray.Count > 0)
                addList[begin].Name = addList[begin].Name + " (" + addList[begin].QuestionArray.Count + ")";
            List.Add(addList[begin]);
        }
        public QuestionsForm()
        {
            InitializeComponent();
            LoadCategoriesData();
            QuestionsForm_SelectCategoryCbo.SelectedIndex = -1;
            QuestionsForm_SelectCategoryCbo.SelectedText = "  Default";
        }

        private void QuestionsForm_SelectCategoryCbo_Click(object sender, EventArgs e)
        {

            LoadCategoriesData();
        }

        private void QuestionsForm_SelectCategoryCbo_DropDown(object sender, EventArgs e)
        {
            LoadCategoriesData();
        }

        private void QuestionsForm_CreateNewQuestionBtn_Click(object sender, EventArgs e)
        {
            AddNewQuestionForm addNewQuestionForm = new AddNewQuestionForm();
            addNewQuestionForm.ShowDialog();
            // Hàm ngay bên dưới này dùng để load lại datagridview, khi nào merge code vào thì ông bỏ // đi hộ tôi nhá
            // showQuestions(selectedCategoriesIdList, showQuestionsFromSubcategories);
        }

        private void QuestionsForm_SelectCategoryCbo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (QuestionsForm_SelectCategoryCbo.SelectedItem == null) return;
            var a = (Categories)QuestionsForm_SelectCategoryCbo.SelectedItem;
            selectedCategoriesIdList.Clear();
            int _parentCategories = a.Id;
            //
            SendParentIdToEditForm = _parentCategories;
            //
            try
            {
                JArray? _categoriesData = JsonProcessing.ImportJsonContentInDefaultFolder("Category.json", null, null);
                if (_categoriesData == null) return;
                List<Categories>? categories = _categoriesData.ToObject<List<Categories>>();
                if (categories == null || _categoriesData == null) return;
                if (_parentCategories >= 0)
                {
                    selectedCategoriesIdList.Add(_parentCategories);
                    GetSubCategories(_parentCategories, ref selectedCategoriesIdList, ref categories);
                    ShowQuestions(selectedCategoriesIdList, showQuestionsFromSubcategories);
                }
            }
            catch (Exception ex)
            {
                DialogResult dialog = MessageBox.Show("Can't get categories data:\n" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (dialog == DialogResult.OK)
                    System.Windows.Forms.Application.Exit();
            }
        }

        private void QuestionsForm_ShowFromSubcategoriesCb_CheckedChanged(object sender, EventArgs e)
        {
            showQuestionsFromSubcategories = QuestionsForm_ShowFromSubcategoriesCb.Checked;
            ShowQuestions(selectedCategoriesIdList, showQuestionsFromSubcategories);
        }

        private void QuestionForm_ShowQuestionsDtg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (QuestionForm_ShowQuestionsDtg.Columns[e.ColumnIndex].Name == "Edit" && e.RowIndex >= 0)
            {
                var a = QuestionForm_ShowQuestionsDtg.Rows[e.RowIndex];
                EditQuestionForm editQuestionForm = new(Convert.ToInt32(a.Cells[3].Value), SendParentIdToEditForm);
                editQuestionForm.ShowDialog();
                ShowQuestions(selectedCategoriesIdList, showQuestionsFromSubcategories);
            }
        }
    }
}
