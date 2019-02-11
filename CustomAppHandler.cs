public class CustomAppHandler : ApplicationEventHandler
 {

 protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Publishing += (sender, arg) => ContentService_Publishing(arg, applicationContext);
        }


        /// <summary>
        /// Content Publishing Event Handler to add redirect for child nodes
        /// </summary>
        /// 
        private void ContentService_Publishing(PublishEventArgs<IContent> args, ApplicationContext applicationContext)
        {
            var contentService = applicationContext.Services.ContentService;

            foreach (var item in args.PublishedEntities)
            {
                var name = item.Name;
                var id = item.Id;
                var nodeBeforeChange = contentService.GetById(id); //get the node from the cache which is not updateded yet

                if(nodeBeforeChange != null)
                {
                    var oldName = nodeBeforeChange.Name;

                    if (name != oldName)
                    {
                        var descendants = nodeBeforeChange.Descendants().ToList(); //filter out only pages not doc types without template
                        var redirectService = applicationContext.Services.RedirectUrlService;
                        if (descendants.Any())
                        {
                            foreach(var d in descendants)
                            {
                                var url = UmbracoContext.Current.UrlProvider.GetUrl(d.Id);
                                redirectService.Register(url, d.Key); //add the old url to the redirect table
                            }
                        }
                    }
                }
            }

        }
}
