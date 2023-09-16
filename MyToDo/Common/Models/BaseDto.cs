using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common.Models
{
    public class BaseDto : INotifyPropertyChanged
    {
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		private DateTime createDateTiem;

		public DateTime CreateDateTiem
        {
			get { return createDateTiem; }
			set { createDateTiem = value; }
		}

		private DateTime updataDateTime;

		public DateTime UpdataDateTime
        {
			get { return updataDateTime; }
			set { updataDateTime = value; }
		}

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 实现通知更新
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
