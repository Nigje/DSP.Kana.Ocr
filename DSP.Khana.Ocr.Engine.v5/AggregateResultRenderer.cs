// Copyright 2012-2022 Charles Weld.
// SPDX-License-Identifier: Apache-2.0
// Derived from https://github.com/charlesw/tesseract.
// Modified for DSP.Khana.Ocr: type visibility, local integration, and build/runtime
// compatibility where applicable; upstream namespaces and type names restored.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy at https://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
// See THIRD-PARTY-NOTICES.md and LICENSES/Tesseract-Apache-2.0.txt.

using System;
using System.Collections.Generic;
using Tesseract.Internal;

namespace Tesseract
{
    /// <summary>
    /// Aggregate result renderer.
    /// </summary>
    internal class AggregateResultRenderer : DisposableBase, IResultRenderer
    {
        /// <summary>
        /// Ensures the renderer's EndDocument when disposed off.
        /// </summary>
        private class EndDocumentOnDispose : DisposableBase
        {
            private readonly AggregateResultRenderer _renderer;
            private List<IDisposable> _children;

            public EndDocumentOnDispose(AggregateResultRenderer renderer, IEnumerable<IDisposable> children)
            {
                _renderer = renderer;
                _children = new List<IDisposable>(children);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing) {
                    Guard.Verify(_renderer._currentDocumentHandle == this, "Expected the Result Render's active document to be this document.");

                    // End the renderer
                    foreach (var child in _children) {
                        child.Dispose();
                    }
                    _children = null;

                    // reset current handle
                    _renderer._currentDocumentHandle = null;
                }
            }
        }

        private List<IResultRenderer> _resultRenderers;
        private int _pageNumber = -1;
        private IDisposable _currentDocumentHandle;

        /// <summary>
        /// Create a new aggregate result renderer with the specified child result renderers.
        /// </summary>
        /// <param name="resultRenderers">The child result renderers.</param>
        public AggregateResultRenderer(params IResultRenderer[] resultRenderers)
            : this((IEnumerable<IResultRenderer>)resultRenderers)
        {
        }

        /// <summary>
        /// Create a new aggregate result renderer with the specified child result renderers.
        /// </summary>
        /// <param name="resultRenderers">The child result renderers.</param>
        public AggregateResultRenderer(IEnumerable<IResultRenderer> resultRenderers)
        {
            Guard.RequireNotNull("resultRenderers", resultRenderers);

            _resultRenderers = new List<IResultRenderer>(resultRenderers);
        }

        /// <summary>
        /// Get's the current page number.
        /// </summary>
        public int PageNumber
        {
            get { return _pageNumber; }
        }

        /// <summary>
        /// Get's the child result renderers.
        /// </summary>
        public IEnumerable<IResultRenderer> ResultRenderers
        {
            get { return _resultRenderers; }
        }

        /// <summary>
        /// Adds a page to each of the child result renderers.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool AddPage(Page page)
        {
            Guard.RequireNotNull("page", page);
            VerifyNotDisposed();

            _pageNumber++;
            foreach (var renderer in ResultRenderers) {
                if (!renderer.AddPage(page)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Begins a new document with the specified title.
        /// </summary>
        /// <param name="title">The title of the document.</param>
        /// <returns></returns>
        public IDisposable BeginDocument(string title)
        {
            Guard.RequireNotNull("title", title);
            VerifyNotDisposed();
            Guard.Verify(_currentDocumentHandle == null, "Cannot begin document \"{0}\" as another document is currently being processed which must be dispose off first.", title);

            // Reset the page numer
            _pageNumber = -1;

            // Begin the document on each child renderer.
            List<IDisposable> children = new List<IDisposable>();
            try {
                foreach (var renderer in ResultRenderers) {
                    children.Add(renderer.BeginDocument(title));
                }

                _currentDocumentHandle = new EndDocumentOnDispose(this, children);
                return _currentDocumentHandle;
            } catch (Exception error) {
                // Dispose of all previously created child document's iff an error occured to prevent a memory leak.
                foreach (var child in children) {
                    try {
                        child.Dispose();
                    } catch (Exception disposalError) {
                        Logger.TraceError("Failed to dispose of child document {0}: {1}", child, disposalError.Message);
                    }
                }

                throw error;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try {
                if (disposing) {
                    // Ensure that if the renderer has an active document when disposed it too is disposed off.
                    if (_currentDocumentHandle != null) {
                        _currentDocumentHandle.Dispose();
                        _currentDocumentHandle = null;
                    }
                }
            } finally {
                // dispose of result renderers
                foreach (var renderer in ResultRenderers) {
                    renderer.Dispose();
                }
                _resultRenderers = null;
            }
        }
    }
}