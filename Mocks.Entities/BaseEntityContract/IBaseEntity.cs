﻿namespace Mocks.Entities.BaseEntityContract
{
    public interface IBaseEntity<HandlerKey, TKey> : IKeyIdentity<TKey>
        where HandlerKey : IKeyHandlerDefinition<TKey>
    {
    }
}
