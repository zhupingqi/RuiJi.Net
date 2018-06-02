using System;

namespace RuiJi.Net.Core.Utils.Suffix
{
    public sealed class Domain
    {
        private readonly string _publicSuffix;
        private readonly string _domain;
        private readonly string _subdomain;
        private readonly string _registrableDomain;
        private readonly string _toString;

        public Domain(string publicSuffix, string domain, string subdomain)
        {
            if (publicSuffix == null) throw new ArgumentNullException("publicSuffix");
            _publicSuffix = publicSuffix;
            _domain = domain;
            _subdomain = subdomain;
            _registrableDomain = string.IsNullOrEmpty(_domain) ? null : domain + "." + publicSuffix;
            _toString = _subdomain == null
                ? _registrableDomain
                : _subdomain + "." + _registrableDomain;
        }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(_domain); }
        }

        public string PublicSuffix
        {
            get { return _publicSuffix; }
        }

        public string RegistrableDomain
        {
            get { return _registrableDomain; }
        }

        public string Subdomain
        {
            get { return _subdomain; }
        }

        public override string ToString()
        {
            return _toString;
        }
    }
}