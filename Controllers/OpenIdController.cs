using HelloOIDC.Repo;
using IdentityModel.Jwk;
using IdentityModel.OidcClient;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HelloOIDC.Controllers {

    [Route("auth")]
    [ApiController]
    public class OpenIdController : ControllerBase {
        private readonly OIDCRepo repo;

        public OpenIdController(OIDCRepo repo)
        {
            this.repo = repo;
        }

        // GET: api/<ValuesController>
        [HttpGet("login")]
        public async Task<IResult> GetAsync() {
            var options = new OidcClientOptions {
                Authority = "https://accounts.google.com",
                ClientId = "557971780811-hv87bg4n69ftct5vhkbkd8coj90u501a.apps.googleusercontent.com",
                ClientSecret = "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",
                RedirectUri = "https://localhost:7078/auth/redirect",
                Scope = "openid profile email",
                Policy = new Policy {
                    Discovery= new IdentityModel.Client.DiscoveryPolicy {
                        ValidateEndpoints = false
                    }
                }
            };

            var client = new OidcClient(options);

            var pars = new IdentityModel.Client.Parameters {
                { "access_type", "offline" }
            };
            // generate start URL, state, nonce, code challenge
            var state = await client.PrepareLoginAsync(pars);
            repo.SetState(new OpenIdSession {
                CodeVerifier = state.CodeVerifier,
                RedirectUri = state.RedirectUri,
                StartUrl = state.StartUrl,
                State = state.State,
                Time = DateTime.UtcNow
            });

            return Results.Redirect(state.StartUrl);
        }

        [HttpGet("redirect")]
        public async Task<string> Redirect(string state, string code, string scope) {
            var options = new OidcClientOptions {
                Authority = "https://accounts.google.com",
                ClientId = "557971780811-hv87bg4n69ftct5vhkbkd8coj90u501a.apps.googleusercontent.com",
                ClientSecret = "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",
                RedirectUri = "https://localhost:7078/auth/redirect",
                Scope = "openid profile email",
                Policy = new Policy {
                    Discovery = new IdentityModel.Client.DiscoveryPolicy {
                        ValidateEndpoints = false
                    }
                },
                
            };

            var client = new OidcClient(options);

            var savedStateObject = repo.GetState(state);
            var savedState = new AuthorizeState {
                CodeVerifier = savedStateObject.CodeVerifier,
                RedirectUri = savedStateObject.RedirectUri,
                StartUrl = savedStateObject.StartUrl,
                State = savedStateObject.State,
            };

            var result = await client.ProcessResponseAsync(Request.QueryString.Value, savedState);
            var userinfo = await client.GetUserInfoAsync(result.AccessToken);

            repo.Cleanup();
            return userinfo.Claims.First(c => c.Type == "email").Value;
        }
    }
}
