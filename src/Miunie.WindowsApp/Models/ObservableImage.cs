﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miunie.WindowsApp.Models
{
    public class ObservableImage : ObservableObject
    {
		private string _proxyUrl;

		public string ProxyUrl
		{
			get { return _proxyUrl; }
			set { 
				_proxyUrl = value;
				RaisePropertyChanged(nameof(ProxyUrl));
			}
		}

		private int? _width;

		public int? Width
		{
			get { return _width; }
			set
			{
				_width = value;
				RaisePropertyChanged(nameof(Width));
			}
		}

		private int? _height;

		public int? Height
		{
			get { return _height; }
			set
			{
				_height = value;
				RaisePropertyChanged(nameof(Height));
			}
		}
	}
}
