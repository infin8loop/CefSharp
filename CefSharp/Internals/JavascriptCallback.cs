﻿// Copyright © 2010-2014 The CefSharp Project. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CefSharp.Internals
{
    [DataContract]
    internal sealed class JavascriptCallback : DisposableResource, IJavascriptCallback
    {
        private readonly long id;
        private readonly int browserId;
        private readonly WeakReference browserProcessWeakReference;

        public JavascriptCallback(long id, int browserId, BrowserProcessServiceHost browserProcess)
        {
            this.id = id;
            this.browserId = browserId;
            browserProcessWeakReference = new WeakReference(browserProcess);
        }

        public Task<JavascriptResponse> ExecuteAsync(params object[] parms)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("JavascriptCallback is already disposed.");
            }

            var browserProcess = (BrowserProcessServiceHost)browserProcessWeakReference.Target;
            if (browserProcess == null)
            {
                throw new ObjectDisposedException("BrowserProcessServiceHost is already disposed.");
            }

            return browserProcess.JavascriptCallback(browserId, id, parms, null);
        }

        protected override void DoDispose(bool isDisposing)
        {
            var browserProcess = (BrowserProcessServiceHost)browserProcessWeakReference.Target;
            if (!IsDisposed && browserProcess != null && browserProcess.State == CommunicationState.Opened)
            {
                browserProcess.DestroyJavascriptCallback(browserId, id);
            }
        }
    }
}
