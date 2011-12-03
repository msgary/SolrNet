﻿#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Commands;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class ExtractCommandTests {
        [Test]
        public void Execute() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            var parameters = new ExtractParameters(null, "1", "text.doc");

            With.Mocks(mocks).Expecting(() => {
                Expect.Call(conn.PostStream("/update/extract", null, null, new List<KeyValuePair<string, string>> {
                    KV.Create("literal.id", parameters.Id),
                    KV.Create("resource.name", parameters.ResourceName),
                }))
                .Repeat.Once()
                .Return("");
            })
            .Verify(() => {
                var cmd = new ExtractCommand(new ExtractParameters(null, "1", "text.doc"));
                cmd.Execute(conn);
            });
        }

        [Test]
        public void ExecuteWithAllParameters() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            var parameters = new ExtractParameters(null, "1", "text.doc");

            With.Mocks(mocks).Expecting(() =>
            {
                Expect.Call(conn.PostStream("/update/extract", "application/word-document", null, new List<KeyValuePair<string, string>> {
                    KV.Create("literal.id", parameters.Id),
                    KV.Create("resource.name", parameters.ResourceName),
                    KV.Create("literal.field1", "value1"),
                    KV.Create("literal.field2", "value2"),
                    KV.Create("stream.type", "application/word-document"),
                    KV.Create("commit", "true"),
                    KV.Create("uprefix", "pref"),
                    KV.Create("defaultField", "field1"),
                    KV.Create("extractOnly", "true"),
                    KV.Create("extractFormat", "text"),
                    KV.Create("capture", "html"),
                    KV.Create("captureAttr", "true"),
                    KV.Create("xpath", "body"),
                    KV.Create("lowernames", "true")
                }))
                .Repeat.Once()
                .Return("");
            })
            .Verify(() =>
            {
                var cmd = new ExtractCommand(new ExtractParameters(null, "1", "text.doc")
                {
                    AutoCommit = true,
                    Capture = "html",
                    CaptureAttributes = true,
                    DefaultField = "field1",
                    ExtractOnly = true,
                    ExtractFormat = ExtractFormat.Text,
                    Fields = new[] { new ExtractField("field1", "value1"), new ExtractField("field2", "value2") },
                    LowerNames = true,
                    XPath = "body",
                    Prefix = "pref",
                    StreamType = "application/word-document"
                });
                cmd.Execute(conn);
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteWithDuplicateIdField()
        {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();

            const string DuplicateId = "duplicateId";
            var cmd = new ExtractCommand(new ExtractParameters(null, DuplicateId, "text.doc")
            {
                Fields = new[] { new ExtractField("id", DuplicateId), new ExtractField("field2", "value2") }
            });
            cmd.Execute(conn);
        }

    }
}
