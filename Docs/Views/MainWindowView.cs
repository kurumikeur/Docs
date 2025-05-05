using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Docs.Models;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Docs.Views
{
    public class MainWindowView : BaseView
    {
        private static string sourcePath = @"C:\Github\Docs\";
        private string filePath = @sourcePath + "Sample.docx";
        private string appendPath = @sourcePath + "SampleAppend.docx";
        private string resultPath = @sourcePath + "Result.docx";
        private List<string> list = new List<string> { "{NAME}", "{COURSE}", "{FIELD_OF_STUDY}", "{GROUP}", "{DISCIPLINE}", "{DOC_BODY}", "{THEME}", "{TEACHER}", "{DATE_YEAR}", "{DATE_MONTH}", "{DATE_DAY}" };
        private ObservableCollection<Append> appendList = new ObservableCollection<Append>();
        private string name = "чё-то";
        private string course = "3";
        private string fieldOfStudy = "Информатика и вычислительная техника";
        private string group = "чё-то";
        private string discipline = "Функциональное и логическое программирование";
        private string doc_body = "чё-то";
        private string theme = "чё-то";
        private string teacher = "x";

        public ObservableCollection<Append> AppendList
        {
            get { return appendList; }
            set { appendList = value; OnPropertyChanged(nameof(AppendList)); }
        }
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(nameof(Name)); }
        }
        public string Course
        {
            get { return course; }
            set { course = value; OnPropertyChanged(nameof(Course)); }
        }
        public string FieldOfStudy
        {
            get { return fieldOfStudy; }
            set { fieldOfStudy = value; OnPropertyChanged(nameof(FieldOfStudy)); }
        }
        public string Group
        {
            get { return group; }
            set { group = value; OnPropertyChanged(nameof(Group)); }
        }
        public string Discipline
        {
            get { return discipline; }
            set { discipline = value; OnPropertyChanged(nameof(Discipline)); }
        }
        public string DocumentBody
        {
            get { return doc_body; }
            set { doc_body = value; OnPropertyChanged(nameof(DocumentBody)); }
        }
        public string Theme
        {
            get { return theme; }
            set { theme = value; OnPropertyChanged(nameof(Theme)); }
        }
        public string Teacher
        {
            get { return teacher; }
            set { teacher = value; OnPropertyChanged(nameof(Teacher)); }
        }

        
        private string GetPlaceholderValue(string line)
        {
            switch (line)
            {
                case "{NAME}":
                    return Name;
                case "{COURSE}":
                    return Course;
                case "{FIELD_OF_STUDY}":
                    return FieldOfStudy;
                case "{GROUP}":
                    return Group;
                case "{DISCIPLINE}":
                    return Discipline;
                case "{THEME}":
                    return Theme;
                case "{TEACHER}":
                    return Teacher;
                case "{DOC_BODY}":
                    return DocumentBody;
                case "{DATE_YEAR}":
                    return DateTime.Now.Year.ToString();
                case "{DATE_MONTH}":
                    return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month); 
                case "{DATE_DAY}":
                    return DateTime.Now.Day.ToString(); 
                default:
                    return "null";
            }
        }
        private void AddAppendToFile(WordprocessingDocument savedDoc)
        {
            Paragraph selectedParagraph = null;
            Paragraph bodyAppendParagraph = new Paragraph();
            Run bodyAppendRun = bodyAppendParagraph.AppendChild(new Run());
            if (appendList.Count() != 0)
            {
                bodyAppendRun.AppendChild(new Break());
                bodyAppendRun.AppendChild(new Text("Приложения: "));
            }
            foreach (var paragraph in savedDoc.MainDocumentPart.Document.Body.Elements<Paragraph>())
            {
                if (paragraph.InnerText.Contains("{DOC_BODY}"))
                    selectedParagraph = paragraph;
            }

            foreach (var app in AppendList)
            {
                Paragraph appendParagraph = new Paragraph();
                Break appendBreak = appendParagraph.AppendChild(new Break { Type = BreakValues.Page });
                savedDoc.MainDocumentPart.Document.Body.AppendChild(appendParagraph);
                using var appendSampleDoc = WordprocessingDocument.Open(appendPath, true);
                using var bufferAppendDoc = appendSampleDoc.Clone(@sourcePath + "$Buffer.docx");
                foreach (var paragraph in bufferAppendDoc.MainDocumentPart.Document.Body.Descendants<Paragraph>())
                {
                    var text = paragraph.Descendants<Text>().FirstOrDefault(x => x.Text.Contains("{APPEND_BODY}"));
                    if (text != null)
                        text.Text = text.Text.Replace("{APPEND_BODY}", app.Body);
                    text = paragraph.Descendants<Text>().FirstOrDefault(x => x.Text.Contains("{APPEND_TITLE}"));
                    if (text != null)
                        text.Text = text.Text.Replace("{APPEND_TITLE}", app.Name);
                    text = paragraph.Descendants<Text>().FirstOrDefault(x => x.Text.Contains("{APPEND_NUMBER}"));
                    if (text != null)
                        text.Text = text.Text.Replace("{APPEND_NUMBER}", appendList.Count() > 1 ? "№" + app.n : "");
                    savedDoc.MainDocumentPart.Document.Body.AppendChild(paragraph.CloneNode(true));
                }
                bodyAppendRun.AppendChild(new Break());
                bodyAppendRun.AppendChild(new Text(app.n + ". " + app.Name));
            }
            for (int i = 4 - AppendList.Count(); i > 0; i--)
            {
                bodyAppendRun.AppendChild(new Break());
            }
            selectedParagraph.InsertAfterSelf(bodyAppendParagraph);
        }
        private void ReplacePlaceholders(WordprocessingDocument savedDoc)
        {
            string? docText = null;
            int i = 0;
            string text_val = null;

            foreach (var line in list)
            {
                text_val = GetPlaceholderValue(line);
                foreach (var paragraph in savedDoc.MainDocumentPart.Document.Body.Descendants<Paragraph>())
                {
                    var text = paragraph.Descendants<Text>().FirstOrDefault(x => x.Text.Contains(line));  
                    if (text != null)
                        text.Text = text.Text.Replace(line, text_val);
                }
                //if (savedDoc.MainDocumentPart is null)
                //{
                //    throw new ArgumentNullException("MainDocumentPart and/or Body is null.");
                //}

                //using (StreamReader sr = new StreamReader(savedDoc.MainDocumentPart.GetStream()))
                //{
                //    docText = sr.ReadToEnd();
                //}

                //Regex regexText = new Regex(line);
                //docText = regexText.Replace(docText, text);

                //using (StreamWriter sw = new StreamWriter(savedDoc.MainDocumentPart.GetStream(FileMode.Create)))
                //{
                //    sw.Write(docText);
                //}

            }
        }
        public void CalculateDoc()
        {
            try
            {
                using var doc = WordprocessingDocument.Open(@filePath, true);
                using var savedDoc = doc.Clone(@resultPath, true);
                AddAppendToFile(savedDoc);
                ReplacePlaceholders(savedDoc);
            }
            catch (Exception) 
            { 
            
            }
            
        }
        public void AddNewAppend()
        {
            Append newApp = new Append();
            newApp.n = AppendList.Count + 1;
            AppendList.Add(newApp);
        }
        public void DeleteAppend(object obj)
        {
            try
            {
                Append app = (Append)obj;
                int index = AppendList.IndexOf(app);
                if (index != -1)
                {
                    AppendList[index].n = index + 1;
                }
                AppendList.RemoveAt(index);

            }
            catch (Exception)
            {

            }
        }
        public void DeleteAllAppend()
        {
            AppendList.Clear();
        }
    }
}
