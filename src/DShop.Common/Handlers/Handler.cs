using System;
using System.Threading.Tasks;
using DShop.Common.Types;

namespace DShop.Common.Handlers
{
	public class Handler : IHandler
	{
        private Func<Task> _handle;
        private Func<Task> _onSuccess;
        private Func<Task> _always;
        private Func<Exception, Task> _onError;
        private Func<DShopException, Task> _onCustomError;
        private bool _rethrowException;
        private bool _rethrowCustomException;

        public Handler()
        {
            _always = () => Task.CompletedTask;
        }

        public IHandler Handle(Func<Task> handle)
        {
            _handle = handle;

            return this;
        }

        public IHandler OnSuccess(Func<Task> onSuccess)
        {
            _onSuccess = onSuccess;

            return this;
        }

        public IHandler Always(Func<Task> always)
        {
            _always = always;

            return this;
        }

        public IHandler OnError(Func<Exception, Task> onError, bool rethrow = false)
        {
            _onError = onError;
            _rethrowException = rethrow;

            return this;
        }

        public IHandler OnCustomError(Func<DShopException, Task> onCustomError, bool rethrow = false)
        {
            _onCustomError = onCustomError;
            _rethrowCustomException = rethrow;

            return this;
        }

        public async Task ExecuteAsync()
        {
            bool isFailure = false;

            try
            {
                await _handle();
            }
            catch (DShopException customException)
            {
                isFailure = true;
                await _onCustomError?.Invoke(customException);
                if (_rethrowCustomException) 
                {
                    throw;
                }
            }
            catch (Exception exception)
            {
                isFailure = true;
                await _onError?.Invoke(exception);
                if (_rethrowException) 
                {
                    throw;
                }
            }
            finally
            {
                if (!isFailure)
                {
                    await _onSuccess?.Invoke();
                }
                await _always?.Invoke();
            }
        }
    }
}