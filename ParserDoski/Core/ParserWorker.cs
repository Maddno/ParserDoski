using ParserDoski.Core.Core;
using System;
using AngleSharp.Html.Parser;
using System.Threading.Tasks;

namespace ParserDoski.Core
{
    internal class ParserWorker<T> where T : class
    {
        private IParser<Task<string[]>> parser;
        private IParserSettings parserSettings;
        private HtmlLoader loader;
        private bool isActive;

        public event Action<object, string[]> OnNewData;
        public event Action<object> OnCompleted;
        //public event Action<object, int> StatusUpdate;

        public IParser<Task<string[]>> Parser
        {
            get { return parser; }
            set { parser = value; }
        }

        public IParserSettings Settings
        {
            get { return parserSettings; }
            set
            {
                parserSettings = value;
                loader = new HtmlLoader(value);
            }
        }

        public bool IsActive => isActive;

        public ParserWorker(IParser<Task<string[]>> parser)
        {
            this.parser = parser;
        }

        public ParserWorker(IParser<Task<string[]>> parser, IParser<string[]> linkParser, IParserSettings parserSettings) : this(parser)
        {
            this.parserSettings = parserSettings;
        }


        public void Start(string linksPPathSave)
        {
            isActive = true;
            Worker(linksPPathSave);
        }

        public void Stop()
        {
            isActive = false;
        }

        private async void Worker(string linksPathSave)
        {
            for (int i = parserSettings.StartPoint; i <= parserSettings.EndPoint; i++)
            {
                var id = (i - 1) * 20;
                if (!isActive)
                {
                    OnCompleted?.Invoke(this);
                    return;
                }

                var source = await loader.GetSourceByPageId(id);
                var domParser = new HtmlParser();

                var document = await domParser.ParseDocumentAsync(source);

                var parsedData = parser.Parse(document, linksPathSave);

                OnNewData?.Invoke(this, await parsedData);
                //StatusUpdate?.Invoke(this, i);
                //await Task.Delay(15000);
            }

            OnCompleted?.Invoke(this);
            isActive = false;
        }
    }
}
