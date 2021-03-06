//
//  Created by Shopify.
//  Copyright (c) 2016 Shopify Inc. All rights reserved.
//  Copyright (c) 2016 Xamarin Inc. All rights reserved.
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using System;
using Foundation;

using Shopify.Buy;

namespace ShopifyiOSSample
{
	public class GetShopOperation : NSOperation
	{
		private readonly BuyClient client;

		private bool done;
		private NSUrlSessionDataTask task;

		public event Action<GetShopOperation, Shop> DidReceiveShop;

		public event Action<GetShopOperation, NSError> FailedToReceiveShop;

		public GetShopOperation (BuyClient client)
		{
			this.client = client;
		}

		public override bool IsFinished {
			get {
				return base.IsFinished && done;
			}
		}

		public override void Cancel ()
		{
			task.Cancel ();

			base.Cancel ();
		}

		public override void Main ()
		{
			PollForCompletionStatusAsync ();
		}

		private void PollForCompletionStatusAsync ()
		{
			if (IsCancelled) {
				return;
			}

			task = client.GetShop ((shop, error) => {
				WillChangeValue ("isFinished");
				done = true;
				DidChangeValue ("isFinished");

				if (error != null) {
					FailedToReceiveShop?.Invoke (this, error);
				} else {
					DidReceiveShop?.Invoke (this, shop);
				}
			});
		}
	}
}
