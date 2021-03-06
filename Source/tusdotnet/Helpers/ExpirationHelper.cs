﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Expiration;

namespace tusdotnet.Helpers
{
    internal class ExpirationHelper
    {
        private readonly ITusExpirationStore _expirationStore;
        private readonly ExpirationBase _expiration;
        private readonly bool _isSupported;

        public bool IsSlidingExpiration => _expiration is SlidingExpiration;

        internal ExpirationHelper(DefaultTusConfiguration configuration)
        {
            _expirationStore = configuration.Store as ITusExpirationStore;
            _expiration = configuration.Expiration;
            _isSupported = _expirationStore != null && _expiration != null;
        }

        internal async Task<DateTimeOffset?> SetExpirationIfSupported(string fileId, CancellationToken cancellationToken)
        {
            if (!_isSupported)
            {
                return null;
            }

            var expires = DateTimeOffset.UtcNow.Add(_expiration.Timeout);
            await _expirationStore.SetExpirationAsync(fileId, expires, cancellationToken);

            return expires;
        }

        internal Task<DateTimeOffset?> GetExpirationIfSupported(string fileId, CancellationToken cancellationToken)
        {
            if (!_isSupported)
            {
                return Task.FromResult<DateTimeOffset?>(null);
            }

            return _expirationStore.GetExpirationAsync(fileId, cancellationToken);
        }

        internal string FormatHeader(DateTimeOffset? expires)
        {
            return expires?.ToString("R");
        }
    }
}