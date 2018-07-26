using System;
using System.Linq;

namespace XAS.Rest.Server {

    public static class NancyExtension {

        // taken from: https://stackoverflow.com/questions/15658627/is-it-possible-to-enable-cors-using-nancyfx
        // with modifications.
        //
        // Follows my understanding of this: https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS
        //

        /// <summary>
        /// An extension to enable CORS processing.
        /// </summary>
        /// <param name="pipelines">A Nancy IPipelines object.</param>
        /// 
        public static void EnableCORS(this Nancy.Bootstrapper.IPipelines pipelines) {

            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx => {

                // is this a CORS request?

                if (ctx.Request.Headers.Keys.Contains("Origin")) {

                    // default CORS responses

                    ctx.Response.Headers["Access-Control-Max-Age"] = "86400";
                    ctx.Response.Headers["Access-Control-Allow-Credentials"] = "true";

                    var accept = "" + String.Join(" ", ctx.Request.Headers["Accept"]);
                    ctx.Response.Headers["Access-Control-Allow-Headers"] = accept;

                    // always return the origin. allows cookies and authentication to work.

                    var origins = "" + String.Join(" ", ctx.Request.Headers["Origin"]);
                    ctx.Response.Headers["Access-Control-Allow-Origin"] = origins;

                    // handle CORS preflight request

                    if (ctx.Request.Method == "OPTIONS") {

                        ctx.Response.Headers["Access-Control-Allow-Methods"] = ctx.Response.Headers["Allow"];

                        if (ctx.Request.Headers.Keys.Contains("Access-Control-Request-Headers")) {

                            var allowedHeaders = "" + String.Join(
                                 ", ",
                                 ctx.Request.Headers["Access-Control-Request-Headers"]);

                            ctx.Response.Headers["Access-Control-Allow-Headers"] = allowedHeaders;

                        }

                    }

                }

            });

        }

    }

}
