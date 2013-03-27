using System;
using System.Drawing;
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ZXing.Mobile;
using System.Collections.Generic;
using MonoTouch.AVFoundation;

namespace ZXing.Mobile
{
	public class ZXingDefaultOverlayView : UIView
	{
		MobileBarcodeScanner Scanner;

		public ZXingDefaultOverlayView (MobileBarcodeScanner scanner, RectangleF frame, Action onCancel, Action onTorch) : base(frame)
		{
			OnCancel = onCancel;
			OnTorch = onTorch;
			Scanner = scanner;
			Initialize();
		}

		Action OnCancel;
		Action OnTorch;

		private void Initialize ()
		{   
			Opaque = false;
			BackgroundColor = UIColor.Clear;
			
			//Add(_mainView);
			var picFrameWidth = Math.Round(Frame.Width * 0.90); //screenFrame.Width;
			var picFrameHeight = Math.Round(Frame.Height * 0.60);
			var picFrameX = (Frame.Width - picFrameWidth) / 2;
			var picFrameY = (Frame.Height - picFrameHeight) / 2;
			
			var picFrame = new RectangleF((int)picFrameX, (int)picFrameY, (int)picFrameWidth, (int)picFrameHeight);


			//Setup Overlay
			var overlaySize = new SizeF (this.Frame.Width, this.Frame.Height - 44);
			
			var topBg = new UIView (new RectangleF (0, 0, this.Frame.Width, (overlaySize.Height - picFrame.Height) / 2));
			topBg.Frame = new RectangleF (0, 0, this.Frame.Width, this.Frame.Height * 0.30f);
			topBg.BackgroundColor = UIColor.Black;
			topBg.Alpha = 0.6f;

			
			var bottomBg = new UIView (new RectangleF (0, topBg.Frame.Height + picFrame.Height, this.Frame.Width, topBg.Frame.Height));
			bottomBg.Frame = new RectangleF (0, this.Frame.Height * 0.70f, this.Frame.Width, this.Frame.Height * 0.30f);
			bottomBg.BackgroundColor = UIColor.Black;
			bottomBg.Alpha = 0.6f;
			
			//var grad = new MonoTouch.CoreAnimation.CAGradientLayer();
			//grad.Frame = bottomBg.Bounds;
			//grad.Colors = new CGColor[] { new CGColor()UIColor.Black, UIColor.FromWhiteAlpha(0.0f, 0.6f) };
			//bottomBg.Layer.InsertSublayer(grad, 0);
			
			//			[v.layer insertSublayer:gradient atIndex:0];
			
			
			var redLine = new UIView (new RectangleF (0, this.Frame.Height * 0.5f - 2.0f, this.Frame.Width, 4.0f));
			redLine.BackgroundColor = UIColor.Red;
			redLine.Alpha = 0.4f;
			
			this.AddSubview (redLine);
			this.AddSubview (topBg);
			this.AddSubview (bottomBg);
			
			var textTop = new UILabel () 
			{
				Frame = new RectangleF(0, this.Frame.Height *  0.10f, this.Frame.Width, 42),
				Text = Scanner.TopText,
				Font = UIFont.SystemFontOfSize(13),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Lines = 2,
				BackgroundColor = UIColor.Clear
			};
			
			this.AddSubview (textTop);
			
			var textBottom = new UILabel () 
			{
				Frame = new RectangleF(0, this.Frame.Height *  0.825f - 32f, this.Frame.Width, 64),
				Text = Scanner.BottomText,
				Font = UIFont.SystemFontOfSize(13),
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				Lines = 3,
				BackgroundColor = UIColor.Clear
				
			};
			
			this.AddSubview (textBottom);


			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);

			bool hasTorch = false;

			if (captureDevice != null)
				hasTorch = captureDevice.TorchAvailable;
			
			InvokeOnMainThread(delegate {
				// Setting tool bar
				var toolBar = new UIToolbar(new RectangleF(0, Frame.Height - 44, Frame.Width, 44));
				
				var buttons = new List<UIBarButtonItem>();
				buttons.Add(new UIBarButtonItem(Scanner.CancelButtonText, UIBarButtonItemStyle.Done, 
				                                delegate {  OnCancel(); })); 
				
				if (hasTorch)
				{
					buttons.Add(new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace));
					buttons.Add(new UIBarButtonItem(Scanner.FlashButtonText, UIBarButtonItemStyle.Done,
					                                delegate { OnTorch(); }));
				}
				
				toolBar.Items = buttons.ToArray();
				
				toolBar.TintColor = UIColor.Black;
				Add(toolBar);
			});	
			


		}

	}
}

