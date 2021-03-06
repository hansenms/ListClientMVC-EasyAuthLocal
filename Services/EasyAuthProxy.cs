using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ListClientMVC.Services
{

    public class AuthClaims {
        public string typ { get; set; }

        public string val { get; set; }
    }

    public class AuthMe {
        public string access_token { get; set; }

        public string id_token { get; set; }

        public string expires_on { get; set; }

        public string refresh_token { get; set; }

        public string user_id { get; set; }

        public string provider_name { get; set; }

        List<AuthClaims> user_claims { get; set; }
    }

    public interface IEasyAuthProxy
    {
        Microsoft.AspNetCore.Http.IHeaderDictionary Headers {get; }
    }

    public class EasyAuthProxy: IEasyAuthProxy
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHostingEnvironment _appEnvironment;
        private IHeaderDictionary _privateHeaders = null;

        public EasyAuthProxy(IHttpContextAccessor contextAccessor,
                             IHostingEnvironment appEnvironment)
        {
            _contextAccessor = contextAccessor;
            _appEnvironment = appEnvironment;

            string authMeFile = _appEnvironment.ContentRootPath + "/wwwroot/.auth/me"; 
            if (File.Exists(authMeFile)) {
                    try {
                    _privateHeaders = new HeaderDictionary();
                    List<AuthMe> authme = JsonConvert.DeserializeObject<List<AuthMe>>(File.ReadAllText(authMeFile));

                    _privateHeaders["X-MS-TOKEN-" + authme[0].provider_name.ToUpper() + "-ID-TOKEN"] = authme[0].id_token;
                    _privateHeaders["X-MS-TOKEN-" + authme[0].provider_name.ToUpper() + "-ACCESS-TOKEN"] = authme[0].access_token;
                    _privateHeaders["X-MS-TOKEN-" + authme[0].provider_name.ToUpper() + "EXPIRES-ON"] = authme[0].expires_on;
                    _privateHeaders["X-MS-CLIENT-PRINCIPAL-ID"] = authme[0].user_id;
                } catch {
                    _privateHeaders = null;
                }
            }
        }

        public IHeaderDictionary Headers {
            get { 
                return _privateHeaders == null ? _contextAccessor.HttpContext.Request.Headers : _privateHeaders; 
            } 
        }
    }
}