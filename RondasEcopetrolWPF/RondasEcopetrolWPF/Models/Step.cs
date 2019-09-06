using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RondasEcopetrolWPF.Models
{
    public class Step
    {
        private string _id;
        private string _type;
        private string _orden;

        public string Step_ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    _id = value;
                }
            }
        }

        public string Step_Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != value)
                {
                    _type = value;
                }
            }
        }

        public string Step_Orden
        {
            get
            {
                return _orden;
            }
            set
            {
                if (_orden != value)
                {
                    _orden = value;
                }
            }
        }
    }
}
