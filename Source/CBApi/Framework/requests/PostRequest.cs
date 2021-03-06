﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using RestSharp;
using CBApi.Framework.Events;

namespace CBApi.Framework.Requests {
    public abstract class PostRequest : BaseRequest {
        private APISettings _Settings = null;
        protected IRestClient _client = new RestClient();
        protected IRestRequest _request = new RestRequest(Method.POST);

        protected PostRequest(APISettings settings) {
            if (settings == null) {
                throw new ArgumentNullException("settings", "You must provide valid API settings");
            }
            _Settings = settings;
            if (_Settings.TargetSite == null) {
                throw new ArgumentNullException("domain", "Please provide a valid domain name");
            }
        }

        public abstract string BaseUrl { get; }

        protected virtual string PostRequestURL() {
            var url = new StringBuilder(20);
            if (_Settings.TargetSite.Secure) {
                url.Append("https://");
            } else {
                url.Append("http://");
            }
            url.Append(_Settings.TargetSite.Domain);
            url.Append(BaseUrl);
            return url.ToString();
        }

        protected virtual void BeforeRequest() {
            _client.BaseUrl = PostRequestURL();
            AddDevKey();
            _request.RequestFormat = DataFormat.Xml;
            _request.Timeout = _Settings.TimeoutMS;
            if (!string.IsNullOrEmpty(_Settings.TargetSite.Host)) {
                _request.AddHeader("Host", _Settings.TargetSite.Host);
            }
            foreach (var item in _Settings.TargetSite.Headers) {
                _request.AddHeader(item.Key, item.Value);
            }
            _BeforeRequestEvent(new RequestEventData(_client, _request, null));
        }

        private void AddDevKey() {
            _request.AddHeader("DeveloperKey", _Settings.DevKey);
        }

        protected virtual void CheckForErrors(IRestResponse response) {
            ErrorParser.CheckForErrors(response);
        }
    }
}