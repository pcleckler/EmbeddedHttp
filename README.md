# EmbeddedHttp
Wrapper for [`HttpListener`](https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener) and basic Static Web Server

## Minimal Usage

The following code segment provides a minimum usage scenario for EmbeddedHttp.

    var server = new EmbeddedHttp.EmbeddedHttp(

        // Listen on localhost, port 8080, for URLs beginning with "/EmbeddedHttpConsole/"
        httpPrefixes: new List<Uri>()
        {
            new Uri("http://localhost:8080/EmbeddedHttpConsole/")
        },

        // Respond to specific routes
        routes: new List<IRoute>()
        {
            new Route("/data", context => new Response(ContentType.Text, "This is some data")) // Defaults to UTF8 Encoding
        },

        // Provide a static website from the directory specified below
        rootDirectory: Path.Combine(Environment.CurrentDirectory, "www"),

        // Optional: Provide a logging interface.
        logger: new Logger());

    server.Start();

## Notes on HttpPrefixes

The port specified in the HTTP prefix can be in use by multiple web servers. This is due to the fact that the `Http.Sys` Windows subsystem handles the initial connection and routes based on requested prefixes.

[`HttpListener`](https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener) operation comes with two restrictions.

1. URL Registration is required in the following circumstances:
   - Listening for connections outside of localhost.
   - Listening for connections on port 80.

    The following is an example URL registration command:

        netsh http add urlacl url=http://warhammer:8080/EmbeddedHttpConsole/ user=Everyone

    The command is similar for HTTPs URL registration:

        netsh http add urlacl url=https://warhammer:8080/EmbeddedHttpConsole/ user=Everyone

    > NOTE: These commands must be run with elevated permissions.

2. HTTPS is only operational in the `HttpPrefixes` list when the server has a registered SSL certificate.

## Static Websites

In the example above, the subdirectory `www` was used as the root directory of the static website. Note that the single registered HttpPrefix included a subdirectory `/EmbeddedHttpConsole/`. Because of URI local path mapping, requests to that subdirectory map to `www\EmbeddedHttpConsole` rather than directly to `www` as might be expected.

With HttpPrefixes that use only a root directory, such as `http://localhost:8080/`, along with the `www` root directory for the static website, the mapping is as expected. Requests to that HttpPrefix will serve up content from the `www` directory and its subdirectories.

