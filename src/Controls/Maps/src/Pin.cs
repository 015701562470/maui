using System;
using System.ComponentModel;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

namespace Microsoft.Maui.Controls.Maps
{
	public class Pin : Element, IMapPin
	{
		public static readonly BindableProperty TypeProperty = BindableProperty.Create(nameof(Type), typeof(PinType), typeof(Pin), default(PinType));

		public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(Location), typeof(Pin), default(Location));

		public static readonly BindableProperty AddressProperty = BindableProperty.Create(nameof(Address), typeof(string), typeof(Pin), default(string));

		public static readonly BindableProperty LabelProperty = BindableProperty.Create(nameof(Label), typeof(string), typeof(Pin), default(string));
		private object _markerId;

		public string Address
		{
			get { return (string)GetValue(AddressProperty); }
			set { SetValue(AddressProperty, value); }
		}

		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		public Location Position
		{
			get { return (Location)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}

		public PinType Type
		{
			get { return (PinType)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object MarkerId
		{
			get => _markerId;
			set
			{
				_markerId = value;
			}
		}

		public event EventHandler<PinClickedEventArgs> MarkerClicked;

		public event EventHandler<PinClickedEventArgs> InfoWindowClicked;

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;
			return Equals((Pin)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
#if NETSTANDARD2_0
				int hashCode = Label?.GetHashCode() ?? 0;
				hashCode = (hashCode * 397) ^ Position.GetHashCode();
				hashCode = (hashCode * 397) ^ (int)Type;
				hashCode = (hashCode * 397) ^ (Address?.GetHashCode() ?? 0);
#else
				int hashCode = Label?.GetHashCode(StringComparison.Ordinal) ?? 0;
				hashCode = (hashCode * 397) ^ Position.GetHashCode();
				hashCode = (hashCode * 397) ^ (int)Type;
				hashCode = (hashCode * 397) ^ (Address?.GetHashCode(StringComparison.Ordinal) ?? 0);
#endif
				return hashCode;
			}
		}

		public static bool operator ==(Pin left, Pin right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Pin left, Pin right)
		{
			return !Equals(left, right);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SendMarkerClick()
		{
			var args = new PinClickedEventArgs();
			MarkerClicked?.Invoke(this, args);
			return args.HideInfoWindow;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SendInfoWindowClick()
		{
			var args = new PinClickedEventArgs();
			InfoWindowClicked?.Invoke(this, args);
			return args.HideInfoWindow;
		}

		bool Equals(Pin other)
		{
			return string.Equals(Label, other.Label, StringComparison.Ordinal) &&
				Equals(Position, other.Position) && Type == other.Type &&
				string.Equals(Address, other.Address, StringComparison.Ordinal);
		}
	}
}
