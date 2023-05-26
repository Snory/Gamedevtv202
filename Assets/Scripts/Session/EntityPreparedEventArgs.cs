using System;

public class EntityPreparedEventArgs : EventArgs
{
    public bool Prepared;
    public EntityPrepareType EntityPrepareType;

    public EntityPreparedEventArgs(EntityPrepareType entityPrepareType, bool prepared)
    {
        this.Prepared = prepared;
        this.EntityPrepareType = entityPrepareType;
    }
}