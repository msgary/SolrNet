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

using System.Linq;
using Xunit;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
    
    public class RequiredFieldsAreMappedRuleTests {
        [Fact]
        public void RequiredSolrFieldForWhichNoCopyFieldExistsShouldReturnError() {
            var mgr = new MappingManager();
            mgr.Add(typeof (SchemaMappingTestDocument).GetProperty("ID"), "id");
            mgr.SetUniqueKey(typeof (SchemaMappingTestDocument).GetProperty("ID"));

            var schemaManager = new MappingValidator(mgr, new[] {new RequiredFieldsAreMappedRule()});

            var schemaXmlDocument = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.solrSchemaBasic.xml");
            var solrSchemaParser = new SolrSchemaParser();
            var schema = solrSchemaParser.Parse(schemaXmlDocument);

            var validationResults = schemaManager.EnumerateValidationResults(typeof (SchemaMappingTestDocument), schema).ToList();
            Assert.Equal(1, validationResults.Count);
        }
    }
}