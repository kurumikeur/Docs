using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docs.Views;

namespace Docs.Models
{
    public class Append : BaseView
    {
        private string name;
        private string body;
        public int n { get; set; }
      
        public string Name { 
            get { return name; } 
            set { name = value; OnPropertyChanged(nameof(Name)); } 
        }
        public string Body
        {
            get { return body; }
            set { body = value; OnPropertyChanged(nameof(Body)); }
        }
    }
}
