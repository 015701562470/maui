﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Maui.Controls.Sample
{
	public class MapPage : ContentPage
	{
		public MapPage()
		{
			var grid = new Grid();
			grid.RowDefinitions.Add(new RowDefinition());
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition());
			grid.ColumnDefinitions.Add(new ColumnDefinition());

			var map = new Microsoft.Maui.Controls.Maps.Map();
			Grid.SetColumnSpan(map, 2);

			var lbl = new Label { Text = "MapType" };
			var picker = new Picker
			{
				VerticalOptions = LayoutOptions.Start,
				ItemsSource = new string[] { nameof(MapType.Street), nameof(MapType.Satellite), nameof(MapType.Hybrid) }
			};
			picker.SelectedIndexChanged += (s, e) =>
			{
				var item = picker.SelectedItem.ToString();
				switch (item)
				{
					case nameof(MapType.Street):
						map.MapType = MapType.Street;
						break;
					case nameof(MapType.Satellite):
						map.MapType = MapType.Satellite;
						break;
					case nameof(MapType.Hybrid):
						map.MapType = MapType.Hybrid;
						break;
					default:
						break;
				}
			};
			picker.SelectedIndex = 0;

			Grid.SetRow(lbl, 1);
			Grid.SetRow(picker, 1);
			Grid.SetColumn(picker, 1);

			AddBoolMapOption(grid, nameof(map.IsShowingUser), 2, map.IsShowingUser, (bool b) => map.IsShowingUser = b);
			AddBoolMapOption(grid, nameof(map.HasScrollEnabled), 3, map.HasScrollEnabled, (bool b) => map.HasScrollEnabled = b);
			AddBoolMapOption(grid, nameof(map.TrafficEnabled), 4, map.TrafficEnabled, (bool b) => map.TrafficEnabled = b);
			AddBoolMapOption(grid, nameof(map.HasZoomEnabled), 5, map.HasZoomEnabled, (bool b) => map.HasZoomEnabled = b);

			grid.Children.Add(map);
			grid.Children.Add(lbl);
			grid.Children.Add(picker);


			Content = grid;
		}

		private static void AddBoolMapOption(Grid grid, string name, int row, bool isToogled, Action<bool> toogled)
		{
			var lbl = new Label { Text = name };
			var swt = new Switch
			{
				IsToggled = isToogled,
				VerticalOptions = LayoutOptions.Start
			};

			swt.Toggled += (s, e) =>
			{
				toogled((s as Switch).IsToggled);
			};

			Grid.SetRow(lbl, row);
			Grid.SetRow(swt, row);
			Grid.SetColumn(swt, 1);

			grid.Children.Add(lbl);
			grid.Children.Add(swt);
		}
	}
}
