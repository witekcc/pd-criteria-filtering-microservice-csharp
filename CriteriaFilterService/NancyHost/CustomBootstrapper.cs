using Nancy;
using Nancy.Diagnostics;

namespace NancyHostX
{
    class CustomBootstrapper : DefaultNancyBootstrapper
    {

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            //TODO: register message provider
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"Password1234" }; }
        }

        //base.ConfigureApplicationContainer();

    }
}
