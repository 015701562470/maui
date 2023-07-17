using System;
using System.Threading.Tasks;
using Microsoft.Maui.DeviceTests.Stubs;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.Maui.DeviceTests
{
	public partial class HandlerTestBasement<THandler, TStub> : HandlerTestBasement
		where THandler : class, IViewHandler, new()
		where TStub : IStubBase, IView, new()
	{
		// Handlers

		protected THandler CreateHandler(IView view, IMauiContext mauiContext = null) =>
			CreateHandler<THandler>(view, mauiContext);

		protected Task<THandler> CreateHandlerAsync(IView view, IMauiContext mauiContext = null) =>
			InvokeOnMainThreadAsync(() => CreateHandler<THandler>(view, mauiContext));

		// AttachAndRun

		protected Task AttachAndRun(IView view, Action<THandler> action) =>
				AttachAndRun<bool>(view, (handler) =>
				{
					action(handler);
					return Task.FromResult(true);
				});

		protected Task AttachAndRun(IView view, Func<THandler, Task> action) =>
				AttachAndRun<bool>(view, async (handler) =>
				{
					await action(handler);
					return true;
				});

		protected Task<T> AttachAndRun<T>(IView view, Func<THandler, T> action)
		{
			Func<THandler, Task<T>> boop = (handler) =>
			{
				return Task.FromResult(action.Invoke(handler));
			};

			return AttachAndRun<T>(view, boop);
		}

		protected Task<T> AttachAndRun<T>(IView view, Func<THandler, Task<T>> action)
		{
			return view.AttachAndRun<T, IPlatformViewHandler>((handler) =>
			{
				return action.Invoke((THandler)handler);
			}, MauiContext, async (view) => (IPlatformViewHandler)(await CreateHandlerAsync(view)));
		}

		// Values

		protected Task<TValue> GetValueAsync<TValue>(IView view, Func<THandler, TValue> func)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				var handler = CreateHandler(view);
				return func(handler);
			});
		}

		protected Task<TValue> GetValueAsync<TValue>(IView view, Func<THandler, Task<TValue>> func)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				var handler = CreateHandler(view);
				return func(handler);
			});
		}

		protected Task SetValueAsync<TValue>(IView view, TValue value, Action<THandler, TValue> func)
		{
			return SetValueAsync<TValue, THandler>(view, value, func);
		}

		protected async Task ValidatePropertyInitValue<TValue>(
			IView view,
			Func<TValue> GetValue,
			Func<THandler, TValue> GetPlatformValue,
			TValue expectedValue)
		{
			var values = await GetValueAsync(view, (handler) =>
			{
				return new
				{
					ViewValue = GetValue(),
					PlatformViewValue = GetPlatformValue(handler)
				};
			});

			Assert.Equal(expectedValue, values.ViewValue);
			Assert.Equal(expectedValue, values.PlatformViewValue);
		}

		protected async Task ValidatePropertyInitValue<TValue>(
			IView view,
			Func<TValue> GetValue,
			Func<THandler, TValue> GetPlatformValue,
			TValue expectedValue,
			TValue expectedPlatformValue)
		{
			var values = await GetValueAsync(view, (handler) =>
			{
				return new
				{
					ViewValue = GetValue(),
					PlatformViewValue = GetPlatformValue(handler)
				};
			});

			Assert.Equal(expectedValue, values.ViewValue);
			Assert.Equal(expectedPlatformValue, values.PlatformViewValue);
		}

		protected async Task ValidatePropertyUpdatesValue<TValue>(
			IView view,
			string property,
			Func<THandler, TValue> GetPlatformValue,
			TValue expectedSetValue,
			TValue expectedUnsetValue)
		{
			var propInfo = view.GetType().GetProperty(property);

			// set initial values

			propInfo.SetValue(view, expectedSetValue);

			var (handler, viewVal, nativeVal) = await InvokeOnMainThreadAsync(() =>
			{
				var handler = CreateHandler(view);
				return (handler, (TValue)propInfo.GetValue(view), GetPlatformValue(handler));
			});

			Assert.Equal(expectedSetValue, viewVal);
			Assert.Equal(expectedSetValue, nativeVal);

			await ValidatePropertyUpdatesAfterInitValue(handler, property, GetPlatformValue, expectedSetValue, expectedUnsetValue);
		}

		protected async Task ValidatePropertyUpdatesAfterInitValue<TValue>(
			THandler handler,
			string property,
			Func<THandler, TValue> GetPlatformValue,
			TValue expectedSetValue,
			TValue expectedUnsetValue)
		{
			var view = handler.VirtualView;
			var propInfo = handler.VirtualView.GetType().GetProperty(property);

			// confirm can update

			var (viewVal, nativeVal) = await InvokeOnMainThreadAsync(() =>
			{
				propInfo.SetValue(view, expectedUnsetValue);
				handler.UpdateValue(property);

				return ((TValue)propInfo.GetValue(view), GetPlatformValue(handler));
			});

			Assert.Equal(expectedUnsetValue, viewVal);
			Assert.Equal(expectedUnsetValue, nativeVal);

			// confirm can revert

			(viewVal, nativeVal) = await InvokeOnMainThreadAsync(() =>
			{
				propInfo.SetValue(view, expectedSetValue);
				handler.UpdateValue(property);

				return ((TValue)propInfo.GetValue(view), GetPlatformValue(handler));
			});

			Assert.Equal(expectedSetValue, viewVal);
			Assert.Equal(expectedSetValue, nativeVal);
		}

		protected async Task ValidateUnrelatedPropertyUnaffected<TValue>(
			IView view,
			Func<THandler, TValue> GetPlatformValue,
			string property,
			Action SetUnrelatedProperty)
		{
			// get initial values

			var (handler, initialNativeVal) = await InvokeOnMainThreadAsync(() =>
			{
				var handler = CreateHandler(view);
				return (handler, GetPlatformValue(handler));
			});

			// run update

			var newNativeVal = await InvokeOnMainThreadAsync(() =>
			{
				SetUnrelatedProperty();
				handler.UpdateValue(property);

				return GetPlatformValue(handler);
			});

			// ensure unchanged

			Assert.Equal(initialNativeVal, newNativeVal);
		}

		// Helpers

		protected void AssertWithinTolerance(double expected, double actual, double tolerance = 0.2, string message = "Value was not within tolerance.")
		{
			var diff = System.Math.Abs(expected - actual);
			if (diff > tolerance)
			{
				throw new XunitException($"{message} Expected: {expected}; Actual: {actual}; Tolerance {tolerance}");
			}
		}

		protected void AssertWithinTolerance(Graphics.Size expected, Graphics.Size actual, double tolerance = 0.2)
		{
			AssertWithinTolerance(expected.Height, actual.Height, tolerance, "Height was not within tolerance.");
			AssertWithinTolerance(expected.Width, actual.Width, tolerance, "Width was not within tolerance.");
		}
	}
}