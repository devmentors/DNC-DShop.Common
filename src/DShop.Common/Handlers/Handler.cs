using System;
using System.Threading.Tasks;
using DShop.Common.Types;

namespace DShop.Common.Handlers
{
	public class Handler : IHandler
	{
        private Func<Task> _handle;
        private Func<Task> _onSuccess;
        private Func<Exception, Task> _onError;
        private Func<DShopException, Task> _onDShopError;


        IHandler IHandler.Handle(Func<Task> handle)
        {
            _handle = handle;
            return this;
        }

        IHandler IHandler.OnSuccess(Func<Task> onSuccess)
        {
            _onSuccess = onSuccess;
            return this;
        }

        IHandler IHandler.OnError(Func<Exception, Task> onError)
        {
            _onError = onError;
            return this;
        }

        IHandler IHandler.OnDShopError(Func<DShopException, Task> onDShopError)
        {
            _onDShopError = onDShopError;
            return this;
        }

        async Task IHandler.ExecuteAsync()
        {
            bool isFailure = false;

            try
            {
                await _handle();
            }
            catch(DShopException emmaException)
            {
                isFailure = true;
                await _onDShopError?.Invoke(emmaException);
            }
            catch(Exception exceptoin)
            {
                isFailure = true;
                await _onError?.Invoke(exceptoin);
            }
            finally
            {
                if(!isFailure)
                {
                    await _onSuccess?.Invoke();
                }
            }
        }
	}
}