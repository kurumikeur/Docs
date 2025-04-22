using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Docs.Views
{
    public class MainWindowView : BaseView
    {
        private List<string> list = new List<string>{ "{NAME}", "{COURSE}", "{FIELD_OF_STUDY}", "{GROUP}", "{DISCIPLINE}", "{THEME}", "{TEACHER}", "{DATE_YEAR}", "{DATE_MONTH}", "{DATE_DAY}" };
        private string name = "x";
        private string course = "3";
        private string fieldOfStudy = "Информатика и вычислительная техника";
        private string group = "x";
        private string discipline = "Функциональное и логическое программирование";
        private string theme = "x";
        private string teacher = "x";
        WordprocessingDocument doc = WordprocessingDocument.Open(@"C:\Github\Docs\test.docx", true);

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

        public void CalculateDoc()
        {
            var body = doc.MainDocumentPart.Document.Body;
            var paras = body.Elements<Paragraph>();
            string? docText = null;
            int i = 0;
            string text = null;
            using (var savedDoc = doc.Clone(@"C:/Github/Docs/result.docx"))
            {
                foreach (var line in list)
                {
                    switch (line)
                    {
                        case "{NAME}":
                            text = Name;
                            break;
                        case "{COURSE}":
                            text = Course;
                            break;
                        case "{FIELD_OF_STUDY}":
                            text = FieldOfStudy;
                            break;
                        case "{GROUP}":
                            text = Group;
                            break;
                        case "{DISCIPLINE}":
                            text = Discipline;
                            break;
                        case "{THEME}":
                            text = Theme;
                            break;
                        case "{TEACHER}":
                            text = Teacher;
                            break;
                        case "{DATE_YEAR}":
                            text = DateTime.Now.Year.ToString();
                            break;
                        case "{DATE_MONTH}":
                            text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month);
                            break;
                        case "{DATE_DAY}":
                            text = DateTime.Now.Day.ToString();
                            break;
                        default:
                            break;
                    }
                    if (savedDoc.MainDocumentPart is null)
                    {
                        throw new ArgumentNullException("MainDocumentPart and/or Body is null.");
                    }

                    using (StreamReader sr = new StreamReader(savedDoc.MainDocumentPart.GetStream()))
                    {
                        docText = sr.ReadToEnd();
                    }

                    Regex regexText = new Regex(line);
                    docText = regexText.Replace(docText, text);

                    using (StreamWriter sw = new StreamWriter(savedDoc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(docText);
                    }
                }
            }
        }
    
    }

}
