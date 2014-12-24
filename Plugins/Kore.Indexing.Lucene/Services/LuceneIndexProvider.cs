﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Castle.Core.Logging;
using Kore.Indexing.Lucene.Models;
using Kore.Localization;
using Kore.Web.ContentManagement.Configuration;
using Kore.Web.Indexing;
using Kore.Web.IO.FileSystems.AppData;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace Kore.Indexing.Lucene.Services
{
    /// <summary>
    /// Represents the default implementation of an IIndexProvider, based on Lucene
    /// </summary>
    public class LuceneIndexProvider : IIndexProvider
    {
        private readonly IAppDataFolder _appDataFolder;
        public static readonly Version LuceneVersion = Version.LUCENE_29;
        private readonly Analyzer _analyzer;
        private readonly string _basePath;
        public static readonly DateTime DefaultMinDateTime = new DateTime(1980, 1, 1);

        public LuceneIndexProvider(IAppDataFolder appDataFolder, ContentManagementSettings shellSettings)
        {
            _appDataFolder = appDataFolder;
            _analyzer = CreateAnalyzer();

            // TODO: (sebros) Find a common way to get where tenant's specific files should go. "Sites/Tenant" is hard coded in multiple places
            _basePath = _appDataFolder.Combine("Sites", shellSettings.SiteName, "Indexes");

            Logger = NullLogger.Instance;

            // Ensures the directory exists
            EnsureDirectoryExists();

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public static Analyzer CreateAnalyzer()
        {
            // StandardAnalyzer does lower-case and stop-word filtering. It also removes punctuation
            return new StandardAnalyzer(LuceneVersion);
        }

        private void EnsureDirectoryExists()
        {
            var directory = new DirectoryInfo(_appDataFolder.MapPath(_basePath));
            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        protected virtual Directory GetDirectory(string indexName)
        {
            var directoryInfo = new DirectoryInfo(_appDataFolder.MapPath(_appDataFolder.Combine(_basePath, indexName)));
            return FSDirectory.Open(directoryInfo);
        }

        private static Document CreateDocument(LuceneDocumentIndex indexDocument)
        {
            var doc = new Document();

            indexDocument.PrepareForIndexing();
            foreach (var field in indexDocument.Fields)
            {
                doc.Add(field);
            }
            return doc;
        }

        public bool Exists(string indexName)
        {
            return new DirectoryInfo(_appDataFolder.MapPath(_appDataFolder.Combine(_basePath, indexName))).Exists;
        }

        public IEnumerable<string> List()
        {
            return _appDataFolder.ListDirectories(_basePath).Select(Path.GetFileNameWithoutExtension);
        }

        public bool IsEmpty(string indexName)
        {
            if (!Exists(indexName))
            {
                return true;
            }

            using (var reader = IndexReader.Open(GetDirectory(indexName), true))
            {
                return reader.NumDocs() == 0;
            }
        }

        public int NumDocs(string indexName)
        {
            if (!Exists(indexName))
            {
                return 0;
            }

            using (var reader = IndexReader.Open(GetDirectory(indexName), true))
            {
                return reader.NumDocs();
            }
        }

        public void CreateIndex(string indexName)
        {
            using (new IndexWriter(GetDirectory(indexName), _analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
            }
        }

        public void DeleteIndex(string indexName)
        {
            new DirectoryInfo(_appDataFolder.MapPath(_appDataFolder.Combine(_basePath, indexName)))
                .Delete(true);
        }

        public void Store(string indexName, IDocumentIndex indexDocument)
        {
            Store(indexName, new[] { (LuceneDocumentIndex)indexDocument });
        }

        public void Store(string indexName, IEnumerable<IDocumentIndex> indexDocuments)
        {
            Store(indexName, indexDocuments.Cast<LuceneDocumentIndex>());
        }

        public void Store(string indexName, IEnumerable<LuceneDocumentIndex> indexDocuments)
        {
            indexDocuments = indexDocuments.ToArray();

            if (!indexDocuments.Any())
            {
                return;
            }

            // Remove any previous document for these content items
            Delete(indexName, indexDocuments.Select(i => i.ContentItemId));

            using (var writer = new IndexWriter(GetDirectory(indexName), _analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var indexDocument in indexDocuments)
                {
                    var doc = CreateDocument(indexDocument);

                    writer.AddDocument(doc);
                    Logger.DebugFormat("Document [{0}] indexed", indexDocument.ContentItemId);
                }
            }
        }

        public void Delete(string indexName, string documentId)
        {
            Delete(indexName, new[] { documentId });
        }

        public void Delete(string indexName, IEnumerable<string> documentIds)
        {
            documentIds = documentIds.ToArray();

            if (!documentIds.Any())
            {
                return;
            }

            using (var writer = new IndexWriter(GetDirectory(indexName), _analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                var query = new BooleanQuery();

                try
                {
                    foreach (var id in documentIds)
                    {
                        query.Add(new BooleanClause(new TermQuery(new Term("id", id.ToString(CultureInfo.InvariantCulture))), Occur.SHOULD));
                    }

                    writer.DeleteDocuments(query);
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat(ex, "An unexpected error occured while removing the documents [{0}] from the index [{1}].", string.Join(", ", documentIds), indexName);
                }
            }
        }

        public IDocumentIndex New(string documentId)
        {
            return new LuceneDocumentIndex(documentId, T);
        }

        public ISearchBuilder CreateSearchBuilder(string indexName)
        {
            return new LuceneSearchBuilder(GetDirectory(indexName)) { Logger = Logger };
        }

        public IEnumerable<string> GetFields(string indexName)
        {
            if (!Exists(indexName))
            {
                return Enumerable.Empty<string>();
            }

            using (var reader = IndexReader.Open(GetDirectory(indexName), true))
            {
                return reader.GetFieldNames(IndexReader.FieldOption.ALL).ToList();
            }
        }
    }
}