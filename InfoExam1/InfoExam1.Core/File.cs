using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Cache;
using System.Reflection.Metadata.Ecma335;

namespace InfoExam1.Core
{
	public class File
	{
		public int Id { get; set; }
		public string FileName { get; set; }
		[Required] [MaxLength(30)] public string ShortDescription { get; set; }
		public string Description { get; set; }
		public int DownloadCountLimit { get; set; }
		public int TimesDownloaded { get; set; }
		public DateTime UploadDateTime { get; set; }
		public DateTime StoreDeadline { get; set; }
		public string OptionalPassword { get; set; }
	}
}